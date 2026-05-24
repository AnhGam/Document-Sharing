/**
 * DSM CI/CD Observability Dashboard Logic
 * Interactive telemetry, live metrics charts, and GitOps report viewer.
 */

document.addEventListener("DOMContentLoaded", () => {
    let telemetryData = [];
    let activeBranch = "all";
    let activeStatus = "all";
    let searchQuery = "";
    
    // Chart References
    let durationChartInstance = null;
    let capacityChartInstance = null;

    // DOM Elements
    const metricDoraRating = document.getElementById("metric-dora-rating");
    const metricDoraSub = document.getElementById("metric-dora-sub");
    const metricBuildDuration = document.getElementById("metric-build-duration");
    const metricDurationSub = document.getElementById("metric-duration-sub");
    const metricInstallerSize = document.getElementById("metric-installer-size");
    const metricInstallerSub = document.getElementById("metric-installer-sub");
    const metricRepoSize = document.getElementById("metric-repo-size");
    const metricRepoSub = document.getElementById("metric-repo-sub");
    
    const installerProgress = document.getElementById("installer-progress");
    const repoProgress = document.getElementById("repo-progress");
    
    const branchFilter = document.getElementById("branch-filter");
    const statusFilter = document.getElementById("status-filter");
    const searchInput = document.getElementById("search-input");
    const buildCount = document.getElementById("build-count");
    const historyTableBody = document.getElementById("history-table-body");

    // Active Context Banner Elements
    const contextCommitInfo = document.getElementById("context-commit-info");
    const contextBranchBadge = document.getElementById("context-branch-badge");
    const contextStatusBadge = document.getElementById("context-status-badge");

    // Modal Elements
    const reportModal = document.getElementById("report-modal");
    const modalCloseBtn = document.getElementById("modal-close-btn");
    const modalCloseBtnFooter = document.getElementById("modal-close-btn-footer");
    const modalBackdrop = document.getElementById("modal-backdrop");
    const modalTitle = document.getElementById("modal-title");
    const modalBodyContent = document.getElementById("modal-body-content");
    const modalReportType = document.getElementById("modal-report-type");
    const modalFooterMeta = document.getElementById("modal-footer-meta");

    // Limits
    const MAX_INSTALLER_MB = 50.0;
    const MAX_REPO_MB = 500.0;

    // 1. Fetch JSON database
    async function initDashboard() {
        try {
            const response = await fetch("history.json");
            if (!response.ok) {
                throw new Error("Could not load history.json");
            }
            telemetryData = await response.json();
            
            // Format array in case it's not
            if (!Array.isArray(telemetryData)) {
                telemetryData = [telemetryData];
            }
            
            populateFilters(telemetryData);
            updateDashboard(telemetryData);
            
            // Event Listeners
            branchFilter.addEventListener("change", (e) => {
                activeBranch = e.target.value;
                filterAndRender();
            });
            statusFilter.addEventListener("change", (e) => {
                activeStatus = e.target.value;
                filterAndRender();
            });
            searchInput.addEventListener("input", (e) => {
                searchQuery = e.target.value.toLowerCase().trim();
                filterAndRender();
            });

            // Modal Close Events
            const closeModal = () => reportModal.classList.remove("active");
            modalCloseBtn.addEventListener("click", closeModal);
            modalCloseBtnFooter.addEventListener("click", closeModal);
            modalBackdrop.addEventListener("click", closeModal);

        } catch (error) {
            console.error("Dashboard initialization failed:", error);
            showErrorState("Could not fetch telemetry records. Ensure history.json is generated in the root of the 'logs' branch.");
        }
    }

    // Helper to format PR branch references beautifully
    function formatBranchName(branch) {
        if (!branch) return "main";
        const prMatch = branch.match(/^(\d+)\/merge$/);
        return prMatch ? `PR #${prMatch[1]}` : branch;
    }

    // 2. Populate Filters
    function populateFilters(data) {
        const branches = new Set();
        data.forEach(item => {
            if (item.branch) branches.add(item.branch);
        });

        branches.forEach(branch => {
            const option = document.createElement("option");
            option.value = branch;
            option.textContent = formatBranchName(branch);
            branchFilter.appendChild(option);
        });
    }

    // 3. Update all widgets, charts, and table
    function updateDashboard(data) {
        if (!data || data.length === 0) {
            // Reset metric cards to empty state
            metricDoraRating.textContent = "-";
            metricDoraRating.className = "metric-value text-glowing-cyan";
            metricDoraSub.textContent = "No successful builds found";
            
            metricBuildDuration.textContent = "-";
            metricDurationSub.textContent = "Latest: - min";
            
            metricInstallerSize.textContent = "-";
            installerProgress.style.width = "0%";
            metricInstallerSub.textContent = "Limit: 50.0 MB";
            
            metricRepoSize.textContent = "-";
            repoProgress.style.width = "0%";
            metricRepoSub.textContent = "Limit: 500.0 MB";

            // Reset active context banner
            if (contextCommitInfo) {
                contextCommitInfo.textContent = "No build history matches the active filters.";
            }
            if (contextBranchBadge) {
                contextBranchBadge.innerHTML = `<i data-lucide="git-branch" style="width:12px;height:12px;vertical-align:middle;margin-right:2px;"></i> ${formatBranchName(activeBranch)}`;
                contextBranchBadge.className = "context-badge";
            }
            if (contextStatusBadge) {
                contextStatusBadge.textContent = activeStatus.toUpperCase();
                contextStatusBadge.className = "context-badge";
            }

            // Reset charts
            if (durationChartInstance) {
                durationChartInstance.data.labels = [];
                durationChartInstance.data.datasets[0].data = [];
                durationChartInstance.update();
            }
            if (capacityChartInstance) {
                capacityChartInstance.data.labels = [];
                capacityChartInstance.data.datasets[0].data = [];
                capacityChartInstance.data.datasets[1].data = [];
                capacityChartInstance.update();
            }
            
            showEmptyState();
            return;
        }

        // Metrics from latest build
        const latestBuild = data[0];
        const latestSuccess = data.find(item => item.buildStatus === "success" || item.buildStatus === "SUCCESS");

        // --- Update Active Context Banner ---
        if (contextCommitInfo) {
            const shortSha = latestBuild.shortSha || (latestBuild.commitSha ? latestBuild.commitSha.substring(0, 7) : 'N/A');
            const commitMsg = latestBuild.commitMessage || "No commit message provided";
            const author = latestBuild.actor || "Workflow Bot";
            const time = latestBuild.time || "";
            const date = latestBuild.date || "";
            const dateText = (date || time) ? ` (${date} ${time})` : "";
            contextCommitInfo.innerHTML = `<strong>#${latestBuild.runId || 'N/A'}</strong> - <span style="font-family: var(--font-code); color: var(--color-cyan);">${shortSha}</span> - <em>"${commitMsg}"</em> by <strong>${author}</strong>${dateText}`;
        }
        if (contextBranchBadge) {
            const bName = formatBranchName(latestBuild.branch);
            contextBranchBadge.innerHTML = `<i data-lucide="${(latestBuild.branch || "").includes('/merge') ? 'git-pull-request' : 'git-branch'}" style="width:12px;height:12px;vertical-align:middle;margin-right:2px;"></i> ${bName}`;
            contextBranchBadge.className = "context-badge branch-badge";
        }
        if (contextStatusBadge) {
            const isSuccess = latestBuild.buildStatus === "success" || latestBuild.buildStatus === "SUCCESS";
            contextStatusBadge.textContent = isSuccess ? "SUCCESS" : "FAILED";
            contextStatusBadge.className = `context-badge ${isSuccess ? 'status-success' : 'status-failure'}`;
        }
        
        // --- Calculate DORA Lead Time Rating ---
        if (latestSuccess) {
            const duration = parseFloat(latestSuccess.buildDuration || 0);
            let rating = "Elite";
            let ratingClass = "text-glowing-green";
            if (duration > 20) { rating = "High"; ratingClass = "text-glowing-cyan"; }
            if (duration > 60) { rating = "Medium"; ratingClass = "text-glowing-orange"; }
            if (duration > 240) { rating = "Low"; ratingClass = "text-glowing-danger"; }
            
            metricDoraRating.textContent = rating;
            metricDoraRating.className = `metric-value ${ratingClass}`;
            metricDoraSub.innerHTML = `${duration.toFixed(2)} min build lead time<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[PR/Branch: ${formatBranchName(latestSuccess.branch)} | SHA: ${latestSuccess.shortSha || latestSuccess.commitSha.substring(0, 7)}]</span>`;
        } else {
            metricDoraRating.textContent = "N/A";
            metricDoraRating.className = "metric-value text-glowing-danger";
            metricDoraSub.textContent = "No successful builds found";
        }

        // --- Average Build Duration ---
        const successfulBuilds = data.filter(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") && item.buildDuration);
        const avgDuration = successfulBuilds.reduce((sum, item) => sum + parseFloat(item.buildDuration), 0) / (successfulBuilds.length || 1);
        metricBuildDuration.textContent = avgDuration.toFixed(2);
        metricDurationSub.innerHTML = `Latest: ${parseFloat(latestBuild.buildDuration || 0).toFixed(2)} min<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Average of ${successfulBuilds.length} runs]</span>`;

        // --- Installer Size ---
        const installerSize = parseFloat(latestBuild.installerSize || 0);
        metricInstallerSize.textContent = installerSize.toFixed(2);
        
        // Progress bar
        const instPct = Math.min((installerSize / MAX_INSTALLER_MB) * 100, 100);
        installerProgress.style.width = `${instPct}%`;
        if (installerSize > MAX_INSTALLER_MB) {
            metricInstallerSize.className = "metric-value text-glowing-danger";
            metricInstallerSub.innerHTML = `ALERT: Over 50MB Budget! (${installerSize.toFixed(2)}MB)<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Build #${latestBuild.runId || 'N/A'} | ${latestBuild.shortSha || 'N/A'}]</span>`;
        } else {
            metricInstallerSize.className = "metric-value text-glowing-green";
            metricInstallerSub.innerHTML = `Limit: ${MAX_INSTALLER_MB} MB (${instPct.toFixed(0)}% used)<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Build #${latestBuild.runId || 'N/A'} | ${latestBuild.shortSha || 'N/A'}]</span>`;
        }

        // --- Repo Size ---
        const repoSize = parseFloat(latestBuild.repoSize || 0);
        metricRepoSize.textContent = repoSize.toFixed(2);
        
        // Progress bar
        const repoPct = Math.min((repoSize / MAX_REPO_MB) * 100, 100);
        repoProgress.style.width = `${repoPct}%`;
        if (repoSize > MAX_REPO_MB) {
            metricRepoSize.className = "metric-value text-glowing-danger";
            metricRepoSub.innerHTML = `ALERT: Over 500MB Budget! (${repoSize.toFixed(2)}MB)<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Build #${latestBuild.runId || 'N/A'} | ${latestBuild.shortSha || 'N/A'}]</span>`;
        } else {
            metricRepoSize.className = "metric-value text-glowing-orange";
            metricRepoSub.innerHTML = `Limit: ${MAX_REPO_MB} MB (${repoPct.toFixed(0)}% used)<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Build #${latestBuild.runId || 'N/A'} | ${latestBuild.shortSha || 'N/A'}]</span>`;
        }

        // --- Render Charts & Table ---
        renderCharts(data);
        renderTable(data);
    }

    function renderCharts(data) {
        const chronData = [...data].reverse();
        const labels = chronData.map(item => item.shortSha || (item.commitSha ? item.commitSha.substring(0, 7) : 'N/A'));
        const durations = chronData.map(item => parseFloat(item.buildDuration || 0));
        const pointColors = chronData.map(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") ? "#00f2fe" : "#ff1744");
        const pointRadii = chronData.map(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") ? 5 : 7);
        const installerSizes = chronData.map(item => parseFloat(item.installerSize || 0));
        const repoSizes = chronData.map(item => parseFloat(item.repoSize || 0));

        // -- Chart 1: Build Duration Line Chart --
        if (durationChartInstance) durationChartInstance.destroy();
        const ctx1 = document.getElementById("durationChart").getContext("2d");
        
        // Gradient fill
        const purpleGrad = ctx1.createLinearGradient(0, 0, 0, 250);
        purpleGrad.addColorStop(0, "rgba(185, 39, 252, 0.25)");
        purpleGrad.addColorStop(1, "rgba(185, 39, 252, 0.00)");

        durationChartInstance = new Chart(ctx1, {
            type: "line",
            data: {
                labels: labels,
                datasets: [{
                    label: "Build Duration (min)",
                    data: durations,
                    borderColor: "#b927fc",
                    borderWidth: 2,
                    pointBackgroundColor: pointColors,
                    pointBorderColor: "#fff",
                    pointRadius: pointRadii,
                    pointHoverRadius: 8,
                    tension: 0.4,
                    fill: true,
                    backgroundColor: purpleGrad,
                    spanGaps: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    x: {
                        grid: { color: "rgba(255,255,255,0.03)" },
                        ticks: { color: "#8c9ba5", font: { family: "Space Grotesk" } }
                    },
                    y: {
                        grid: { color: "rgba(255,255,255,0.03)" },
                        ticks: { color: "#8c9ba5" }
                    }
                }
            }
        });

        // -- Chart 2: Code Footprint Bar Chart --
        if (capacityChartInstance) capacityChartInstance.destroy();
        const ctx2 = document.getElementById("capacityChart").getContext("2d");

        capacityChartInstance = new Chart(ctx2, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "Installer Size (MB)",
                        data: installerSizes,
                        backgroundColor: "rgba(0, 230, 118, 0.7)",
                        borderColor: "#00e676",
                        borderWidth: 1,
                        borderRadius: 4
                    },
                    {
                        label: "Repository Size (MB)",
                        data: repoSizes,
                        backgroundColor: "rgba(255, 145, 0, 0.7)",
                        borderColor: "#ff9100",
                        borderWidth: 1,
                        borderRadius: 4
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        labels: { color: "#8c9ba5", font: { family: "Plus Jakarta Sans", size: 11 } }
                    }
                },
                scales: {
                    x: {
                        grid: { color: "rgba(255,255,255,0.03)" },
                        ticks: { color: "#8c9ba5", font: { family: "Space Grotesk" } }
                    },
                    y: {
                        grid: { color: "rgba(255,255,255,0.03)" },
                        ticks: { color: "#8c9ba5" }
                    }
                }
            }
        });
    }

    // 5. Render History Logs Table
    function renderTable(data) {
        historyTableBody.innerHTML = "";
        buildCount.textContent = `${data.length} Build${data.length !== 1 ? 's' : ''}`;

        data.forEach(item => {
            const tr = document.createElement("tr");

            // Status Badge
            const isSuccess = item.buildStatus === "success" || item.buildStatus === "SUCCESS";
            const statusClass = isSuccess ? "status-success" : "status-failure";
            const statusText = isSuccess ? "SUCCESS" : "FAILED";
            
            const statusTd = `
                <td>
                    <span class="status-indicator ${statusClass}">
                        ${statusText}
                    </span>
                </td>
            `;

            // Build SHA link
            const commitSha = item.commitSha || "";
            const shortSha = item.shortSha || commitSha.substring(0, 7);
            const buildUrl = `https://github.com/AnhGam/Study-Document-Management/actions/runs/${item.runId}`;
            const commitUrl = `https://github.com/AnhGam/Study-Document-Management/commit/${commitSha}`;
            const buildTd = `
                <td>
                    <div style="font-weight: 700; margin-bottom: 2px;">
                        <a href="${buildUrl}" target="_blank" style="color: #fff; text-decoration: none;">Build #${item.runId || 'N/A'}</a>
                    </div>
                    <a href="${commitUrl}" target="_blank" class="commit-link">${shortSha}</a>
                </td>
            `;

            // Branch
            const isPR = (item.branch || "").match(/^(\d+)\/merge$/);
            const branchIcon = isPR ? "git-pull-request" : "git-branch";
            const displayName = formatBranchName(item.branch);
            
            const branchTd = `
                <td>
                    <span class="branch-tag">
                        <i data-lucide="${branchIcon}"></i>
                        ${displayName}
                    </span>
                </td>
            `;

            // Message and Author
            const commitMsg = item.commitMessage || "No commit message provided";
            const author = item.actor || "Workflow Bot";
            const messageTd = `
                <td>
                    <div class="commit-msg-container">
                        <div class="commit-msg" title="${commitMsg}">${commitMsg}</div>
                        <div class="commit-author">
                            ${author}
                        </div>
                    </div>
                </td>
            `;

            // Duration
            const durationVal = parseFloat(item.buildDuration || 0);
            const durationTd = `
                <td style="font-family: var(--font-code); font-weight: 500;">
                    ${durationVal > 0 ? durationVal.toFixed(2) + " min" : "N/A"}
                </td>
            `;

            // Footprint sizes
            const installerVal = parseFloat(item.installerSize || 0);
            const repoVal = parseFloat(item.repoSize || 0);
            const capacityTd = `
                <td class="details-cell">
                    <div class="details-line">Inst: <strong>${installerVal > 0 ? installerVal.toFixed(2) + " MB" : "N/A"}</strong></div>
                    <div class="details-line">Repo: <strong>${repoVal > 0 ? repoVal.toFixed(2) + " MB" : "N/A"}</strong></div>
                </td>
            `;

            // Action Reports buttons (Dynamic Markdown Viewer!)
            const dateParts = (item.date || "").split("-"); // yyyy-MM-dd
            let reportPathBase = "";
            if (dateParts.length === 3) {
                reportPathBase = `reports/${dateParts[0]}/${dateParts[1]}/${dateParts[2]}/${commitSha}-${item.runId}`;
            }

            let reportButtons = "";
            if (reportPathBase) {
                // AI Log Analysis
                reportButtons += `
                    <button class="btn-report btn-ai" onclick="viewReport('${reportPathBase}/ai_analysis.md', 'AI Log Analysis', 'primary', '${shortSha}')">
                        AI Analysis
                    </button>
                `;
                // Security Audit
                reportButtons += `
                    <button class="btn-report btn-security" onclick="viewReport('${reportPathBase}/security_audit_summary.md', 'Security Audit', 'success', '${shortSha}')">
                        Security
                    </button>
                `;
                // Capacity Governance
                reportButtons += `
                    <button class="btn-report btn-capacity" onclick="viewReport('${reportPathBase}/capacity_report.md', 'Capacity & Governance', 'success', '${shortSha}')">
                        Capacity
                    </button>
                `;
            } else {
                reportButtons = `<span style="color: var(--text-muted); font-size:11px;">No reports available</span>`;
            }

            const actionsTd = `
                <td>
                    <div class="report-actions">
                        ${reportButtons}
                    </div>
                </td>
            `;

            tr.innerHTML = statusTd + buildTd + branchTd + messageTd + durationTd + capacityTd + actionsTd;
            historyTableBody.appendChild(tr);
        });

        // Recreate lucide icons for newly appended elements
        lucide.createIcons();
    }

    // 6. Search and Filter telemetry records
    function filterAndRender() {
        let filtered = telemetryData;

        // Branch Filter
        if (activeBranch !== "all") {
            filtered = filtered.filter(item => item.branch === activeBranch);
        }

        // Status Filter
        if (activeStatus !== "all") {
            filtered = filtered.filter(item => {
                const isSuccess = item.buildStatus === "success" || item.buildStatus === "SUCCESS";
                return activeStatus === "success" ? isSuccess : !isSuccess;
            });
        }

        // Text Search
        if (searchQuery) {
            filtered = filtered.filter(item => {
                const sha = (item.commitSha || "").toLowerCase();
                const msg = (item.commitMessage || "").toLowerCase();
                const author = (item.actor || "").toLowerCase();
                const runId = (item.runId || "").toString();
                return sha.includes(searchQuery) || msg.includes(searchQuery) || author.includes(searchQuery) || runId.includes(searchQuery);
            });
        }

        // Dynamically update both metrics cards, charts, and table based on filtered subset
        updateDashboard(filtered);
    }

    // 7. Render Empty / Error states
    function showEmptyState() {
        historyTableBody.innerHTML = `
            <tr>
                <td colspan="7" class="loading-state">
                    <p style="color: var(--text-secondary); font-size: 14px;">No telemetry logs match the current filters.</p>
                </td>
            </tr>
        `;
    }

    function showErrorState(message) {
        historyTableBody.innerHTML = `
            <tr>
                <td colspan="7" class="loading-state">
                    <i data-lucide="alert-octagon" style="color: var(--color-danger); width:40px; height:40px; margin: 0 auto 16px;"></i>
                    <p style="color: var(--text-secondary); font-size: 14px;">${message}</p>
                </td>
            </tr>
        `;
        lucide.createIcons();
    }

    // Init App
    initDashboard();

    // 9. Client-side Real-time Telemetry: Background polling every 30 seconds (Cache-buster enabled)
    setInterval(async () => {
        try {
            const response = await fetch("history.json?t=" + Date.now());
            if (response.ok) {
                const newData = await response.json();
                const formattedData = Array.isArray(newData) ? newData : [newData];
                
                // Detect if a new build has completed or if database has changed
                const hasNewBuild = (formattedData.length !== telemetryData.length) || 
                                   (formattedData[0] && telemetryData[0] && formattedData[0].runId !== telemetryData[0].runId) ||
                                   (formattedData[0] && telemetryData[0] && formattedData[0].buildStatus !== telemetryData[0].buildStatus);
                
                if (hasNewBuild) {
                    telemetryData = formattedData;
                    
                    // Rebuild branch filter options to include any newly compiled branch
                    const currentBranchSelection = activeBranch;
                    branchFilter.innerHTML = '<option value="all">All Branches</option>';
                    populateFilters(telemetryData);
                    branchFilter.value = currentBranchSelection;
                    
                    // Apply current active filters and re-render dashboard dynamically
                    filterAndRender();
                    console.log("Live Telemetry: Telemetry data updated in real-time.");
                }
            }
        } catch (e) {
            console.warn("Live Telemetry background update failed:", e);
        }
    }, 30000); // 30 seconds interval


    // 8. Inject viewReport into window scope for table button actions
    window.viewReport = async function(filePath, title, badgeType, shortSha) {
        reportModal.classList.add("active");
        
        modalTitle.textContent = title;
        modalReportType.textContent = badgeType.toUpperCase();
        modalReportType.className = `modal-badge ${badgeType}`;
        modalFooterMeta.textContent = `Commit SHA: ${shortSha} | File: ${filePath.split("/").pop()}`;
        
        modalBodyContent.innerHTML = `
            <div class="spinner"></div>
            <p style="text-align: center; color: var(--text-secondary);">Fetching archived report logs...</p>
        `;
        
        try {
            const response = await fetch(filePath);
            if (!response.ok) {
                throw new Error(`Failed to load ${filePath} (File may not exist for this run)`);
            }
            const markdownText = await response.text();
            
            // Set marked.js options for security
            marked.setOptions({
                gfm: true,
                breaks: true,
                sanitize: false // Allow HTML inside markdown (since we control it)
            });
            
            modalBodyContent.innerHTML = marked.parse(markdownText);
        } catch (error) {
            modalBodyContent.innerHTML = `
                <div style="text-align: center; padding: 40px 0; color: var(--text-secondary);">
                    <i data-lucide="file-warning" style="width:48px;height:48px;color:var(--color-orange);margin-bottom:16px;"></i>
                    <p style="font-weight:600; margin-bottom:8px;">Report Unavailable</p>
                    <p style="font-size:13px;">${error.message}</p>
                </div>
            `;
            lucide.createIcons();
        }
    }
});

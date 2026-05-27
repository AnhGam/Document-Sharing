/**
 * DSM CI/CD Observability Dashboard Logic
 * Interactive telemetry, live metrics charts, server health monitoring, and GitOps report viewer.
 */

document.addEventListener("DOMContentLoaded", () => {
    let telemetryData = [];
    let activeBranch = "all";
    let activeStatus = "all";
    let searchQuery = "";
    
    // Chart References
    let durationChartInstance = null;
    let capacityChartInstance = null;

    // Server Monitor Config
    const SERVER_MONITOR_URL = "https://edgeparty.me/api/server-stats";
    const SERVER_POLL_INTERVAL = 10000; // 10 seconds
    let serverOnline = false;

    // DOM Elements
    const metricDoraRating = document.getElementById("metric-dora-rating");
    const metricDoraSub = document.getElementById("metric-dora-sub");
    const metricBuildDuration = document.getElementById("metric-build-duration");
    const metricDurationSub = document.getElementById("metric-duration-sub");
    const metricInstallerSize = document.getElementById("metric-installer-size");
    const metricInstallerSub = document.getElementById("metric-installer-sub");
    const metricRepoSize = document.getElementById("metric-repo-size");
    const metricRepoSub = document.getElementById("metric-repo-sub");
    const metricDeployFrequency = document.getElementById("metric-deploy-frequency");
    const metricDeployFrequencySub = document.getElementById("metric-deploy-frequency-sub");
    const metricChangeFailure = document.getElementById("metric-change-failure");
    const metricChangeFailureSub = document.getElementById("metric-change-failure-sub");
    
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

    // ═══════════════════════════════════════════════════════
    // 1. Fetch JSON database
    // ═══════════════════════════════════════════════════════
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

            if (metricDeployFrequency) {
                metricDeployFrequency.textContent = "-";
                metricDeployFrequency.className = "metric-value text-glowing-purple";
                metricDeployFrequencySub.textContent = "Tổng số: 0 lượt deploy";
            }
            if (metricChangeFailure) {
                metricChangeFailure.textContent = "-";
                metricChangeFailure.className = "metric-value text-glowing-orange";
                metricChangeFailureSub.textContent = "Ghi nhận: 0 lỗi / 0 builds";
            }

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
            const rawMsg = latestBuild.commitMessage || "No commit message provided";
            // Truncate very long messages for the banner
            const commitMsg = rawMsg.length > 80 ? rawMsg.substring(0, 77) + "..." : rawMsg;
            const author = latestBuild.actor || "Workflow Bot";
            const time = latestBuild.time || "";
            const date = latestBuild.date || "";
            const dateText = (date || time) ? ` (${date} ${time})` : "";
            contextCommitInfo.innerHTML = `<strong>#${latestBuild.runId || 'N/A'}</strong> - <span style="font-family: var(--font-code); color: var(--color-cyan);">${shortSha}</span> - <em>"${escapeHtml(commitMsg)}"</em> by <strong>${escapeHtml(author)}</strong>${dateText}`;
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
            const buildDur = parseFloat(latestSuccess.buildDuration || 0);
            const deployDur = parseFloat(latestSuccess.deployDuration || 0);
            const totalDur = buildDur + deployDur;
            
            let rating = "Elite";
            let ratingClass = "text-glowing-green";
            if (totalDur > 20) { rating = "High"; ratingClass = "text-glowing-cyan"; }
            if (totalDur > 60) { rating = "Medium"; ratingClass = "text-glowing-orange"; }
            if (totalDur > 240) { rating = "Low"; ratingClass = "text-glowing-danger"; }
            
            metricDoraRating.textContent = rating;
            metricDoraRating.className = `metric-value ${ratingClass}`;
            
            let subHtml = `${totalDur.toFixed(2)} min total lead time<br>`;
            subHtml += `<span style="font-size: 10px; opacity: 0.8; font-family: var(--font-code);">`;
            subHtml += `[Build: ${buildDur.toFixed(2)}m | Deploy: ${deployDur > 0 ? deployDur.toFixed(2) + 'm' : 'N/A'}]`;
            subHtml += `</span>`;
            metricDoraSub.innerHTML = subHtml;
        } else {
            metricDoraRating.textContent = "N/A";
            metricDoraRating.className = "metric-value text-glowing-danger";
            metricDoraSub.textContent = "No successful builds found";
        }

        // --- Average Build Duration ---
        const successfulBuilds = data.filter(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") && item.buildDuration);
        const avgDuration = successfulBuilds.reduce((sum, item) => sum + parseFloat(item.buildDuration), 0) / (successfulBuilds.length || 1);
        metricBuildDuration.textContent = avgDuration.toFixed(2);
        
        const latestBuildDur = parseFloat(latestBuild.buildDuration || 0).toFixed(2);
        const latestDeployDur = parseFloat(latestBuild.deployDuration || 0);
        const latestDeployText = latestDeployDur > 0 ? `${latestDeployDur.toFixed(2)} min` : "N/A";
        metricDurationSub.innerHTML = `Latest: ${latestBuildDur}m (Build) | ${latestDeployText} (Deploy)<br><span style="font-size:10px; opacity:0.8; font-family:var(--font-code);">[Average: ${avgDuration.toFixed(2)} min build time]</span>`;

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

        // --- Deployment Frequency ---
        if (metricDeployFrequency) {
            const successfulDeploys = data.filter(item => {
                const status = (item.buildStatus || "").toLowerCase();
                return status === "success" && parseFloat(item.deployDuration || 0) > 0;
            });
            const deployCount = successfulDeploys.length;
            
            let freqRating = "Low";
            let freqClass = "text-glowing-danger";
            if (deployCount >= 8) {
                freqRating = "Elite";
                freqClass = "text-glowing-green";
            } else if (deployCount >= 3) {
                freqRating = "High";
                freqClass = "text-glowing-cyan";
            } else if (deployCount >= 1) {
                freqRating = "Medium";
                freqClass = "text-glowing-orange";
            }
            
            metricDeployFrequency.textContent = freqRating;
            metricDeployFrequency.className = `metric-value ${freqClass}`;
            metricDeployFrequencySub.innerHTML = `Tổng số: <strong>${deployCount}</strong> lượt thành công<br><span style="font-size:9px; opacity:0.8; font-family:var(--font-code);">[Dựa trên lịch sử lưu trữ]</span>`;
        }

        // --- Change Failure Rate ---
        if (metricChangeFailure) {
            const failedRuns = data.filter(item => {
                const status = (item.buildStatus || "").toLowerCase();
                return status === "failure" || status === "error";
            }).length;
            const totalRuns = data.length || 1;
            const failureRate = (failedRuns / totalRuns) * 100;
            
            let failureRating = "Elite";
            let failureClass = "text-glowing-green";
            if (failureRate > 30) {
                failureRating = "Low";
                failureClass = "text-glowing-danger";
            } else if (failureRate > 15) {
                failureRating = "Medium";
                failureClass = "text-glowing-orange";
            } else if (failureRate > 5) {
                failureRating = "High";
                failureClass = "text-glowing-cyan";
            }
            
            metricChangeFailure.innerHTML = `${failureRate.toFixed(1)}<span class="unit">%</span>`;
            metricChangeFailure.className = `metric-value ${failureClass}`;
            metricChangeFailureSub.innerHTML = `Đánh giá: <strong>${failureRating}</strong> (${failedRuns} lỗi / ${totalRuns} runs)<br><span style="font-size:9px; opacity:0.8; font-family:var(--font-code);">[Tần suất phát sinh lỗi build/test]</span>`;
        }

        // --- Render Charts & Table ---
        renderCharts(data);
        renderTable(data);
        renderFailedBuilds(latestBuild);
    }

    const escapeHtml = (str) => {
        if (!str) return "";
        return str.toString().replace(/[&<>"']/g, (m) => ({
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        }[m]));
    };

    function renderFailedBuilds(build) {
        const aiSection = document.getElementById("ai-analysis-section");
        const aiContainer = document.getElementById("ai-analysis-container");
        const failCountBadge = document.getElementById("failed-build-count");

        if (!aiSection || !aiContainer || !failCountBadge) return;

        if (!build || (build.buildStatus || "").toLowerCase() !== "failure" && (build.buildStatus || "").toLowerCase() !== "error") {
            aiSection.style.display = "none";
            return;
        }

        aiSection.style.display = "block";
        failCountBadge.textContent = `1 Failure (Current)`;

        aiContainer.innerHTML = "";

        const rawShortSha = build.shortSha || (build.commitSha ? build.commitSha.substring(0, 7) : 'N/A');
        const shortSha = escapeHtml(rawShortSha);
        
        const rawBranch = build.branch || "";
        const branchEscaped = escapeHtml(formatBranchName(rawBranch));

        const durationEscaped = escapeHtml(build.buildDuration || "0");
        const commitMsgEscaped = escapeHtml(build.commitMessage || "N/A");

        const analysis = build.aiAnalysis || "AI analysis is pending or unavailable for this build failure.";
        const escapedAnalysis = escapeHtml(analysis);
        
        // Format analysis to convert markdown-like backticks to code tags if needed safely
        const formattedAnalysis = escapedAnalysis
            .replace(/`([^`]+)`/g, '<code style="background: rgba(255,255,255,0.1); padding: 2px 4px; border-radius: 4px; font-family: var(--font-code); font-size: 0.85em;">$1</code>')
            .replace(/\r?\n/g, '<br>');

        const card = document.createElement("div");
        card.className = "failure-card shadow-neon";
        card.style.border = "1px solid rgba(255, 60, 60, 0.3)";
        card.style.borderRadius = "8px";
        card.style.padding = "1rem";
        card.style.background = "var(--bg-card)";
        card.style.position = "relative";
        card.style.overflow = "hidden";

        card.innerHTML = `
            <div style="position: absolute; top: 0; left: 0; width: 4px; height: 100%; background: #ff1744; box-shadow: 0 0 10px #ff1744;"></div>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.5rem;">
                <h3 style="margin: 0; display: flex; align-items: center; gap: 0.5rem; color: #ff6b6b; font-size: 1.1rem;">
                    <i data-lucide="alert-triangle" style="width: 18px; height: 18px;"></i>
                    Build Failed: ${shortSha}
                </h3>
                <span style="font-size: 0.85rem; color: var(--text-secondary);">${escapeHtml(new Date(build.timestamp).toLocaleString())}</span>
            </div>
            <div style="font-size: 0.9rem; margin-bottom: 1rem; color: var(--text-secondary);">
                <strong>Branch:</strong> <span style="color:var(--text-primary);">${branchEscaped}</span> &nbsp;|&nbsp; 
                <strong>Duration:</strong> <span style="color:var(--text-primary);">${durationEscaped} min</span> &nbsp;|&nbsp;
                <strong>Message:</strong> <span style="color:var(--text-primary);">${commitMsgEscaped}</span>
            </div>
            <div style="background: rgba(0,0,0,0.3); padding: 1rem; border-radius: 6px; border-left: 3px solid #00f2fe;">
                <div style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem; color: #00f2fe; font-weight: bold; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 1px;">
                    <i data-lucide="sparkles" style="width: 14px; height: 14px;"></i> AI Diagnostic
                </div>
                <div style="color: var(--text-primary); line-height: 1.6; font-size: 0.95rem;">
                    ${formattedAnalysis}
                </div>
            </div>
        `;
        aiContainer.appendChild(card);
        
        if (window.lucide) {
            window.lucide.createIcons();
        }
    }

    function renderCharts(data) {
        const chronData = [...data].reverse();
        const labels = chronData.map(item => item.shortSha || (item.commitSha ? item.commitSha.substring(0, 7) : 'N/A'));
        const durations = chronData.map(item => parseFloat(item.buildDuration || 0));
        const deployDurations = chronData.map(item => parseFloat(item.deployDuration || 0));
        const pointColors = chronData.map(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") ? "#b927fc" : "#ff1744");
        const pointRadii = chronData.map(item => (item.buildStatus === "success" || item.buildStatus === "SUCCESS") ? 5 : 7);
        const installerSizes = chronData.map(item => parseFloat(item.installerSize || 0));
        const repoSizes = chronData.map(item => parseFloat(item.repoSize || 0));

        // -- Chart 1: Build Duration Line Chart --
        if (durationChartInstance) durationChartInstance.destroy();
        const ctx1 = document.getElementById("durationChart").getContext("2d");
        
        // Gradient fill for Build
        const purpleGrad = ctx1.createLinearGradient(0, 0, 0, 250);
        purpleGrad.addColorStop(0, "rgba(185, 39, 252, 0.25)");
        purpleGrad.addColorStop(1, "rgba(185, 39, 252, 0.00)");

        // Gradient fill for Deploy
        const cyanGrad = ctx1.createLinearGradient(0, 0, 0, 250);
        cyanGrad.addColorStop(0, "rgba(0, 242, 254, 0.25)");
        cyanGrad.addColorStop(1, "rgba(0, 242, 254, 0.00)");

        durationChartInstance = new Chart(ctx1, {
            type: "line",
            data: {
                labels: labels,
                datasets: [
                    {
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
                    },
                    {
                        label: "Deploy Duration (min)",
                        data: deployDurations,
                        borderColor: "#00f2fe",
                        borderWidth: 2,
                        pointBackgroundColor: "#00f2fe",
                        pointBorderColor: "#fff",
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        tension: 0.4,
                        fill: true,
                        backgroundColor: cyanGrad,
                        spanGaps: true
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
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
                        <a href="${buildUrl}" target="_blank" rel="noopener noreferrer" style="color: #fff; text-decoration: none;">Build #${item.runId || 'N/A'}</a>
                    </div>
                    <a href="${commitUrl}" target="_blank" rel="noopener noreferrer" class="commit-link">${shortSha}</a>
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
                        <div class="commit-msg" title="${escapeHtml(commitMsg)}">${escapeHtml(commitMsg)}</div>
                        <div class="commit-author">
                            ${escapeHtml(author)}
                        </div>
                    </div>
                </td>
            `;

            // Duration
            const durationVal = parseFloat(item.buildDuration || 0);
            const deployDurVal = parseFloat(item.deployDuration || 0);
            const durationTd = `
                <td class="details-cell" style="font-family: var(--font-code);">
                    <div class="details-line">Build: <strong>${durationVal > 0 ? durationVal.toFixed(2) + "m" : "N/A"}</strong></div>
                    <div class="details-line">Deploy: <strong>${deployDurVal > 0 ? deployDurVal.toFixed(2) + "m" : "N/A"}</strong></div>
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

    // ═══════════════════════════════════════════════════════
    // SERVER HEALTH MONITORING
    // ═══════════════════════════════════════════════════════

    function drawGauge(canvasId, percent, color) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext("2d");
        const w = canvas.width;
        const h = canvas.height;
        const cx = w / 2;
        const cy = h / 2;
        const radius = Math.min(cx, cy) - 10;
        const startAngle = 0.75 * Math.PI;
        const endAngle = 2.25 * Math.PI;
        const valueAngle = startAngle + (percent / 100) * (endAngle - startAngle);

        ctx.clearRect(0, 0, w, h);

        // Background arc
        ctx.beginPath();
        ctx.arc(cx, cy, radius, startAngle, endAngle);
        ctx.lineWidth = 8;
        ctx.strokeStyle = "rgba(255,255,255,0.06)";
        ctx.lineCap = "round";
        ctx.stroke();

        // Value arc
        if (percent > 0) {
            ctx.beginPath();
            ctx.arc(cx, cy, radius, startAngle, valueAngle);
            ctx.lineWidth = 8;
            ctx.strokeStyle = color;
            ctx.lineCap = "round";
            ctx.shadowColor = color;
            ctx.shadowBlur = 10;
            ctx.stroke();
            ctx.shadowBlur = 0;
        }
    }

    function getGaugeColor(percent) {
        if (percent < 50) return "#00e676";
        if (percent < 75) return "#ff9100";
        return "#ff1744";
    }

    async function pollServerHealth() {
        const badge = document.getElementById("server-status-badge");

        try {
            const res = await fetch(SERVER_MONITOR_URL, { signal: AbortSignal.timeout(8000) });
            if (!res.ok) throw new Error("Server responded with " + res.status);
            const stats = await res.json();
            serverOnline = true;

            // Status badge
            if (badge) {
                badge.innerHTML = '<span class="server-beacon online"></span> Online';
                badge.className = "badge server-status-badge online";
            }

            // CPU
            const cpuPct = stats.cpu?.usagePercent || 0;
            drawGauge("gauge-cpu", cpuPct, getGaugeColor(cpuPct));
            const cpuValEl = document.getElementById("gauge-cpu-value");
            if (cpuValEl) cpuValEl.textContent = cpuPct.toFixed(1) + "%";
            const cpuDetail = document.getElementById("cpu-detail");
            if (cpuDetail) cpuDetail.textContent = `${stats.cpu?.cores || "--"} cores | ${stats.cpu?.model || ""}`.substring(0, 40);

            // Memory
            const memPct = stats.memory?.usagePercent || 0;
            drawGauge("gauge-memory", memPct, getGaugeColor(memPct));
            const memValEl = document.getElementById("gauge-memory-value");
            if (memValEl) memValEl.textContent = memPct.toFixed(1) + "%";
            const memDetail = document.getElementById("memory-detail");
            if (memDetail) memDetail.textContent = `${stats.memory?.usedMB || "--"} / ${stats.memory?.totalMB || "--"} MB`;

            // Disk
            const diskPct = stats.disk?.usagePercent || 0;
            drawGauge("gauge-disk", diskPct, getGaugeColor(diskPct));
            const diskValEl = document.getElementById("gauge-disk-value");
            if (diskValEl) diskValEl.textContent = diskPct.toFixed(1) + "%";
            const diskDetail = document.getElementById("disk-detail");
            if (diskDetail) diskDetail.textContent = `${stats.disk?.usedGB || "--"} / ${stats.disk?.totalGB || "--"} GB`;

            // Network
            const netHostname = document.getElementById("net-hostname");
            if (netHostname) netHostname.textContent = stats.network?.hostname || "--";
            const netIp = document.getElementById("net-ip");
            if (netIp) {
                const iface = stats.network?.interfaces?.[0];
                netIp.textContent = iface?.address || "--";
            }
            const netOs = document.getElementById("net-os");
            if (netOs) netOs.textContent = `${stats.platform?.os || "--"} ${stats.platform?.arch || ""}`;

            // Uptime
            const uptimeText = document.getElementById("server-uptime-text");
            if (uptimeText) uptimeText.textContent = `Uptime: ${stats.uptime?.formatted || "--"}`;

            // Docker containers
            const dockerBody = document.getElementById("docker-table-body");
            const containerCount = document.getElementById("container-count");
            const containers = stats.docker || [];

            if (containerCount) containerCount.textContent = `${containers.length} container${containers.length !== 1 ? 's' : ''}`;

            if (dockerBody) {
                if (containers.length === 0) {
                    dockerBody.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--text-muted);padding:20px;">No Docker containers detected</td></tr>';
                } else {
                    dockerBody.innerHTML = containers.map(c => `
                        <tr>
                            <td>
                                <div style="display:flex;align-items:center;gap:8px;">
                                    <span class="container-dot running"></span>
                                    <strong>${escapeHtml(c.name)}</strong>
                                </div>
                            </td>
                            <td style="font-family:var(--font-code);color:var(--color-cyan);">${escapeHtml(c.cpuPercent)}</td>
                            <td style="font-family:var(--font-code);">${escapeHtml(c.memUsage)}</td>
                            <td style="font-family:var(--font-code);font-size:12px;">${escapeHtml(c.netIO)}</td>
                            <td style="font-family:var(--font-code);">${escapeHtml(c.pids)}</td>
                        </tr>
                    `).join("");
                }
            }

        } catch (err) {
            serverOnline = false;
            if (badge) {
                badge.innerHTML = '<span class="server-beacon offline"></span> Offline';
                badge.className = "badge server-status-badge offline";
            }
            console.warn("Server health poll failed:", err.message);
        }
    }

    // ═══════════════════════════════════════════════════════
    // INIT
    // ═══════════════════════════════════════════════════════

    // Init App
    initDashboard();

    // Start server health polling
    pollServerHealth();
    setInterval(pollServerHealth, SERVER_POLL_INTERVAL);

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

    // ═══════════════════════════════════════════════════════
    // EXTERNAL LINK HANDLER
    // ═══════════════════════════════════════════════════════
    window.handleExternalLink = function(el) {
        const url = el.href;
        // Try to open in new tab
        const newWindow = window.open(url, '_blank', 'noopener,noreferrer');
        if (!newWindow || newWindow.closed || typeof newWindow.closed === 'undefined') {
            // Popup blocked or failed — show a notification
            const toast = document.createElement("div");
            toast.className = "toast-notification";
            toast.innerHTML = `
                <i data-lucide="external-link" style="width:16px;height:16px;"></i>
                <span>Opening: <a href="${url}" target="_blank" rel="noopener noreferrer" style="color:var(--color-cyan);text-decoration:underline;">${url}</a></span>
            `;
            document.body.appendChild(toast);
            setTimeout(() => { toast.classList.add("show"); }, 10);
            setTimeout(() => {
                toast.classList.remove("show");
                setTimeout(() => toast.remove(), 300);
            }, 5000);
            lucide.createIcons();
        }
        return false; // prevent default navigation
    }
});

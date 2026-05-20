# publish-reports.ps1
# GitOps script to archive CI reports in a separate 'logs' branch and update the dashboard

param(
    [string]$RepoUrl,
    [string]$CommitSha,
    [string]$RunId,
    [string]$BuildStatus,
    [string]$InstallerSize = "0.0",
    [string]$RepoSize = "0.0",
    [string]$BuildDuration = "0.0",
    [string]$Branch = "main",
    [string]$Actor = "Workflow Bot",
    [string]$CommitMessage = ""
)

$ErrorActionPreference = "Continue" # Change to continue to manage errors manually

Write-Host "--- Initializing GitOps Reporting System ---"
Write-Host "Parameters Received:"
Write-Host " - Commit SHA: $CommitSha"
Write-Host " - Run ID: $RunId"
Write-Host " - Build Status: $BuildStatus"
Write-Host " - Installer Size: $InstallerSize MB"
Write-Host " - Repo Size: $RepoSize MB"
Write-Host " - Build Duration: $BuildDuration min"
Write-Host " - Branch: $Branch"
Write-Host " - Actor: $Actor"
Write-Host " - Commit Message: $CommitMessage"

# 1. Setup Identity (global so it applies to temp repos too)
git config --global user.name "github-actions[bot]"
git config --global user.email "github-actions[bot]@users.noreply.github.com"

# 2. Preparation
$logsDir = "logs-branch-temp"
if (Test-Path $logsDir) { Remove-Item -Recurse -Force $logsDir }

# 3. Clone or Initialize 'logs' branch
Write-Host "Checking for existing 'logs' branch..."
git clone --branch logs --depth 1 $RepoUrl $logsDir 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Branch 'logs' not found in remote. Creating a new orphan branch locally..."
    mkdir $logsDir -ErrorAction SilentlyContinue
    cd $logsDir
    git init
    git checkout --orphan logs
    " # Build History Dashboard`n`nArchived build reports and DORA telemetry." | Out-File README.md
    git add README.md
    git commit -m "Initialize logs branch"
    git remote add origin $RepoUrl
    cd ..
}

# 4. Organize Reports
$dateStr = Get-Date -Format "yyyy/MM/dd"
$timeStr = Get-Date -Format "HH:mm"
$targetPath = "$logsDir/reports/$dateStr/$CommitSha-$RunId"
if (-not (Test-Path $targetPath)) { New-Item -ItemType Directory -Path $targetPath -Force }

# Copy all available reports
$reportFiles = @("ai_analysis.md", "security_audit_summary.md", "pr_review_ai.md", "capacity_report.md")
foreach ($file in $reportFiles) {
    if (Test-Path $file) {
        Copy-Item $file -Destination "$targetPath/$file"
        Write-Host "Copied $file to reports archive."
    } else {
        Write-Host "Report $file not found - skipping copy."
    }
}

# 5. Copy Dashboard Web Assets into logs root
Write-Host "Deploying dashboard web assets..."
$dashboardAssets = @("dashboard/index.html", "dashboard/style.css", "dashboard/app.js")
foreach ($asset in $dashboardAssets) {
    if (Test-Path $asset) {
        Copy-Item $asset -Destination "$logsDir/" -Force
        Write-Host "Deployed: $asset"
    } else {
        Write-Host "WARNING: Asset $asset not found in main workspace."
    }
}

# 6. Update JSON Database (history.json)
cd $logsDir
$historyFile = "history.json"
$history = @()

if (Test-Path $historyFile) {
    try {
        $history = Get-Content $historyFile -Raw | ConvertFrom-Json
        # If history is a single object, wrap it in an array
        if ($history -isnot [array]) {
            $history = @($history)
        }
        Write-Host "Successfully loaded existing history.json ($( $history.Count ) entries)."
    } catch {
        Write-Host "WARNING: Failed to parse history.json, resetting database..."
        $history = @()
    }
}

# Ensure numeric values are numbers
$cDuration = 0.0
if (![string]::IsNullOrWhiteSpace($BuildDuration) -and ($BuildDuration -as [double])) { $cDuration = [double]$BuildDuration }
$cInstaller = 0.0
if (![string]::IsNullOrWhiteSpace($InstallerSize) -and ($InstallerSize -as [double])) { $cInstaller = [double]$InstallerSize }
$cRepo = 0.0
if (![string]::IsNullOrWhiteSpace($RepoSize) -and ($RepoSize -as [double])) { $cRepo = [double]$RepoSize }

$shortSha = $CommitSha.Substring(0, [Math]::Min(7, $CommitSha.Length))

# Create the new database record
$newRecord = [PSCustomObject]@{
    commitSha     = $CommitSha
    shortSha      = $shortSha
    runId         = $RunId
    date          = Get-Date -Format 'yyyy-MM-dd'
    time          = $timeStr
    buildStatus   = $BuildStatus
    installerSize = $cInstaller
    repoSize      = $cRepo
    buildDuration = $cDuration
    branch        = $Branch
    actor         = $Actor
    commitMessage = $CommitMessage
}

# Prepend new entry so latest is always first [0]
$history = @($newRecord) + $history

# Cap history size at 100 entries to optimize performance
if ($history.Count -gt 100) {
    $history = $history[0..99]
}

# Write back to history.json
$history | ConvertTo-Json -Depth 5 | Out-File $historyFile -Encoding utf8
Write-Host "Updated history.json with new entry."

# 7. Maintain Markdown README.md for standard GitOps view
$dashboardFile = "README.md"
if (-not (Test-Path $dashboardFile)) {
    " # Build History Dashboard" | Out-File $dashboardFile
}

$content = Get-Content $dashboardFile | Out-String
if (-not ($content -match "\| Date \| Commit \| Status \|")) {
    $header = "`n## Build History Table`n`n| Date | Time | Commit | Status | Reports |`n| :--- | :--- | :--- | :--- | :--- |"
    $content = $content + $header
}

$reportsLink = " [View Reports](./reports/$dateStr/$CommitSha-$RunId/)"
$newEntry = "`n| $(Get-Date -Format 'yyyy-MM-dd') | $timeStr | $shortSha | $BuildStatus | $reportsLink |"
$content = $content + $newEntry

$content | Out-File $dashboardFile -Encoding utf8

# 8. Commit and Push back to 'logs' branch
Write-Host "Committing and pushing telemetry reports and dashboard updates..."
git add .
git commit -m "Archive reports and telemetry for commit $shortSha [Run: $RunId]"
git push origin logs

cd ..
Write-Host "--- GitOps Reporting Success! ---"

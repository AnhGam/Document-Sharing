# publish-reports.ps1
# GitOps script to archive CI reports in a separate 'logs' branch and update the dashboard

param(
    [string]$RepoUrl,
    [string]$CommitSha,
    [string]$RunId,
    [string]$BuildStatus
)

$ErrorActionPreference = "Continue" # Change to continue to manage errors manually

Write-Host "--- Initializing GitOps Reporting System ---"

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
    " # Build History Dashboard`n`nArchived build reports and AI analyses." | Out-File README.md
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

$reportFiles = @("ai_analysis.md", "security_audit_summary.md", "pr_review_ai.md")
foreach ($file in $reportFiles) {
    if (Test-Path $file) {
        Copy-Item $file -Destination "$targetPath/$file"
    }
}

# 5. Update Dashboard
cd $logsDir
$dashboardFile = "README.md"
if (-not (Test-Path $dashboardFile)) {
    " # Build History Dashboard" | Out-File $dashboardFile
}

$content = Get-Content $dashboardFile | Out-String
if (-not ($content -match "\| Date \| Commit \| Status \|")) {
    $header = "`n## Build History Table`n`n| Date | Time | Commit | Status | Reports |`n| :--- | :--- | :--- | :--- | :--- |"
    $content = $content + $header
}

$shortSha = $CommitSha.Substring(0, 7)
$reportsLink = " [View Reports](./reports/$dateStr/$CommitSha-$RunId/)"
$newEntry = "`n| $(Get-Date -Format 'yyyy-MM-dd') | $timeStr | $shortSha | $BuildStatus | $reportsLink |"
$content = $content + $newEntry

$content | Out-File $dashboardFile -Encoding utf8

# 6. Push
git add .
git commit -m "Archive reports for commit $shortSha [Run: $RunId]"
git push origin logs

cd ..
Write-Host "--- GitOps Reporting Success! ---"

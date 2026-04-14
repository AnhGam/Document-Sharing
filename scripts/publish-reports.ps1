# publish-reports.ps1
# GitOps script to archive CI reports in a separate 'logs' branch and update the dashboard

param(
    [string]$RepoUrl,
    [string]$CommitSha,
    [string]$RunId,
    [string]$BuildStatus
)

$ErrorActionPreference = "Stop"

Write-Host "--- Initializing GitOps Reporting System ---"

# 1. Setup Identity
git config user.name "github-actions[bot]"
git config user.email "github-actions[bot]@users.noreply.github.com"

# 2. Preparation: Create a temporary directory for the logs branch
$logsDir = "logs-branch-temp"
if (Test-Path $logsDir) { Remove-Item -Recurse -Force $logsDir }

# 3. Clone only the 'logs' branch (or create it if it doesn't exist)
try {
    Write-Host "Attempting to clone 'logs' branch..."
    git clone --branch logs --depth 1 $RepoUrl $logsDir
} catch {
    Write-Host "Branch 'logs' not found. Initializing new orphan branch..."
    mkdir $logsDir
    cd $logsDir
    git init
    git checkout --orphan logs
    " # Build History Dashboard" | Out-File README.md
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
$movedCount = 0

foreach ($file in $reportFiles) {
    if (Test-Path $file) {
        Copy-Item $file -Destination "$targetPath/$file"
        $movedCount++
    }
}

Write-Host "Archived $movedCount reports to $targetPath"

# 5. Update Dashboard (README.md in logs branch)
cd $logsDir
$dashboardFile = "README.md"
$content = Get-Content $dashboardFile | Out-String

if (-not ($content -match "\| Date \| Commit \| Status \|")) {
    $header = "`n## Build History Dashboard`n`n| Date | Time | Commit | Status | Reports |`n| :--- | :--- | :--- | :--- | :--- |"
    $content = $content + $header
}

$shortSha = $CommitSha.Substring(0, 7)
$reportsLink = " [View Reports](./reports/$dateStr/$CommitSha-$RunId/)"
$newEntry = "`n| $(Get-Date -Format 'yyyy-MM-dd') | $timeStr | ``$shortSha`` | $BuildStatus | $reportsLink |"
$content = $content + $newEntry

$content | Out-File $dashboardFile -Encoding utf8

# 6. Commit and Push
git add .
git commit -m "Archive reports for commit $shortSha [Run: $RunId]"
git push origin logs

cd ..
Write-Host "--- GitOps Reporting Success! ---"

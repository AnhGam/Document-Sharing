# capacity-governance.ps1
# Enterprise Capacity Check and FinOps Governance Script

param(
    [int]$MaxInstallerSizeMB = 50,
    [int]$MaxRepoSizeMB = 500,
    [string]$InstallerDir = "installer"
)

Write-Host "--- Starting Capacity and Governance Check ---"

# 1. Check Installer Size
$installerFile = Get-ChildItem -Path $InstallerDir -Filter *.exe | Select-Object -First 1
if ($installerFile) {
    $sizeMB = [Math]::Round($installerFile.Length / 1MB, 2)
    Write-Host "Installer: $($installerFile.Name)"
    Write-Host "Size: $sizeMB MB"
    
    if ($sizeMB -gt $MaxInstallerSizeMB) {
        Write-Host "ERROR: Installer size ($sizeMB MB) exceeds limit ($MaxInstallerSizeMB MB)!" -ForegroundColor Red
        $installerFail = $true
    } else {
        Write-Host "SUCCESS: Installer size is within limits." -ForegroundColor Green
    }
} else {
    Write-Host "WARNING: No installer found in $InstallerDir." -ForegroundColor Yellow
}

# 2. Check Repository Size (excluding .git)
$repoFiles = Get-ChildItem -Path "." -Recurse -File | Where-Object { $_.FullName -notmatch "\\\.git\\" }
$totalSize = ($repoFiles | Measure-Object -Property Length -Sum).Sum
$totalSizeMB = [Math]::Round($totalSize / 1MB, 2)

Write-Host "Total Repository Size (Source): $totalSizeMB MB"
if ($totalSizeMB -gt $MaxRepoSizeMB) {
    Write-Host "ERROR: Repository size ($totalSizeMB MB) exceeds limit ($MaxRepoSizeMB MB)!" -ForegroundColor Red
    $repoFail = $true
} else {
    Write-Host "SUCCESS: Repository size is within limits." -ForegroundColor Green
}

# 3. FinOps Report for Workflow Summary
$report = @"
### 📊 Capacity & FinOps Report
| Metric | Value | Limit | Status |
| :--- | :--- | :--- | :--- |
| **Installer Size** | $sizeMB MB | $MaxInstallerSizeMB MB | $(if($installerFail){"❌ FAIL"}else{"✅ PASS"}) |
| **Repo Source Size** | $totalSizeMB MB | $MaxRepoSizeMB MB | $(if($repoFail){"❌ FAIL"}else{"✅ PASS"}) |

*Recommendation: Maintain small binary footprints to reduce distribution costs and improve CI speed.*
"@

$report | Out-File "capacity_report.md" -Encoding utf8

if ($installerFail -or $repoFail) {
    exit 1
}

exit 0

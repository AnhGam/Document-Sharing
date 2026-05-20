# scripts/smoke-test-installer.ps1
# Automated Silent Installer Smoke Testing for GHA Windows Runner

param(
    [string]$Version = "3.1.0"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Starting Installer Smoke Test for Version $Version ==="

$installerPath = "installer/DocumentSharingManager_v${Version}_Setup.exe"
if (-not (Test-Path $installerPath)) {
    Write-Error "Installer not found at $installerPath!"
    exit 1
}

$installDir = Join-Path (Get-Location) "temp-install-test"
if (Test-Path $installDir) { Remove-Item -Recurse -Force $installDir }
New-Item -ItemType Directory -Path $installDir -Force | Out-Null

Write-Host "1. Running installer silently into $installDir..."
# Inno Setup switches:
# /VERYSILENT: Install without showing wizard or progress window
# /SUPPRESSMSGBOXES: Answer all message boxes with default answer
# /NORESTART: Prevent system reboot
# /DIR: Override the installation directory
$process = Start-Process -FilePath $installerPath -ArgumentList "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /DIR=""$installDir""" -Wait -NoNewWindow -PassThru

if ($process.ExitCode -ne 0) {
    Write-Error "Installer failed with exit code $($process.ExitCode)"
    exit 1
}
Write-Host "Installer completed successfully."

# 2. Verify files are present
$exePath = Join-Path $installDir "document-sharing-manager.exe"
if (-not (Test-Path $exePath)) {
    Write-Error "Verification FAILED: document-sharing-manager.exe was not created in $installDir!"
    exit 1
}
Write-Host "Verification SUCCESS: Application executable found at $exePath."

# 3. Launch the app to verify it boots without immediate crash
Write-Host "2. Launching application process for sanity test..."
$appProc = Start-Process -FilePath $exePath -PassThru -NoNewWindow

# Wait 5 seconds to ensure it doesn't crash on boot (e.g. missing dlls or config errors)
Start-Sleep -Seconds 5

if ($appProc.HasExited) {
    $exitCode = $appProc.ExitCode
    Write-Error "App process crashed on startup! Exit code: $exitCode"
    exit 1
}

Write-Host "App process successfully launched and is running (PID: $($appProc.Id))."

# Terminate process
Write-Host "Stopping app process..."
Stop-Process -Id $appProc.Id -Force
Write-Host "App process stopped."

# 4. Run silent uninstaller to ensure cleanup is functional
$uninstaller = Join-Path $installDir "unins000.exe"
if (Test-Path $uninstaller) {
    Write-Host "3. Running uninstaller silently..."
    $unProc = Start-Process -FilePath $uninstaller -ArgumentList "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART" -Wait -NoNewWindow -PassThru
    Write-Host "Uninstaller finished with exit code $($unProc.ExitCode)."
} else {
    Write-Warning "Uninstaller unins000.exe not found. Skipping silent uninstall check."
}

# Clean up temporary directory
Write-Host "4. Cleaning up temporary installation directory..."
Start-Sleep -Seconds 2 # Give OS a moment to release file handles
if (Test-Path $installDir) {
    Remove-Item -Recurse -Force $installDir -ErrorAction SilentlyContinue
}

Write-Host "=== SMOKE TEST COMPLETED SUCCESSFULLY! ==="
exit 0

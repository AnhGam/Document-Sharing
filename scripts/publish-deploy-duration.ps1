# publish-deploy-duration.ps1
# GitOps script to update deploy duration in the 'logs' branch telemetry data

param(
    [string]$RepoUrl,
    [string]$RunId,
    [double]$DeployDuration
)

$ErrorActionPreference = "Continue"

Write-Host "--- GitOps: Updating Deploy Duration ---"
Write-Host "Parameters Received:"
Write-Host " - Run ID: $RunId"
Write-Host " - Deploy Duration: $DeployDuration min"

if ([string]::IsNullOrWhiteSpace($RepoUrl) -or [string]::IsNullOrWhiteSpace($RunId)) {
    Write-Host "ERROR: Missing required parameters RepoUrl or RunId."
    exit 1
}

# 1. Setup Identity
git config --global user.name "github-actions[bot]"
git config --global user.email "github-actions[bot]@users.noreply.github.com"

# 2. Setup temp directory
$logsDir = "logs-branch-temp-deploy"
if (Test-Path $logsDir) { Remove-Item -Recurse -Force $logsDir }

# 3. Clone the 'logs' branch
Write-Host "Cloning 'logs' branch..."
git clone --branch logs --depth 1 $RepoUrl $logsDir 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to clone 'logs' branch."
    exit 1
}

# 4. Update the JSON Database (history.json)
Set-Location $logsDir
$historyFile = "history.json"

if (Test-Path $historyFile) {
    try {
        $history = Get-Content $historyFile -Raw | ConvertFrom-Json
        # If history is a single object, wrap it in an array
        if ($history -isnot [array]) {
            $history = @($history)
        }
        Write-Host "Successfully loaded history.json ($( $history.Count ) entries)."
        
        $updated = $false
        foreach ($record in $history) {
            # Match runId as string or int to be safe
            if ($record.runId -eq $RunId -or "$($record.runId)" -eq "$RunId") {
                $record.deployDuration = $DeployDuration
                $updated = $true
                Write-Host "Updated record for Run ID $RunId with deploy duration $DeployDuration min."
                break
            }
        }

        if ($updated) {
            # Save back to history.json
            $history | ConvertTo-Json -Depth 5 | Out-File $historyFile -Encoding utf8
            Write-Host "Saved updated history.json."
            
            # Commit and push
            git add $historyFile
            git commit -m "Update deploy duration for Run $RunId [Skip CI]"
            git push origin logs
            Write-Host "Deploy duration successfully pushed to logs branch!"
        } else {
            Write-Host "WARNING: No entry found in history.json for Run ID $RunId. Deploy duration could not be updated."
        }
    } catch {
        Write-Host "ERROR: Failed to parse or update history.json: $_"
    }
} else {
    Write-Host "ERROR: history.json not found in logs branch."
}

Set-Location ..
if (Test-Path $logsDir) { Remove-Item -Recurse -Force $logsDir }
Write-Host "--- GitOps Update Deploy Duration Completed ---"

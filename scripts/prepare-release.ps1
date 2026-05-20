# prepare-release.ps1
# Automated version updater for AssemblyInfo.cs and setup.iss based on CI/CD build number

param(
    [string]$BuildNumber = "0"
)

$baseVersion = "3.1"
$fullVersion = "$baseVersion.$BuildNumber"
Write-Host "Configuring build for Version: $fullVersion"

# 1. Update AssemblyInfo.cs
$assemblyInfoPath = "document-sharing-manager/Properties/AssemblyInfo.cs"
if (Test-Path $assemblyInfoPath) {
    $content = Get-Content $assemblyInfoPath -Raw
    $content = $content -replace '\[assembly: AssemblyVersion\("[^"]+"\)]', "[assembly: AssemblyVersion(""$fullVersion.0"")]"
    $content = $content -replace '\[assembly: AssemblyFileVersion\("[^"]+"\)]', "[assembly: AssemblyFileVersion(""$fullVersion.0"")]"
    $content | Out-File $assemblyInfoPath -Encoding utf8 -Force
    Write-Host "Updated AssemblyInfo.cs to version $fullVersion.0"
} else {
    Write-Warning "AssemblyInfo.cs not found at $assemblyInfoPath"
}

# 2. Update setup.iss
$setupIssPath = "setup.iss"
if (Test-Path $setupIssPath) {
    $content = Get-Content $setupIssPath -Raw
    $content = $content -replace '#define MyAppVersion "[^"]+"', "#define MyAppVersion ""$fullVersion"""
    $content | Out-File $setupIssPath -Encoding utf8 -Force
    Write-Host "Updated setup.iss to version $fullVersion"
} else {
    Write-Warning "setup.iss not found at $setupIssPath"
}

# Output the version for GitHub Actions
if ($env:GITHUB_OUTPUT) {
    "version=$fullVersion" | Out-File -FilePath $env:GITHUB_OUTPUT -Append -Encoding utf8
}

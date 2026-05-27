# ai-log-analyzer-groq.ps1
# AI-powered build log analysis using Groq (Llama 3)
# Generates ai_analysis.md with insights about the build

param(
    [string]$ApiKey,
    [string]$BuildStatus = "unknown",
    [string]$CommitSha = "",
    [string]$Branch = "main",
    [string]$CommitMessage = ""
)

# Helper: strip non-ASCII characters
function Remove-NonAscii {
    param([string]$Text)
    $Text -replace '[^\x20-\x7E\r\n]', ''
}

if (-not $ApiKey) {
    Write-Host "WARNING: GROQ_API_KEY is missing. Generating local-only build analysis."
    $localReport = @"
## AI Build Analysis

**Status:** $BuildStatus

AI analysis was skipped because the GROQ_API_KEY secret is not configured.
Configure it in your repository settings to enable AI-powered build insights.

---
*Generated at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') | AI: Skipped*
"@
    $localReport | Out-File "ai_analysis.md" -Encoding utf8
    exit 0
}

Write-Host "--- AI Build Log Analysis (Groq Llama 3) ---"

# Capture actual git code changes details
$gitChanges = "No git diff statistics available."
if ($CommitSha) {
    try {
        $gitChanges = (git show --stat --oneline $CommitSha) -join "`n"
        Write-Host "Successfully captured git show statistics for commit: $CommitSha"
    } catch {
        Write-Host "WARNING: Failed to capture git show stat: $_"
    }
}
if ([string]::IsNullOrWhiteSpace($gitChanges) -or $gitChanges -eq "No git diff statistics available.") {
    try {
        $gitChanges = (git show --stat --oneline HEAD) -join "`n"
        Write-Host "Successfully captured git show statistics for HEAD as fallback."
    } catch {}
}

# Collect build context
$buildLogs = ""

# 1. Capture build output if available
$logSources = @()

# Test results
if (Test-Path "document-sharing-manager.Tests/TestResults/*.trx") {
    $trxFile = Get-ChildItem "document-sharing-manager.Tests/TestResults/*.trx" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($trxFile) {
        $trxContent = Get-Content $trxFile.FullName -Raw -ErrorAction SilentlyContinue
        # Extract summary from TRX XML
        if ($trxContent -match 'outcome="([^"]+)"') {
            $logSources += "Test Outcome: $($Matches[1])"
        }
        if ($trxContent -match 'total="(\d+)".*passed="(\d+)".*failed="(\d+)"') {
            $logSources += "Tests: Total=$($Matches[1]) Passed=$($Matches[2]) Failed=$($Matches[3])"
        }
    }
}

# Audit log
if (Test-Path "audit_log.txt") {
    $auditContent = (Get-Content "audit_log.txt" -Raw -ErrorAction SilentlyContinue)
    if ($auditContent) {
        $logSources += "--- NuGet Audit ---`n$auditContent"
    }
}

# Capacity report
if (Test-Path "capacity_report.md") {
    $capacityContent = (Get-Content "capacity_report.md" -Raw -ErrorAction SilentlyContinue)
    if ($capacityContent) {
        $logSources += "--- Capacity Report ---`n$capacityContent"
    }
}

# Security audit
if (Test-Path "security_audit_summary.md") {
    $secContent = (Get-Content "security_audit_summary.md" -Raw -ErrorAction SilentlyContinue)
    if ($secContent) {
        $logSources += "--- Security Audit ---`n$secContent"
    }
}

$buildLogs = $logSources -join "`n`n"

# Truncate to avoid token limits
if ($buildLogs.Length -gt 4000) {
    $buildLogs = $buildLogs.Substring(0, 4000) + "`n... (truncated)"
}

# If no logs collected at all, still produce a useful analysis
if ([string]::IsNullOrWhiteSpace($buildLogs)) {
    $buildLogs = "No detailed build logs were captured for this run."
}

$shortSha = if ($CommitSha.Length -ge 7) { $CommitSha.Substring(0, 7) } else { $CommitSha }

$prompt = @"
You are a DevOps/CI/Software Engineering expert. Analyze this CI/CD build for a .NET WinForms project called "Document Sharing Manager".

COMMIT DETAILS & CODE CHANGES (GIT STAT):
$gitChanges

BUILD CONTEXT:
- Commit: $shortSha
- Branch: $Branch
- Status: $BuildStatus
- Message: $CommitMessage
- Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

BUILD LOGS AND REPORTS:
$buildLogs

Based on the actual code files that were added, modified, or deleted in this commit (shown in the GIT STAT above) and the build logs:
- Explain what functional or structural changes this commit introduced to the codebase.
- Provide a highly specific analysis of this commit rather than a generic template.
- Identify any potential risks, impact, or recommendations specific to the modified code files (e.g. database, security, UI, etc.).

Provide a structured analysis with these sections:
1. **Build Summary** - One-line verdict explaining what this build achieved or why it failed, referencing the actual feature implemented or bug fixed.
2. **Key Findings** - Technical analysis of the specific code changes and reports (use bullet points, link to the modified areas if applicable).
3. **Test Results** - Summary of test execution if available.
4. **Security Posture** - Assessment based on audit data.
5. **Performance & Capacity** - Build duration, footprint impact (installer/repo size changes).
6. **Recommendations** - 3 actionable next steps specific to the code changed in this commit.

FORMAT RULES:
- Use plain ASCII text only. No emoji, no unicode symbols, no special characters.
- Use proper markdown formatting (headers ##, tables, bold **text**, code blocks).
- Keep total response under 500 words.
"@

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "user"; content = $prompt }
    )
    temperature = 0.4
    max_tokens = 1200
} | ConvertTo-Json -Depth 5

try {
    $response = Invoke-RestMethod -Uri "https://api.groq.com/openai/v1/chat/completions" `
        -Method Post -Headers @{ Authorization = "Bearer $ApiKey" } `
        -Body $body -ContentType "application/json"
    
    $analysis = $response.choices[0].message.content
    
    # Strip non-ASCII
    $analysis = Remove-NonAscii -Text $analysis
    
    Write-Host "`nAI BUILD ANALYSIS:`n"
    Write-Host $analysis
    
    $finalReport = @"
## AI Build Analysis - Commit $shortSha

**Branch:** $Branch | **Status:** $BuildStatus | **Date:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

---

$analysis

---
*Analysis by Groq AI (Llama 3.3) | Commit: $shortSha | Run: $(Get-Date -Format 'yyyyMMdd-HHmmss')*
"@
    
    $finalReport | Out-File "ai_analysis.md" -Encoding utf8
    Write-Host "[+] AI analysis report generated: ai_analysis.md"
    
} catch {
    Write-Host "WARNING: Groq API call failed: $_"
    
    # Generate fallback report
    $fallbackReport = @"
## AI Build Analysis - Commit $shortSha

**Branch:** $Branch | **Status:** $BuildStatus | **Date:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

---

### Build Summary
Build completed with status: **$BuildStatus**

### Notes
AI analysis could not be performed due to an API connectivity issue.
The build artifacts and test results should be reviewed manually.

### Available Data
``````
$buildLogs
``````

---
*Fallback Report (AI unavailable) | Commit: $shortSha*
"@
    $fallbackReport | Out-File "ai_analysis.md" -Encoding utf8
    Write-Host "[*] Fallback analysis report generated: ai_analysis.md"
}

exit 0

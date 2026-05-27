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
$gitDiff = "No git patch diff available."
$gitHistory = "No recent git commit logs available."

if ($CommitSha) {
    try {
        $gitChanges = (git show --stat --oneline $CommitSha) -join "`n"
        $gitDiff = (git show --oneline -p $CommitSha) -join "`n"
        Write-Host "Successfully captured git show statistics and diff for commit: $CommitSha"
    } catch {
        Write-Host "WARNING: Failed to capture git show data: $_"
    }
}
if ([string]::IsNullOrWhiteSpace($gitChanges) -or $gitChanges -eq "No git diff statistics available.") {
    try {
        $gitChanges = (git show --stat --oneline HEAD) -join "`n"
        $gitDiff = (git show --oneline -p HEAD) -join "`n"
        Write-Host "Successfully captured git show statistics and diff for HEAD as fallback."
    } catch {}
}

# Capture recent commit history (last 5 commits)
try {
    $gitHistory = (git log -n 5 --oneline) -join "`n"
} catch {}

# Capture full contents of modified source files for deep context
$modifiedFilesContent = ""
try {
    $modifiedFiles = @()
    if ($CommitSha) {
        $modifiedFiles = (git show --pretty="" --name-only $CommitSha)
    }
    if ($modifiedFiles.Count -eq 0 -or -not $modifiedFiles) {
        $modifiedFiles = (git show --pretty="" --name-only HEAD)
    }
    
    $textExtensions = @(".cs", ".js", ".html", ".css", ".yml", ".json", ".sql", ".txt", ".md", ".ini", ".xml", ".ps1")
    foreach ($file in $modifiedFiles) {
        $trimmedFile = $file.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmedFile)) { continue }
        if (Test-Path $trimmedFile) {
            $ext = [System.IO.Path]::GetExtension($trimmedFile).ToLower()
            if ($textExtensions -contains $ext) {
                $size = (Get-Item $trimmedFile).Length
                if ($size -lt 100000) {
                    $fileContent = Get-Content $trimmedFile -Raw -ErrorAction SilentlyContinue
                    if ($fileContent) {
                        $modifiedFilesContent += "=== FULL FILE CONTENT: $trimmedFile ===`n$fileContent`n`n"
                        Write-Host "Captured full file content: $trimmedFile"
                    }
                }
            }
        }
    }
} catch {
    Write-Host "WARNING: Failed to capture modified files content: $_"
}

if ([string]::IsNullOrWhiteSpace($modifiedFilesContent)) {
    $modifiedFilesContent = "No full source file contents available."
}

# Truncate gitDiff to avoid too large payload (max 15,000 chars)
if ($gitDiff.Length -gt 15000) {
    $gitDiff = $gitDiff.Substring(0, 15000) + "`n... (diff truncated to fit context limits)"
}

# Collect build context
$buildLogs = ""

# 1. Capture build output if available
$logSources = @()

# Build logs
if (Test-Path "build_output.txt") {
    $buildOutput = Get-Content "build_output.txt" -Raw -ErrorAction SilentlyContinue
    if ($buildOutput) {
        if ($buildOutput.Length -gt 8000) {
            $buildOutput = $buildOutput.Substring(0, 4000) + "`n`n... [TRUNCATED MIDDLE MSBUILD LOGS] ...`n`n" + $buildOutput.Substring($buildOutput.Length - 4000)
        }
        $logSources += "--- MSBuild Compile Console Log ---`n$buildOutput"
    }
}

# Test console logs
if (Test-Path "test_output.txt") {
    $testOutput = Get-Content "test_output.txt" -Raw -ErrorAction SilentlyContinue
    if ($testOutput) {
        if ($testOutput.Length -gt 8000) {
            $testOutput = $testOutput.Substring(0, 4000) + "`n`n... [TRUNCATED MIDDLE TEST LOGS] ...`n`n" + $testOutput.Substring($testOutput.Length - 4000)
        }
        $logSources += "--- NUnit Test Console Log ---`n$testOutput"
    }
}

# Test results (TRX metrics)
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

# Truncate to avoid token limits (allowing up to 35,000 characters for rich logs)
if ($buildLogs.Length -gt 35000) {
    $buildLogs = $buildLogs.Substring(0, 35000) + "`n... (build logs truncated to fit context limits)"
}

# If no logs collected at all, still produce a useful analysis
if ([string]::IsNullOrWhiteSpace($buildLogs)) {
    $buildLogs = "No detailed build logs were captured for this run."
}

$shortSha = if ($CommitSha.Length -ge 7) { $CommitSha.Substring(0, 7) } else { $CommitSha }

$prompt = @"
You are a DevOps/CI/Software Engineering expert. Analyze this CI/CD build for a .NET WinForms project called "Document Sharing Manager".

RECENT GIT COMMIT HISTORY:
$gitHistory

COMMIT DETAILS & CODE CHANGES (GIT STAT):
$gitChanges

FULL CODE DIFF (GIT PATCH):
$gitDiff

ACTUAL FULL CONTENT OF MODIFIED FILES:
$modifiedFilesContent

BUILD CONTEXT:
- Commit: $shortSha
- Branch: $Branch
- Status: $BuildStatus
- Message: $CommitMessage
- Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

BUILD LOGS, COMPILER OUTPUT, TEST RUNS AND REPORTS:
$buildLogs

Based on the actual code changes (GIT PATCH, GIT STAT, and ACTUAL FULL CONTENT OF MODIFIED FILES above) and the compiler/test execution console logs:
1. Explain what functional or structural changes this commit introduced to the codebase. Review the actual code lines modified in the GIT PATCH and understand their context within the modified files.
2. Provide an extremely detailed, highly specific, line-by-line engineering review of this commit. Do not use generic templates or empty general statements. Mention specific class names, function names, and variable names modified.
3. If there are any compiler warnings, errors, or failed test stack traces in the logs, perform a deep diagnostic of them. If the build succeeded with zero warnings, explicitly highlight this achievement.
4. Perform a rigorous, deep code design and architectural review of the modified code files:
   - Identify potential design flaws, code smells, or style issues.
   - Scan for resource leakages (e.g., disposable WinForms controls, Pen/Brush/Font objects, SQL commands/connections, streams not enclosed in 'using' blocks).
   - Check thread safety, especially since WinForms UI controls can only be updated from the main UI thread.
   - Assess error handling (e.g. catch blocks that swallow exceptions silently, lack of validation).
5. Give highly concrete, copy-pasteable refactored code snippets showing exactly how to improve the code.

Provide a structured analysis with these sections:
1. **Build Summary** - A detailed build verdict explaining what this build achieved (the feature implemented or bug fixed) and the overall build quality.
2. **Line-by-Line Code Review & Key Findings** - A deep technical, line-by-line review of the specific code changes using code snippets where relevant. Do not hold back on details.
3. **WinForms UI & System Architecture Review** - Evaluation of UI design, resource leakages, layout, and background task safety.
4. **Test Results & Diagnostics** - Summary of test execution, and deep debugging of any failed tests.
5. **Security & Supply Chain Posture** - Assessment based on audit data.
6. **Performance, Capacity & Footprint** - Build duration, installer size, and repository size impact.
7. **Actionable Recommendations** - At least 3 extremely specific, concrete, code-level actionable recommendations with code examples.

FORMAT RULES:
- Use plain ASCII text only. No emoji, no unicode symbols, no special characters.
- Use proper markdown formatting (headers ##, tables, bold **text**, code blocks).
- Be incredibly comprehensive and detailed. Do NOT limit your word count. Spend as many words as necessary to give a professional, world-class code review.
"@

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "user"; content = $prompt }
    )
    temperature = 0.4
    max_tokens = 5000
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

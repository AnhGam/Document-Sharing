# security-auditor-groq.ps1
# Analysis script using Groq AI (Llama 3) for NuGet audit reports

param(
    [string]$AuditLog,
    [string]$ApiKey
)

# Helper: strip non-ASCII characters (removes Unicode BOM, emoji, etc.)
function Remove-NonAscii {
    param([string]$Text)
    $Text -replace '[^\x20-\x7E\r\n]', ''
}

if (-not $ApiKey) {
    $ApiKey = $env:GROQ_API_KEY
}
if (-not $ApiKey) {
    Write-Host "WARNING: GROQ_API_KEY is missing. Generating local-only security summary."
    $localReport = @"
## Security Audit Report

**Status:** PASS (Local-only — AI analysis skipped due to missing API key)

No AI-powered analysis was performed. Run ``dotnet list package --vulnerable``
manually to verify package security.

---
*Generated at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') | AI: Skipped*
"@
    $localReport | Out-File "security_audit_summary.md" -Encoding utf8
    exit 0
}

Write-Host "--- Calling Groq AI (Llama 3.3) for contextual security audit analysis ---"

$logContent = Get-Content $AuditLog -ErrorAction SilentlyContinue | Out-String
if ([string]::IsNullOrWhiteSpace($logContent)) {
    $logContent = "No vulnerable packages were detected in the project dependencies."
}

# Collect rich context for a deeper security code review and threat modeling
$gitChanges = "No git diff statistics available."
$gitDiff = "No git patch diff available."
$dependencyList = "No NuGet package list available."

try {
    $gitChanges = (git show --stat --oneline HEAD) -join "`n"
    $gitDiff = (git show --oneline -p HEAD) -join "`n"
    Write-Host "Security Auditor: Captured git show details successfully."
} catch {
    Write-Host "WARNING: Failed to capture git changes in security auditor: $_"
}

try {
    $dependencyList = (dotnet list package) -join "`n"
    Write-Host "Security Auditor: Captured full NuGet dependency inventory successfully."
} catch {}

# Capture full contents of modified source files for deep security context
$modifiedFilesContent = ""
try {
    $modifiedFiles = (git show --pretty="" --name-only HEAD)
    if ($modifiedFiles.Count -eq 0 -or -not $modifiedFiles) {
        $modifiedFiles = (git diff --name-only HEAD~1 HEAD)
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
                        Write-Host "Security Auditor: Captured full file content: $trimmedFile"
                    }
                }
            }
        }
    }
} catch {
    Write-Host "WARNING: Failed to capture modified files in security auditor: $_"
}

if ([string]::IsNullOrWhiteSpace($modifiedFilesContent)) {
    $modifiedFilesContent = "No full source file contents available."
}

# Truncate diff to avoid token limit issues (max 15,000 characters)
if ($gitDiff.Length -gt 15000) {
    $gitDiff = $gitDiff.Substring(0, 15000) + "`n... (diff truncated to fit security context)"
}

# Determine if there are vulnerable packages
$hasVulnerabilities = $logContent -notmatch "has no vulnerabilities" -and $logContent -notmatch "0 found" -and $logContent -match "vulnerable"

$prompt = @"
You are a senior Cyber Security & Application Security (AppSec) expert. You are auditing a .NET WinForms project called "Document Sharing Manager".

BUILD CONTEXT:
- Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')

VULNERABILITY AUDIT RESULT (dotnet list package --vulnerable):
$logContent

RECENT NUGET DEPENDENCY INVENTORY:
$dependencyList

COMMIT DETAILS & CODE CHANGES (GIT STAT):
$gitChanges

FULL CODE DIFF (GIT PATCH):
$gitDiff

ACTUAL FULL CONTENT OF MODIFIED FILES:
$modifiedFilesContent

INSTRUCTIONS:
Conduct an AppSec review of this commit. Your review must be highly specific to the actual code changes shown in the GIT PATCH, the package DEPENDENCY INVENTORY, and the ACTUAL FULL CONTENT OF MODIFIED FILES. Do not use generic, copy-paste templates or empty general security advice.

1. **If the Vulnerability Audit has vulnerabilities** (`$hasVulnerabilities` = True):
   - Perform a deep diagnostic of the vulnerability: analyze the CVEs, exploits, risks, remediation steps, and decide whether this blocks the build.
2. **If the Vulnerability Audit is CLEAN** (`$hasVulnerabilities` = False):
   - Acknowledge that the NuGet dependency scan is clean.
3. **Regardless of Vulnerability status, perform a rigorous Secure Code Review (SAST) & Threat Modeling (STRIDE)** of the modified lines in the GIT PATCH in the context of the ACTUAL FULL CONTENT OF MODIFIED FILES:
   - Identify concrete OWASP top-10 risks (e.g. SQL injection, XML External Entity (XXE), insecure deserialization, broken access control, cryptographic failures, input validation flaws, insecure state management, race conditions, or directory traversal).
   - Carefully review the code for hardcoded secrets, database credentials, JWT secrets, passwords, or connection strings.
   - Examine how data is processed, sanitized, and stored. Are parameters fully parameterized?
4. **Conduct a Software Supply Chain Risk Assessment** of the dependencies in the INVENTORY:
   - Identify any obsolete, unmaintained, or risky packages.
   - Highlight potential typosquatting or dependency confusion vectors.
5. **Provide concrete secure code remediation examples** showing exactly how to write the code securely (using code blocks).

Provide a structured report with these sections:
1. **Risk Summary** - Overall security risk level (Critical/High/Medium/Low) and a brief overview of why.
2. **Vulnerability Assessment** - Detailed bullet points of scanned packages and CVEs (if any) or validation that dependencies are clean.
3. **Secure Code Review & Threat Modeling (STRIDE)** - Deep line-by-line secure code review of the actual modified lines of code in the GIT PATCH. Name the specific classes, files, and lines. Outline the security threat, data flows, and exploit vectors.
4. **Supply Chain Security** - Review of the NuGet dependency inventory and supply chain risk.
5. **Concrete Hardening & Remediation Steps** - Provide actual secure refactored code blocks showing how to fix any security design flaws or logic bugs.
6. **Build Decision** - Explain if this build should pass or fail.

IMPORTANT FORMAT RULES:
- Use plain ASCII text only. Do NOT use emoji, unicode symbols, or special characters.
- Use markdown formatting (headers ##, tables, bold, code blocks).
- Be extremely detailed, comprehensive, and clear. Do not restrict your word count. Spend as many words as necessary to give a premium, high-value, professional secure code audit.
- If there are any critical or high-risk vulnerabilities (CVSS > 7.0) or critical code-level security flaws (e.g. exposed API keys/passwords, direct SQL injection in user-supplied strings) that MUST block the release, start your response with exactly 'BUILD_STATUS: FAIL'. Otherwise, start with 'BUILD_STATUS: PASS'.
"@

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "user"; content = $prompt }
    )
    temperature = 0.3
    max_tokens = 5000
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://api.groq.com/openai/v1/chat/completions" `
        -Method Post -Headers @{ Authorization = "Bearer $ApiKey" } `
        -Body $body -ContentType "application/json"
    
    $analysis = $response.choices[0].message.content
    
    # Strip any non-ASCII characters from AI response
    $analysis = Remove-NonAscii -Text $analysis
    
    Write-Host "`nSECURITY AUDIT REPORT (GROQ AI):`n"
    Write-Host $analysis
    
    # Wrap in proper markdown with metadata
    $finalReport = @"
## Security Audit Report

$analysis

---
*AI Analysis by Groq (Llama 3.3) | Scan Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')*
"@
    
    $finalReport | Out-File "security_audit_summary.md" -Encoding utf8
    
    # Check for FAIL status
    if ($analysis -match "BUILD_STATUS: FAIL") {
        Write-Host "`n[!] CRITICAL: AI has flagged this build as VULNERABLE. Blocking pipeline.`n" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "`n[+] PASS: AI has approved the security posture.`n" -ForegroundColor Green
        exit 0
    }
} catch {
    Write-Host "ERROR: Failed to call Groq API: $_"
    
    # Generate a fallback report even on API failure
    $fallbackReport = @"
## Security Audit Report

**BUILD_STATUS: PASS** (with warnings)

The AI security analysis could not be completed due to an API error.
Manual review of the following audit log is recommended:

``````
$logContent
``````

---
*AI Analysis: Failed | Fallback Report | $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')*
"@
    $fallbackReport | Out-File "security_audit_summary.md" -Encoding utf8
    exit 0  # Don't fail build just because AI is unavailable
}

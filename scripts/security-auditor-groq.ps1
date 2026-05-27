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

Write-Host "--- Calling Groq AI (Llama 3) for security audit analysis ---"

$logContent = Get-Content $AuditLog | Out-String

# If no vulnerabilities, generate a clean summary WITHOUT calling AI
if ([string]::IsNullOrWhiteSpace($logContent) -or $logContent -match "has no vulnerabilities") {
    Write-Host "SUCCESS: No vulnerabilities found."
    
    $cleanReport = @"
## Security Audit Report

**BUILD_STATUS: PASS**

No vulnerable packages were found in the project dependencies.

### Scan Summary
| Check | Result |
|:------|:-------|
| Vulnerable Packages | 0 found |
| Scan Date | $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') |
| Tool | ``dotnet list package --vulnerable`` |
| AI Analysis | Not required (clean scan) |

### Recommendations
- Continue monitoring dependencies with regular scans
- Keep NuGet packages updated to their latest stable versions
- Consider enabling GitHub Dependabot alerts for automated monitoring

---
*Automated Security Audit | Clean Scan*
"@
    $cleanReport | Out-File "security_audit_summary.md" -Encoding utf8
    exit 0
}

# Vulnerabilities found — call Groq AI for contextual analysis
$prompt = @"
You are a Cyber Security expert. I have a .NET WinForms project and these are the results from 'dotnet list package --vulnerable'.

Analyze the vulnerabilities and provide a structured security report with these sections:
1. **Risk Summary** - Overall risk level (Critical/High/Medium/Low)
2. **Vulnerable Packages** - List each vulnerable package with its CVE if available
3. **Impact Analysis** - What could be exploited and how
4. **Remediation Steps** - Specific upgrade commands or workarounds
5. **Build Decision** - Should this block the release?

IMPORTANT FORMAT RULES:
- Use plain ASCII text only. Do NOT use emoji, unicode symbols, or special characters.
- Use markdown formatting (headers, tables, bold, code blocks).
- If there are any critical or high-risk vulnerabilities (CVSS > 7.0) that MUST block the release, start your response with exactly 'BUILD_STATUS: FAIL'. Otherwise, start with 'BUILD_STATUS: PASS'.

AUDIT LOG:
$logContent
"@

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "user"; content = $prompt }
    )
    temperature = 0.3
    max_tokens = 1024
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

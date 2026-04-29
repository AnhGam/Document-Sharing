# security-auditor-groq.ps1
# Analysis script using Groq AI (Llama 3) for NuGet audit reports

param(
    [string]$AuditLog,
    [string]$ApiKey
)

if (-not $ApiKey) {
    Write-Host "WARNING: GROQ_API_KEY is missing. Skipping security AI audit."
    exit 0
}

Write-Host "--- Calling Groq AI (Llama 3) for security audit analysis ---"

$logContent = Get-Content $AuditLog | Out-String
if ([string]::IsNullOrWhiteSpace($logContent) -or $logContent -match "has no vulnerabilities") {
    Write-Host "SUCCESS: No vulnerabilities found to analyze."
    exit 0
}

$prompt = @"
You are a Cyber Security expert. I have a WinForms project and these are the results from 'dotnet list package --vulnerable'.
Summarize the most critical risks and provide brief advice (max 150 words, in English) on what should be prioritized.

IMPORTANT: If there are any critical or high-risk vulnerabilities (e.g., CVSS > 7.0) that MUST block the release, start your response with exactly 'BUILD_STATUS: FAIL'. Otherwise, start with 'BUILD_STATUS: PASS'.

LOG:
$logContent
"@

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "user"; content = $prompt }
    )
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://api.groq.com/openai/v1/chat/completions" `
        -Method Post -Headers @{ Authorization = "Bearer $ApiKey" } `
        -Body $body -ContentType "application/json"
    
    $analysis = $response.choices[0].message.content
    
    Write-Host "`nSECURITY AUDIT REPORT (GROQ AI):`n"
    Write-Host $analysis
    
    # Save for summary
    $analysis | Out-File "security_audit_summary.md"
    
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
    exit 1 # Fail build if auditor fails
}

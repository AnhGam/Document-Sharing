# optimize-assets.ps1
# Asset inspection and optimization script for installer resources

Write-Host "--- Checking Asset Optimization ---"

$assetPath = "document-sharing-manager/assets"
$maxSizeKB = 500

$largeFiles = Get-ChildItem -Path $assetPath -Recurse -Include *.png, *.jpg, *.ico | Where-Object { $_.Length / 1KB -gt $maxSizeKB }

if ($largeFiles) {
    Write-Host "ERROR: Files exceeding size limit ($maxSizeKB KB):" -ForegroundColor Red
    foreach ($file in $largeFiles) {
        Write-Host "  - $($file.FullName) ($([Math]::Round($file.Length / 1KB, 2)) KB)"
    }
    Write-Host "Action Required: Compress these files to reduce installer size and fix the build." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "SUCCESS: All assets meet size standards." -ForegroundColor Green
}

# (Optional) Tích hợp lệnh nén nếu có optipng
if (Get-Command optipng -ErrorAction SilentlyContinue) {
    Write-Host "Đang tự động nén ảnh bằng optipng..."
    Get-ChildItem -Path $assetPath -Recurse -Include *.png | ForEach-Object {
        optipng -quiet -o2 $_.FullName
    }
}

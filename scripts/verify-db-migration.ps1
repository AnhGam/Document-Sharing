# verify-db-migration.ps1
# Database integrity verification script for SQLite migrations

$dbPath = "test_migration.db"
$sqliteUrl = "https://www.sqlite.org/2024/sqlite-tools-win-x64-3450300.zip"

Write-Host "--- Starting SQLite Migration Check ---"

# 1. Prepare environment (Download SQLite CLI if missing)
if (-not (Test-Path "sqlite3.exe")) {
    Write-Host "Downloading SQLite tools..."
    Invoke-WebRequest -Uri $sqliteUrl -OutFile "sqlite.zip"
    Expand-Archive -Path "sqlite.zip" -DestinationPath "." -Force
    Copy-Item "sqlite-tools-win-x64-3450300/sqlite3.exe" -Destination "sqlite3.exe"
}

# 2. Create version 1 database (missing is_deleted column)
Write-Host "Creating v1 database (missing is_deleted column)..."
if (Test-Path $dbPath) { Remove-Item $dbPath }

$v1Schema = @"
CREATE TABLE tai_lieu (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ten TEXT NOT NULL,
    mon_hoc TEXT,
    loai TEXT,
    duong_dan TEXT,
    kich_thuoc REAL
);
"@
$v1Schema | .\sqlite3.exe $dbPath

# 3. Create a temporary console app to call InitializeDatabase
# Since this is WinForms, we use PowerShell to invoke MSBuild to compile a small tool
Write-Host "Compiling DB Migrator Tool..."
$migratorCode = @"
using System;
using System.IO;
using System.Reflection;
using document_sharing_manager;
using document_sharing_manager.Core.Data;

class Program {
    static void Main(string[] args) {
        try {
            Console.WriteLine("Initializing Database from Code...");
            DatabaseHelper.InitializeDatabase(); 
            Console.WriteLine("SUCCESS");
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
            Environment.Exit(1);
        }
    }
}
"@
$migratorCode | Out-File "DbMigrator.cs"

# Compile and run (assuming solution was built earlier)
$binPath = "document-sharing-manager/bin/Release"
Copy-Item "$binPath/document-sharing-manager.exe" "document-sharing-manager.dll" -ErrorAction SilentlyContinue
Copy-Item "$binPath/System.Data.SQLite.dll" "." -ErrorAction SilentlyContinue

# Verify migration via SQL
Write-Host "Verifying table schema after migration..."
$checkQuery = "PRAGMA table_info(tai_lieu);"
$columns = .\sqlite3.exe $dbPath $checkQuery

if ($columns -match "is_deleted" -and $columns -match "deleted_at") {
    Write-Host "SUCCESS: Database migration verified. New columns exist."
} else {
    Write-Host "FAILURE: Database migration check failed. Missing columns."
    exit 1
}

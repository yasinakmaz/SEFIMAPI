# SEFIM API Service Publisher Script
# This script publishes the service as a single executable with all dependencies

param(
    [string]$OutputPath = ".\Published",
    [switch]$IncludeDebugSymbols
)

Write-Host "SEFIM API Service Publisher" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan
Write-Host ""

# Clean output directory
if (Test-Path $OutputPath) {
    Write-Host "Cleaning output directory..." -ForegroundColor Yellow
    Remove-Item -Path $OutputPath -Recurse -Force
}

# Create output directory structure
$servicePath = Join-Path $OutputPath "SEFIMAPIService"
New-Item -ItemType Directory -Path $servicePath -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $servicePath "logs") -Force | Out-Null

# Publish parameters
$publishParams = @{
    Path = ".\SEFIMAPI.AppHost\SEFIMAPI.AppHost.csproj"
    Output = $servicePath
    Configuration = "Release"
    Runtime = "win-x64"
    SelfContained = $true
    PublishSingleFile = $true
    PublishTrimmed = $false
    IncludeNativeLibrariesForSelfExtract = $true
    EnableCompressionInSingleFile = $true
}

if (-not $IncludeDebugSymbols) {
    $publishParams.Add("DebugType", "None")
    $publishParams.Add("DebugSymbols", $false)
}

# Publish the service
Write-Host "Publishing service..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Gray

dotnet publish @publishParams

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

# Copy additional files
Write-Host "`nCopying additional files..." -ForegroundColor Yellow

# Copy service management scripts
Copy-Item -Path ".\ServiceInstaller.bat" -Destination $servicePath -Force
Copy-Item -Path ".\ServiceManager.ps1" -Destination $servicePath -Force

# Create README
$readmeContent = @"
# SEFIM API Service

## Installation Instructions

### Quick Start (Batch File)
1. Run `ServiceInstaller.bat` as Administrator
2. Select option 1 to install the service
3. Select option 2 to start the service

### PowerShell (Alternative)
1. Open PowerShell as Administrator
2. Navigate to this directory
3. Run: `.\ServiceManager.ps1`
4. Follow the menu options

## Service Information
- Service Name: SEFIMAPIService
- Display Name: SEFIM API Service
- API Endpoint: https://localhost:5001
- Logs Location: .\logs\

## Troubleshooting
- Ensure you run installers as Administrator
- Check logs in the `logs` folder for errors
- Use option 5 in the installer to check service status

## Uninstallation
1. Stop the service (option 3)
2. Uninstall the service (option 4)
3. Delete this folder

---
Version: 1.0.0
"@

$readmeContent | Out-File -FilePath (Join-Path $servicePath "README.txt") -Encoding UTF8

# Create a simple config file
$configContent = @"
{
  "ServiceConfiguration": {
    "ApiPort": 5001,
    "LogRetentionDays": 30,
    "MaxLogFileSizeMB": 10
  }
}
"@

$configContent | Out-File -FilePath (Join-Path $servicePath "service.config.json") -Encoding UTF8

# Calculate size
$size = (Get-ChildItem -Path $servicePath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB

Write-Host "`nPublish completed successfully!" -ForegroundColor Green
Write-Host "Output location: $servicePath" -ForegroundColor Cyan
Write-Host "Total size: $([math]::Round($size, 2)) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Copy the '$servicePath' folder to the target machine"
Write-Host "2. Run ServiceInstaller.bat as Administrator"
Write-Host "3. Follow the installation prompts"

# Optional: Create ZIP file
$createZip = Read-Host "`nDo you want to create a ZIP file for distribution? (Y/N)"
if ($createZip -eq 'Y' -or $createZip -eq 'y') {
    $zipPath = Join-Path $OutputPath "SEFIMAPIService.zip"
    Write-Host "Creating ZIP file..." -ForegroundColor Yellow
    
    Compress-Archive -Path $servicePath -DestinationPath $zipPath -CompressionLevel Optimal -Force
    
    $zipSize = (Get-Item $zipPath).Length / 1MB
    Write-Host "ZIP file created: $zipPath" -ForegroundColor Green
    Write-Host "ZIP size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Cyan
}
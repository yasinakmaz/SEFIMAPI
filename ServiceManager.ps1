# SEFIM API Service Manager PowerShell Script
# Requires Administrator privileges

#Requires -RunAsAdministrator

# Service configuration
$ServiceName = "SEFIMAPIService"
$ServiceDisplayName = "SEFIM API Service"
$ServiceDescription = "SEFIM API service for MAUI applications"
$ServicePath = Join-Path $PSScriptRoot "SEFIMAPIService.exe"
$LogPath = Join-Path $PSScriptRoot "logs"

# Colors for output
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Error { Write-Host $args -ForegroundColor Red }
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }

# Show banner
function Show-Banner {
    Clear-Host
    Write-Host @"

 ====================================================
   _____ ______ ______ _____ __  __            _____ _____ 
  / ____|  ____|  ____|_   _|  \/  |     /\   |  __ \_   _|
 | (___ | |__  | |__    | | | \  / |    /  \  | |__) || |  
  \___ \|  __| |  __|   | | | |\/| |   / /\ \ |  ___/ | |  
  ____) | |____| |     _| |_| |  | |  / ____ \| |    _| |_ 
 |_____/|______|_|    |_____|_|  |_| /_/    \_\_|   |_____|

             Windows Service Manager v1.0
 ====================================================

"@ -ForegroundColor Cyan
}

# Check if service exists
function Test-ServiceExists {
    return (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) -ne $null
}

# Install service
function Install-Service {
    Write-Info "`n[*] Installing service..."
    
    if (Test-ServiceExists) {
        Write-Warning "[!] Service already exists. Please uninstall first."
        return
    }
    
    if (-not (Test-Path $ServicePath)) {
        Write-Error "[-] Service executable not found at: $ServicePath"
        return
    }
    
    try {
        New-Service -Name $ServiceName `
                   -BinaryPathName $ServicePath `
                   -DisplayName $ServiceDisplayName `
                   -Description $ServiceDescription `
                   -StartupType Automatic `
                   -ErrorAction Stop
        
        # Configure recovery options
        & sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null
        
        Write-Success "[+] Service installed successfully!"
        
        $start = Read-Host "`nDo you want to start the service now? (Y/N)"
        if ($start -eq 'Y' -or $start -eq 'y') {
            Start-ServiceNow
        }
    }
    catch {
        Write-Error "[-] Failed to install service: $_"
    }
}

# Start service
function Start-ServiceNow {
    Write-Info "`n[*] Starting service..."
    
    if (-not (Test-ServiceExists)) {
        Write-Error "[-] Service is not installed."
        return
    }
    
    try {
        Start-Service -Name $ServiceName -ErrorAction Stop
        Write-Success "[+] Service started successfully!"
        
        # Show initial status
        Get-Service -Name $ServiceName | Format-Table -AutoSize
    }
    catch {
        Write-Error "[-] Failed to start service: $_"
    }
}

# Stop service
function Stop-ServiceNow {
    Write-Info "`n[*] Stopping service..."
    
    if (-not (Test-ServiceExists)) {
        Write-Error "[-] Service is not installed."
        return
    }
    
    try {
        Stop-Service -Name $ServiceName -Force -ErrorAction Stop
        Write-Success "[+] Service stopped successfully!"
    }
    catch {
        Write-Error "[-] Failed to stop service: $_"
    }
}

# Uninstall service
function Uninstall-Service {
    $confirm = Read-Host "`n[!] Are you sure you want to uninstall the service? (Y/N)"
    if ($confirm -ne 'Y' -and $confirm -ne 'y') {
        return
    }
    
    Write-Info "[*] Uninstalling service..."
    
    if (-not (Test-ServiceExists)) {
        Write-Warning "[!] Service is not installed."
        return
    }
    
    try {
        # Stop service first
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
        
        # Remove service
        & sc.exe delete $ServiceName | Out-Null
        
        Write-Success "[+] Service uninstalled successfully!"
    }
    catch {
        Write-Error "[-] Failed to uninstall service: $_"
    }
}

# Show service status
function Show-ServiceStatus {
    Write-Info "`n[*] Service Status:"
    
    if (-not (Test-ServiceExists)) {
        Write-Warning "[!] Service is not installed."
        return
    }
    
    $service = Get-Service -Name $ServiceName
    $wmiService = Get-WmiObject -Class Win32_Service -Filter "Name='$ServiceName'"
    
    Write-Host "`nService Information:" -ForegroundColor Yellow
    Write-Host "  Name:         $($service.Name)"
    Write-Host "  Display Name: $($service.DisplayName)"
    Write-Host "  Status:       $($service.Status)" -ForegroundColor $(if ($service.Status -eq 'Running') { 'Green' } else { 'Red' })
    Write-Host "  Start Type:   $($service.StartType)"
    Write-Host "  Path:         $($wmiService.PathName)"
    
    if ($service.Status -eq 'Running') {
        $process = Get-Process -Id $wmiService.ProcessId -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "`nProcess Information:" -ForegroundColor Yellow
            Write-Host "  Process ID:   $($process.Id)"
            Write-Host "  CPU Time:     $($process.TotalProcessorTime)"
            Write-Host "  Memory (MB):  $([math]::Round($process.WorkingSet64 / 1MB, 2))"
            Write-Host "  Threads:      $($process.Threads.Count)"
            Write-Host "  Start Time:   $($process.StartTime)"
            Write-Host "  Uptime:       $((Get-Date) - $process.StartTime)"
        }
    }
}

# Show logs
function Show-Logs {
    Write-Info "`n[*] Opening logs folder..."
    
    if (Test-Path $LogPath) {
        # Get latest log file
        $latestLog = Get-ChildItem -Path $LogPath -Filter "*.log" | 
                     Sort-Object LastWriteTime -Descending | 
                     Select-Object -First 1
        
        if ($latestLog) {
            Write-Info "Latest log file: $($latestLog.Name)"
            $viewLog = Read-Host "Do you want to view the latest log? (Y/N)"
            
            if ($viewLog -eq 'Y' -or $viewLog -eq 'y') {
                # Show last 50 lines
                Get-Content $latestLog.FullName -Tail 50
            }
        }
        
        # Open folder
        Start-Process explorer.exe -ArgumentList $LogPath
    }
    else {
        Write-Warning "[!] Logs folder not found at: $LogPath"
    }
}

# Main menu
function Show-Menu {
    while ($true) {
        Write-Host "`n Please select an option:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  [1] Install Service"
        Write-Host "  [2] Start Service"
        Write-Host "  [3] Stop Service"
        Write-Host "  [4] Uninstall Service"
        Write-Host "  [5] Show Service Status"
        Write-Host "  [6] View Logs"
        Write-Host "  [7] Exit"
        Write-Host ""
        
        $choice = Read-Host "Your choice (1-7)"
        
        switch ($choice) {
            '1' { Install-Service }
            '2' { Start-ServiceNow }
            '3' { Stop-ServiceNow }
            '4' { Uninstall-Service }
            '5' { Show-ServiceStatus }
            '6' { Show-Logs }
            '7' { 
                Write-Host "`nExiting SEFIM API Service Manager..."
                Write-Host "Have a nice day!" -ForegroundColor Green
                return
            }
            default { 
                Write-Warning "Invalid choice! Please enter a number between 1-7."
            }
        }
        
        Write-Host "`nPress any key to continue..."
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        Show-Banner
    }
}

# Entry point
Show-Banner
Show-Menu
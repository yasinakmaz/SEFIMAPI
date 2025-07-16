@echo off
setlocal enabledelayedexpansion

:: Admin kontrolü
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Bu scripti yonetici olarak calistirmaniz gerekiyor!
    echo Lutfen sag tiklayin ve "Yonetici olarak calistir" secenegini secin.
    pause
    exit /b 1
)

:: ASCII Art Logo
echo.
echo  ====================================================
echo    _____ ______ ______ _____ __  __            _____ _____ 
echo   / ____^|  ____^|  ____^|_   _^|  \/  ^|     /\   ^|  __ \_   _^|
echo  ^| (___ ^| ^|__  ^| ^|__    ^| ^| ^| \  / ^|    /  \  ^| ^|__) ^|^| ^|  
echo   \___ \^|  __^| ^|  __^|   ^| ^| ^| ^|\/^| ^|   / /\ \ ^|  ___/ ^| ^|  
echo   ____) ^| ^|____^| ^|     _^| ^|_^| ^|  ^| ^|  / ____ \^| ^|    _^| ^|_ 
echo  ^|_____/^|______^|_^|    ^|_____^|_^|  ^|_^| /_/    \_\_^|   ^|_____^|
echo.
echo              Windows Service Installer v1.0
echo  ====================================================
echo.

:: Mevcut dizini al
set "SERVICE_PATH=%~dp0SEFIMAPIService.exe"
set "SERVICE_NAME=SEFIMAPIService"
set "SERVICE_DISPLAY=SEFIM API Service"
set "SERVICE_DESC=SEFIM API service for MAUI applications"

:MENU
echo  Lutfen bir islem secin:
echo.
echo  [1] Servisi Yukle (Install)
echo  [2] Servisi Baslat (Start)
echo  [3] Servisi Durdur (Stop)
echo  [4] Servisi Kaldir (Uninstall)
echo  [5] Servis Durumunu Goster (Status)
echo  [6] Log Dosyalarini Goster
echo  [7] Cikis
echo.
set /p choice="Seciminiz (1-7): "

if "%choice%"=="1" goto INSTALL
if "%choice%"=="2" goto START
if "%choice%"=="3" goto STOP
if "%choice%"=="4" goto UNINSTALL
if "%choice%"=="5" goto STATUS
if "%choice%"=="6" goto LOGS
if "%choice%"=="7" goto EXIT

echo Gecersiz secim! Lutfen 1-7 arasinda bir sayi girin.
pause
cls
goto MENU

:INSTALL
echo.
echo [*] Servis yukleniyor...

:: Eski servisi kontrol et
sc query %SERVICE_NAME% >nul 2>&1
if %errorLevel% equ 0 (
    echo [!] Servis zaten yuklu. Once kaldirin.
    pause
    cls
    goto MENU
)

:: Servisi oluþtur
sc create %SERVICE_NAME% binPath= "%SERVICE_PATH%" DisplayName= "%SERVICE_DISPLAY%" start= auto
if %errorLevel% equ 0 (
    echo [+] Servis basariyla olusturuldu.
    
    :: Servis açýklamasýný ayarla
    sc description %SERVICE_NAME% "%SERVICE_DESC%"
    
    :: Hata durumunda yeniden baþlatma ayarlarý
    sc failure %SERVICE_NAME% reset= 86400 actions= restart/60000/restart/60000/restart/60000
    
    echo [+] Servis ayarlari yapildi.
    echo.
    echo Servisi baslatmak ister misiniz? (E/H)
    set /p startservice=
    if /i "!startservice!"=="E" goto START_AFTER_INSTALL
) else (
    echo [-] Servis olusturulamadi! Hata kodu: %errorLevel%
)

pause
cls
goto MENU

:START_AFTER_INSTALL
net start %SERVICE_NAME%
pause
cls
goto MENU

:START
echo.
echo [*] Servis baslatiliyor...
net start %SERVICE_NAME%
if %errorLevel% equ 0 (
    echo [+] Servis basariyla basladi.
) else (
    echo [-] Servis baslatilamadi! Hata kodu: %errorLevel%
)
pause
cls
goto MENU

:STOP
echo.
echo [*] Servis durduruluyor...
net stop %SERVICE_NAME%
if %errorLevel% equ 0 (
    echo [+] Servis basariyla durduruldu.
) else (
    echo [-] Servis durdurulamadi! Hata kodu: %errorLevel%
)
pause
cls
goto MENU

:UNINSTALL
echo.
echo [!] Servisi kaldirmak istediginizden emin misiniz? (E/H)
set /p confirm=
if /i not "!confirm!"=="E" (
    cls
    goto MENU
)

echo [*] Servis durduruluyor...
net stop %SERVICE_NAME% >nul 2>&1

echo [*] Servis kaldiriliyor...
sc delete %SERVICE_NAME%
if %errorLevel% equ 0 (
    echo [+] Servis basariyla kaldirildi.
) else (
    echo [-] Servis kaldirilamadi! Hata kodu: %errorLevel%
)
pause
cls
goto MENU

:STATUS
echo.
echo [*] Servis durumu kontrol ediliyor...
echo.
sc query %SERVICE_NAME%
echo.
pause
cls
goto MENU

:LOGS
echo.
echo [*] Log klasoru aciliyor...
start "" "%~dp0logs"
pause
cls
goto MENU

:EXIT
echo.
echo SEFIM API Service Installer'dan cikiliyor...
echo Iyi gunler!
timeout /t 2 >nul
exit /b 0
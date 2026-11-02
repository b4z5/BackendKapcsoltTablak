@echo off
setlocal
where dotnet >nul 2>&1
if errorlevel 1 (
echo HIBA: Telepitsd a .NET 10 SDK-t.
pause
exit /b 1
)
pushd "%~dp0Konyvtar"
if errorlevel 1 exit /b 1
set "ASPNETCORE_URLS=http://127.0.0.1:5072"
echo Konyvtar API: http://127.0.0.1:5072/api/olvasok
echo Leallitas: Ctrl+C
dotnet run --no-launch-profile
set "status=%errorlevel%"
popd
if not "%status%"=="0" pause
exit /b %status%

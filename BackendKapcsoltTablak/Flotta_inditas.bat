@echo off
setlocal
where dotnet >nul 2>&1
if errorlevel 1 (
echo HIBA: Telepitsd a .NET 9 SDK-t.
pause
exit /b 1
)
pushd "%~dp0CegAutoFlottaNyilv"
if errorlevel 1 exit /b 1
set "ASPNETCORE_ENVIRONMENT=Development"
set "ASPNETCORE_URLS=http://127.0.0.1:5071"
echo Ceges flotta API: http://127.0.0.1:5071/api/emberek
echo OpenAPI: http://127.0.0.1:5071/openapi/v1.json
echo Leallitas: Ctrl+C
dotnet run --no-launch-profile
set "status=%errorlevel%"
popd
if not "%status%"=="0" pause
exit /b %status%

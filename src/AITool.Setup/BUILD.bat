@echo off


"%~dp0INNO\ISCC.exe" "%~dp0Script.iss" 
echo done.  Result=%errorlevel%.

:: Pause
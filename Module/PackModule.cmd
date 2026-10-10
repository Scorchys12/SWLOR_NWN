@echo off
setlocal
pushd "%~dp0"
echo Building the CLI and packing the module. This may take a few minutes.
echo Output is being saved to "%~dp0pack-module.log".
call "..\tools\SWLOR.CLI\RunCLI.cmd" -p ".\Star Wars LOR v2.mod" > "pack-module.log" 2>&1
set "PACK_EXIT_CODE=%ERRORLEVEL%"
type "pack-module.log"
if not "%PACK_EXIT_CODE%"=="0" (
    echo.
    echo Packing failed. The full error is saved in "%~dp0pack-module.log".
    pause
)
popd
exit /b %PACK_EXIT_CODE%

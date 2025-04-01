@echo off
REM Enable delayed variable expansion
setlocal enabledelayedexpansion

REM ====================================
REM 1. Get the directory where the script is located
REM ====================================
REM %~dp0 returns the script's directory (including trailing backslash)
set "script_dir=%~dp0"

REM Remove the trailing backslash
if "%script_dir:~-1%"=="\" set "script_dir=%script_dir:~0,-1%"

REM ====================================
REM 2. Get the parent directory of the script's directory
REM ====================================
for %%i in ("%script_dir%\..") do set "parent_dir=%%~fi"

REM ====================================
REM 3. Define default input and output paths
REM ====================================
REM Input path: out\client under the script directory
set "input_path=%script_dir%\out\client"

REM Output path: move up to the parent directory then to UnityProject\Assets\GameScripts\HotFix\HotfixCommon\Proto
set "output_path=%parent_dir%\..\UnityProject\Assets\GameScripts\HotFix\HotfixCommon\Proto"

REM Normalize the output path to absolute path
for %%i in ("%output_path%") do set "output_path=%%~fi"

REM ====================================
REM 4. Check if the input path exists
REM ====================================
if not exist "%input_path%" (
    echo Input path does not exist: %input_path%
    exit /b 1
)

REM ====================================
REM 5. Delete the output path if it exists
REM ====================================
if exist "%output_path%" (
    echo Deleting old output path: %output_path%
    rmdir /s /q "%output_path%"
)

REM ====================================
REM 6. Create the output path
REM ====================================
echo Creating new output path: %output_path%
mkdir "%output_path%"

REM ====================================
REM 7. Find all .cs files in the input path and copy them to the output path
REM ====================================
echo Copying .cs files to the output path...
for /r "%input_path%" %%f in (*.cs) do (
    copy "%%f" "%output_path%" >nul
)

echo Copy completed

REM End delayed variable expansion
endlocal

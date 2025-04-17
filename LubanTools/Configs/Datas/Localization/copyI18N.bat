@echo off
setlocal enabledelayedexpansion

REM Get the current script directory
set "script_dir=%~dp0"
set "script_dir=%script_dir:~0,-1%"

REM Get the parent directory of the script
for %%I in ("%script_dir%\..") do set "parent_dir=%%~fI"

REM Define default input and output paths relative to the script directory
set "input_file=%script_dir%\Localization.csv"
set "output_dir=%parent_dir%\..\..\..\UnityProject\Assets\Editor\I2Localization\Localization"

REM Check if input file exists
if not exist "%input_file%" (
    echo Input file does not exist: %input_file%
    exit /b 1
)

REM Copy Localization.csv file to output path
echo Copying Localization.csv file to output path...
echo "%output_dir%"
copy "%input_file%" "%output_dir%" >nul

echo Copy completed

pause

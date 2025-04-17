@echo off
setlocal enabledelayedexpansion

REM Execute Python script
python generateI18N.py

REM Get the current script directory
set "script_dir=%~dp0"
set "script_dir=%script_dir:~0,-1%"

REM Get the parent directory of the script
for %%i in ("%script_dir%\..") do set "parent_dir=%%~fi"

REM Define default input and output paths relative to the script directory
set "input_path=%script_dir%\Localization\I18N"
set "output_path=%parent_dir%\..\..\..\..\UnityProject\Assets\AssetRaw\Configs\I18N"
echo Output path: %output_path%

REM Check if input path exists
if not exist "%input_path%" (
    echo Input path does not exist: %input_path%
    exit /b 1
)

REM Delete output path (if it exists)
if exist "%output_path%" (
    echo Deleting old output path: %output_path%
    rd /s /q "%output_path%"
)

REM Create output path
echo Creating new output path: %output_path%
mkdir "%output_path%" 2>nul

REM Copy all .csv files from input path to output path
echo Copying localization files to %output_path%
for /r "%input_path%" %%f in (*.csv) do (
    copy "%%f" "%output_path%" >nul
)

echo Copy completed

REM Execute copyI18N.bat
call copyI18N.bat

echo Copying localization files to Resource
REM call run_unity_function.bat
echo Localization/Refresh Localizations

REM Pause script execution, wait for user input
pause

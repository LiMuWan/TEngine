@echo off
setlocal enabledelayedexpansion

REM Check if Python is installed
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Python is not installed. Please install Python first.
    pause
    exit /b 1
)

REM Check if pip is installed
pip --version >nul 2>&1
if %errorlevel% neq 0 (
    echo pip is not installed. Please ensure pip is correctly installed.
    pause
    exit /b 1
)

REM Define packages to be installed
set "packages=pandas openpyxl"

REM Check and install packages
for %%p in (%packages%) do (
    python -c "import %%p" 2>nul
    if !errorlevel! neq 0 (
        echo Installing %%p...
        pip install %%p
        if !errorlevel! neq 0 (
            echo Failed to install %%p.
            pause
            exit /b 1
        )
    ) else (
        echo %%p is already installed.
    )
)

echo All necessary packages have been installed.
pause

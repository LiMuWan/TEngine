@echo off
REM ====================================
REM Batch script: Run genProto.bat with parameter 'client'
REM ====================================

REM Get the directory where the script is located
set "SCRIPT_DIR=%~dp0"

REM Change to the script's directory
cd /D "%SCRIPT_DIR%"
call gen_proto.bat client
pause

@echo off
setlocal enabledelayedexpansion
REM 获取脚本所在目录（去掉末尾的反斜杠）
for %%I in ("%~dp0.") do set "SCRIPT_DIR=%%~fI"
set "SCRIPT_DIR=%SCRIPT_DIR:~0%"
REM 定义输出目录
set "OUT_DIR=%SCRIPT_DIR%\out"
set "CLIENT_OUT_DIR=%OUT_DIR%\client"
set "SERVER_OUT_DIR=%OUT_DIR%\server"
REM 确保out目录存在
if not exist "%OUT_DIR%" (
    echo Creating output directory: %OUT_DIR%
    mkdir "%OUT_DIR%"
)
REM 主逻辑
if "%~1"=="" (
    echo Usage: %0 client^|server
    pause
    exit /b 1
)
if /i "%~1"=="client" (
    echo Preparing client output directory: %CLIENT_OUT_DIR%
    if exist "%CLIENT_OUT_DIR%" (
        echo Removing existing directory: %CLIENT_OUT_DIR%
        rmdir /s /q "%CLIENT_OUT_DIR%"
    )
    mkdir "%CLIENT_OUT_DIR%"
    
    echo Compiling proto files for client...
    "%SCRIPT_DIR%\protoc_to_cs.exe" --proto_path="%SCRIPT_DIR%\Proto" --csharp_out="%CLIENT_OUT_DIR%" "%SCRIPT_DIR%\Proto\*.proto"
    if errorlevel 1 (
        echo Failed to compile proto files
        pause
        exit /b 1
    )
    
    echo Generating additional files...
    python "%SCRIPT_DIR%\gen_proto.py" client
    if errorlevel 1 (
        echo Failed to generate files
        pause
        exit /b 1
    )
    
    echo Copying files to client...
    if exist "%SCRIPT_DIR%\copy_to_client.bat" (
        call "%SCRIPT_DIR%\copy_to_client.bat"
    ) else if exist "%SCRIPT_DIR%\copy_to_client.sh" (
        sh "%SCRIPT_DIR%\copy_to_client.sh"
    ) else (
        echo Warning: copy_to_client script not found
    )
    
) else if /i "%~1"=="server" (
    echo Preparing server output directory: %SERVER_OUT_DIR%
    if exist "%SERVER_OUT_DIR%" (
        echo Removing existing directory: %SERVER_OUT_DIR%
        rmdir /s /q "%SERVER_OUT_DIR%"
    )
    mkdir "%SERVER_OUT_DIR%"
    
    echo Compiling proto files for server...
    "%SCRIPT_DIR%\protoc.exe" --proto_path="%SCRIPT_DIR%\Proto" --go_out="%SERVER_OUT_DIR%" "%SCRIPT_DIR%\Proto\*.proto"
    if errorlevel 1 (
        echo Failed to compile proto files
        pause
        exit /b 1
    )
    
    echo Generating additional files...
    python "%SCRIPT_DIR%\gen_proto.py" server
    if errorlevel 1 (
        echo Failed to generate files
        pause
        exit /b 1
    )
) else (
    echo Invalid argument: %~1
    echo Usage: %0 client^|server
    pause
    exit /b 1
)
echo Operation completed successfully!
pause

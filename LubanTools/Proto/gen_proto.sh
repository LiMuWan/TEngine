@echo off

REM Function to pause execution on error
:pause_on_error
    echo An error occurred! Press any key to continue...
    pause >nul
    goto :eof

REM Main function
:main
    REM Check if argument is provided
    if "%~1"=="" (
        echo No argument provided. Usage: %0 client^|server
        call :pause_on_error
        exit /b 1
    )

    REM Get the directory where the script is located
    set "SCRIPT_DIR=%~dp0"
    
    REM Remove the trailing backslash from the path
    if "%SCRIPT_DIR:~-1%"=="\" set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"
    
    REM Add the script directory to PATH
    set "PATH=%PATH%;%SCRIPT_DIR%"
    
    echo %0 -- %1
    
    REM Define paths
    set "PROTO_DIR=%SCRIPT_DIR%\Proto"
    set "CLIENT_OUT_DIR=%SCRIPT_DIR%\out\client"
    set "SERVER_OUT_DIR=%SCRIPT_DIR%\out\server"
   
    
    REM Compile protobuf files based on the parameter
    if /i "%~1"=="client" (
        echo Compiling client proto files...
        "%SCRIPT_DIR%\protoc_to_cs.exe" --proto_path="%PROTO_DIR%" --csharp_out="%CLIENT_OUT_DIR%" "%PROTO_DIR%\*.proto"
        if errorlevel 1 (
            echo Error compiling client proto files.
            call :pause_on_error
            exit /b 1
        )
    ) else if /i "%~1"=="server" (
        echo Compiling server proto files...
        "%SCRIPT_DIR%\protoc.exe" --proto_path="%PROTO_DIR%" --go_out="%SERVER_OUT_DIR%" "%PROTO_DIR%\*.proto"
        if errorlevel 1 (
            echo Error compiling server proto files.
            call :pause_on_error
            exit /b 1
        )
    )
    
    REM Generate proto files
    echo Generating proto files...
    python "%SCRIPT_DIR%\gen_proto.py" "%~1"
    if errorlevel 1 (
        echo Error generating proto files. Exiting...
        call :pause_on_error
        exit /b 1
    )
    
    REM Additional step for client
    if /i "%~1"=="client" (
        echo Copying proto files to client...
        if exist "%SCRIPT_DIR%\copy_to_client.bat" (
            call "%SCRIPT_DIR%\copy_to_client.bat"
            if errorlevel 1 (
                echo Error copying proto files to client.
                call :pause_on_error
                exit /b 1
            )
        ) else if exist "%SCRIPT_DIR%\copy_to_client.sh" (
            REM If using Git Bash or another shell, call sh to execute
            sh "%SCRIPT_DIR%\copy_to_client.sh"
            if errorlevel 1 (
                echo Error copying proto files to client.
                call :pause_on_error
                exit /b 1
            )
        ) else (
            echo copy_to_client script not found.
            call :pause_on_error
            exit /b 1
        )
    )
    
    echo All done!
    pause
    exit /b 0

REM Call the main function and pass the first argument
call :main %1

REM End the script
endlocal

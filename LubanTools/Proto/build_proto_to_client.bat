@echo off
cd %~dp0
set ROOT_PATH=%~dp0
set WORKSPACE=..

set PROTOC_PATH=%WORKSPACE%/protoc/bin/protoc.exe
%PROTOC_PATH% --version

echo =================start gen proto code=================
set pb_path=pb_message
set out_path=../../UnityProject/Assets/GameScripts/Hotfix/HotfixCommonProto

REM 删除输出路径中的所有文件
del /f /s /q "%out_path%\*.*"

REM 循环处理每个 proto 文件
for /f "delims=" %%i in ('dir /b "%pb_path%"') do (
    echo ------------%%i start gen
    %PROTOC_PATH% -I="%pb_path%" --csharp_out="%out_path%" "%pb_path%\%%i"
    if errorlevel 1 (
        echo ------------%%i gen failed
        exit /b 1
    )
    echo ------------%%i gen success
)

echo =================end gen proto code=================

set GEN_PROTOBUFRESOLVER=%WORKSPACE%\Tools\ProtobufResolver\ProtobufResolver.exe
set INPUT_DATA_DIR=%ROOT_PATH%pb_message
set OUTEVENTPATH=%WORKSPACE%\..\UnityProject\Assets\GameScripts\HotFix\HotFixCommon\Definition\Constant

REM 执行 ProtobufResolver
"%GEN_PROTOBUFRESOLVER%" --input_data_dir "%INPUT_DATA_DIR%" --output_proto_dir "%OUTEVENTPATH%"
echo =================end gen proto event=================
pause

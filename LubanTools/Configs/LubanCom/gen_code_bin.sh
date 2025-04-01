#!/bin/zsh
WORKSPACE=..

GEN_CLIENT=${WORKSPACE}/Tools/Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=${WORKSPACE}/Configs

OUTPUT_CODE_PATH=${WORKSPACE}/../UnityProject/Assets/GameScripts/HotFix/HotFixCommon/LubanConfig
OUTPUT_DATA_PATH=${WORKSPACE}/../UnityProject/Assets/AssetRaw/Configs

#OUTPUT_DATA_PATH=/Users/admin/Documents/user/UnityProject/Assets/GameScripts/HotFix/HotFixCommon/LubanConfig
#OUTPUT_CODE_PATH=/Users/admin/Documents/user/UnityProject/Assets/AssetRaw/Configs
dotnet ${GEN_CLIENT} -h 127.0.0.1 -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_PATH} \
 --output_data_dir ${OUTPUT_DATA_PATH} \
 --gen_types code_cs_unity_bin,data_bin \
 -s all 

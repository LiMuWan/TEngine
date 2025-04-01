#!/bin/bash
# 增加错误捕获和显示功能
set -e  # 遇到错误时退出脚本执行
trap 'read -p "发生错误，脚本执行中断。按任意键继续..."' ERR  # 捕获错误并暂停

CURRENT_DIR=$(cd `dirname \$0`; pwd)  # 修正了这里的转义符
cd ${CURRENT_DIR}
WORKSPACE=..
echo "当前工作目录: $(pwd)"
echo "=================start gen proto code================="

pb_path=pb_message
PROTOC_PATH=${CURRENT_DIR}/protoc/bin/protoc.exe
out_path=../../UnityProject/Assets/GameScripts/Hotfix/HotfixCommon/Proto

# 检查 protoc 可执行文件是否存在
if [ ! -f "${PROTOC_PATH}" ]; then
    echo "错误: protoc.exe 不存在于路径 ${PROTOC_PATH}"
    echo "尝试查找可能的 protoc 位置..."
    
    # 列出当前目录下的所有 exe 文件
    find ${CURRENT_DIR} -name "*.exe" -type f
    
    echo "如果在上面找到了 protoc.exe，请更新脚本中的 PROTOC_PATH 变量"
    read -p "protoc.exe 不存在，无法继续执行。按任意键退出..." 
    exit 1
fi

# 检查输出目录是否存在，如果不存在则创建
if [ ! -d "$out_path" ]; then
    echo "创建输出目录: $out_path"
    mkdir -p $out_path
fi

# 删除输出路径中的所有文件
echo "清空输出目录: $out_path"
rm -f $out_path/*

# 检查 proto 文件目录是否存在
if [ ! -d "$pb_path" ]; then
    echo "错误: proto 文件目录 $pb_path 不存在"
    read -p "无法找到 proto 文件，按任意键退出..." 
    exit 1
fi

# 检查 proto 文件目录是否为空
if [ -z "$(ls -A $pb_path)" ]; then
    echo "警告: $pb_path 目录为空，没有 proto 文件可处理"
    read -p "没有找到 proto 文件，按任意键继续..." 
fi

# 循环处理每个 proto 文件
for file in $pb_path/*
do
  if [ -f "$file" ]; then
    echo "------------${file##*/} start gen"
    # 添加错误处理，显示具体错误信息并暂停
    ${PROTOC_PATH} -I=pb_message --csharp_out=$out_path $file
    if [ $? -ne 0 ]; then
      echo "------------${file##*/} gen failed"
      read -p "生成失败，按任意键继续..."
      exit 1
    fi
    echo "------------${file##*/} gen success"
  fi
done

echo "=================end gen proto code================="

GEN_PROTOBUFRESOLVER=${WORKSPACE}/Tools/ProtobufResolver/ProtobufResolver.dll
INPUT_DATA_DIR=${CURRENT_DIR}/pb_message
OUTEVENTPATH=${WORKSPACE}/../UnityProject/Assets/GameScripts/HotFix/HotFixCommon/Definition/Constant

# 检查 ProtobufResolver.dll 是否存在
if [ ! -f "${GEN_PROTOBUFRESOLVER}" ]; then
    echo "错误: ProtobufResolver.dll 不存在于路径 ${GEN_PROTOBUFRESOLVER}"
    read -p "ProtobufResolver.dll 不存在，无法继续执行。按任意键退出..." 
    exit 1
fi

# 检查输出目录是否存在，如果不存在则创建
if [ ! -d "$OUTEVENTPATH" ]; then
    echo "创建输出目录: $OUTEVENTPATH"
    mkdir -p $OUTEVENTPATH
fi

# 执行 ProtobufResolver
echo "执行 ProtobufResolver..."
dotnet ${GEN_PROTOBUFRESOLVER} --input_data_dir ${INPUT_DATA_DIR} --output_proto_dir ${OUTEVENTPATH}
if [ $? -ne 0 ]; then
    echo "ProtobufResolver 执行失败"
    read -p "按任意键继续..."
    exit 1
fi

echo "=================end gen proto event================="
echo "执行成功完成！"
read -p "按任意键退出..."

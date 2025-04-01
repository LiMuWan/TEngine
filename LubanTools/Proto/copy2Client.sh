#!/bin/bash

# 获取当前脚本所在的目录
script_dir=$(dirname "$(realpath "$0")")

# 获取脚本所在目录的上一级目录
parent_dir=$(dirname "$script_dir")

# 定义默认的输入路径和输出路径，相对于脚本所在目录
input_path="$script_dir/out/client"
output_path="$parent_dir/../UnityProject/Assets/GameScripts/HotFix/HotfixCommon/Proto"

# 检查输入路径是否存在
if [ ! -d "$input_path" ]; then
    echo "输入路径不存在: $input_path"
    exit 1
fi

# 删除输出路径（如果存在）
if [ -d "$output_path" ]; then
    echo "删除旧的输出路径: $output_path"
    rm -rf "$output_path"
fi

# 创建输出路径
echo "创建新的输出路径: $output_path"
mkdir -p "$output_path"

# 查找输入路径下的所有.cs文件并拷贝到输出路径
echo "拷贝 .cs 文件到输出路径..."
find "$input_path" -type f -name "*.cs" -exec cp {} "$output_path" \;

echo "拷贝完成"

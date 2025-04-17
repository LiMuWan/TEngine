#!/bin/bash

# 获取当前脚本所在的目录
script_dir=$(dirname "$(realpath "$0")")

# 获取脚本所在目录的上一级目录
parent_dir=$(dirname "$script_dir")

# 定义默认的输入路径和输出路径，相对于脚本所在目录
input_file="$script_dir/Localization/Localization.csv"
output_dir="$parent_dir/client/UnityProject/Assets/Editor/I2Localization/Localization"

# 检查输入文件是否存在
if [ ! -f "$input_file" ]; then
    echo "输入文件不存在: $input_file"
    exit 1
fi

## 删除输出路径（如果存在）
#if [ -d "$output_dir" ]; then
#    echo "删除旧的输出路径: $output_dir"
#    rm -rf "$output_dir"
#fi
#
## 创建输出路径
#echo "创建新的输出路径: $output_dir"
#mkdir -p "$output_dir"

# 拷贝 Localization.csv 文件到输出路径
echo "拷贝 Localization.csv 文件到输出路径..."
cp "$input_file" "$output_dir"

echo "拷贝完成"

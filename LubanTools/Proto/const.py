# -*- coding: utf-8 -*-
import os

# 获取当前脚本所在目录的绝对路径
script_dir = os.path.dirname(os.path.abspath(__file__))

# 项目根目录（假设 'xproject' 在脚本所在目录的上一级）
ProjectRoot = os.path.abspath(os.path.join(script_dir, '..', 'xproject'))

# Proto 目录路径
proto_path = os.path.join(script_dir, 'Proto')

# C# 文件输出路径
csfile_path = os.path.join(script_dir, 'out', 'client', 'ProtoDic.cs')

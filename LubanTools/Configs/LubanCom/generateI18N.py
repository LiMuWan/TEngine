import csv
import os

import utils

# 获取当前脚本所在的路径
current_directory = os.path.dirname(os.path.abspath(__file__))

# 定义文件名前缀
prefix = "I2_"
input = "Localization"
output = "Localization/I18N"

localizationXLSX = os.path.join(current_directory, f'{input}/Localization.xlsx')
localizationCSV = os.path.join(current_directory, f'{input}/Localization.csv')
print( 'export = ' , localizationXLSX,"to  CSV  =  ",localizationCSV)
utils.xlsx_to_csv(localizationXLSX,localizationCSV)


# 文件路径
chinese_file_path = os.path.join(current_directory, f'{output}/{prefix}Chinese.csv')
arabic_file_path = os.path.join(current_directory, f'{output}/{prefix}Arabic.csv')

# 确保输出目录存在
if not os.path.exists(os.path.join(current_directory, output)):
    os.makedirs(os.path.join(current_directory, output))

# 删除现有的文件（如果存在）
if os.path.exists(chinese_file_path):
    os.remove(chinese_file_path)

if os.path.exists(arabic_file_path):
    os.remove(arabic_file_path)

# 读取原始 CSV 文件
print('locallization path = ' , localizationCSV)
with open(localizationCSV, 'r', encoding='utf-8') as infile:
    reader = csv.DictReader(infile)
    chinese_rows = []
    arabic_rows = []

    # 提取 Chinese 和 Arabic 对应的列
    for row in reader:
        chinese_rows.append({'Key': row['Key'], 'Chinese': row['Chinese']})
        arabic_rows.append({'Key': row['Key'], 'Arabic': row['Arabic']})

# 写入 Chinese CSV 文件
with open(chinese_file_path, 'w', encoding='utf-8', newline='') as outfile:
    writer = csv.DictWriter(outfile, fieldnames=['Key', 'Chinese'])
    writer.writeheader()
    writer.writerows(chinese_rows)

# 写入 Arabic CSV 文件
with open(arabic_file_path, 'w', encoding='utf-8', newline='') as outfile:
    writer = csv.DictWriter(outfile, fieldnames=['Key', 'Arabic'])
    writer.writeheader()
    writer.writerows(arabic_rows)

print("CSV 文件已成功创建。")

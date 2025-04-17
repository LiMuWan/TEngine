import csv
import os
import utils

# 获取当前脚本所在的路径
current_directory = os.path.dirname(os.path.abspath(__file__))

# 定义文件名前缀、输入和输出路径
prefix = "I2_"
input_dir = "Localization"
output_dir = os.path.join(input_dir, "I18N")  # 确保输出路径的使用

localization_xlsx = os.path.join(current_directory, 'Localization.xlsx')
localization_csv = os.path.join(current_directory, 'Localization.csv')
print('export =', localization_xlsx, "to CSV =", localization_csv)

# 导出 Excel 到 CSV
if not os.path.exists(localization_xlsx):
    print(f"错误: 输入文件 {localization_xlsx} 不存在。")
else:
    try:
        utils.xlsx_to_csv(localization_xlsx, localization_csv)
    except Exception as e:
        print(f"转换 Excel 到 CSV 时发生错误: {e}")
        exit(1)

# 确保输出目录存在
output_dir_path = os.path.join(current_directory, output_dir)
if not os.path.exists(output_dir_path):
    os.makedirs(output_dir_path)

# 文件路径
chinese_file_path = os.path.join(output_dir_path, f'{prefix}Chinese.csv')
arabic_file_path = os.path.join(output_dir_path, f'{prefix}Arabic.csv')

# 删除现有的文件（如果存在）
for file_path in [chinese_file_path, arabic_file_path]:
    if os.path.exists(file_path):
        os.remove(file_path)

# 读取原始 CSV 文件
print('localization path =', localization_csv)

try:
    with open(localization_csv, 'r', encoding='utf-8') as infile:
        reader = csv.DictReader(infile)
        chinese_rows = []
        arabic_rows = []

        # 提取 Chinese 和 Arabic 对应的列
        for row in reader:
            chinese_rows.append({'Key': row['Key'], 'Chinese': row.get('Chinese', '')})
            arabic_rows.append({'Key': row['Key'], 'Arabic': row.get('Arabic', '')})

except FileNotFoundError:
    print(f"错误: 找不到文件 {localization_csv}。请确认该文件存在。")
    exit(1)  # 退出程序
except Exception as e:
    print(f"读取 CSV 文件时发生错误: {e}")
    exit(1)  # 退出程序

# 写入 Chinese CSV 文件
try:
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

except Exception as e:
    print(f"写入 CSV 文件时发生错误: {e}")

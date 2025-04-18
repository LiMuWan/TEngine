import csv
import os
import json
import utils

# 获取当前脚本所在的路径
current_directory = os.path.dirname(os.path.abspath(__file__))

# 定义文件名前缀、输入和输出路径
prefix = "I2_"
input_dir = "Localization"
output_dir = os.path.join(input_dir, "I18N")

localization_xlsx = os.path.join(current_directory, 'Localization.xlsx')
localization_csv = os.path.join(current_directory, 'Localization.csv')
language_json_path = os.path.join(current_directory, "Language.json")  # 更新为新的 JSON 文件路径

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

# 读取原始 CSV 文件
print('localization path =', localization_csv)

def read_localization_csv(file_path):
    """读取 CSV 文件并返回语言列与数据"""
    language_data = {}
    try:
        with open(file_path, 'r', encoding='utf-8') as infile:
            reader = csv.DictReader(infile)
            language_columns = [header for header in reader.fieldnames if header not in ['Key', 'Type', 'Desc']]
    
            # 提取数据
            for row in reader:
                for lang in language_columns:
                    if lang not in language_data:
                        language_data[lang] = []
                    language_data[lang].append({'Key': row['Key'], lang: row.get(lang, '')})

    except FileNotFoundError:
        print(f"错误: 找不到文件 {file_path}。请确认该文件存在。")
        exit(1)  # 退出程序
    except Exception as e:
        print(f"读取 CSV 文件时发生错误: {e}")
        exit(1)  # 退出程序
    
    return language_columns, language_data

def write_language_csv(file_path, data, lang):
    """将特定语言的数据写入 CSV 文件"""
    try:
        with open(file_path, 'w', encoding='utf-8', newline='') as outfile:
            writer = csv.DictWriter(outfile, fieldnames=['Key', lang])
            writer.writeheader()
            writer.writerows(data)
        print(f"{lang} CSV 文件已成功创建。")
    except Exception as e:
        print(f"写入 {lang} CSV 文件时发生错误: {e}")

# 读取语言数据
language_columns, language_data = read_localization_csv(localization_csv)

# 删除现有的语言 CSV 文件（如果存在）
for language in language_columns:
    lang_file_path = os.path.join(output_dir_path, f'{prefix}{language}.csv')
    if os.path.exists(lang_file_path):
        os.remove(lang_file_path)

# 写入每种语言的数据
for language in language_columns:
    write_language_csv(os.path.join(output_dir_path, f'{prefix}{language}.csv'), language_data[language], language)

# 导出语言名称到 JSON 文件
try:
    with open(language_json_path, 'w', encoding='utf-8') as jsonfile:
        json.dump(language_columns, jsonfile, ensure_ascii=False, indent=4)
    print(f"语言名称列表已成功导出为 JSON 文件: {language_json_path}")

except Exception as e:
    print(f"写入 JSON 文件时发生错误: {e}")

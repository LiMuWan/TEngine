import pandas as pd

def xlsx_to_csv(xlsx_file, csv_file, sheet_name=0):
    """
    将 xlsx 文件导出为 csv 文件。

    参数:
    xlsx_file (str): 输入的 xlsx 文件路径。
    csv_file (str): 输出的 csv 文件路径。
    sheet_name (str 或 int): 要导出的工作表名称或索引，默认是第一个工作表。

    """
    try:
        # 读取 xlsx 文件
        df = pd.read_excel(xlsx_file, sheet_name=sheet_name)

        # 导出为 csv 文件
        df.to_csv(csv_file, index=False)
        print(f"成功将 {xlsx_file} 导出为 {csv_file}")
    except Exception as e:
        print(f"导出过程中发生错误: {e}")

# 使用示例
# xlsx_file = 'example.xlsx'
# csv_file = 'example.csv'
# xlsx_to_csv(xlsx_file, csv_file)

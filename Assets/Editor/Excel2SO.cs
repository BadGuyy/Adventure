using System;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

public class Excel2SO
{
    [MenuItem("Tools/Excel/Read Task Table")]
    static void ReadTaskTable()
    {
        string[] selectedAssets = Selection.assetGUIDs;
        if (selectedAssets.Length == 0)
        {
            Debug.LogWarning("Please select an Excel file.");
            return;
        }
        for (int i = 0; i < selectedAssets.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(selectedAssets[i]);
            if (Path.GetExtension(assetPath) != ".xlsx")
            {
                Debug.LogWarning($"Selected file is not an Excel file: {assetPath}");
                continue;
            }
            ReadExcelAndCreateSO(assetPath);
        }
    }

    private static void ReadExcelAndCreateSO(string assetPath)
    {
        // 通过原路径获取Resources文件夹下目标SO路径
        string category = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        string targetDirectory = Path.Combine("Assets", "Resources", category);
        string targetAssetPath = Path.Combine(targetDirectory, assetName + ".asset");

        // 检查目标目录是否存在，不存在则创建
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        // 创建序列化类
        TaskList_SO data = ScriptableObject.CreateInstance<TaskList_SO>();
        // 读取Excel文件，using会在读取完毕后自动释放资源
        FileInfo fileInfo = new FileInfo(assetPath);
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            // 读取"Sheet1"工作表
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
            // 从第3行开始遍历行
            for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
            {
                // 创建行对象
                TaskData rowObject = new();
                Type rowType = rowObject.GetType();

                // 遍历列
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    // 用反射的方式设置rowObject的属性值，可以适配这类Excel模板
                    // 第2行是属性名，用来对应rowObject的成员变量
                    FieldInfo varible = rowType.GetField(worksheet.GetValue<string>(2, j));
                    // 第i行(i > 2)获取的第j列的值，用来设置rowObject的属性值
                    string cellValue = worksheet.GetValue<string>(i, j);

                    // 处理bool类型的特殊转换
                    if (varible.FieldType == typeof(bool))
                    {
                        varible.SetValue(rowObject, cellValue == "1" || cellValue.Equals("true"));
                        continue;
                    }
                    else
                    {
                        varible.SetValue(rowObject, Convert.ChangeType(cellValue, varible.FieldType));
                    }

                }

                // 添加rowObject到List
                data.TaskList.Add(rowObject);
            }
        }

        // 删除目标asset文件，避免重复创建
        if (File.Exists(targetAssetPath))
        {
            AssetDatabase.DeleteAsset(targetAssetPath);
            AssetDatabase.Refresh();
        }
        // 保存序列化类为asset文件
        AssetDatabase.CreateAsset(data, targetAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using OfficeOpenXml;

public static class ExcelReader 
{
    enum ColType
    {
        DataType = 4,   //数据类型
        FieldName = 5,  //字段名称
    }

    public static void ReadCfg(string cfgPath)
    {
        string[] cfgArr = Directory.GetFiles(cfgPath, "*.xlsx");
        List<string> codeList = new List<string>();
        Dictionary<string, List<ConfigData[]>> dataDict = new Dictionary<string, List<ConfigData[]>>();


        for(int i = 0; i < cfgArr.Length; i++)
        {
            string cfg = cfgArr[i];
            FileInfo newFile = new FileInfo(cfg);
            ExcelPackage package = new ExcelPackage(newFile);
            ExcelWorkbook excelWorkbook = package.Workbook;
            ExcelWorksheets excelWorksheets = excelWorkbook.Worksheets;
            ExcelWorksheet excelWorksheet = excelWorksheets[1];

            string[] fieldTypeArr = new string[excelWorksheet.Dimension.End.Column];
            string[] fieldNameArr = new string[excelWorksheet.Dimension.End.Column];
            List<ConfigData[]> dataList = new List<ConfigData[]>();

            for (int rowStart = excelWorksheet.Dimension.Start.Row, rowEnd = excelWorksheet.Dimension.End.Row; rowStart <= rowEnd; rowStart++)
            {
                for (int colStart = excelWorksheet.Dimension.Start.Column, colEnd = excelWorksheet.Dimension.End.Column; colStart <= colEnd; colStart++)
                {
                    string str = "null";
                    if (excelWorksheet.GetValue<string>(rowStart, colStart) != null)
                        str = excelWorksheet.GetValue<string>(rowStart, colStart);

                    if (rowStart >= (int)ColType.DataType)
                    {
                        int index = colStart - 1;
                        if (rowStart == (int)ColType.DataType)
                            fieldTypeArr[index] = str;
                        else if (rowStart == (int)ColType.FieldName)
                            fieldNameArr[index] = str;
                        else
                        {
                            List<ConfigData> configDataList = new List<ConfigData>();
                            ConfigData data = GetData();
                            data.fieldType = fieldTypeArr[index];
                            data.fieldName = fieldNameArr[index];
                            data.fieldData = str;
                            configDataList.Add(data);
                            dataList.Add(configDataList.ToArray());
                        }
                    }
                }
            }

            string className = newFile.Name;
            int pos = className.IndexOf('.');
            className = className.Substring(0, pos);
            string scriptTxt = GenerateScript(className, fieldTypeArr, fieldNameArr);
            //所有生成的类最终保存在这个链表中
            codeList.Add(scriptTxt);
            if (dataDict.ContainsKey(className))
                Debug.LogError("相同的表名 " + className);
            else
                dataDict.Add(className, dataList);
            CodeCompiler.CompileCode(codeList, dataDict);
        }
    }

    private static ConfigData GetData()
    {
        return new ConfigData();
    }

    public static string GenerateScript(string className, string[] fieldTypeArr, string[] fieldNameArr)
    {
        return ScriptGenerator.Generate(className, fieldTypeArr, fieldNameArr);
    }

}

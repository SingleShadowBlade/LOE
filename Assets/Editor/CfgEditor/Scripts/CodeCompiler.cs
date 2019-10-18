using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;

public static class CodeCompiler
{
    public static void CompileCode(List<string> codeList, Dictionary<string, List<ConfigData[]>> dataDict)
    {
        Assembly assembly = CompileCode(codeList.ToArray());
        string path = Constant.dllPath;
        //if (Directory.Exists(path))
        //    Directory.Delete(path, true);
        //    Directory.CreateDirectory(path);
        foreach (KeyValuePair<string, List<ConfigData[]>> each in dataDict)
        {
            object container = assembly.CreateInstance(each.Key + "Container");
            Type temp = assembly.GetType(each.Key);
            Serialize(container, temp, each.Value, path);
        }
        CreateDataManager(assembly);
    }

    //编译代码
    private static Assembly CompileCode(string[] scripts)
    {
        string path = Constant.dllPath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        //编译参数
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        CompilerParameters objCompilerParameters = new CompilerParameters();
        objCompilerParameters.ReferencedAssemblies.AddRange(new string[] { "System.dll" });
        objCompilerParameters.OutputAssembly = path + "Config.dll";
        objCompilerParameters.GenerateExecutable = false;
        objCompilerParameters.GenerateInMemory = true;

        //开始编译脚本
        CompilerResults cr = codeProvider.CompileAssemblyFromSource(objCompilerParameters, scripts);
        if (cr.Errors.HasErrors)
        {
            Debug.LogError("编译错误：");
            foreach (CompilerError err in cr.Errors)
                Console.WriteLine(err.ErrorText);
            return null;
        }
        return cr.CompiledAssembly;
    }

    //序列化对象
    private static void Serialize(object container, Type temp, List<ConfigData[]> dataList, string path)
    {
        //设置数据
        foreach (ConfigData[] datas in dataList)
        {
            object t = temp.Assembly.CreateInstance(temp.FullName);
            foreach (ConfigData data in datas)
            {
                FieldInfo info = temp.GetField(data.fieldName);
                info.SetValue(t, ParseValue(data.fieldType, data.fieldData));
            }

            object id = temp.GetField("id").GetValue(t);
            FieldInfo dictInfo = container.GetType().GetField("Dict");
            object dict = dictInfo.GetValue(container);

            bool isExist = (bool)dict.GetType().GetMethod("ContainsKey").Invoke(dict, new object[] { id });
            if (isExist)
            {
                Debug.LogError("repetitive key " + id + " in " + container.GetType().Name);
                break;
            }
            dict.GetType().GetMethod("Add").Invoke(dict, new object[] { id, t });
        }

        IFormatter f = new BinaryFormatter();
        Stream s = new FileStream(path + temp.Name + ".bytes", FileMode.OpenOrCreate,
                  FileAccess.Write, FileShare.Write);
        f.Serialize(s, container);
        s.Close();
    }

    private static object ParseValue(string fieldType, string fieldData)
    {
        if (fieldType == Constant.INT)
            return int.Parse(fieldData);
        else if (fieldType == Constant.FLOAT)
            return float.Parse(fieldData);

        return fieldData;
    }

    //创建数据管理器脚本
    private static void CreateDataManager(Assembly assembly)
    {
        IEnumerable types = assembly.GetTypes().Where(t => { return t.Name.Contains("Container"); });

        StringBuilder source = new StringBuilder();
        source.Append("/*Auto create\n");
        source.Append("Don't Edit it*/\n");
        source.Append("\n");

        source.Append("using System;\n");
        source.Append("using UnityEngine;\n");
        source.Append("using System.Runtime.Serialization;\n");
        source.Append("using System.Runtime.Serialization.Formatters.Binary;\n");
        source.Append("using System.IO;\n\n");
        source.Append("[Serializable]\n");
        source.Append("public class DataManager : SingletonTemplate<DataManager>\n");
        source.Append("{\n");

        //定义变量
        foreach (Type t in types)
        {
            source.Append("\tpublic " + t.Name + " " + t.Name.Remove(0, 2) + ";\n");
        }
        source.Append("\n");

        //定义方法
        foreach (Type t in types)
        {
            string typeName = t.Name.Remove(t.Name.IndexOf("Container"));
            string funcName = t.Name.Remove(0, 2);
            funcName = funcName.Substring(0, 1).ToUpper() + funcName.Substring(1);
            funcName = funcName.Remove(funcName.IndexOf("Container"));
            source.Append("\tpublic " + typeName + " Get" + funcName + "(int id)\n");
            source.Append("\t{\n");
            source.Append("\t\t" + typeName + " t = null;\n");
            source.Append("\t\t" + t.Name.Remove(0, 2) + ".Dict.TryGetValue(id, out t);\n");
            source.Append("\t\tif (t == null) Debug.LogError(" + '"' + "can't find the id " + '"' + " + id " + "+ " + '"' + " in " + t.Name + '"' + ");\n");
            source.Append("\t\treturn t;\n");
            source.Append("\t}\n");
        }

        ////加载所有配置表
        source.Append("\tpublic void LoadAll()\n");
        source.Append("\t{\n");
        foreach (Type t in types)
        {
            string typeName = t.Name.Remove(t.Name.IndexOf("Container"));
            source.Append("\t\t" + t.Name.Remove(0, 2) + " = Load(" + '"' + typeName + '"' + ") as " + t.Name + ";\n");
        }
        source.Append("\t}\n\n");

        //反序列化
        source.Append("\tprivate System.Object Load(string name)\n");
        source.Append("\t{\n");
        source.Append("\t\tIFormatter f = new BinaryFormatter();\n");
        source.Append("\t\tTextAsset text = Resources.Load<TextAsset>(" + '"' + "ConfigBin/" + '"' + " + name);\n");
        source.Append("\t\tStream s = new MemoryStream(text.bytes);\n");
        source.Append("\t\tSystem.Object obj = f.Deserialize(s);\n");
        source.Append("\t\ts.Close();\n");
        source.Append("\t\treturn obj;\n");
        source.Append("\t}\n");

        source.Append("}\n");
        //保存脚本
        string path = Constant.csScriptPath;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        StreamWriter sw = new StreamWriter(path + "DataManager.cs");
        sw.WriteLine(source.ToString());
        sw.Close();
    }
}

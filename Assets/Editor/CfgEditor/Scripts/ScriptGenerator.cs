using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class ScriptGenerator
{
    public static string[] fieldTypeArr;
    public static string[] fieldNameArr;
    public static string className;

    //开始生成脚本
    public static string Generate(string className, string[] fieldTypeArr, string[] fieldNameArr)
    {
        //生成类
        StringBuilder classSource = new StringBuilder();
        classSource.Append("/*Auto create\n");
        classSource.Append("Don't Edit it*/\n");
        classSource.Append("\n");
        classSource.Append("using System;\n");
        classSource.Append("using System.Reflection;\n");
        classSource.Append("using System.Collections.Generic;\n");
        classSource.Append("[Serializable]\n");
        classSource.Append("public class " + className + "\n");
        classSource.Append("{\n");
        //设置成员
        for (int i = 0; i < fieldNameArr.Length; ++i)
        {
            classSource.Append(PropertyString(fieldTypeArr[i], fieldNameArr[i]));
        }
        classSource.Append("}\n");

        //生成Container
        classSource.Append("\n");
        classSource.Append("[Serializable]\n");
        classSource.Append("public class " + className + "Container\n");
        classSource.Append("{\n");
        classSource.Append("\tpublic " + "Dictionary<int, " + className + ">" + " Dict" + " = new Dictionary<int, " + className + ">();\n");
        classSource.Append("}\n");
        return classSource.ToString();
    }

    private static string PropertyString(string type, string propertyName)
    {
        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(propertyName))
            return null;

        //if (type.Equals("intArr"))
        //    type = "int[]";
        //else if (type.Equals("floatArr"))
        //    type = "float[]";
        //else if (type.Equals("stringArr"))
        //    type = "string[]";
        StringBuilder sbProperty = new StringBuilder();
        sbProperty.Append("\tpublic " + type + " " + propertyName + ";\n");
        return sbProperty.ToString();
    }
}

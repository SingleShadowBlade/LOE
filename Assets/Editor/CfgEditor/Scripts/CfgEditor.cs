using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CfgEditor : MonoBehaviour
{
    [MenuItem("CfgTool/BuildCfg")]
    public static void BuildCfg()
    {
        ExcelReader.ReadCfg(Constant.cfgPath);
    }
}

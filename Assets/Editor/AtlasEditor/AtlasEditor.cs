using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AtlasEditor : MonoBehaviour
{
    public const string m_kAtlasAssetPath = "Assets/Bundle/UI/Atlas/";
    private readonly static List<string> ignoreNames = new List<string>() { "CommonAtlas" };

    [MenuItem("AtlasTool/MakeAtlas")]
    public static void MakeAtlasSpriteMap()
    {
        //  AllAtlasMap
        CreateAtlasMap();
    }

    static private void CreateAtlasMap()
    {
        var files = GetAtlasPath();
        //create atlas map asset 
        foreach (var f in files)
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath(f);
            var name = System.IO.Path.GetFileName(f);
            name = name.Replace(".png", ".asset");
            var allAtlasMap = AtlasMap.CreateInstance<AtlasMap>();//m_kAtlasAssetPath + name
            List<Sprite> sps = new List<Sprite>();
            foreach (var o in asset)
            {
                if (o is Sprite)
                {
                    sps.Add((Sprite)o);
                }
            }

            allAtlasMap.spriteNames = new List<int>(sps.Count);
            allAtlasMap.sprites = new Sprite[sps.Count];
            // allAtlasMap.texture = (Texture)AssetDatabase.LoadAssetAtPath(f, typeof(Texture));
            for (int i = 0; i < sps.Count; i++)
            {
                var sprite = sps[i];
                int hash = Animator.StringToHash(sprite.name);
                allAtlasMap.spriteNames.Add(hash);
                allAtlasMap.sprites[i] = sprite;
            }

            string assetPath = m_kAtlasAssetPath + name;
            AssetDatabase.CreateAsset(allAtlasMap, assetPath);
            AssetDatabase.SaveAssets();
        }
        // var allAtlasMap =  AtlasMap.CreateInstance(m_kAtlasAssetPath);
        // read atlas info

    }


    static private List<string> GetAtlasPath()
    {
        string dpath = Application.dataPath;
        Debug.Log(dpath);
        DirectoryInfo dinfo = new DirectoryInfo(m_kAtlasAssetPath);
        var files = dinfo.GetFiles("*.png");
        List<string> re = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.Contains("CommonAtlas"))
            {
                continue;
            }

            string path = files[i].FullName.Replace("\\", "/").Replace(dpath, "Assets");
            string fname = Path.GetFileNameWithoutExtension(path);
            if (!ignoreNames.Contains(fname))
                re.Add(path);
            else
            {
                Debug.LogWarning(" ignore file = " + path);
            }
        }

        return re;
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ProtoBuf.Meta;

public static class Util 
{
    #region 

    private static ProtobufSerializer protobufSerializer = new ProtobufSerializer();

    public static string PersistPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.persistentDataPath + "/StreamingAssets/PC/Assets/";
#elif UNITY_ANDROID
                return Application.persistentDataPath + "/StreamingAssets/Android/Assets/";
#elif UNITY_IPHONE
                return Application.persistentDataPath + "/StreamingAssets/iOS/Assets/";
#else
                return Application.persistentDataPath + "/StreamingAssets/PC/Assets/";
#endif
        }
    }

    #endregion

    #region
    public static void WriteBytesToFile(string pbName, byte[] bytes)
    {
        string localPath = PersistPath + pbName;
        if (!File.Exists(localPath))
            File.Create(localPath).Dispose();
        File.WriteAllBytes(localPath, bytes);
    }

    public static void WritePBToFile<T>(T PB)
    {
        byte[] byteArr = SerializeProtocol(PB);
        WriteBytesToFile(typeof(T).Name, byteArr);
    }

    public static T ReadPBFromFile<T>(string pbName)
    {
        string localPath = PersistPath + pbName;
        byte[] byteArr = File.ReadAllBytes(localPath);
        return DeserializeProtocol<T>(byteArr);
    }

    public static byte[] SerializeProtocol<T>(T kPB)
    {
        MemoryStream kStream = new MemoryStream();

        try
        {
            protobufSerializer.Serialize(kStream, kPB);
            return kStream.ToArray();
        }
        catch
        {
            Debug.LogError("Can't Encode PB " + typeof(T).Name);
            return null;
        }
    }

    public static T DeserializeProtocol<T>(byte[] kbyte)
    {
        try
        {
            T kProtocol = default(T);
            using (MemoryStream kStream = new MemoryStream(kbyte))
            {
                kProtocol = (T)protobufSerializer.Deserialize(kStream, null, typeof(T));
            }
            return kProtocol;
        }
        catch
        {
            Debug.LogError("Can't Decode PB " + typeof(T).Name);
            return default(T);
        }
    }
    #endregion
}

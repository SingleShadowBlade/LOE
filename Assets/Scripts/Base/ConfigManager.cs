/*Auto create
Don't Edit it*/

using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class ConfigManager : Singleton<ConfigManager>
{
	private MainCfgContainer mMainCfgContainer;
	private TestCfgContainer mTestCfgContainer;

	public MainCfg GetMainCfg(int id)
	{
		MainCfg t = null;
		mMainCfgContainer.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in MainCfgContainer");
		return t;
	}
	public TestCfg GetTestCfg(int id)
	{
		TestCfg t = null;
		mTestCfgContainer.Dict.TryGetValue(id, out t);
		if (t == null) Debug.LogError("can't find the id " + id + " in TestCfgContainer");
		return t;
	}
	public void LoadAll()
	{
		mMainCfgContainer = Load("MainCfg") as MainCfgContainer;
		mTestCfgContainer = Load("TestCfg") as TestCfgContainer;
	}

	private System.Object Load(string name)
	{
		IFormatter f = new BinaryFormatter();
		TextAsset text = Resources.Load<TextAsset>("ConfigBin/" + name);
		Stream s = new MemoryStream(text.bytes);
		System.Object obj = f.Deserialize(s);
		s.Close();
		return obj;
	}
}


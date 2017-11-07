using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ABAssetBuildMgr
{
	public class AssetBundleBuildEX
	{
		public string assetBundleName;

		public List<string> assetNames = new List<string>();

		public string assetBundleVariant;

		public ABAssetBuildMgr.E_ABBUNLDE_TYPE meType = ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER;

		public AssetBundleBuild Parse()
		{
			return new AssetBundleBuild
			{
				assetBundleName = this.assetBundleName,
				assetBundleVariant = this.assetBundleVariant,
				assetNames = this.assetNames.ToArray()
			};
		}

		public void RemoveAsset(string name)
		{
			int num = -1;
			for (int i = 0; i < this.assetNames.Count; i++)
			{
				if (name.Equals(this.assetNames[i]))
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this.assetNames.RemoveAt(num);
			}
		}
	}

	public enum E_ABBUNLDE_TYPE
	{
		E_SHADER,
		E_TEXTURE,
		E_EFFECT,
		E_SHARED,
		E_GlobalCommon,
		E_UI,
		E_BT,
		E_SCENECOMMON,
		E_SCENE,
		E_CHARACTER,
		E_ACTION,
		E_ACTORINFO,
		E_GAMEDATA,
		E_FONT,
		E_SOUND,
		E_WEAPON,
		E_GEM,
		E_ILLEGAL,
        E_CONFIG,
        E_CONFIG_MAP,
	}

	public class CollectRedundancy
	{
		public enum E_LEVEL
		{
			E_LEVEL1,
			E_LEVEL2,
			E_LEVEL3
		}

		public int miIndex;

		private static readonly int msSplitCount = 20;

		public ABAssetBuildMgr.E_ABBUNLDE_TYPE meType;

		public ABAssetBuildMgr.CollectRedundancy.E_LEVEL meLevel;

		public List<string> mAssetNames = new List<string>();

		public List<ABAssetBuildMgr.AssetBundleBuildEX> mABBundleExList = new List<ABAssetBuildMgr.AssetBundleBuildEX>();

		public CollectRedundancy(ABAssetBuildMgr.E_ABBUNLDE_TYPE tt, ABAssetBuildMgr.CollectRedundancy.E_LEVEL ll, List<string> assets)
		{
			this.meLevel = ll;
			this.meType = tt;
			this.mAssetNames = assets;
			if (assets != null && assets.Count > 0)
			{
				this.CreateABBundleEX();
			}
		}

		private ABAssetBuildMgr.AssetBundleBuildEX CreateABBundleEX()
		{
			string abName = string.Concat(new object[]
			{
				this.meType.ToString().ToLower(),
				this.meLevel.ToString().ToLower(),
				"shared",
				this.miIndex,
				AB_Common.AB_EXT
			});
			ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuild = ABAssetBuildMgr.GetAssetBundleBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_SHARED, abName);
			this.mABBundleExList.Add(assetBundleBuild);
			this.miIndex++;
			return assetBundleBuild;
		}

		public void Parse()
		{
			if (this.mAssetNames.Count < ABAssetBuildMgr.CollectRedundancy.msSplitCount)
			{
				this.ParseSimple();
				return;
			}
			this.ParseComplex();
		}

		private void ParseSimple()
		{
			this.mABBundleExList[0].assetNames.AddRange(this.mAssetNames);
		}

		private void ParseComplex()
		{
			this.CreateABBundleEX();
			this.CreateABBundleEX();
			for (int i = 0; i < this.mAssetNames.Count; i++)
			{
				string text = CFileManager.GetExtension(this.mAssetNames[i]).ToLower();
				if (text.Contains("jpg") || text.Contains("png") || text.Contains("tga"))
				{
					this.mABBundleExList[1].assetNames.Add(this.mAssetNames[i]);
				}
				else
				{
					this.mABBundleExList[0].assetNames.Add(this.mAssetNames[i]);
				}
			}
		}
	}

	private static Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, List<ABAssetBuildMgr.AssetBundleBuildEX>> mAssetBundleMap = new Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, List<ABAssetBuildMgr.AssetBundleBuildEX>>();

	private static string[] mExcludeExt = new string[]
	{
		".cs",
		".asset"
	};

	private static Dictionary<string, List<ABAssetBuildMgr.AssetBundleBuildEX>> mRedundancyMap = new Dictionary<string, List<ABAssetBuildMgr.AssetBundleBuildEX>>();

	public static AssetBundleManifest mManifest;

	public static ABAssetBuildMgr.AssetBundleBuildEX GetAssetBundleBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE tt, string abName)
	{
		if (!ABAssetBuildMgr.mAssetBundleMap.ContainsKey(tt))
		{
			List<ABAssetBuildMgr.AssetBundleBuildEX> value = new List<ABAssetBuildMgr.AssetBundleBuildEX>();
			ABAssetBuildMgr.mAssetBundleMap.Add(tt, value);
			ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX = new ABAssetBuildMgr.AssetBundleBuildEX();
			assetBundleBuildEX.assetBundleName = abName;
			assetBundleBuildEX.meType = tt;
			ABAssetBuildMgr.mAssetBundleMap[tt].Add(assetBundleBuildEX);
			return assetBundleBuildEX;
		}
		foreach (ABAssetBuildMgr.AssetBundleBuildEX current in ABAssetBuildMgr.mAssetBundleMap[tt])
		{
			if (current.assetBundleName.ToLower().Equals(abName.ToLower()))
			{
				return current;
			}
		}
		ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX2 = new ABAssetBuildMgr.AssetBundleBuildEX();
		assetBundleBuildEX2.assetBundleName = abName;
		assetBundleBuildEX2.meType = tt;
		ABAssetBuildMgr.mAssetBundleMap[tt].Add(assetBundleBuildEX2);
		return assetBundleBuildEX2;
	}

	public static ABAssetBuildMgr.AssetBundleBuildEX FindAssetBundleBuildEx(ABAssetBuildMgr.E_ABBUNLDE_TYPE tt, string name)
	{
		foreach (ABAssetBuildMgr.AssetBundleBuildEX current in ABAssetBuildMgr.mAssetBundleMap[tt])
		{
			if (current.assetBundleName.ToLower().Equals(name.ToLower()))
			{
				return current;
			}
		}
		return null;
	}

	public static ABAssetBuildMgr.AssetBundleBuildEX FindAssetBundleBuildEx(string name)
	{
		using (Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, List<ABAssetBuildMgr.AssetBundleBuildEX>>.KeyCollection.Enumerator enumerator = ABAssetBuildMgr.mAssetBundleMap.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX = ABAssetBuildMgr.FindAssetBundleBuildEx(enumerator.Current, name);
				if (assetBundleBuildEX != null)
				{
					return assetBundleBuildEX;
				}
			}
		}
		return null;
	}

	public static void AddAsset(ABAssetBuildMgr.AssetBundleBuildEX ab, string assetName)
	{
		if (!ABAssetBuildMgr.ContainAsset(ab, assetName))
		{
			bool flag = ABAssetBuildMgr.CheckRedundancy(assetName, ab);
			string extension = CFileManager.GetExtension(assetName);
			bool flag2 = false;
			for (int i = 0; i < ABAssetBuildMgr.mExcludeExt.Length; i++)
			{
				if (extension.ToLower().Equals(ABAssetBuildMgr.mExcludeExt[i]))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2 && !flag)
			{
				ab.assetNames.Add(assetName);
			}
		}
	}

	public static void ForceAddAsset(ABAssetBuildMgr.AssetBundleBuildEX ab, string assetName)
	{
		if (!ABAssetBuildMgr.ContainAsset(ab, assetName) && !ABAssetBuildMgr.CheckRedundancy(assetName, ab))
		{
			ab.assetNames.Add(assetName);
		}
	}

	public static bool ContainAsset(ABAssetBuildMgr.AssetBundleBuildEX ab, string assetname)
	{
		for (int i = 0; i < ab.assetNames.Count<string>(); i++)
		{
			if (assetname.ToLower().Equals(ab.assetNames[i].ToLower()))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainAsset(ABAssetBuildMgr.E_ABBUNLDE_TYPE tt, string assetname)
	{
		if (!ABAssetBuildMgr.mAssetBundleMap.ContainsKey(tt))
		{
			return false;
		}
		using (List<ABAssetBuildMgr.AssetBundleBuildEX>.Enumerator enumerator = ABAssetBuildMgr.mAssetBundleMap[tt].GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				bool flag = ABAssetBuildMgr.ContainAsset(enumerator.Current, assetname);
				if (flag)
				{
					return flag;
				}
			}
		}
		return false;
	}

	public static AssetBundleBuild[] Parse()
	{
		ABAssetBuildMgr.AutoCollectRedundancyData();
		AssetBundleBuild[] array = new AssetBundleBuild[ABAssetBuildMgr.Count()];
		int num = 0;
		using (Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, List<ABAssetBuildMgr.AssetBundleBuildEX>>.ValueCollection.Enumerator enumerator = ABAssetBuildMgr.mAssetBundleMap.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				foreach (ABAssetBuildMgr.AssetBundleBuildEX current in enumerator.Current)
				{
					array[num] = current.Parse();
					num++;
				}
			}
		}
		return array;
	}

	public static void Write()
	{
		FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/bundle.txt", FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		foreach (ABAssetBuildMgr.E_ABBUNLDE_TYPE current in ABAssetBuildMgr.mAssetBundleMap.Keys)
		{
			streamWriter.WriteLine("===============" + current.ToString() + "==============");
			foreach (ABAssetBuildMgr.AssetBundleBuildEX current2 in ABAssetBuildMgr.mAssetBundleMap[current])
			{
				streamWriter.WriteLine("AB Name: " + current2.assetBundleName);
				streamWriter.WriteLine("AB Variant: " + current2.assetBundleVariant);
				streamWriter.WriteLine("AB Asset Names: ");
				foreach (string current3 in current2.assetNames)
				{
					streamWriter.WriteLine("      " + current3);
				}
			}
		}
		streamWriter.WriteLine("------------------Redundancy--------------");
		foreach (string current4 in ABAssetBuildMgr.mRedundancyMap.Keys)
		{
			if (ABAssetBuildMgr.mRedundancyMap[current4].Count != 1)
			{
				streamWriter.WriteLine("Asset Name: " + current4);
				streamWriter.WriteLine("AB Names: ");
				foreach (ABAssetBuildMgr.AssetBundleBuildEX current5 in ABAssetBuildMgr.mRedundancyMap[current4])
				{
					streamWriter.WriteLine("      " + current5.assetBundleName);
				}
			}
		}
		streamWriter.Close();
		fileStream.Close();
	}

	private static int Count()
	{
		int num = 0;
		foreach (List<ABAssetBuildMgr.AssetBundleBuildEX> current in ABAssetBuildMgr.mAssetBundleMap.Values)
		{
			num += current.Count;
		}
		return num;
	}

	private static bool CheckRedundancy(string assetName, ABAssetBuildMgr.AssetBundleBuildEX bundle)
	{
		if (ABAssetBuildMgr.mRedundancyMap.ContainsKey(assetName))
		{
			bool flag = false;
			for (int i = 0; i < ABAssetBuildMgr.mRedundancyMap[assetName].Count; i++)
			{
				if (ABAssetBuildMgr.mRedundancyMap[assetName][i].assetBundleName == bundle.assetBundleName)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ABAssetBuildMgr.mRedundancyMap[assetName].Add(bundle);
			}
			return true;
		}
		ABAssetBuildMgr.mRedundancyMap.Add(assetName, new List<ABAssetBuildMgr.AssetBundleBuildEX>());
		ABAssetBuildMgr.mRedundancyMap[assetName].Add(bundle);
		return false;
	}

	public static void Clear()
	{
		ABAssetBuildMgr.mRedundancyMap.Clear();
		ABAssetBuildMgr.mAssetBundleMap.Clear();
	}

	public static void AutoCollectRedundancyData()
	{
		Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, Dictionary<ABAssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>> dictionary = new Dictionary<ABAssetBuildMgr.E_ABBUNLDE_TYPE, Dictionary<ABAssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>>();
		foreach (string current in ABAssetBuildMgr.mRedundancyMap.Keys)
		{
			if (ABAssetBuildMgr.mRedundancyMap[current].Count > 1)
			{
				ABAssetBuildMgr.mRedundancyMap[current][0].RemoveAsset(current);
				if (ABAssetBuildMgr.mRedundancyMap[current].Count > 2)
				{
					ABAssetBuildMgr.E_ABBUNLDE_TYPE redundancyType = ABAssetBuildMgr.GetRedundancyType(ABAssetBuildMgr.mRedundancyMap[current]);
					ABAssetBuildMgr.CollectRedundancy.E_LEVEL key = ABAssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL1;
					if (ABAssetBuildMgr.mRedundancyMap[current].Count > 3)
					{
						key = ABAssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL2;
					}
					if (ABAssetBuildMgr.mRedundancyMap[current].Count > 5)
					{
						key = ABAssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL3;
					}
					if (!dictionary.ContainsKey(redundancyType))
					{
						dictionary.Add(redundancyType, new Dictionary<ABAssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>());
					}
					if (!dictionary[redundancyType].ContainsKey(key))
					{
						dictionary[redundancyType].Add(key, new List<string>());
					}
					dictionary[redundancyType][key].Add(current);
				}
			}
		}
		List<ABAssetBuildMgr.CollectRedundancy> list = new List<ABAssetBuildMgr.CollectRedundancy>();
		foreach (ABAssetBuildMgr.E_ABBUNLDE_TYPE current2 in dictionary.Keys)
		{
			foreach (ABAssetBuildMgr.CollectRedundancy.E_LEVEL current3 in dictionary[current2].Keys)
			{
				if (dictionary[current2][current3].Count > 0)
				{
					ABAssetBuildMgr.CollectRedundancy collectRedundancy = new ABAssetBuildMgr.CollectRedundancy(current2, current3, dictionary[current2][current3]);
					collectRedundancy.Parse();
					list.Add(collectRedundancy);
				}
			}
		}
		AB_GatherResInfo.GatherCollectRedundancy(list);
	}

	private static ABAssetBuildMgr.E_ABBUNLDE_TYPE GetRedundancyType(List<ABAssetBuildMgr.AssetBundleBuildEX> abList)
	{
		ABAssetBuildMgr.E_ABBUNLDE_TYPE meType = abList[0].meType;
		for (int i = 1; i < abList.Count; i++)
		{
			if (meType != abList[i].meType)
			{
				return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_GlobalCommon;
			}
		}
		return meType;
	}

	public static void LoadManifest()
	{
		AssetBundle expr_14 = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/AssetBundle");
		ABAssetBuildMgr.mManifest = expr_14.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		expr_14.Unload(false);
	}
}

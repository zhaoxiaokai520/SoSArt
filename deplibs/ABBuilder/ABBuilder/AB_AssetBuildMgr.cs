using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AB_AssetBuildMgr
{
	public class AssetBundleBuildEX
	{
		public string assetBundleName;

		public List<string> assetNames = new List<string>();

		public string assetBundleVariant;

		public AB_AssetBuildMgr.E_ABBUNLDE_TYPE meType = AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER;

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
		E_ILLEGAL
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

		public AB_AssetBuildMgr.E_ABBUNLDE_TYPE meType;

		public AB_AssetBuildMgr.CollectRedundancy.E_LEVEL meLevel;

		public List<string> mAssetNames = new List<string>();

		public List<AB_AssetBuildMgr.AssetBundleBuildEX> mABBundleExList = new List<AB_AssetBuildMgr.AssetBundleBuildEX>();

		public CollectRedundancy(AB_AssetBuildMgr.E_ABBUNLDE_TYPE tt, AB_AssetBuildMgr.CollectRedundancy.E_LEVEL ll, List<string> assets)
		{
			this.meLevel = ll;
			this.meType = tt;
			this.mAssetNames = assets;
			if (assets != null && assets.Count > 0)
			{
				this.CreateABBundleEX();
			}
		}

		private AB_AssetBuildMgr.AssetBundleBuildEX CreateABBundleEX()
		{
			string abName = string.Concat(new object[]
			{
				this.meType.ToString().ToLower(),
				this.meLevel.ToString().ToLower(),
				"shared",
				this.miIndex,
				AB_Common.AB_EXT
			});
			AB_AssetBuildMgr.AssetBundleBuildEX assetBundleBuild = AB_AssetBuildMgr.GetAssetBundleBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SHARED, abName);
			this.mABBundleExList.Add(assetBundleBuild);
			this.miIndex++;
			return assetBundleBuild;
		}

		public void Parse()
		{
			if (this.mAssetNames.Count < AB_AssetBuildMgr.CollectRedundancy.msSplitCount)
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

	private static Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, List<AB_AssetBuildMgr.AssetBundleBuildEX>> mAssetBundleMap = new Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, List<AB_AssetBuildMgr.AssetBundleBuildEX>>();

	private static string[] mExcludeExt = new string[]
	{
		".cs",
		".asset"
	};

	private static Dictionary<string, List<AB_AssetBuildMgr.AssetBundleBuildEX>> mRedundancyMap = new Dictionary<string, List<AB_AssetBuildMgr.AssetBundleBuildEX>>();

	public static AssetBundleManifest mManifest;

	public static AB_AssetBuildMgr.AssetBundleBuildEX GetAssetBundleBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE tt, string abName)
	{
		if (!AB_AssetBuildMgr.mAssetBundleMap.ContainsKey(tt))
		{
			List<AB_AssetBuildMgr.AssetBundleBuildEX> value = new List<AB_AssetBuildMgr.AssetBundleBuildEX>();
			AB_AssetBuildMgr.mAssetBundleMap.Add(tt, value);
			AB_AssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX = new AB_AssetBuildMgr.AssetBundleBuildEX();
			assetBundleBuildEX.assetBundleName = abName;
			assetBundleBuildEX.meType = tt;
			AB_AssetBuildMgr.mAssetBundleMap[tt].Add(assetBundleBuildEX);
			return assetBundleBuildEX;
		}
		foreach (AB_AssetBuildMgr.AssetBundleBuildEX current in AB_AssetBuildMgr.mAssetBundleMap[tt])
		{
			if (current.assetBundleName.ToLower().Equals(abName.ToLower()))
			{
				return current;
			}
		}
		AB_AssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX2 = new AB_AssetBuildMgr.AssetBundleBuildEX();
		assetBundleBuildEX2.assetBundleName = abName;
		assetBundleBuildEX2.meType = tt;
		AB_AssetBuildMgr.mAssetBundleMap[tt].Add(assetBundleBuildEX2);
		return assetBundleBuildEX2;
	}

	public static AB_AssetBuildMgr.AssetBundleBuildEX FindAssetBundleBuildEx(AB_AssetBuildMgr.E_ABBUNLDE_TYPE tt, string name)
	{
		foreach (AB_AssetBuildMgr.AssetBundleBuildEX current in AB_AssetBuildMgr.mAssetBundleMap[tt])
		{
			if (current.assetBundleName.ToLower().Equals(name.ToLower()))
			{
				return current;
			}
		}
		return null;
	}

	public static AB_AssetBuildMgr.AssetBundleBuildEX FindAssetBundleBuildEx(string name)
	{
		using (Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, List<AB_AssetBuildMgr.AssetBundleBuildEX>>.KeyCollection.Enumerator enumerator = AB_AssetBuildMgr.mAssetBundleMap.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				AB_AssetBuildMgr.AssetBundleBuildEX assetBundleBuildEX = AB_AssetBuildMgr.FindAssetBundleBuildEx(enumerator.Current, name);
				if (assetBundleBuildEX != null)
				{
					return assetBundleBuildEX;
				}
			}
		}
		return null;
	}

	public static void AddAsset(AB_AssetBuildMgr.AssetBundleBuildEX ab, string assetName)
	{
		if (!AB_AssetBuildMgr.ContainAsset(ab, assetName))
		{
			bool flag = AB_AssetBuildMgr.CheckRedundancy(assetName, ab);
			string extension = CFileManager.GetExtension(assetName);
			bool flag2 = false;
			for (int i = 0; i < AB_AssetBuildMgr.mExcludeExt.Length; i++)
			{
				if (extension.ToLower().Equals(AB_AssetBuildMgr.mExcludeExt[i]))
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

	public static void ForceAddAsset(AB_AssetBuildMgr.AssetBundleBuildEX ab, string assetName)
	{
		if (!AB_AssetBuildMgr.ContainAsset(ab, assetName) && !AB_AssetBuildMgr.CheckRedundancy(assetName, ab))
		{
			ab.assetNames.Add(assetName);
		}
	}

	public static bool ContainAsset(AB_AssetBuildMgr.AssetBundleBuildEX ab, string assetname)
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

	public static bool ContainAsset(AB_AssetBuildMgr.E_ABBUNLDE_TYPE tt, string assetname)
	{
		if (!AB_AssetBuildMgr.mAssetBundleMap.ContainsKey(tt))
		{
			return false;
		}
		using (List<AB_AssetBuildMgr.AssetBundleBuildEX>.Enumerator enumerator = AB_AssetBuildMgr.mAssetBundleMap[tt].GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				bool flag = AB_AssetBuildMgr.ContainAsset(enumerator.Current, assetname);
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
		AB_AssetBuildMgr.AutoCollectRedundancyData();
		AssetBundleBuild[] array = new AssetBundleBuild[AB_AssetBuildMgr.Count()];
		int num = 0;
		using (Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, List<AB_AssetBuildMgr.AssetBundleBuildEX>>.ValueCollection.Enumerator enumerator = AB_AssetBuildMgr.mAssetBundleMap.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				foreach (AB_AssetBuildMgr.AssetBundleBuildEX current in enumerator.Current)
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
		FileStream fileStream = new FileStream(Application.get_streamingAssetsPath() + "/bundle.txt", FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		foreach (AB_AssetBuildMgr.E_ABBUNLDE_TYPE current in AB_AssetBuildMgr.mAssetBundleMap.Keys)
		{
			streamWriter.WriteLine("===============" + current.ToString() + "==============");
			foreach (AB_AssetBuildMgr.AssetBundleBuildEX current2 in AB_AssetBuildMgr.mAssetBundleMap[current])
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
		foreach (string current4 in AB_AssetBuildMgr.mRedundancyMap.Keys)
		{
			if (AB_AssetBuildMgr.mRedundancyMap[current4].Count != 1)
			{
				streamWriter.WriteLine("Asset Name: " + current4);
				streamWriter.WriteLine("AB Names: ");
				foreach (AB_AssetBuildMgr.AssetBundleBuildEX current5 in AB_AssetBuildMgr.mRedundancyMap[current4])
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
		foreach (List<AB_AssetBuildMgr.AssetBundleBuildEX> current in AB_AssetBuildMgr.mAssetBundleMap.Values)
		{
			num += current.Count;
		}
		return num;
	}

	private static bool CheckRedundancy(string assetName, AB_AssetBuildMgr.AssetBundleBuildEX bundle)
	{
		if (AB_AssetBuildMgr.mRedundancyMap.ContainsKey(assetName))
		{
			bool flag = false;
			for (int i = 0; i < AB_AssetBuildMgr.mRedundancyMap[assetName].Count; i++)
			{
				if (AB_AssetBuildMgr.mRedundancyMap[assetName][i].assetBundleName == bundle.assetBundleName)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				AB_AssetBuildMgr.mRedundancyMap[assetName].Add(bundle);
			}
			return true;
		}
		AB_AssetBuildMgr.mRedundancyMap.Add(assetName, new List<AB_AssetBuildMgr.AssetBundleBuildEX>());
		AB_AssetBuildMgr.mRedundancyMap[assetName].Add(bundle);
		return false;
	}

	public static void Clear()
	{
		AB_AssetBuildMgr.mRedundancyMap.Clear();
		AB_AssetBuildMgr.mAssetBundleMap.Clear();
	}

	public static void AutoCollectRedundancyData()
	{
		Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, Dictionary<AB_AssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>> dictionary = new Dictionary<AB_AssetBuildMgr.E_ABBUNLDE_TYPE, Dictionary<AB_AssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>>();
		foreach (string current in AB_AssetBuildMgr.mRedundancyMap.Keys)
		{
			if (AB_AssetBuildMgr.mRedundancyMap[current].Count > 1)
			{
				AB_AssetBuildMgr.mRedundancyMap[current][0].RemoveAsset(current);
				if (AB_AssetBuildMgr.mRedundancyMap[current].Count > 2)
				{
					AB_AssetBuildMgr.E_ABBUNLDE_TYPE redundancyType = AB_AssetBuildMgr.GetRedundancyType(AB_AssetBuildMgr.mRedundancyMap[current]);
					AB_AssetBuildMgr.CollectRedundancy.E_LEVEL key = AB_AssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL1;
					if (AB_AssetBuildMgr.mRedundancyMap[current].Count > 3)
					{
						key = AB_AssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL2;
					}
					if (AB_AssetBuildMgr.mRedundancyMap[current].Count > 5)
					{
						key = AB_AssetBuildMgr.CollectRedundancy.E_LEVEL.E_LEVEL3;
					}
					if (!dictionary.ContainsKey(redundancyType))
					{
						dictionary.Add(redundancyType, new Dictionary<AB_AssetBuildMgr.CollectRedundancy.E_LEVEL, List<string>>());
					}
					if (!dictionary[redundancyType].ContainsKey(key))
					{
						dictionary[redundancyType].Add(key, new List<string>());
					}
					dictionary[redundancyType][key].Add(current);
				}
			}
		}
		List<AB_AssetBuildMgr.CollectRedundancy> list = new List<AB_AssetBuildMgr.CollectRedundancy>();
		foreach (AB_AssetBuildMgr.E_ABBUNLDE_TYPE current2 in dictionary.Keys)
		{
			foreach (AB_AssetBuildMgr.CollectRedundancy.E_LEVEL current3 in dictionary[current2].Keys)
			{
				if (dictionary[current2][current3].Count > 0)
				{
					AB_AssetBuildMgr.CollectRedundancy collectRedundancy = new AB_AssetBuildMgr.CollectRedundancy(current2, current3, dictionary[current2][current3]);
					collectRedundancy.Parse();
					list.Add(collectRedundancy);
				}
			}
		}
		AB_GatherResInfo.GatherCollectRedundancy(list);
	}

	private static AB_AssetBuildMgr.E_ABBUNLDE_TYPE GetRedundancyType(List<AB_AssetBuildMgr.AssetBundleBuildEX> abList)
	{
		AB_AssetBuildMgr.E_ABBUNLDE_TYPE meType = abList[0].meType;
		for (int i = 1; i < abList.Count; i++)
		{
			if (meType != abList[i].meType)
			{
				return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_GlobalCommon;
			}
		}
		return meType;
	}

	public static void LoadManifest()
	{
		AssetBundle expr_14 = AssetBundle.LoadFromFile(Application.get_streamingAssetsPath() + "/AssetBundle/AssetBundle");
		AB_AssetBuildMgr.mManifest = expr_14.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		expr_14.Unload(false);
	}
}

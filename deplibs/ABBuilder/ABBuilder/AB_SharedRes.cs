using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AB_SharedRes
{
	public static string msSharedFolder = "Assets/Exporter/Common/";

	public static List<AssetGroupInfo_t> mResPackerInfos = new List<AssetGroupInfo_t>();

	public static List<DirectoryInfo> mFolders = new List<DirectoryInfo>();

	public static Dictionary<string, int> mSharedResMap = new Dictionary<string, int>();

	public static Dictionary<string, AB_HeroCmdAction> mCommonActionMap = new Dictionary<string, AB_HeroCmdAction>();

	public static List<AB_HeroPacketBuild> mPackerBuilderList = new List<AB_HeroPacketBuild>();

	public static void Init()
	{
		AB_SharedRes.mResPackerInfos.Clear();
		AB_SharedRes.mFolders.Clear();
		AB_SharedRes.mSharedResMap.Clear();
		AB_SharedRes.mCommonActionMap.Clear();
		AB_SharedRes.mPackerBuilderList.Clear();
	}

	public static void BuildSharedRes()
	{
		if (AB_Common.bOldBuildMethod)
		{
			Debug.LogWarning("Haven't implement for U4!!");
			return;
		}
		AB_SharedRes.BuildSharedResU5Ex();
	}

	private static void BuildSharedResU5()
	{
		AB_SharedRes.FindFolder(new DirectoryInfo(AB_SharedRes.msSharedFolder));
		for (int i = 0; i < AB_SharedRes.mFolders.Count; i++)
		{
			AB_SharedRes.BuildAssetBundle(AB_SharedRes.mFolders[i]);
			AB_SharedRes.BuildResourceInfo(AB_SharedRes.mFolders[i]);
		}
	}

	private static void BuildAssetBundle(DirectoryInfo dd)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (AB_SharedRes.GetFolder(dd.FullName, ref empty, ref empty2))
		{
			string assetBundleName = AB_SharedRes.GetAssetBundleName(empty, empty2);
			AB_AssetBuildMgr.AssetBundleBuildEX assetBundleBuild = AB_AssetBuildMgr.GetAssetBundleBuild(AB_SharedRes.GetType(empty), assetBundleName);
			if (dd.Name.ToLower().Equals("action") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "equip") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "soldiereffect"))
			{
				AB_HeroCmdAction aB_HeroCmdAction = new AB_HeroCmdAction(dd);
				AB_SharedRes.mCommonActionMap.Add(dd.Name, aB_HeroCmdAction);
				aB_HeroCmdAction.BuildCmd();
				AssetDatabase.Refresh();
				List<string> aBFileList = aB_HeroCmdAction.GetABFileList();
				for (int i = 0; i < aBFileList.Count; i++)
				{
					AB_AssetBuildMgr.AddAsset(assetBundleBuild, AB_Common.Absolute2RelativePath(aBFileList[i]));
				}
				return;
			}
			FileInfo[] files = dd.GetFiles();
			for (int j = 0; j < files.Length; j++)
			{
				FileInfo fileInfo = files[j];
				if (!fileInfo.Extension.Equals(".meta") && !empty.ToLower().Contains("action"))
				{
					if (fileInfo.Extension.Equals(".prefab"))
					{
						string[] dependencies = AssetDatabase.GetDependencies(AB_Common.Absolute2RelativePath(fileInfo.FullName));
						for (int k = 0; k < dependencies.Length; k++)
						{
							if (!AB_AssetBuildMgr.ContainAsset(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SHADER, dependencies[k]))
							{
								if (!AB_SharedRes.mSharedResMap.ContainsKey(dependencies[k]))
								{
									AB_SharedRes.mSharedResMap.Add(dependencies[k], 1);
									AB_AssetBuildMgr.AddAsset(assetBundleBuild, dependencies[k]);
								}
								Dictionary<string, int> arg_1B9_0 = AB_SharedRes.mSharedResMap;
								string key = dependencies[k];
								int num = arg_1B9_0[key];
								arg_1B9_0[key] = num + 1;
							}
						}
					}
					else
					{
						string text = AB_Common.Absolute2RelativePath(fileInfo.FullName);
						if (!AB_SharedRes.mSharedResMap.ContainsKey(text))
						{
							AB_SharedRes.mSharedResMap.Add(text, 1);
							AB_AssetBuildMgr.AddAsset(assetBundleBuild, text);
						}
						Dictionary<string, int> arg_218_0 = AB_SharedRes.mSharedResMap;
						string key = text;
						int num = arg_218_0[key];
						arg_218_0[key] = num + 1;
					}
				}
			}
		}
	}

	private static void BuildResourceInfo(DirectoryInfo dd)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (AB_SharedRes.GetFolder(dd.FullName, ref empty, ref empty2))
		{
			string assetGroupBundleName = AB_SharedRes.GetAssetGroupBundleName(empty, empty2);
			AssetGroupInfo_t assetGroupInfo_t = AB_GatherResInfo.CreateCResourcePackerInfo();
			assetGroupInfo_t.m_pathInIFS = assetGroupBundleName;
			assetGroupInfo_t.m_tag = AB_SharedRes.GetTag(empty);
			if (dd.Name.ToLower().Equals("action") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "equip") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "soldiereffect"))
			{
				assetGroupInfo_t.SetFlag(eBundleFlag.Resident);
				foreach (string current in AB_SharedRes.mCommonActionMap[dd.Name].GetAssetGroupFileList())
				{
					AssetInfo_t item = default(AssetInfo_t);
					item.m_pathName = CFileManager.EraseExtension(AB_Common.PathRemoveAssets(current));
					item.m_extension = "bytes";
					assetGroupInfo_t.m_resourceInfos.Add(item);
				}
				AB_SharedRes.mResPackerInfos.Add(assetGroupInfo_t);
				return;
			}
			if (empty.ToLower().Contains("font"))
			{
				assetGroupInfo_t.SetFlag(eBundleFlag.Resident);
			}
			if (empty.ToLower().Contains("texture") && empty2.ToLower().Contains("common"))
			{
				assetGroupInfo_t.m_tag = 3;
				assetGroupInfo_t.SetFlag(eBundleFlag.PermanentAfterLoad);
			}
			FileInfo[] files = dd.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				if (!fileInfo.Extension.Equals(".meta") && !empty.ToLower().Contains("action"))
				{
					AssetInfo_t item2 = default(AssetInfo_t);
					item2.m_pathName = CFileManager.EraseExtension(AB_Common.PathRemoveAssets(fileInfo.FullName));
					item2.m_extension = fileInfo.Extension;
					assetGroupInfo_t.m_resourceInfos.Add(item2);
				}
			}
			AB_SharedRes.mResPackerInfos.Add(assetGroupInfo_t);
		}
	}

	private static void FindFolder(DirectoryInfo path)
	{
		FileInfo[] files = path.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			if (files[i].Extension != ".meta")
			{
				AB_SharedRes.mFolders.Add(path);
				break;
			}
		}
		DirectoryInfo[] directories = path.GetDirectories();
		for (int j = 0; j < directories.Length; j++)
		{
			AB_SharedRes.FindFolder(directories[j]);
		}
	}

	private static bool GetFolder(string path, ref string folder1, ref string folder2)
	{
		path = path.Replace("\\", "/");
		int num = path.IndexOf(AB_SharedRes.msSharedFolder);
		if (num == -1)
		{
			return false;
		}
		string text = path.Substring(num + AB_SharedRes.msSharedFolder.Length);
		if (text.Contains("/"))
		{
			string[] array = text.Split(new char[]
			{
				'/'
			});
			folder1 = array[0];
			folder2 = array[array.Length - 1];
		}
		else
		{
			folder1 = text;
			folder2 = string.Empty;
		}
		return true;
	}

	private static AB_AssetBuildMgr.E_ABBUNLDE_TYPE GetType(string folder1)
	{
		if (folder1.ToLower().Contains("texture") || folder1.ToLower().Contains("skymanage"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_TEXTURE;
		}
		if (folder1.ToLower().Contains("action"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTION;
		}
		if (folder1.ToLower().Contains("actorinfo"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTORINFO;
		}
		if (folder1.ToLower().Contains("effect"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		if (folder1.ToLower().Contains("gamedata") || folder1.ToLower().Contains("scenedesign"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_GAMEDATA;
		}
		if (folder1.ToLower().Contains("font"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_FONT;
		}
		if (folder1.ToLower().Equals("ui") || folder1.ToLower().Equals("icon"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_UI;
		}
		if (folder1.ToLower().Contains("sound"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SOUND;
		}
		if (folder1.ToLower().Equals("equipment"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		if (folder1.ToLower().Equals("uimodel"))
		{
			return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		Debug.LogError("Error Type: " + folder1);
		return AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ILLEGAL;
	}

	private static int GetTag(string folder1)
	{
		if (folder1.ToLower().Contains("texture") || folder1.ToLower().Contains("skymanage"))
		{
			return 8;
		}
		if (folder1.ToLower().Contains("action"))
		{
			return 5;
		}
		if (folder1.ToLower().Contains("actorinfo"))
		{
			return 6;
		}
		if (folder1.ToLower().Contains("effect"))
		{
			return 11;
		}
		if (folder1.ToLower().Contains("gamedata") || folder1.ToLower().Contains("scenedesign"))
		{
			return 4;
		}
		if (folder1.ToLower().Contains("font"))
		{
			return 13;
		}
		if (folder1.ToLower().Equals("ui") || folder1.ToLower().Equals("icon"))
		{
			return 3;
		}
		if (folder1.ToLower().Contains("sound"))
		{
			return 12;
		}
		Debug.LogError("Error Tag: " + folder1);
		return 17;
	}

	private static void BuildSharedResU5Ex()
	{
		AB_SharedRes.FindFolder(new DirectoryInfo(AB_SharedRes.msSharedFolder));
		for (int i = 0; i < AB_SharedRes.mFolders.Count; i++)
		{
			AB_SharedRes.BuildAssetBundleEx(AB_SharedRes.mFolders[i]);
		}
		using (List<AB_HeroPacketBuild>.Enumerator enumerator = AB_SharedRes.mPackerBuilderList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				foreach (string current in enumerator.Current.mAllABFileList.Keys)
				{
					if (!AB_SharedRes.mSharedResMap.ContainsKey(current))
					{
						AB_SharedRes.mSharedResMap.Add(current, 1);
					}
				}
			}
		}
	}

	private static void BuildAssetBundleEx(DirectoryInfo dd)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (AB_SharedRes.GetFolder(dd.FullName, ref empty, ref empty2))
		{
			string assetBundleName = AB_SharedRes.GetAssetBundleName(empty, empty2);
			int num = 0;
			if (empty.ToLower().Contains("font"))
			{
				num |= 4;
			}
			else if (empty.ToLower().Contains("texture") && empty2.ToLower().Contains("common"))
			{
				num |= 64;
			}
			else if (empty.ToLower().Equals("effect") || empty.ToLower().Equals("equipment") || empty.ToLower().Equals("indicator_effect") || empty.ToLower().Equals("scenedesign") || empty.ToLower().Equals("sound") || empty.ToLower().Equals("texture") || empty.ToLower().Contains("head") || empty.ToLower().Contains("icon"))
			{
				num |= 64;
			}
			if (empty.ToLower().Contains("newplayaction") || empty.ToLower().Equals("action"))
			{
				num |= 8;
				num |= 64;
			}
			AB_HeroPacketBuild aB_HeroPacketBuild = new AB_HeroPacketBuild(AB_SharedRes.GetType(empty), assetBundleName, num);
			AB_HeroCmdBase cmd = AB_SharedRes.CreateCmd(empty, dd);
			aB_HeroPacketBuild.AddCmd(cmd);
			aB_HeroPacketBuild.BuildCmd();
			aB_HeroPacketBuild.BuildRes(null, true);
			AB_SharedRes.mPackerBuilderList.Add(aB_HeroPacketBuild);
		}
	}

	private static AB_HeroCmdBase CreateCmd(string folder1, DirectoryInfo folder1Info)
	{
		if (folder1.ToLower().Contains("texture") || folder1.ToLower().Equals("icon"))
		{
			return new AB_HeroCmdIcon(folder1Info);
		}
		if (folder1.ToLower().Equals("skymanage"))
		{
			return new AB_SharedCmd(folder1Info);
		}
		if (folder1.ToLower().Contains("action"))
		{
			return new AB_HeroCmdAction(folder1Info);
		}
		if (folder1.ToLower().Contains("actorinfo"))
		{
			return new AB_HeroCmdActorInfo(folder1Info);
		}
		if (folder1.ToLower().Contains("effect") || folder1.ToLower().Equals("equipment"))
		{
			return new AB_HeroCmdEffect(folder1Info);
		}
		if (folder1.ToLower().Contains("scenedesign"))
		{
			return new AB_HeroCmdIcon(folder1Info);
		}
		if (folder1.ToLower().Contains("font"))
		{
			return new AB_SharedCmd(folder1Info);
		}
		if (folder1.ToLower().Contains("sound"))
		{
			return new AB_HeroCmdSound(folder1Info);
		}
		Debug.LogWarning("Other Cmd: " + folder1);
		return new AB_SharedCmd(folder1Info);
	}

	private static string GetAssetBundleName(string folder1, string folder2)
	{
		return ("shared" + folder1 + folder2).ToLower() + AB_Common.AB_EXT;
	}

	private static string GetAssetGroupBundleName(string folder1, string folder2)
	{
		string text = "shared" + folder1 + folder2;
		return AB_Common.AB_RES_INFO_PATH + text.ToLower() + AB_Common.AB_EXT;
	}
}

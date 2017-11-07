using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABSharedRes
{
	public static string msSharedFolder = "Assets/RawRes/";

	public static List<AssetGroupInfo_t> mResPackerInfos = new List<AssetGroupInfo_t>();

	public static List<DirectoryInfo> mFolders = new List<DirectoryInfo>();

	public static Dictionary<string, int> mSharedResMap = new Dictionary<string, int>();

	public static Dictionary<string, AB_HeroCmdAction> mCommonActionMap = new Dictionary<string, AB_HeroCmdAction>();

	public static void Init()
	{
		ABSharedRes.mResPackerInfos.Clear();
		ABSharedRes.mFolders.Clear();
		ABSharedRes.mSharedResMap.Clear();
		ABSharedRes.mCommonActionMap.Clear();
	}

	public static void BuildSharedRes()
	{
		ABSharedRes.BuildSharedResU5();
	}

	private static void BuildSharedResU5()
	{
		ABSharedRes.FindFolder(new DirectoryInfo(msSharedFolder), "Public*");
		for (int i = 0; i < ABSharedRes.mFolders.Count; i++)
		{
			ABSharedRes.BuildAssetBundle(ABSharedRes.mFolders[i]);
			ABSharedRes.BuildResourceInfo(ABSharedRes.mFolders[i]);
		}
	}

	private static void BuildAssetBundle(DirectoryInfo dd)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (ABSharedRes.GetFolder(dd.FullName, ref empty, ref empty2))
		{
			string assetBundleName = ABSharedRes.GetAssetBundleName(empty, empty2);
			ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuild = ABAssetBuildMgr.GetAssetBundleBuild(ABSharedRes.GetType(empty), assetBundleName);
			if (dd.Name.ToLower().Equals("action") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "equip") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "soldiereffect"))
			{
				AB_HeroCmdAction aB_HeroCmdAction = new AB_HeroCmdAction(dd);
				ABSharedRes.mCommonActionMap.Add(dd.Name, aB_HeroCmdAction);
				aB_HeroCmdAction.BuildCmd();
				AssetDatabase.Refresh();
				List<string> aBFileList = aB_HeroCmdAction.GetABFileList();
				for (int i = 0; i < aBFileList.Count; i++)
				{
					ABAssetBuildMgr.AddAsset(assetBundleBuild, AB_Common.Absolute2RelativePath(aBFileList[i]));
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
							if (!ABAssetBuildMgr.ContainAsset(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_SHADER, dependencies[k]))
							{
								if (!ABSharedRes.mSharedResMap.ContainsKey(dependencies[k]))
								{
									ABSharedRes.mSharedResMap.Add(dependencies[k], 1);
									ABAssetBuildMgr.AddAsset(assetBundleBuild, dependencies[k]);
								}
								Dictionary<string, int> arg_1B9_0 = ABSharedRes.mSharedResMap;
								string key = dependencies[k];
								int num = arg_1B9_0[key];
								arg_1B9_0[key] = num + 1;
							}
						}
					}
					else
					{
						string text = AB_Common.Absolute2RelativePath(fileInfo.FullName);
						if (!ABSharedRes.mSharedResMap.ContainsKey(text))
						{
							ABSharedRes.mSharedResMap.Add(text, 1);
							ABAssetBuildMgr.AddAsset(assetBundleBuild, text);
						}
						Dictionary<string, int> arg_218_0 = ABSharedRes.mSharedResMap;
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
		if (ABSharedRes.GetFolder(dd.FullName, ref empty, ref empty2))
		{
			string assetGroupBundleName = ABSharedRes.GetAssetGroupBundleName(empty, empty2);
			AssetGroupInfo_t assetGroupInfo_t = AB_GatherResInfo.CreateCResourcePackerInfo();
			assetGroupInfo_t.m_pathInIFS = assetGroupBundleName;
			assetGroupInfo_t.m_tag = ABSharedRes.GetTag(empty);
			if (dd.Name.ToLower().Equals("action") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "equip") || dd.FullName.ToLower().Contains("newplayaction" + Path.DirectorySeparatorChar.ToString() + "soldiereffect"))
			{
				assetGroupInfo_t.SetFlag(eBundleFlag.Resident);
				foreach (string current in ABSharedRes.mCommonActionMap[dd.Name].GetAssetGroupFileList())
				{
					AssetInfo_t item = default(AssetInfo_t);
					item.m_pathName = CFileManager.EraseExtension(AB_Common.PathRemoveAssets(current));
					item.m_extension = "bytes";
					assetGroupInfo_t.m_resourceInfos.Add(item);
				}
				ABSharedRes.mResPackerInfos.Add(assetGroupInfo_t);
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
			ABSharedRes.mResPackerInfos.Add(assetGroupInfo_t);
		}
	}

	private static void FindFolder(DirectoryInfo path)
	{
		FileInfo[] files = path.GetFiles("Public*");
		for (int i = 0; i < files.Length; i++)
		{
			if (files[i].Extension != ".meta")
			{
				ABSharedRes.mFolders.Add(path);
				break;
			}
		}
		DirectoryInfo[] directories = path.GetDirectories();
		for (int j = 0; j < directories.Length; j++)
		{
			ABSharedRes.FindFolder(directories[j]);
		}
	}

    private static void FindFolder(DirectoryInfo path, string searchPattern)
    {
        DirectoryInfo[] dirs = path.GetDirectories(searchPattern);
        for (int i = 0; i < dirs.Length; i++)
        {
            if (null != dirs[i])
            {
                ABSharedRes.mFolders.Add(dirs[i]);
            }
        }
    }

    private static bool GetFolder(string path, ref string folder1, ref string folder2)
	{
		path = path.Replace("\\", "/");
		int num = path.IndexOf(ABSharedRes.msSharedFolder);
		if (num == -1)
		{
			return false;
		}
		string text = path.Substring(num + ABSharedRes.msSharedFolder.Length);
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

	private static ABAssetBuildMgr.E_ABBUNLDE_TYPE GetType(string folder1)
	{
		if (folder1.ToLower().Contains("texture") || folder1.ToLower().Contains("skymanage"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_TEXTURE;
		}
		if (folder1.ToLower().Contains("action"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTION;
		}
		if (folder1.ToLower().Contains("actorinfo"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTORINFO;
		}
		if (folder1.ToLower().Contains("effect"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		if (folder1.ToLower().Contains("gamedata") || folder1.ToLower().Contains("scenedesign"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_GAMEDATA;
		}
		if (folder1.ToLower().Contains("font"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_FONT;
		}
		if (folder1.ToLower().Equals("ui") || folder1.ToLower().Equals("icon"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_UI;
		}
		if (folder1.ToLower().Contains("sound"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_SOUND;
		}
		if (folder1.ToLower().Equals("equipment"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		if (folder1.ToLower().Equals("uimodel"))
		{
			return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT;
		}
		Debug.LogError("Error Type: " + folder1);
		return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ILLEGAL;
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

	private static ABCmdBase CreateCmd(string folder1, DirectoryInfo folder1Info)
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
		return ("public" + folder1 + folder2).ToLower() + AB_Common.AB_EXT;
	}

	private static string GetAssetGroupBundleName(string folder1, string folder2)
	{
		string text = "public" + folder1 + folder2;
		return AB_Common.AB_RES_INFO_PATH + text.ToLower() + AB_Common.AB_EXT;
	}
}

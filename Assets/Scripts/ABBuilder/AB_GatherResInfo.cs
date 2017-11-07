using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AB_GatherResInfo
{
	private static AssetManifest_t mResPackerInfoSet;

	private static string msAssetGroupInfoSetName = "ManifestInfo.bytes";

	private static List<AssetGroupInfo_t> mAssetGroupInfos = new List<AssetGroupInfo_t>();

	[MenuItem("AssetsBundle/Load Resource Packer")]
	public static bool LoadAssetGroupSet()
	{
		bool result = false;
		if (CFileManager.IsFileExist(AB_GatherResInfo.GetAssetGroupInfoSetPath()))
		{
			byte[] array = CFileManager.ReadFile(AB_GatherResInfo.GetAssetGroupInfoSetPath());
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = CFileManager.Decode(array[i]);
			}
			int num = 0;
			AB_GatherResInfo.mResPackerInfoSet = new AssetManifest_t();
			AB_GatherResInfo.mResPackerInfoSet.Read(array, ref num);
			if (num > 0)
			{
				result = true;
			}
		}
		return result;
	}

	public static void GatherResInfo()
	{
		AB_GatherResInfo.mResPackerInfoSet = new AssetManifest_t();
		foreach (AssetGroupInfo_t current in AB_GatherResInfo.mAssetGroupInfos)
		{
			AB_GatherResInfo.mResPackerInfoSet.AddAssetGroupInfo(current);
		}
	}

	public static void PostBuild()
	{
		AB_GatherResInfo.MakeManifest2PackerDependency();
		AB_GatherResInfo.SaveAssetGroupSet();
	}

	public static string GenerateABMD5()
	{
		if (ABAssetBuildMgr.mManifest == null)
		{
			ABAssetBuildMgr.LoadManifest();
		}
		string[] allAssetBundles = ABAssetBuildMgr.mManifest.GetAllAssetBundles();
		string text = "";
		for (int i = 0; i < allAssetBundles.Length; i++)
		{
			text += ABAssetBuildMgr.mManifest.GetAssetBundleHash(allAssetBundles[i]);
		}
		return CFileManager.GetMd5(text);
	}

	private static void MakeManifest2PackerDependency()
	{
		ABAssetBuildMgr.LoadManifest();
		foreach (string current in AB_GatherResInfo.mResPackerInfoSet.m_assetGroupInfosAll.Keys)
		{
			string text = Path.GetFileName(current);
			if (!text.Contains(AB_Common.AB_EXT))
			{
				text += AB_Common.AB_EXT;
			}
			AssetGroupInfo_t assetGroupInfo_t = AB_GatherResInfo.mResPackerInfoSet.m_assetGroupInfosAll[current];
			string[] allDependencies = ABAssetBuildMgr.mManifest.GetAllDependencies(text);
			for (int i = 0; i < allDependencies.Length; i++)
			{
				string text2 = allDependencies[i];
				if (!text2.Contains("shaderlist"))
				{
					assetGroupInfo_t.m_dependencies.Add(AB_Common.AB_RES_INFO_PATH + text2);
				}
			}
		}
	}

	public static void Init()
	{
		AB_GatherResInfo.mAssetGroupInfos.Clear();
	}

	private static bool GatherSharedResList()
	{
		for (int i = 0; i < ABSharedRes.mResPackerInfos.Count; i++)
		{
			if (!AB_GatherResInfo.mResPackerInfoSet.m_assetGroupInfosAll.ContainsKey(ABSharedRes.mResPackerInfos[i].m_pathInIFS))
			{
				AB_GatherResInfo.mResPackerInfoSet.AddAssetGroupInfo(ABSharedRes.mResPackerInfos[i]);
			}
			else
			{
				AB_GatherResInfo.mResPackerInfoSet.m_assetGroupInfosAll[ABSharedRes.mResPackerInfos[i].m_pathInIFS] = ABSharedRes.mResPackerInfos[i];
			}
		}
		return true;
	}

	public static void GatherCollectRedundancy(List<ABAssetBuildMgr.CollectRedundancy> ll)
	{
		foreach (ABAssetBuildMgr.CollectRedundancy current in ll)
		{
			for (int i = 0; i < current.mABBundleExList.Count; i++)
			{
				AssetGroupInfo_t assetGroupInfo_t = AB_GatherResInfo.CreateCResourcePackerInfo();
				assetGroupInfo_t.m_pathInIFS = AB_Common.AB_RES_INFO_PATH + current.mABBundleExList[i].assetBundleName.ToLower();
				assetGroupInfo_t.m_tag = 14;
				foreach (string current2 in current.mABBundleExList[i].assetNames)
				{
					AssetInfo_t item = default(AssetInfo_t);
					item.m_pathName = CFileManager.EraseExtension(AB_Common.PathRemoveAssets(current2));
					item.m_extension = CFileManager.GetExtension(current2);
					assetGroupInfo_t.m_resourceInfos.Add(item);
				}
				AB_GatherResInfo.mResPackerInfoSet.AddAssetGroupInfo(assetGroupInfo_t);
			}
		}
	}

	public static string GetAssetGroupInfoSetPath()
	{
		return AB_Common.AB_RESINFO_PATH + AB_GatherResInfo.msAssetGroupInfoSetName;
	}

	public static void SaveAssetGroupSet()
	{
		if (AB_GatherResInfo.mResPackerInfoSet != null)
		{
			byte[] array = new byte[2097152];
			int num = 0;
			AB_GatherResInfo.mResPackerInfoSet.m_version = AB_Common.AB_VERSION;
			AB_GatherResInfo.mResPackerInfoSet.m_publish = AB_GatherResInfo.GenerateABMD5();
			AB_GatherResInfo.mResPackerInfoSet.m_pakPath = AB_GatherResInfo.DefaultIFSPath();
			AB_GatherResInfo.mResPackerInfoSet.Write(array, ref num);
			for (int i = 0; i < num; i++)
			{
				array[i] = CFileManager.Encode(array[i]);
			}
			CFileManager.WriteFile(AB_GatherResInfo.GetAssetGroupInfoSetPath(), array, 0, num);
			Debug.Log("AssetManifest_t Version: " + AB_GatherResInfo.mResPackerInfoSet.m_version);
		}
	}

	public static AssetGroupInfo_t CreateCResourcePackerInfo()
	{
		AssetGroupInfo_t assetGroupInfo_t = new AssetGroupInfo_t(true);
		AB_GatherResInfo.mAssetGroupInfos.Add(assetGroupInfo_t);
		return assetGroupInfo_t;
	}

	public static string GetVersion()
	{
		return AB_GatherResInfo.mResPackerInfoSet.m_version;
	}

	public static string GetPublish()
	{
		return AB_GatherResInfo.mResPackerInfoSet.m_publish;
	}

	private static string DefaultIFSPath()
	{
		return AB_Common.AB_IFS_PATH;
	}
}

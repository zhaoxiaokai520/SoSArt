using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABSharedRes
{
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
        List<DirectoryInfo> folders = new List<DirectoryInfo>();

        ABUtils.FindFolder(new DirectoryInfo(ABUtils.BaseFolder), "Public*", folders);
        for (int i = 0; i < folders.Count; i++)
        {
            ABSharedRes.BuildAssetBundle(folders[i]);
            ABSharedRes.BuildResourceInfo(folders[i]);
        }
    }

    private static void BuildAssetBundle(DirectoryInfo dd)
    {
        string empty = string.Empty;
        string empty2 = string.Empty;
        if (ABUtils.GetFolder(dd.FullName, ref empty, ref empty2))
        {
            string assetBundleName = ABUtils.GetAssetBundleName(empty, empty2);
            ABAssetBuildMgr.AssetBundleBuildEX assetBundleBuild = ABAssetBuildMgr.GetAssetBundleBuild(ABSharedRes.GetType(empty), assetBundleName);

            FileInfo[] files = dd.GetFiles();
            for (int j = 0; j < files.Length; j++)
            {
                FileInfo fileInfo = files[j];
                if (!fileInfo.Extension.Equals(".meta"))
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
                                    ABAssetBuildMgr.AddAsset(assetBundleBuild, dependencies[k], true);
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
                            ABAssetBuildMgr.AddAsset(assetBundleBuild, text, true);
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
        if (ABUtils.GetFolder(dd.FullName, ref empty, ref empty2))
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

    private static ABAssetBuildMgr.E_ABBUNLDE_TYPE GetType(string folder1)
    {
        if (folder1.ToLower().Contains("config.map"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CONFIG_MAP;
        }

        if (folder1.ToLower().Contains("config"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CONFIG;
        }

        Debug.LogError("Error Type: " + folder1);
        return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ILLEGAL;
    }

    private static int GetTag(string folder1)
    {
        if (folder1.ToLower().Contains("config"))
        {
            return 1;
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

    private static string GetAssetGroupBundleName(string folder1, string folder2)
    {
        string text = "public" + folder1 + folder2;
        return AB_Common.AB_RES_INFO_PATH + text.ToLower() + AB_Common.AB_EXT;
    }
}

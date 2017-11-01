using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AB_GatherResInfo {
    static AssetManifest_t mResPackerInfoSet;
    private static string msAssetGroupInfoSetName = "ManifestInfo.bytes";
    static List<AssetGroupInfo_t> mAssetGroupInfos = new List<AssetGroupInfo_t>();

    [MenuItem("AssetsBundle/Load Resource Packer")]
    public static bool LoadAssetGroupSet()
    {
        bool ret = false;
        if (CFileManager.IsFileExist(GetAssetGroupInfoSetPath()))
        {
            byte[] data = CFileManager.ReadFile(GetAssetGroupInfoSetPath());
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = CFileManager.Decode(data[i]);
            }
            int num = 0;
            mResPackerInfoSet = new AssetManifest_t();
            mResPackerInfoSet.Read(data, ref num);
            if(num > 0)
            {
                ret = true;
            }
        }
        return ret;
    }

    /*
     * this only check resourcepacker simplely, because of dependencies and common
     * is not right without build ab
     */
#if false
    [MenuItem("AssetsBundle/Build Resource Packer")]
    static void BuildAssetGroupSet()
    {
        //AB_SKIP_BUILD = true, buildscene and buildshader only build resource info packer
        AB_Common.AB_SKIP_BUILD = true;
        AB_Common.InitAll();
        AB_HeroList.BuildHero();
        AB_SceneList.BuildScene();
        AB_GameData.BuildGameData();
        AB_SharedRes.BuildSharedRes();
        GatherResInfo();
    }
#endif

    public static void GatherResInfo()
    {
        mResPackerInfoSet = new AssetManifest_t();
        foreach (AssetGroupInfo_t info in mAssetGroupInfos)
        {
            mResPackerInfoSet.AddAssetGroupInfo(info);
        }
    }

    public static void PostBuild()
    {
        MakeManifest2PackerDependency();
        SaveAssetGroupSet();
    }

    public static string GenerateABMD5()
    {
        if (AB_AssetBuildMgr.mManifest == null)
        {
            AB_AssetBuildMgr.LoadManifest();
        }
        string[] fileName = AB_AssetBuildMgr.mManifest.GetAllAssetBundles();
        string retStr = "";
        for (int i = 0; i < fileName.Length; i++)
        {
            retStr += AB_AssetBuildMgr.mManifest.GetAssetBundleHash(fileName[i]);
        }
        return CFileManager.GetMd5(retStr);
    }

    static void MakeManifest2PackerDependency()
    {
        AB_AssetBuildMgr.LoadManifest();
        foreach (string str in mResPackerInfoSet.m_assetGroupInfosAll.Keys)
        {
            string filename = System.IO.Path.GetFileName(str);
            if (!filename.Contains(AB_Common.AB_EXT))
            {
                filename += AB_Common.AB_EXT;
            }
            AssetGroupInfo_t info = mResPackerInfoSet.m_assetGroupInfosAll[str];
            string[] deps = AB_AssetBuildMgr.mManifest.GetAllDependencies(filename);
            foreach (string dep in deps)
            {
                if(dep.Contains("shaderlist"))
                    continue;
                info.m_dependencies.Add(AB_Common.AB_RES_INFO_PATH+dep);
            }
        }
    }

    static public void Init()
    {
        mAssetGroupInfos.Clear();
        //mResPackerInfoSet.m_assetGroupInfosAll.Clear();
    }

    static bool GatherSharedResList()
    {
        for (int i = 0; i < AB_SharedRes.mResPackerInfos.Count; i++)
        {
            if (!mResPackerInfoSet.m_assetGroupInfosAll.ContainsKey(AB_SharedRes.mResPackerInfos[i].m_pathInIFS))
            {
                mResPackerInfoSet.AddAssetGroupInfo(AB_SharedRes.mResPackerInfos[i]);
            }
            else
            {
                mResPackerInfoSet.m_assetGroupInfosAll[AB_SharedRes.mResPackerInfos[i].m_pathInIFS] =
                    AB_SharedRes.mResPackerInfos[i];
            }
        }
        return true;
    }

    //set common bundle into resourcepacker
    static public void GatherCollectRedundancy(List<AB_AssetBuildMgr.CollectRedundancy> ll)
    {
        foreach (AB_AssetBuildMgr.CollectRedundancy cc in ll)
        {
            for (int j = 0; j < cc.mABBundleExList.Count; j++)
            {
                AssetGroupInfo_t rr = CreateCResourcePackerInfo();
                rr.m_pathInIFS = AB_Common.AB_RES_INFO_PATH + cc.mABBundleExList[j].assetBundleName.ToLower();
                rr.m_tag = (int) (AssetGroupInfo_t.E_TAG_TYPE.E_AUTO_SHARED);
                foreach (string str in cc.mABBundleExList[j].assetNames)
                {
                    AssetInfo_t Info_t = new AssetInfo_t();
                    Info_t.m_pathName =
                        CFileManager.EraseExtension(AB_Common.PathRemoveAssets(str));
                    Info_t.m_extension = CFileManager.GetExtension(str);
                    rr.m_resourceInfos.Add(Info_t);
                }
                mResPackerInfoSet.AddAssetGroupInfo(rr);
            }
        }
    }

    public static string GetAssetGroupInfoSetPath()
    {
        return AB_Common.AB_RESINFO_PATH + msAssetGroupInfoSetName;
    }

    static public void SaveAssetGroupSet()
    {
        if (mResPackerInfoSet != null)
        {
            byte[] data = new byte[1024*1024*2];
            int offset = 0;
            mResPackerInfoSet.m_version = AB_Common.AB_VERSION;
            mResPackerInfoSet.m_publish = GenerateABMD5();
            mResPackerInfoSet.m_pakPath = DefaultIFSPath();
            mResPackerInfoSet.Write(data, ref offset);
            for (int i = 0; i < offset; i++)
            {
                data[i] = CFileManager.Encode(data[i]);
            }
            CFileManager.WriteFile(GetAssetGroupInfoSetPath(), data, 0, offset);
            Debug.Log("AssetManifest_t Version: " + mResPackerInfoSet.m_version);
        }
    }

    static public AssetGroupInfo_t CreateCResourcePackerInfo()
    {
        AssetGroupInfo_t res = new AssetGroupInfo_t(true);
        mAssetGroupInfos.Add(res);
        return res;
    }

    static public string GetVersion()
    {
        return mResPackerInfoSet.m_version;
    }

    static public string GetPublish()
    {
        return mResPackerInfoSet.m_publish;
    }

    static string DefaultIFSPath()
    {
        return AB_Common.AB_IFS_PATH;
    }
}

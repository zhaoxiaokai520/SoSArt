//For Unity5 AssetBundle

using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_5
public class AB_AssetBuildMgr
{
    public class AssetBundleBuildEX
    {
        public string assetBundleName;
        public List<string> assetNames = new List<string>();
        public string assetBundleVariant;
        public AB_AssetBuildMgr.E_ABBUNLDE_TYPE meType = E_ABBUNLDE_TYPE.E_CHARACTER;

        public AssetBundleBuild Parse() 
        {
            AssetBundleBuild ab = new AssetBundleBuild();
            ab.assetBundleName = assetBundleName;
            ab.assetBundleVariant = assetBundleVariant;
            ab.assetNames = assetNames.ToArray();
            return ab;
        }

        public void RemoveAsset(string name)
        {
            int t = -1;
            for (int i=0; i < assetNames.Count; i++)
            {
                if (name.Equals(assetNames[i]))
                {
                    t = i;
                    break;
                }
            }
            if (t != -1)
            {
                assetNames.RemoveAt(t);
            }
        }
    }

    public enum E_ABBUNLDE_TYPE
    {
        E_SHADER,
        E_TEXTURE,
        E_EFFECT,
        E_SHARED, //把冗余的asset打成公用的bundle
        E_GlobalCommon, //临时用的，放跨type的公用asset

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
    };
    private static Dictionary<E_ABBUNLDE_TYPE, List<AssetBundleBuildEX>> mAssetBundleMap = new Dictionary<E_ABBUNLDE_TYPE, List<AssetBundleBuildEX>>();
    static string[] mExcludeExt = new string[]{".cs", ".asset"};
    //冗余队列，因为U5的AB不允许一个asset在多个AB包里，所以目前策略是让冗余asset冗余，
    //冗余asset不放在任何AB包里，之后考虑把频繁冗余的asset放入common AB
    //asset name   List of AB the asset should be contains。
    static Dictionary<string, List<AssetBundleBuildEX>> mRedundancyMap = new Dictionary<string, List<AssetBundleBuildEX>>();
    static public AssetBundleManifest mManifest;
    
    //Get assetbundle in some type, if not exsit, new a new one
    public static AssetBundleBuildEX GetAssetBundleBuild(E_ABBUNLDE_TYPE tt, string abName)
    {
        if (!mAssetBundleMap.ContainsKey(tt))
        {
            List<AssetBundleBuildEX> ll = new List<AssetBundleBuildEX>();
            mAssetBundleMap.Add(tt, ll);
            AssetBundleBuildEX ab = new AssetBundleBuildEX();
            ab.assetBundleName = abName;
            ab.meType = tt;
            mAssetBundleMap[tt].Add(ab);
            return ab;
        }
        else
        {
            //find exsit ab
            foreach (AssetBundleBuildEX ab in mAssetBundleMap[tt])
            {
                if (ab.assetBundleName.ToLower().Equals(abName.ToLower()))
                {
                    return ab;
                }
            }
            AssetBundleBuildEX newab = new AssetBundleBuildEX();
            newab.assetBundleName = abName;
            newab.meType = tt;
            mAssetBundleMap[tt].Add(newab);
            return newab;
        }
    }

    public static AssetBundleBuildEX FindAssetBundleBuildEx(E_ABBUNLDE_TYPE tt, string name)
    {
        foreach (AssetBundleBuildEX ab in mAssetBundleMap[tt])
        {
            if (ab.assetBundleName.ToLower().Equals(name.ToLower()))
            {
                return ab;
            }
        }
        return null;
    }

    public static AssetBundleBuildEX FindAssetBundleBuildEx(string name)
    {
        AssetBundleBuildEX ret = null;
        foreach (E_ABBUNLDE_TYPE tt in mAssetBundleMap.Keys)
        {
            ret = FindAssetBundleBuildEx(tt, name);
            if (ret != null)
            {
                return ret;
            }
        }
        return null;
    }

    public static void AddAsset(AssetBundleBuildEX ab, string assetName)
    {
        if (!ContainAsset(ab, assetName))
        {
            //check redundancy
            bool bCheckR = CheckRedundancy(assetName, ab);

            //exclude some ext asset, such as .cs .asset 
            string ext = CFileManager.GetExtension(assetName);
            bool bFind = false;
            for (int i = 0; i < mExcludeExt.Length; i++)
            {
                if (ext.ToLower().Equals(mExcludeExt[i]))
                {
                    bFind = true;
                    break;
                }
            }
            if(!bFind && !bCheckR)
                ab.assetNames.Add(assetName);
        }        
    }

    public static void ForceAddAsset(AssetBundleBuildEX ab, string assetName)
    {
        if (!ContainAsset(ab, assetName))
        {
            bool bCheckR = CheckRedundancy(assetName, ab);
            
            if ( !bCheckR )
                ab.assetNames.Add(assetName);
        }
    }

    //asset in AssetBundleBuild
    //eg: shaderlist.unity3d contains diffuse.shader ?
    static public bool ContainAsset(AssetBundleBuildEX ab, string assetname)
    {
        for (int i = 0; i < ab.assetNames.Count(); i++)
        {
            if (assetname.ToLower().Equals(ab.assetNames[i].ToLower()))
            {
                return true;
            }
        }
        return false;
    }

    //asset in a type
    //eg: does diffuse.shader in map which's key is E_SHADER
    static public bool ContainAsset(E_ABBUNLDE_TYPE tt, string assetname)
    {
        if (!mAssetBundleMap.ContainsKey(tt))
        {
            return false;
        }
        else
        {
            foreach (AssetBundleBuildEX bb in mAssetBundleMap[tt])
            {
                bool bRet = ContainAsset(bb, assetname);
                if (bRet)
                {
                    return bRet;
                }
            }
            return false;
        }
    }

    public static AssetBundleBuild[] Parse()
    {
        AutoCollectRedundancyData();
        int iCount = Count();
        AssetBundleBuild[] abArray = new AssetBundleBuild[iCount];
        int i = 0;
        foreach (List<AssetBundleBuildEX> abList in mAssetBundleMap.Values)
        {
            foreach (AssetBundleBuildEX ab in abList)
            {
                abArray[i] = ab.Parse();
                i++;
            }
        }
        return abArray;
    }

    public static void Write()
    {
        FileStream fp = new FileStream(UnityEngine.Application.streamingAssetsPath+"/bundle.txt", FileMode.Create);
        StreamWriter sw = new StreamWriter(fp);
        foreach (E_ABBUNLDE_TYPE tt in mAssetBundleMap.Keys)
        {
            sw.WriteLine("==============="+tt.ToString()+"==============");
            foreach (AssetBundleBuildEX ab in mAssetBundleMap[tt])
            {
                sw.WriteLine("AB Name: " + ab.assetBundleName);
                sw.WriteLine("AB Variant: " + ab.assetBundleVariant);
                sw.WriteLine("AB Asset Names: ");
                foreach (string pp in ab.assetNames)
                {
                    sw.WriteLine("      " + pp);
                }
            }
        }

        //Redundancy List
        sw.WriteLine("------------------Redundancy--------------");
        foreach (string assetName in mRedundancyMap.Keys)
        {
            if (mRedundancyMap[assetName].Count == 1)
                continue;
            sw.WriteLine("Asset Name: " + assetName);
            sw.WriteLine("AB Names: ");
            foreach (AssetBundleBuildEX pp in mRedundancyMap[assetName])
            {
                sw.WriteLine("      " + pp.assetBundleName);
            }
        }
        sw.Close();
        //fp.Flush();
        fp.Close();
    }

    static int Count()
    {
        int iCount = 0;
        foreach (List<AssetBundleBuildEX> abList in mAssetBundleMap.Values)
        {
            iCount += abList.Count;
        }
        return iCount;
    }

    static bool CheckRedundancy(string assetName, AssetBundleBuildEX bundle)
    {
        if (mRedundancyMap.ContainsKey(assetName))
        {
            bool bFind = false;
            for (int i = 0; i < mRedundancyMap[assetName].Count; i++)
            {
                if (mRedundancyMap[assetName][i].assetBundleName == bundle.assetBundleName)
                {
                    bFind = true;
                    break;
                }
            }
            if (!bFind)
            {
                mRedundancyMap[assetName].Add(bundle);
            }
            return true;
        }
        else
        {
           mRedundancyMap.Add(assetName, new List<AssetBundleBuildEX>());
           mRedundancyMap[assetName].Add(bundle);
           return false;
        }
    }

    static public void Clear()
    {
        mRedundancyMap.Clear();
        mAssetBundleMap.Clear();
    }

    public class CollectRedundancy
    {
        public enum E_LEVEL
        {
            E_LEVEL1,
            E_LEVEL2,
            E_LEVEL3,
        }
        public int miIndex = 0;

        public CollectRedundancy(E_ABBUNLDE_TYPE tt, E_LEVEL ll, List<string> assets)
        {
            meLevel = ll;
            meType = tt;
            mAssetNames = assets;
            if (assets != null && assets.Count > 0)
            {
                CreateABBundleEX();                
            }
        }

        AssetBundleBuildEX CreateABBundleEX()
        {
            string sName = meType.ToString().ToLower() + meLevel.ToString().ToLower() + "shared" + miIndex + AB_Common.AB_EXT;
            AssetBundleBuildEX ABBundleEx = AB_AssetBuildMgr.GetAssetBundleBuild(E_ABBUNLDE_TYPE.E_SHARED, sName);
            mABBundleExList.Add(ABBundleEx);
            miIndex++;
            return ABBundleEx;
        }

        //set mAssetNames into multi AssetBundleBuildEX's assetNames
        public void Parse()
        {
            if (mAssetNames.Count < msSplitCount)
            {
                ParseSimple();                
            }
            else
            {
                ParseComplex();
            }
        }

        //简单策略，测试，把所有mAssetNames都加到第一个ABBundleEx
        void ParseSimple()
        {
            mABBundleExList[0].assetNames.AddRange(mAssetNames); 
        }

        static readonly int msSplitCount = 20;
        //复杂策略，把所有mAssetNames平均分配到几个ABBundleEx
        //TODO:可能出现相互依赖，比如shared0包里有prefab但是shared1里有该prefab的material
        //然后shared0里又存在material的texture，所以区别类型，材质和模型打一个AB1，贴图打一个AB2
        //这样只会AB1依赖AB2
        void ParseComplex()
        {
            //int idx = 0;
            //for(int i=0; i<mAssetNames.Count; i++)
            //{
            //    idx = i / msSplitCount;
            //    if(idx == miIndex)
            //    {
            //        CreateABBundleEX();
            //    }
            //    mABBundleExList[idx].assetNames.Add(mAssetNames[i]);
            //}
            //AB1 mat add fbx and other
            CreateABBundleEX();
            //AB2 texture
            CreateABBundleEX();
            for(int i=0; i<mAssetNames.Count; i++)
            {
                string ext = CFileManager.GetExtension(mAssetNames[i]).ToLower();
                if(ext.Contains("jpg") || ext.Contains("png") || ext.Contains("tga"))
                {
                    mABBundleExList[1].assetNames.Add(mAssetNames[i]);
                }
                else
                {
                    mABBundleExList[0].assetNames.Add(mAssetNames[i]);
                }
            }
        }

        public E_ABBUNLDE_TYPE meType;
        public E_LEVEL meLevel;
        public List<string> mAssetNames = new List<string>();
        public List<AssetBundleBuildEX> mABBundleExList = new List<AssetBundleBuildEX>();
    }

    static public void AutoCollectRedundancyData()
    {
        Dictionary<E_ABBUNLDE_TYPE, Dictionary<CollectRedundancy.E_LEVEL, List<string>>> tempCollectMap = 
            new Dictionary<E_ABBUNLDE_TYPE, Dictionary<CollectRedundancy.E_LEVEL, List<string>>>();
        foreach (string name in mRedundancyMap.Keys)
        {
            // >1表示存在冗余
            if (mRedundancyMap[name].Count > 1)
            {
                //从第一个ab ex里去掉name的asset,这样能允许冗余
                AssetBundleBuildEX ab = mRedundancyMap[name][0];
                ab.RemoveAsset(name);

                if (mRedundancyMap[name].Count > 2)
                {
                    E_ABBUNLDE_TYPE rType = GetRedundancyType(mRedundancyMap[name]);
                    CollectRedundancy.E_LEVEL ll = CollectRedundancy.E_LEVEL.E_LEVEL1;
                    if (mRedundancyMap[name].Count > 3)
                    {
                        ll = CollectRedundancy.E_LEVEL.E_LEVEL2;
                    }
                    if (mRedundancyMap[name].Count > 5)
                    {
                        ll = CollectRedundancy.E_LEVEL.E_LEVEL3;
                    }
                    if (!tempCollectMap.ContainsKey(rType))
                    {
                        tempCollectMap.Add(rType, new Dictionary<CollectRedundancy.E_LEVEL, List<string>>());
                    }
                    if (!tempCollectMap[rType].ContainsKey(ll))
                    {
                        tempCollectMap[rType].Add(ll, new List<string>());
                    }
                    tempCollectMap[rType][ll].Add(name);
                }
            }
        }
        List<CollectRedundancy> CollectRedundancyList = new List<CollectRedundancy>();
        foreach (E_ABBUNLDE_TYPE ttt in tempCollectMap.Keys)
        {
            foreach (CollectRedundancy.E_LEVEL lll in tempCollectMap[ttt].Keys)
            {
                if (tempCollectMap[ttt][lll].Count > 0)
                {
                    CollectRedundancy ddd = new CollectRedundancy(ttt, lll, tempCollectMap[ttt][lll]);
                    ddd.Parse();
                    CollectRedundancyList.Add(ddd);
                }
            }
        }
        //set common bundle into resourcepacker
        AB_GatherResInfo.GatherCollectRedundancy(CollectRedundancyList);
    }

    static E_ABBUNLDE_TYPE GetRedundancyType(List<AssetBundleBuildEX> abList)
    {
        E_ABBUNLDE_TYPE ret = abList[0].meType;
        for (int i = 1; i < abList.Count; i++)
        {
            if (ret != abList[i].meType)
            {
                return E_ABBUNLDE_TYPE.E_GlobalCommon;
            }
        }
        return ret;
    }


    public static void LoadManifest()
    {
        string url = Application.streamingAssetsPath+"/AssetBundle/AssetBundle";
        AssetBundle assetBundle = AssetBundle.LoadFromFile(url);
        mManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");       
        assetBundle.Unload(false);
    }

    //[MenuItem("AssetsBundle/Test Load Manifest")]
    //static void LoadMFS()
    //{
    //    LoadManifest();
    //}

}
#endif

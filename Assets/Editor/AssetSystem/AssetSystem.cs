/***
 * 
 * 
 * 
 */

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
////using System.Diagnostics;

public class AssetSystem : Singleton<AssetSystem>
{
	public delegate void OnResourceLoaded(Asset_t resource);

	private AssetManifest_t m_assetPackerInfoSet;

	private CUtilDic<string, Asset_t> m_cachedAssetMap;

	private bool m_clearUnusedAssets;

	private int m_clearUnusedAssetsExecuteFrame;

    private int m_updateAsyncLoadingFrame = 5;

    private List<AssetGroupInfo_t> m_AsyncLoadingBundle = new List<AssetGroupInfo_t>();

    private List<Asset_t> m_AsyncLoadingAsset = new List<Asset_t>();

	private static int s_frameCounter;

	public static bool isBattleState;

    public string publishVer
    {
        get { return m_assetPackerInfoSet != null ? m_assetPackerInfoSet.m_publish : default(string); }
    }

    public string version
    {
        get { return m_assetPackerInfoSet != null ? m_assetPackerInfoSet.m_version : default(string); }
    }

	public override void Init()
	{
		m_assetPackerInfoSet = null;
		m_cachedAssetMap = new CUtilDic<string, Asset_t>();
	}

	public void CustomUpdate()
	{
        UpdateAsyncLoading();

        AssetSystem.s_frameCounter++;
		if (m_clearUnusedAssets && m_clearUnusedAssetsExecuteFrame == AssetSystem.s_frameCounter)
		{
			ExecuteUnloadUnusedAssets();
			m_clearUnusedAssets = false;
		}
	}

   

    internal void AddAsyncLoadingBundle(AssetGroupInfo_t assetgroup)
    {
        assetgroup.TimeStartSyncLoading = Time.realtimeSinceStartup;
        m_AsyncLoadingBundle.Add(assetgroup);
    }

    internal void AddAsyncLoadingAsset(Asset_t asset)
    {
        asset.TimeStartSyncLoading = Time.realtimeSinceStartup;
        m_AsyncLoadingAsset.Add(asset);
    }
    
    public bool UpdateAsyncLoading(int framePass=1)
    {
        m_updateAsyncLoadingFrame -= framePass;
        if (m_updateAsyncLoadingFrame>0)
        {
            return false;
        }
        float curTime = Time.realtimeSinceStartup;
        for(int i=0;i<m_AsyncLoadingBundle.Count;)
        {
            m_AsyncLoadingBundle[i].UpdateAsyncLoad();
            if(!m_AsyncLoadingBundle[i].IsAssetBundleInLoading())
            {
                m_AsyncLoadingBundle.RemoveAt(i);
            }else
            {
                if((curTime - m_AsyncLoadingBundle[i].TimeStartSyncLoading)>60)
                {
                    Debug.LogError("AsyncLoading bundle over time " + m_AsyncLoadingBundle[i].m_pathInIFS);
                    m_AsyncLoadingBundle[i].DumpState();
                    m_AsyncLoadingBundle[i].TimeStartSyncLoading = curTime;
                }
                ++i;
            }
        }

        for (int i = 0; i < m_AsyncLoadingAsset.Count;)
        {
            m_AsyncLoadingAsset[i].UpdateAsyncLoad();
            if (m_AsyncLoadingAsset[i].m_state != eAssetState.Loading)
            {
                m_AsyncLoadingAsset.RemoveAt(i);
            }
            else
            {
                if ((curTime - m_AsyncLoadingAsset[i].TimeStartSyncLoading) > 60)
                {
                    Debug.LogError("AsyncLoading Asset over time " + m_AsyncLoadingAsset[i].m_pathName);
                    m_AsyncLoadingAsset[i].DumpState();
                    m_AsyncLoadingAsset[i].TimeStartSyncLoading = curTime;
                }
                ++i;
            }
        }

        m_updateAsyncLoadingFrame = 5;

        return m_AsyncLoadingAsset.Count == 0 && m_AsyncLoadingAsset.Count == 0;
    }


	//public CUtilDic<string, Asset_t> GetCachedAssetMap()
	//{
	//	return m_cachedAssetMap;
	//}



	public void LoadAssetManifest()
	{
		if (m_assetPackerInfoSet != null)
		{
			m_assetPackerInfoSet.Dispose();
			m_assetPackerInfoSet = null;
		}
		string filePath = CFileManager.CombinePath(CFileManager.GetIFSExtractPath(), AssetManifest_t.s_assetGroupInfoSetFileName);
        Debug.Log(filePath);
	    if (CFileManager.IsFileExist(filePath))
	    {
            Debug.LogWarning(" Successs to Load AssetGroupInfoSet " + filePath);
	        byte[] data = CFileManager.ReadFile(filePath);
	        for (int i = 0; i < data.Length; i++)
	        {
	            data[i] = CFileManager.Decode(data[i]);
	        }
	        int num = 0;
	        m_assetPackerInfoSet = new AssetManifest_t();
	        m_assetPackerInfoSet.Read(data, ref num);
	        CVersion.SetUsedResourceVersion(m_assetPackerInfoSet.m_version);
	        m_assetPackerInfoSet.Prepare();
	    }
	    else
	    {
            Debug.LogError(" Failed to Load AssetGroupInfoSet "+ filePath);
	    }
	}

	////[DebuggerHidden]
	public IEnumerator LoadResidentAssetBundles()
	{
        //DebugHelper.LogWarning(Time.frameCount+"LoadResidentAssetBundles: " + m_assetPackerInfoSet);

        if (m_assetPackerInfoSet!=null)
        {
            foreach (AssetGroupInfo_t assetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
            {
                if (assetGroupInfo.m_isAssetBundle 
                    && assetGroupInfo.IsResident() && 
                    !assetGroupInfo.IsAssetBundleLoaded()) 
                {
                    //Debug.LogWarning(assetGroupInfo.m_pathInIFS);
                    assetGroupInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
                    yield return 1;
                }
            }

            yield return this.WarmUpShader();

        }
	}

	public bool CheckCachedAsset(string fullPathInResources)
	{
		string key = CFileManager.ToKey(fullPathInResources); 
		Asset_t cResource = null;
		return m_cachedAssetMap.TryGetValue(key, out cResource);
	}

    //this function for simple get prefab such as ui effect etc
    public GameObject GetPrefab(string fullPathInResources)
    {
        Asset_t res = GetAsset(fullPathInResources, typeof (GameObject), eAssetType.UIPrefab);
        if (res != null)
        {
            GameObject gObj = res.m_content as GameObject;
            if (gObj != null)
            {
                return gObj;                
            }
            else
            {
                Debug.LogError("Content is not GameObject!!!: "+fullPathInResources);
            }
        }
        else
        {
            Debug.LogError("Resource is not Find!!!: " + fullPathInResources);
        }
        return null;
    }

#if true
    public static System.Diagnostics.Stopwatch mWatch = System.Diagnostics.Stopwatch.StartNew();
    public static long time_bundle = 0;
#endif

    /// <summary>
    /// </summary>
    /// <param name="fullPathInResources"></param>
    /// <param name="resourceContentType"></param>
    /// <param name="resourceType"></param>
    /// <param name="needCached"></param>
    /// <param name="unloadBelongedAssetBundleAfterLoaded"></param>
    /// <returns></returns>
	public Asset_t GetAsset(string fullPathInResources, Type resourceContentType, eAssetType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
	{
        if (string.IsNullOrEmpty(fullPathInResources))
		{
			return new Asset_t(string.Empty, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded,false);
		}
        string key = CFileManager.ToKey(fullPathInResources);
		Asset_t cResource = null;
		if (m_cachedAssetMap.TryGetValue(key, out cResource))
		{
			if (cResource.m_resourceType != resourceType)
			{
				cResource.m_resourceType = resourceType;
			}
			return cResource;
		}
		cResource = new Asset_t(key, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded,false);

		LoadAsset(cResource);
		if (needCached)
		{
			m_cachedAssetMap.Add(key, cResource);
		}
		return cResource;
	}

    public Asset_t GetAssetAsync(string fullPathInResources, Type resourceContentType, eAssetType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            return new Asset_t(string.Empty, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded, needCached);
        }
        string key = CFileManager.ToKey(fullPathInResources);
        Asset_t cResource = null;
        if (m_cachedAssetMap.TryGetValue(key, out cResource))
        {
            if (cResource.m_resourceType != resourceType)
            {
                cResource.m_resourceType = resourceType;
            }
            return cResource;
        }
        cResource = new Asset_t(key, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded, needCached);

        LoadAssetAsync(cResource);
        return cResource;
    }

    //for new scene loader
    public Asset_t GetScene(string fullPathInResources, bool needCached = false,
        bool unloadBelongedAssetBundleAfterLoaded = false)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            Debug.LogError("path is Empty!!");
            return null;
        }
        string key = CFileManager.ToKey(fullPathInResources);
        Asset_t cResource = null;
        if (m_cachedAssetMap.TryGetValue(key, out cResource))
        {
            return cResource;
        }        
        cResource = new Asset_t(key, fullPathInResources, unloadBelongedAssetBundleAfterLoaded,false);
        LoadAsset(cResource);
        if (needCached)
        {
            m_cachedAssetMap.Add(key, cResource);
        }
        return cResource;  
    }

    public Asset_t GetSceneAsync(string fullPathInResources, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
    {
        if (string.IsNullOrEmpty(fullPathInResources))
        {
            Debug.LogError("path is Empty!!");
            return null;
        }
        string key = CFileManager.ToKey(fullPathInResources);
        Asset_t cResource = null;
        if (m_cachedAssetMap.TryGetValue(key, out cResource))
        {
            return cResource;
        }
        cResource = new Asset_t(key, fullPathInResources, unloadBelongedAssetBundleAfterLoaded, needCached);
        LoadAssetAsync(cResource);       
        return cResource;
    }

    public void RemoveCachedAsset(string fullPathInResources)
	{
		string key = CFileManager.ToKey(fullPathInResources);
        Asset_t cResource = null;
		if (m_cachedAssetMap.TryGetValue(key, out cResource))
		{
			cResource.Unload();
			m_cachedAssetMap.Remove(key);
		}
	}

	public void RemoveCachedAssets(eAssetType resourceType, bool clearImmediately = true)
	{
		List<string> list = new List<string>();
		CUtilDic<string, Asset_t>.Enumerator enumerator = m_cachedAssetMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Asset_t> current = enumerator.Current;
			Asset_t value = current.Value;
			if (value.m_resourceType == resourceType)
			{
				value.Unload();
				list.Add(value.m_key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			m_cachedAssetMap.Remove(list[i]);
		}
		if (clearImmediately)
		{
			UnloadUnusedAssetBundles();
			UnloadUnusedAssets();
		}
	}

	public void RemoveCachedAssets(eAssetType[] resourceTypes)
	{
		for (int i = 0; i < resourceTypes.Length; i++)
		{
			RemoveCachedAssets(resourceTypes[i], false);
		}
		UnloadUnusedAssetBundles();
		UnloadUnusedAssets();
	}

    internal void AddCachedAsset(Asset_t asset)
    {
        if (m_cachedAssetMap.ContainsKey(asset.m_key))
            return;
        m_cachedAssetMap.Add(asset.m_key,asset);
    }

	public void RemoveAllCachedAssets()
	{
		RemoveCachedAssets((eAssetType[])Enum.GetValues(typeof(eAssetType)));
	}

    public void CheckAndFixBundleLoadedState()
    {
        Dictionary<string,AssetGroupInfo_t>.Enumerator e = m_assetPackerInfoSet.m_assetGroupInfosAll.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.Value.hasCheckStated = false;
        }

        e = m_assetPackerInfoSet.m_assetGroupInfosAll.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.Value.CheckAndFixBundleLoadedState();
        }
    }

	public void UnloadBelongedAssetbundle(string fullPathInResources, bool unloadTrue = false, bool checkRef = false)
	{
		AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(fullPathInResources);
		if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
		{
            resourceBelongedPackerInfo.UnloadAssetBundle(unloadTrue, false,checkRef);
		}
	}

    public void LockBelongedAssetbundle(string fullPathInResources, bool block)
    {
        AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(fullPathInResources);
        if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
        {
            resourceBelongedPackerInfo.Locked = block;
        }
    }

    public void UnloadAssetBundlesByTag(AssetGroupInfo_t.E_TAG_TYPE tag, bool unloadAsset = false)
	{
	    int iTag = (int) tag;
		if (m_assetPackerInfoSet != null && m_assetPackerInfoSet.m_assetGroupTagMap!=null)
		{
            List<AssetGroupInfo_t> lstbundles = null;
            if (m_assetPackerInfoSet.m_assetGroupTagMap.TryGetValue(iTag,out lstbundles))
		    {
                int iCount = lstbundles.Count;
		        for (int i = 0; i < iCount; i++)
		        {
                    lstbundles[i].UnloadAssetBundle(unloadAsset, false,false);
		        }
		    }
		}
	}

	public void UnloadUnusedAssets()
	{
		m_clearUnusedAssets = true;
		m_clearUnusedAssetsExecuteFrame = AssetSystem.s_frameCounter + 1;
	}

    public void Profile()
    {
#if true
        int iCacheCount = m_cachedAssetMap.Count;
        int iUnLoadCount = 0;
        StringHelper.ClearFormater();
        foreach (AssetGroupInfo_t assetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
        {
            if (assetGroupInfo.IsAssetBundleLoaded())
            {
                StringHelper.Formater.Append(" " + assetGroupInfo.m_pathInIFS);
                iUnLoadCount++;
            }
        }
        Debug.LogWarning("Cache Count: " + iCacheCount + " UnLoad AB Count: "+iUnLoadCount);
        Debug.LogWarning("Remain AB: "+StringHelper.Formater.ToString());
#endif
    }

	private void ExecuteUnloadUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void UnloadUnusedAssetBundles()
	{
		if (m_assetPackerInfoSet == null)
		{
			return;
		}
        foreach (AssetGroupInfo_t cAssetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
		{
			if (cAssetGroupInfo.IsAssetBundleLoaded())
			{
				bool flag = true;
				for (int j = 0; j < cAssetGroupInfo.m_resourceInfos.Count; j++)
				{
					if (CheckCachedAsset(cAssetGroupInfo.m_resourceInfos[j].m_pathName))
					{
						flag = false;
					}
				}
				if (flag)
				{
					cAssetGroupInfo.UnloadAssetBundle(false);
				}
			}
		}
	}

	public AssetGroupInfo_t GetAssetInGroup(string fullPathInResources)
	{
		if (string.IsNullOrEmpty(fullPathInResources))
		{
			return null;
		}
		if (m_assetPackerInfoSet != null)
		{
			return m_assetPackerInfoSet.GetAssetInGroup(CFileManager.ToKey(fullPathInResources));
		}
		return null;
	}

	public string GetAssetBundleInfoString()
	{
		if (m_assetPackerInfoSet == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		int num = 0;
        foreach (AssetGroupInfo_t cAssetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
		{
            if (cAssetGroupInfo.IsAssetBundleLoaded())
			{
				num++;
                text += CFileManager.GetFullName(cAssetGroupInfo.m_pathInIFS);
			}
		}
		return text;
	}

    public void FillLoadedBundle(List<AssetGroupInfo_t> lstInfos)
    {
        foreach (AssetGroupInfo_t cAssetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
        {
            if (cAssetGroupInfo.IsAssetBundleLoaded())
            {
                lstInfos.Add(cAssetGroupInfo);
            }
        }
    }

	public void LoadAssetBundle(string fullPathInIFS)
	{
		LoadAssetBundle(GetAssetGroupInfo(fullPathInIFS));
	}

	public void LoadAssetBundle(AssetGroupInfo_t assetGroupInfo)
	{
		if (assetGroupInfo == null || !assetGroupInfo.m_isAssetBundle || assetGroupInfo.IsAssetBundleLoaded())
		{
			return;
		}
		assetGroupInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
	}

	public void LoadAllAssetInGroup(string fullPathInIFS, eAssetType resourceType)
	{
		LoadAllAssetInGroup(GetAssetGroupInfo(fullPathInIFS), resourceType);
	}

	public void LoadAllAssetInGroup(AssetGroupInfo_t assetGroupInfo, eAssetType resourceType)
	{
		if (assetGroupInfo == null)
		{
			return;
		}
		for (int i = 0; i < assetGroupInfo.m_resourceInfos.Count; i++)
		{
			GetAsset(assetGroupInfo.m_resourceInfos[i].m_pathName, GetAssetContentType(assetGroupInfo.m_resourceInfos[i].m_extension), resourceType, true, i == assetGroupInfo.m_resourceInfos.Count - 1);
		}
	}

	public Type GetAssetContentType(string extension)
	{
		Type result = null;
        if (string.Equals(extension, ".prefab", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(GameObject);
		}
        else if (string.Equals(extension, ".bytes", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(TextAsset);
		}
        else if (string.Equals(extension, ".asset", StringComparison.OrdinalIgnoreCase))
		{
			result = typeof(ScriptableObject);
		}
		return result;
	}

	public AssetGroupInfo_t GetAssetGroupInfo(string fullPathInIFS)
	{
		if (string.IsNullOrEmpty(fullPathInIFS) || m_assetPackerInfoSet == null)
		{
			return null;
		}
	    if (m_assetPackerInfoSet.m_assetGroupInfosAll.ContainsKey(fullPathInIFS))
	    {
	        return m_assetPackerInfoSet.m_assetGroupInfosAll[fullPathInIFS];
	    }
		return null;
	}

    private void LoadAsset(Asset_t resource)
	{
		AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(resource);
		if (resourceBelongedPackerInfo != null)
		{
			if (resourceBelongedPackerInfo.m_isAssetBundle)
			{
                resource.m_relatedBundle = resourceBelongedPackerInfo;
                if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
				{
#if DEBUG_LOGOUT
                    mWatch.Reset();
                    mWatch.Start();
#endif
                    resourceBelongedPackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
#if DEBUG_LOGOUT
                    time_bundle = mWatch.ElapsedMilliseconds;
                    DebugHelper.LogOutResouceLoad("==PRINT_LOAD== Load AssetBundle " + resource.m_relatedBundle.m_pathInIFS + " Time: "  + mWatch.ElapsedMilliseconds);
#endif
				}
                //for new scene loader
                if (resource.m_resourceType != eAssetType.Scene)
				    resource.LoadAsset();
				if (resource.m_unloadBelongedAssetBundleAfterLoaded)
				{
					resourceBelongedPackerInfo.UnloadAssetBundle(false);
				}
			}
			else
			{
				resource.Load(CFileManager.GetIFSExtractPath());
			}
		}
		else
		{
			resource.Load();
		}
	}

    
    private void LoadAssetAsync(Asset_t resource)
    {
        AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(resource);
        if (resourceBelongedPackerInfo != null)
        {
            if (resourceBelongedPackerInfo.m_isAssetBundle)
            {
                resource.m_relatedBundle = resourceBelongedPackerInfo;
                if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
                {
                    resourceBelongedPackerInfo.AsyncLoadAssetBundle(CFileManager.GetIFSExtractPath());
                }
                resource.LoadAsync();
            }
            else
            {
                resource.Load(CFileManager.GetIFSExtractPath());
            }
        }
        else
        {
            resource.Load();
        }
    }

    public void UpdateAssetGroupState()
    {

    }

    private AssetGroupInfo_t GetAssetInGroup(Asset_t resource)
	{
		if (m_assetPackerInfoSet != null)
		{
			AssetGroupInfo_t resourceBelongedPackerInfo = m_assetPackerInfoSet.GetAssetInGroup(resource.m_key);
			return resourceBelongedPackerInfo;
		}
		return null;
	}

    public IEnumerator WarmUpShader()
    {
        AssetGroupInfo_t assetGroupInfo = Singleton<AssetSystem>.GetInstance().GetAssetGroupInfo("AssetBundle/shaderlist.unity3d");
        if (assetGroupInfo != null && assetGroupInfo.m_assetBundle != null)
        {
            ShaderVariantCollection[] array = assetGroupInfo.m_assetBundle.LoadAllAssets<ShaderVariantCollection>();
            //int num;
            for (int i = 0; i < array.Length; ++i)// = num + 1)
            {
                if (!array[i].isWarmedUp)
                {
                    array[i].WarmUp();
                    yield return 1;
                }
                //num = i;
            }
            array = null;
        }
        yield break;
    }
}

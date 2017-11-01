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

	private static int s_frameCounter;

	public static bool isBattleState;

	public override void Init()
	{
		m_assetPackerInfoSet = null;
		m_cachedAssetMap = new CUtilDic<string, Asset_t>();
	}

	public void CustomUpdate()
	{
		AssetSystem.s_frameCounter++;
		if (m_clearUnusedAssets && m_clearUnusedAssetsExecuteFrame == AssetSystem.s_frameCounter)
		{
			ExecuteUnloadUnusedAssets();
			m_clearUnusedAssets = false;
		}
	}

	public CUtilDic<string, Asset_t> GetCachedAssetMap()
	{
		return m_cachedAssetMap;
	}

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
	        int num = 0;
	        m_assetPackerInfoSet = new AssetManifest_t();
	        m_assetPackerInfoSet.Read(data, ref num);
	        CVersion.SetUsedResourceVersion(m_assetPackerInfoSet.m_version);
	        m_assetPackerInfoSet.CreateAssetMap();
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
                    assetGroupInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
                    yield return 1;
                }
            }
        }
	}

	public bool CheckCachedAsset(string fullPathInResources)
	{
		string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
		Asset_t cResource = null;
		return m_cachedAssetMap.TryGetValue(key, out cResource);
	}

    /// <summary>
    /// </summary>
    /// <param name="fullPathInResources"></param>
    /// <param name="resourceContentType"></param>
    /// <param name="resourceType"></param>
    /// <param name="needCached"></param>
    /// <param name="unloadBelongedAssetBundleAfterLoaded"></param>
    /// <returns></returns>
	public Asset_t GetAsset(string fullPathInResources, Type resourceContentType, kAssetType resourceType, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
	{
		if (string.IsNullOrEmpty(fullPathInResources))
		{
			return new Asset_t(string.Empty, string.Empty, null, resourceType, unloadBelongedAssetBundleAfterLoaded);
		}
		string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
		Asset_t cResource = null;
		if (m_cachedAssetMap.TryGetValue(key, out cResource))
		{
			if (cResource.m_resourceType != resourceType)
			{
				cResource.m_resourceType = resourceType;
			}
			return cResource;
		}
		cResource = new Asset_t(key, fullPathInResources, resourceContentType, resourceType, unloadBelongedAssetBundleAfterLoaded);
		LoadAsset(cResource);
		if (needCached)
		{
			m_cachedAssetMap.Add(key, cResource);
		}
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
        string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
        Asset_t cResource = null;
        if (m_cachedAssetMap.TryGetValue(key, out cResource))
        {
            return cResource;
        }
        cResource = new Asset_t(key, fullPathInResources, unloadBelongedAssetBundleAfterLoaded);
        LoadAsset(cResource);
        if (needCached)
        {
            m_cachedAssetMap.Add(key, cResource);
        }
        return cResource;  
    }

	public void RemoveCachedAsset(string fullPathInResources)
	{
		string key = CFileManager.EraseExtension(fullPathInResources).ToLower();
		Asset_t cResource = null;
		if (m_cachedAssetMap.TryGetValue(key, out cResource))
		{
			cResource.Unload();
			m_cachedAssetMap.Remove(key);
		}
	}

	public void RemoveCachedAssets(kAssetType resourceType, bool clearImmediately = true)
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

	public void RemoveCachedAssets(kAssetType[] resourceTypes)
	{
		for (int i = 0; i < resourceTypes.Length; i++)
		{
			RemoveCachedAssets(resourceTypes[i], false);
		}
		UnloadUnusedAssetBundles();
		UnloadUnusedAssets();
	}

	public void RemoveAllCachedAssets()
	{
		RemoveCachedAssets((kAssetType[])Enum.GetValues(typeof(kAssetType)));
	}

	public void UnloadBelongedAssetbundle(string fullPathInResources)
	{
		AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(fullPathInResources);
		if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
		{
			resourceBelongedPackerInfo.UnloadAssetBundle(false);
		}
	}

	public void UnloadAssetBundlesByTag(AssetGroupInfo_t.E_TAG_TYPE tag)
	{
	    int iTag = (int) tag;
		if (m_assetPackerInfoSet != null && m_assetPackerInfoSet.m_assetGroupTagMap!=null)
		{
            if (m_assetPackerInfoSet.m_assetGroupTagMap.ContainsKey(iTag))
		    {
                int iCount = m_assetPackerInfoSet.m_assetGroupTagMap[iTag].Count;
		        for (int i = 0; i < iCount; i++)
		        {
                    m_assetPackerInfoSet.m_assetGroupTagMap[iTag][i].UnloadAssetBundle();
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
#if Profile_Mode
        int iCacheCount = m_cachedAssetMap.Count;
        int iUnLoadCount = 0;
        StringHelper.ClearFormater();
        foreach (AssetGroupInfo_t assetGroupInfo in m_assetPackerInfoSet.m_assetGroupInfosAll.Values)
        {
            if (assetGroupInfo.miRefCnt > 0 && assetGroupInfo.m_assetBundle != null)
            {
                StringHelper.Formater.Append(" " + assetGroupInfo.m_pathInIFS);
                iUnLoadCount++;
            }
        }
        Debug.LogWarning("Cache Count: " + iCacheCount + " UnLoad AB Count: "+iUnLoadCount);
        Debug.LogWarning("Unload AB: "+StringHelper.Formater.ToString());
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
			return m_assetPackerInfoSet.GetAssetInGroup(CFileManager.EraseExtension(fullPathInResources).ToLower());
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

	public void LoadAllAssetInGroup(string fullPathInIFS, kAssetType resourceType)
	{
		LoadAllAssetInGroup(GetAssetGroupInfo(fullPathInIFS), resourceType);
	}

	public void LoadAllAssetInGroup(AssetGroupInfo_t assetGroupInfo, kAssetType resourceType)
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
        //如果在manifest里就用ab load，不然就用resource load
		AssetGroupInfo_t resourceBelongedPackerInfo = GetAssetInGroup(resource);
		if (resourceBelongedPackerInfo != null)
		{
			if (resourceBelongedPackerInfo.m_isAssetBundle)
			{
				if (!resourceBelongedPackerInfo.IsAssetBundleLoaded())
				{
					resourceBelongedPackerInfo.LoadAssetBundle(CFileManager.GetIFSExtractPath());
				}
                //for new scene loader
                if(resource.m_resourceType != kAssetType.Scene)
				    resource.Load(resourceBelongedPackerInfo.m_assetBundle);
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

	private AssetGroupInfo_t GetAssetInGroup(Asset_t resource)
	{
		if (m_assetPackerInfoSet != null)
		{
			AssetGroupInfo_t resourceBelongedPackerInfo = m_assetPackerInfoSet.GetAssetInGroup(resource.m_key);
#if false	//sunwenhao meaningless code		
            if (resourceBelongedPackerInfo != null)
			{
				string empty = string.Empty;
				if (!resourceBelongedPackerInfo.m_fileExtMap.TryGetValue(resource.m_pathName.ToLower(), out empty))
				{
					UnityEngine.Debug.LogError("No Resource " + resource.m_pathName + " found in ext name map of bundle:" + resourceBelongedPackerInfo.m_pathInIFS);
				}
				resource.m_fileFullPathInResources = resource.m_pathName + "." + empty;
			}
#endif
			return resourceBelongedPackerInfo;
		}
		return null;
	}
}

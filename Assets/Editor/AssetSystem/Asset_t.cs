using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public enum eAssetState
{
    Unload,
    Loading,
    Loaded
}

public enum eAssetType
{
    BattleScene,
    Numeric,
    Sound,
    UIForm,
    UIPrefab,
    UI3DImage,
    UISprite,
    Scene
}


public class Asset_t
{
	public string m_key;

	public string m_name;

	public string m_extName;

	public string m_fullPathInResources;

	public string m_pathName;

	public string m_fileFullPathInResources;

	public eAssetType m_resourceType;

	public eAssetState m_state;

	public Type m_contentType;

	public UnityEngine.Object m_content;

	public bool m_isAbandon;

	public bool m_unloadBelongedAssetBundleAfterLoaded;

    public delegate void LoadCompletedDelegate(Asset_t ctx);

    public AssetGroupInfo_t m_relatedBundle = null;

    public AssetBundleRequest m_BundleRequest = null;

    public bool m_bNeedCache = false;

    public LoadCompletedDelegate onloadfinish = null;

    public float TimeStartSyncLoading = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fullPathInResources"></param>
    /// <param name="contentType"></param>
    /// <param name="resourceType"></param>
    /// <param name="unloadBelongedAssetBundleAfterLoaded"></param>
	public Asset_t(string key, string fullPathInResources, Type contentType, eAssetType resourceType, bool unloadBelongedAssetBundleAfterLoaded,bool needCache)
	{
		m_key = key;
        m_extName = CFileManager.GetExtension(fullPathInResources);
		m_fullPathInResources = fullPathInResources;
		m_pathName = CFileManager.EraseExtension(m_fullPathInResources);
		m_name = CFileManager.EraseExtension(CFileManager.GetFullName(fullPathInResources));
		m_resourceType = resourceType;
		m_state = eAssetState.Unload;
		m_contentType = contentType;
		m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
		m_content = null;
		m_isAbandon = false;
        m_bNeedCache = needCache;

    }

    //Resource for new scene loader
    public Asset_t(string key, string fullPathInResources, bool unloadBelongedAssetBundleAfterLoaded,bool needCache)
    {
        m_key = key;
        m_fullPathInResources = fullPathInResources;
        m_pathName = CFileManager.EraseExtension(m_fullPathInResources);
        m_name = CFileManager.EraseExtension(CFileManager.GetFullName(fullPathInResources));
        m_resourceType = eAssetType.Scene;
        m_state = eAssetState.Unload;
        m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
        m_content = null;
        m_isAbandon = false;
        m_bNeedCache = needCache;
    }

	public void Load()
	{
		if (m_isAbandon)
		{
			m_state = eAssetState.Unload;
			return;
		}
		if (m_contentType == null)
		{
            m_content = Resources.Load(m_pathName);
		}
		else
		{
            m_content = Resources.Load(m_pathName, m_contentType);
		}
		m_state = eAssetState.Loaded;
	    HandleTextAsset();        
	}

	public void Load(string ifsExtractPath)
	{
		if (m_isAbandon)
		{
			m_state = eAssetState.Unload;
			return;
		}
		byte[] array = CFileManager.ReadFile(CFileManager.CombinePath(ifsExtractPath, m_fileFullPathInResources));
		m_state = eAssetState.Loaded;
		if (array != null)
		{
			CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
			cBinaryObject.m_data = array;
			cBinaryObject.name=(m_name);
			m_content = cBinaryObject;
		}
	}

#if true
    public static System.Diagnostics.Stopwatch mWatch = System.Diagnostics.Stopwatch.StartNew();
    public static long time_asset = 0;
#endif

	public void LoadAsset()
	{
		string name = m_name;
        if (m_relatedBundle == null || !m_relatedBundle.IsAssetBundleLoaded())
            return;
	    if (name.Contains("."))
	    {
	        string ext = CFileManager.GetExtension(name);
	        if (!ext.Equals(m_extName))
	        {
	            name += m_extName;
	        }
	    }

        //if (m_state == eAssetState.Loading)
        //{
        //    BlockAsyncLoad();
        //}

        if (m_state == eAssetState.Loaded)
        {
            return;
        }

#if false //add log by sj
        if (AssetGroupInfo_t.s_inBattle_no_bundle_load)
        {
            DebugHelper.LogOutResouceLoad("Load Resource in Battle bundle resource name:" + name + "  key"+m_key);
            DebugHelper.LogError("Load Resource in Battle bundle resource name:" + name + "  key" + m_key);
        }
#endif
		if (m_contentType == null)
		{
			m_content = m_relatedBundle.m_assetBundle.LoadAsset(name);
		}
		else
		{
#if DEBUG_LOGOUT
            //mWatch.Reset();
            //mWatch.Start();
#endif   
			m_content = m_relatedBundle.m_assetBundle.LoadAsset(name, m_contentType);
#if DEBUG_LOGOUT
            //time_asset = mWatch.ElapsedMilliseconds;
            //DebugHelper.LogWarning("==PRINT_LOAD_TIME== Load AssetBundle Time: " + mWatch.ElapsedMilliseconds);
#endif
		}

		m_state = eAssetState.Loaded;
	    HandleTextAsset();
	}

    //public void BlockAsyncLoad()
    //{
    //    if (m_state == eAssetState.Loading )
    //    {
    //        if (m_BundleRequest != null)
    //        {
    //            while (!m_BundleRequest.isDone)
    //            {
    //                DebugHelper.LogOutResouceLoad("==PRINT_LOAD_TIME== Block 2");
    //                DebugHelper.Flush(SLogCategory.ResourceLoad);
    //                Thread.Sleep(50);
    //            }

    //            m_content = m_BundleRequest.asset;
    //            m_state = eAssetState.Loaded;
    //            HandleTextAsset();
    //        }
    //        else
    //        {
    //            m_state = eAssetState.Unload;
    //        }
    //    }
    //}

    void LoadAssetAsync()
    {      

        if (m_resourceType == eAssetType.Scene)
        {
            m_state = eAssetState.Loaded;
            return;
        }

        if (m_relatedBundle.m_assetBundle == null)
        {
            m_state = eAssetState.Unload;
            return;
        }

        string name = m_name;
        if (name.Contains("."))
        {
            string ext = CFileManager.GetExtension(name);
            if (!ext.Equals(m_extName))
            {
                name += m_extName;
            }
        }

        if (m_contentType == null)
        {
            Debug.LogWarning("Content Type is null, Then Default load assetbundle async with typeof GameObject");
            m_BundleRequest = m_relatedBundle.m_assetBundle.LoadAssetAsync(name, typeof(GameObject));
        }
        else
        {
            m_BundleRequest = m_relatedBundle.m_assetBundle.LoadAssetAsync(name, m_contentType);
        }

        if (m_BundleRequest == null)
        {
            m_state = eAssetState.Unload;
            return;
        }
    }
    public void LoadAsync()
    {
        if (m_relatedBundle == null)
            return;

        m_isAbandon = false;

        if (m_state == eAssetState.Loaded)
            return;

        if (m_state == eAssetState.Loading)
            return;

        m_state = eAssetState.Loading;

        m_BundleRequest = null;

        if (m_relatedBundle.IsAssetBundleLoaded())
        {
            LoadAssetAsync();
        }

        if (m_state == eAssetState.Loading)
            AssetSystem.instance.AddAsyncLoadingAsset(this);
    }
    public void DumpState()
    {
        Debug.LogError("Name: " + m_name + "State: " + m_state);


        if (m_relatedBundle == null)
        {
            Debug.LogError("bundle is null");
        }else
        {
            Debug.LogError("bundle: " + m_relatedBundle.m_pathInIFS);
        }

        if (m_BundleRequest != null)
        {
            Debug.LogError("request progress: " + m_BundleRequest.progress + " done: " + m_BundleRequest.isDone);
        }
        else
        {
            Debug.LogError("null request");
        }
    }
    public void UpdateAsyncLoad()
    {
        if (m_state != eAssetState.Loading)
            return;
        if (m_relatedBundle == null)
        {
            m_state = eAssetState.Unload;
            return;
        }

        if ( m_relatedBundle.IsAssetBundleLoaded())
        {
            if(m_BundleRequest==null)
                LoadAssetAsync();
        }

        if(m_state == eAssetState.Loading)
        {
            if (m_relatedBundle.IsAssetBundleInLoading())
                return;

            if(!m_relatedBundle.IsAssetBundleLoaded())
            {
                m_state = eAssetState.Unload;
                return;
            }

            if (m_BundleRequest == null)
            {
                m_state = eAssetState.Unload;
                return;
            }

            if(m_BundleRequest.isDone)
            {
                m_content = m_BundleRequest.asset;

                m_state = eAssetState.Loaded;

                HandleTextAsset();

                if (m_unloadBelongedAssetBundleAfterLoaded)
                {
                    m_relatedBundle.UnloadAssetBundle(false);
                }

                if(m_isAbandon)
                {
                    Unload();
                }else
                {
                    if(m_bNeedCache)
                    {
                        AssetSystem.instance.AddCachedAsset(this);
                    }

                    if(onloadfinish!=null)
                    {
                        onloadfinish(this);
                    }
                }
            }
        }
    }

	public void Unload()
	{
		if (m_state == eAssetState.Loaded)
		{
            if (m_contentType == typeof(TextAsset))
            {
                CBinaryObject cBinObj = m_content as CBinaryObject;
                if (cBinObj != null)
                    cBinObj.Destroy();
		    }
			m_content = null;
			m_state = eAssetState.Unload;
		}
		else if (m_state == eAssetState.Loading)
		{
			m_isAbandon = true;
            m_state = eAssetState.Unload;
        }
	}

    void HandleTextAsset()
    {
        if (m_content != null && m_content.GetType() == typeof(TextAsset))
        {
            CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
            TextAsset textAsset = m_content as TextAsset;
            if (textAsset != null)
            {
                cBinaryObject.m_data = textAsset.bytes;
                m_content = cBinaryObject;
            }
        }
    }
}

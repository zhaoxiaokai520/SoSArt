using UnityEngine;
using System;
using System.Collections;

public enum kAssetState
{
    Unload,
    Loading,
    Loaded
}

public enum kAssetType
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

	public kAssetType m_resourceType;

	public kAssetState m_state;

	public Type m_contentType;

	public UnityEngine.Object m_content;

	public bool m_isAbandon;

	public bool m_unloadBelongedAssetBundleAfterLoaded;

    public delegate void LoadCompletedDelegate(Asset_t ctx);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fullPathInResources"></param>
    /// <param name="contentType"></param>
    /// <param name="resourceType"></param>
    /// <param name="unloadBelongedAssetBundleAfterLoaded"></param>
	public Asset_t(string key, string fullPathInResources, Type contentType, kAssetType resourceType, bool unloadBelongedAssetBundleAfterLoaded)
	{
		m_key = key;
        m_extName = CFileManager.GetExtension(fullPathInResources);
		m_fullPathInResources = fullPathInResources;
		m_pathName = CFileManager.EraseExtension(m_fullPathInResources);
		m_name = CFileManager.EraseExtension(CFileManager.GetFullName(fullPathInResources));
		m_resourceType = resourceType;
		m_state = kAssetState.Unload;
		m_contentType = contentType;
		m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
		m_content = null;
		m_isAbandon = false;
	}

    //Resource for new scene loader
    public Asset_t(string key, string fullPathInResources, bool unloadBelongedAssetBundleAfterLoaded)
    {
        m_key = key;
        m_fullPathInResources = fullPathInResources;
        m_pathName = CFileManager.EraseExtension(m_fullPathInResources);
        m_name = CFileManager.EraseExtension(CFileManager.GetFullName(fullPathInResources));
        m_resourceType = kAssetType.Scene;
        m_state = kAssetState.Unload;
        m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
        m_content = null;
        m_isAbandon = false;
    }

	public void Load()
	{
		if (m_isAbandon)
		{
			m_state = kAssetState.Unload;
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
		m_state = kAssetState.Loaded;
	    HandleTextAsset();
	}

    public IEnumerator LoadAsync(LoadCompletedDelegate cb = null)
    {
        if (m_isAbandon)
        {
            m_state = kAssetState.Unload;
        }
        else
        {
            if (m_contentType == null)
            {
                ResourceRequest request = Resources.LoadAsync(m_pathName);
                yield return request;
                m_content = request.asset;
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync(m_pathName, m_contentType);
                yield return request;
                m_content = request.asset;
            }
            m_state = kAssetState.Loaded;
            HandleTextAsset();
            if (cb != null)
            {
                cb(this);
            }
        } 
    }

	public void Load(string ifsExtractPath)
	{
		if (m_isAbandon)
		{
			m_state = kAssetState.Unload;
			return;
		}
		byte[] array = CFileManager.ReadFile(CFileManager.CombinePath(ifsExtractPath, m_fileFullPathInResources));
		m_state = kAssetState.Loaded;
		if (array != null)
		{
			CBinaryObject cBinaryObject = ScriptableObject.CreateInstance<CBinaryObject>();
			cBinaryObject.m_data = array;
			cBinaryObject.name=(m_name);
			m_content = cBinaryObject;
		}
	}

	public void Load(AssetBundle assetBundle)
	{
		if (m_isAbandon)
		{
			m_state = kAssetState.Unload;
			return;
		}
		string name = m_name;
        if (assetBundle == null)
            return;
        //unity loadasset 分析：首先为判断name里是否有. 如果有的话去掉后缀名
        //behaviac.bb进来时会自动去掉后缀变成behaviac这样就找不到了。
        //解决方法：如果有. 那就添加后缀让unity内部去掉后找
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
			m_content = assetBundle.LoadAsset(name);
		}
		else
		{            
			m_content = assetBundle.LoadAsset(name, m_contentType);
		}
		m_state = kAssetState.Loaded;
	    HandleTextAsset();
	}

    public IEnumerator LoadAsync(AssetBundle assetBundle, LoadCompletedDelegate cb = null)
    {
        if (m_isAbandon)
        {
            m_state = kAssetState.Unload;
            yield break;
        }
        string name = m_name;
        if (assetBundle == null)
        {
            yield break;
        }
        if (m_contentType == null)
        {
            Debug.LogWarning("Content Type is null, Then Default load assetbundle async with typeof GameObject");
            AssetBundleRequest request = assetBundle.LoadAssetAsync(name, typeof(GameObject));
            yield return request;
            m_content = request.asset;
        }
        else
        {
            AssetBundleRequest request = assetBundle.LoadAssetAsync(name, m_contentType);
            yield return request;
            m_content = request.asset;
        }
        m_state = kAssetState.Loaded;
        HandleTextAsset();
        if (cb != null)
        {
            cb(this);
        }
    }

	public void Unload()
	{
		if (m_state == kAssetState.Loaded)
		{
            if (m_contentType == typeof(TextAsset))
            {
                CBinaryObject cBinObj = m_content as CBinaryObject;
                if (cBinObj != null)
                    cBinObj.Destroy();
		    }
			m_content = null;
			m_state = kAssetState.Unload;
		}
		else if (m_state == kAssetState.Loading)
		{
			m_isAbandon = true;
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

/***
 * 资源包信息：一个资源包可能是一个AB？或者是IFS资源(zip包)
 * 
 * 
 * 
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Threading;

public enum eAssetBundleState
{
    Unload    ,
    Loading   ,
    SelfLoaded,//self is loaded and wait parents to loaded
    Loaded
}

public enum eBundleFlag
{
    KeepInResources = 1,
    UnCompress,
    Resident = 4,
    EncryptBundle = 8,
    UnCompleteAsset = 16,
    InExtraIfs = 32,
    PermanentAfterLoad = 64,
}

public struct AssetInfo_t
{
    public string m_pathName;

    public string m_extension;

    public int m_flags;
}

public class AssetGroupInfo_t: BaseAssetGroupInfo
{
#if true
    public static bool s_inBattle_no_bundle_load = false;
#endif

    public enum E_TAG_TYPE
    {
        E_DEFAULT,
        E_SCENE,
        E_HERO,
        E_UI,
        E_GAMEDATA,
        E_ACTION,
        E_ACTION_INFO,
        E_TXT,
        E_TEXTURE,
        E_BT,
        E_SHADER,
        E_EFFECT,
        E_SOUND,
        E_FONT,

        E_AUTO_SHARED,
        E_WEAPON,
        E_GEM,
        E_ILLEGAL,
    };

    //public const string DEFAULT_TAG = "DT";

    internal bool m_isAssetBundle;


    public int m_tag = 0; 
	public List<AssetInfo_t> m_resourceInfos = new List<AssetInfo_t>();

	public int m_flags;
  

    public CUtilList<string> m_dependencies = new CUtilList<string>();

    public float TimeStartSyncLoading;

   
    internal void PrepareParentBundle()
    {
        m_parentbundles.Clear();
        for (int s = 0; s < m_dependencies.Count; s++)
        {
            AssetGroupInfo_t dependency = AssetSystem.instance.GetAssetGroupInfo(m_dependencies[s]);
            if (dependency != null && dependency.m_isAssetBundle)
            {
                AddParentBundle(dependency);
            }
            else
            {
                if(dependency==null)
                    DebugHelper.LogError("Dependencies is null " + m_dependencies[s]);
            }
        }
    }

    protected override void OnLoading()   
    {
        base.OnLoading();

        AssetSystem.instance.AddAsyncLoadingBundle(this);
    }

   
   

	public AssetGroupInfo_t(bool isAssetBundle)
	{
		m_isAssetBundle = isAssetBundle;
        Locked = false;
    }

	public bool IsResident()
	{
        return m_dependencies.Count==0 && HasFlag(eBundleFlag.Resident);
	}

    public bool IsPermanentAfterLoad()
    {
        return m_dependencies.Count == 0 && HasFlag(eBundleFlag.PermanentAfterLoad);
    }

	public bool IsUnCompress()
	{
		return HasFlag(eBundleFlag.UnCompress);
	}

	public bool IsKeepInResources()
	{
		return HasFlag(eBundleFlag.KeepInResources);
	}

    public bool IsEncryptBundle()
	{
        return HasFlag(eBundleFlag.EncryptBundle);
	}

	public bool IsCompleteAssets()
	{
		return !HasFlag(eBundleFlag.UnCompleteAsset);
	}

	public void AddResourceInfo(ref AssetInfo_t resourceInfo)
	{
		m_resourceInfos.Add(resourceInfo);
	}

	public bool HasFlag(eBundleFlag flag)
	{
		return (m_flags & (int)flag) > 0;
	}


    public void SetFlag(eBundleFlag flag)
    {
        m_flags = m_flags | (int) flag;
    }

	public bool AlreadyContainResourcePath(string fullPath)
	{
		for (int i = 0; i < m_resourceInfos.Count; i++)
		{
			if (string.Equals(m_resourceInfos[i].m_pathName + m_resourceInfos[i].m_extension, fullPath))
			{
				return true;
			}
		}
		return false;
	}

	public virtual void Write(byte[] data, ref int offset)
	{
        CMemoryManager.WriteByte((byte)((!m_isAssetBundle) ? 0 : 1), data, ref offset);
		CMemoryManager.WriteString(m_pathInIFS, data, ref offset);
		CMemoryManager.WriteInt(m_tag, data, ref offset);
		CMemoryManager.WriteInt(m_flags, data, ref offset);
		CMemoryManager.WriteShort((short)m_resourceInfos.Count, data, ref offset);
		for (int i = 0; i < m_resourceInfos.Count; i++)
		{
			CMemoryManager.WriteString(m_resourceInfos[i].m_pathName, data, ref offset);
			CMemoryManager.WriteString(m_resourceInfos[i].m_extension, data, ref offset);
			CMemoryManager.WriteInt(m_resourceInfos[i].m_flags, data, ref offset);
		}
		CMemoryManager.WriteShort((short)m_dependencies.Count, data, ref offset);
        for (int j = 0; j < m_dependencies.Count; j++)
		{
            CMemoryManager.WriteString(m_dependencies[j], data, ref offset);
		}
	}

	public virtual void Read(byte[] data, ref int offset)
	{
		m_isAssetBundle = (CMemoryManager.ReadByte(data, ref offset) > 0);
		m_pathInIFS = CMemoryManager.ReadString(data, ref offset);
		m_tag = CMemoryManager.ReadInt(data, ref offset);
		m_flags = CMemoryManager.ReadInt(data, ref offset);
		int num = CMemoryManager.ReadShort(data, ref offset);
		m_resourceInfos.Clear();
		for (int i = 0; i < num; i++)
		{
			AssetInfo_t ai = default(AssetInfo_t);
            ai.m_pathName = CMemoryManager.ReadString(data, ref offset);
            ai.m_extension = CMemoryManager.ReadString(data, ref offset);
            ai.m_flags = CMemoryManager.ReadInt(data, ref offset);
			//string text = AssetInfo_t.m_extension.Replace(".", string.Empty);
			//m_fileExtMap[AssetInfo_t.m_pathName.ToLower()] = text;
			m_resourceInfos.Add(ai);
		}
		num = CMemoryManager.ReadShort(data, ref offset);
		m_dependencies.Clear();
		for (int j = 0; j < num; j++)
		{
            string dependency = CMemoryManager.ReadString(data, ref offset);
			m_dependencies.Add(dependency);
		}
	}

    public virtual void Read(byte[] data, ref int offset,XmlDocument doc, XmlElement pNode)
    {
        m_isAssetBundle = (CMemoryManager.ReadByte(data, ref offset) > 0);
        m_pathInIFS = CMemoryManager.ReadString(data, ref offset);
        m_tag = CMemoryManager.ReadInt(data, ref offset);
        m_flags = CMemoryManager.ReadInt(data, ref offset);
        int num = CMemoryManager.ReadShort(data, ref offset);
        m_resourceInfos.Clear();

        XmlElement ele = doc.CreateElement("AB");
        XmlAttribute abName = doc.CreateAttribute("Name");
        abName.Value = m_pathInIFS;
        ele.Attributes.Append(abName);
        pNode.AppendChild(ele);

        for (int i = 0; i < num; i++)
        {
            AssetInfo_t ai = default(AssetInfo_t);
            ai.m_pathName = CMemoryManager.ReadString(data, ref offset);
            ai.m_extension = CMemoryManager.ReadString(data, ref offset);
            ai.m_flags = CMemoryManager.ReadInt(data, ref offset);
            //string text = AssetInfo_t.m_extension.Replace(".", string.Empty);
            //m_fileExtMap[AssetInfo_t.m_pathName.ToLower()] = text;
            m_resourceInfos.Add(ai);

            XmlElement fEle = doc.CreateElement("File");
            XmlAttribute fName = doc.CreateAttribute("Name");
            fName.Value = ai.m_pathName;
            fEle.Attributes.Append(fName);
            ele.AppendChild(fEle);
        }
        num = CMemoryManager.ReadShort(data, ref offset);
        m_dependencies.Clear();
        for (int j = 0; j < num; j++)
        {
            string dependency = CMemoryManager.ReadString(data, ref offset);
            XmlElement fEle = doc.CreateElement("Dependency");
            XmlAttribute fName = doc.CreateAttribute("Name");
            fName.Value = dependency;
            fEle.Attributes.Append(fName);
            ele.AppendChild(fEle);
        }
    }

    public override string ToString()
    {
        return m_pathInIFS;
    }

#if DEBUG_LOGOUT
    public static System.Diagnostics.Stopwatch mWatch = System.Diagnostics.Stopwatch.StartNew();
#endif

    protected override void DoLoad(string ifsExtractPath)
    {
        AssetBundle resBundle = null;
        string text = CFileManager.CombinePath(ifsExtractPath, m_pathInIFS);
        if (!CFileManager.IsFileExist(text))
        {
            DebugHelper.LogError("File " + text + " can not be found!!!");
            return;
        }
#if DEBUG_LOGOUT
        mWatch.Reset();
        mWatch.Start();
#endif

        int num = 0;
        while (true)
        {
            try
            {
#if Profile_Mode && DEBUG_LOGOUT
                if (s_inBattle_no_bundle_load)
                {
                DebugHelper.LogOutResouceLoad("Load AssetBundle in Battle:" + text);
                DebugHelper.LogError("Load AssetBundle in Battle:" + text);
                }
                Debug.Log("===AssetGroupInfo_t Load Asset Bundle: " + text);
#endif
                if (HasFlag(eBundleFlag.EncryptBundle))
                {
                    byte[] rawdata = CFileManager.ReadFile(text);
                    byte[] decodeBytes = CFileManager.AESDecrypt(rawdata, "forthelichking!!");
                    if (decodeBytes != null)
                    {
                        resBundle = AssetBundle.LoadFromMemory(decodeBytes);
                    }
                    else
                    {
                        DebugHelper.LogError("AES Error: " + text);
                    }
                }
                else
                {
                    resBundle = AssetBundle.LoadFromFile(text);
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogError(e.ToString());
                resBundle = null;
            }
            if (resBundle != null)
            {
                break;
            }
            num++;
            DebugHelper.Log(string.Concat(new object[]
            {
                "Create AssetBundle ",
                text,
                " From File Error! Try Count = ",
                num
            }));
            if (num >= 3)
            {
                CFileManager.s_delegateOnOperateFileFail(text, eFileOperation.ReadFile);
                break;
            }
        }

#if DEBUG_LOGOUT
        DebugHelper.LogOutResouceLoad("==PRINT_LOAD== Load Inner AssetBundle " + m_pathInIFS + " Time: " + mWatch.ElapsedMilliseconds);
#endif

        if (resBundle == null)
        {
            string text2 = string.Empty;
            try
            {
                text2 = CFileManager.GetFileMd5(text);
            }
            catch (Exception)
            {
                text2 = string.Empty;
            }
            string text3 = string.Format("Load AssetBundle {0} Error!!! App version = {1}, Build = {2}, Reversion = {3}, Resource version = {4}, File md5 = {5}", new object[]
            {
                text,
                CVersion.GetAppVersion(),
                CVersion.GetBuildNumber(),
                CVersion.GetRevisonNumber(),
                CVersion.GetUsedResourceVersion(),
                text2
            });
            DebugHelper.LogError(text3);        
        }
        OnBundleLoadResult(resBundle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ifsExtractPath"></param>
    public void LoadAssetBundle(string ifsExtractPath)
	{
		if (m_isAssetBundle)
		{
            Use(ifsExtractPath,false);
		}
	}

    //public void BlockAsyncLoad()
    //{
    //    if (m_assetBundleState == eAssetBundleState.Loading)
    //    {
    //        if (m_AsyncLoadingOpe != null)
    //        {
    //            while (!m_AsyncLoadingOpe.isDone)
    //            {
    //                DebugHelper.LogOutResouceLoad("==PRINT_LOAD_TIME== Block 1");
    //                DebugHelper.Flush(SLogCategory.ResourceLoad);
    //                Thread.Sleep(50);
    //            }
    //            if(m_AsyncLoadingOpe.assetBundle==null)
    //            {
    //                m_assetBundleState = eAssetBundleState.Unload;
    //                return;
    //            }

    //            m_assetBundleState = eAssetBundleState.Loaded;
    //            m_assetBundle = m_AsyncLoadingOpe.assetBundle;
    //            if (m_abandonSyncLoad)
    //                UnloadAssetBundle();
    //            else
    //            {
    //                for (int s = 0; s < m_dependencies.Count; s++)
    //                {
    //                    AssetGroupInfo_t dependency = AssetSystem.instance.GetAssetGroupInfo(m_dependencies[s]);
    //                    if (dependency != null)
    //                        dependency.BlockAsyncLoad();
    //                }
    //            }
    //        }
    //        else
    //        {
    //            m_assetBundleState = eAssetBundleState.Unload;
    //        }
    //    }
    //}
    public void DumpState()
    {
        Debug.LogError("Name: " + m_pathInIFS + "State: " + BundleState);
        for (int s = 0; s < m_dependencies.Count; s++)
        {
            Debug.LogError("Dependency: " + m_dependencies[s]);

            AssetGroupInfo_t dependency = AssetSystem.instance.GetAssetGroupInfo(m_dependencies[s]);
            if (dependency != null)
            {
                Debug.LogError("state: " + dependency.BundleState);
            }
            else
            {
                Debug.LogError("null");
            }
        }

        if (m_AsyncLoadingOpe != null)
        {
            Debug.LogError("ope progress: " + m_AsyncLoadingOpe.progress + " done: " + m_AsyncLoadingOpe.isDone);
            if (m_AsyncLoadingOpe.assetBundle == null)
            {
                Debug.LogError("bundle null");
            }
        }else
        {
            Debug.LogError("null ope");
        }
    }

    protected override void DoAsyncLoad(string ifsExtractPath)
    {
        AssetBundleCreateRequest createRequest = null;
        string text = CFileManager.CombinePath(ifsExtractPath, m_pathInIFS);
        if (!CFileManager.IsFileExist(text))
        {
            DebugHelper.Log("File " + text + " can not be found!!!");
            return;
        }

        int num = 0;
        while (true)
        {
            try
            {
#if Profile_Mode && DEBUG_LOGOUT
                if (s_inBattle_no_bundle_load)
                {
                    DebugHelper.LogOutResouceLoad("Load AssetBundle in Battle:" + text);
                    DebugHelper.LogError("Load AssetBundle in Battle:" + text);
                }
                DebugHelper.Log("===AssetGroupInfo_t Load Asset Bundle: " + text);
#endif
                if (HasFlag(eBundleFlag.EncryptBundle))
                {
                    byte[] rawdata = CFileManager.ReadFile(text);
                    byte[] decodeBytes = CFileManager.AESDecrypt(rawdata, "forthelichking!!");
                    if (decodeBytes != null)
                    {
                        createRequest = AssetBundle.LoadFromMemoryAsync(decodeBytes);
                    }
                    else
                    {
                        DebugHelper.LogError("AES Error: " + text);
                    }
                }
                else
                {
                    createRequest = AssetBundle.LoadFromFileAsync(text);
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogError(e.ToString());
                createRequest = null;
            }
            if (createRequest != null)
            {
                break;
            }
            num++;
            DebugHelper.Log(string.Concat(new object[]
            {
                "Create AsyncLoadingOpe ",
                text,
                " From File Error! Try Count = ",
                num
            }));
            if (num >= 3)
            {
                CFileManager.s_delegateOnOperateFileFail(text, eFileOperation.ReadFile);
                break;
            }
        }
        if (createRequest == null)
        {
            string text2 = string.Empty;
            try
            {
                text2 = CFileManager.GetFileMd5(text);
            }
            catch (Exception)
            {
                text2 = string.Empty;
            }
            string text3 = string.Format("Load AssetBundle {0} Error!!! App version = {1}, Build = {2}, Reversion = {3}, Resource version = {4}, File md5 = {5}", new object[]
            {
                text,
                CVersion.GetAppVersion(),
                CVersion.GetBuildNumber(),
                CVersion.GetRevisonNumber(),
                CVersion.GetUsedResourceVersion(),
                text2
            });
            DebugHelper.LogError(text3);
        }

        OnAsyncBundleLoadResult(createRequest);
    }
    //[System.Diagnostics.DebuggerHidden]
    public void AsyncLoadAssetBundle(string ifsExtractPath)
	{
        if (m_isAssetBundle)
        {
            Use(ifsExtractPath,true);            
        }       
	}



    protected override void DoUnload(bool bunloadAssets)
    {
        if (m_assetBundle != null)
        {
#if DEBUG_LOGOUT
            DebugHelper.LogOutResouceLoad("===AssetGroupInfo_t UnLoad Asset Bundle: " + m_pathInIFS);
#endif
            m_assetBundle.Unload(bunloadAssets);
        }
        OnBundleUnLoadResult();        
    }

    public void UnloadAssetBundle(bool force = false,bool checkref = true )
    {
        UnloadAssetBundle(false, force,checkref);
    }

    public bool Locked
    {
        get;
        set;
    }

    public void UnloadAssetBundle(bool unloadTrue, bool force,bool checkref)
	{
		if (!m_isAssetBundle)
		{
			return;
		}
		if ((IsResident() || IsPermanentAfterLoad() || Locked) && !force)
		{
			return;
		}
        Unload(unloadTrue,(!force)&&checkref);       
	}

	public bool IsAssetBundleLoaded()
	{
		return m_isAssetBundle && BundleState == eAssetBundleState.Loaded;
	}

	public bool IsAssetBundleInLoading()
	{
        if (!m_isAssetBundle)
            return false;
        if (BundleState == eAssetBundleState.Loading)
            return true;
        else if (BundleState == eAssetBundleState.SelfLoaded)
            return (ParentBundleState == eAssetBundleState.Loading);

        return false;
	}

    private bool IsAssetBundleSelfLoaded()
    {
        return m_isAssetBundle && (BundleState >= eAssetBundleState.SelfLoaded);
    }

    public void AddToAssetMap(CUtilDic<string, CUtilList<AssetGroupInfo_t>> map)
	{
		for (int i = 0; i < m_resourceInfos.Count; i++)
		{
			string key = m_resourceInfos[i].m_pathName.ToLower();
			CUtilList<AssetGroupInfo_t> listView = null;
			if (!map.TryGetValue(key, out listView))
			{
				listView = new CUtilList<AssetGroupInfo_t>();
				map.Add(key, listView);
			}
			listView.Add(this);
		}
	}
}

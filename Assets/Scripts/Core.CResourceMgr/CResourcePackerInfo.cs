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

public struct AssetInfo_t
{
    public string m_pathName;

    public string m_extension;

    public int m_flags;
}

public class AssetGroupInfo_t
{
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

	public bool m_isAssetBundle;

	public kAssetBundleState m_assetBundleState;

	public bool m_useASyncLoadingData;

	public AssetBundle m_assetBundle;

	public string m_pathInIFS;

    public int m_tag = 0; //DT=Default, SD=Shader, SP=ScenePrefab, SC=Scene, CH=Character, TX=Text, AT=Action
    //all sub resources in this bundle
	public List<AssetInfo_t> m_resourceInfos = new List<AssetInfo_t>();
    //key: sub resource name without ext  value: ext
    //aim to get full name according name without ext+ext, fucking meaningless
	//public Dictionary<string, string> m_fileExtMap = new Dictionary<string, string>();

	public int m_flags;
    public int miRefCnt = 0;

	//private AssetGroupInfo_t m_parent;

	//public CUtilList<AssetGroupInfo_t> m_children = new CUtilList<AssetGroupInfo_t>();
    public CUtilList<string> m_dependencies = new CUtilList<string>();

	public AssetGroupInfo_t(bool isAssetBundle)
	{
		m_isAssetBundle = isAssetBundle;
		m_assetBundleState = kAssetBundleState.Unload;
		m_useASyncLoadingData = false;
	    miRefCnt = 0;
	}

	public bool IsResident()
	{
        return m_dependencies.Count==0 && HasFlag(kBundleFlag.Resident);
	}

	public bool IsUnCompress()
	{
		return HasFlag(kBundleFlag.UnCompress);
	}

	public bool IsKeepInResources()
	{
		return HasFlag(kBundleFlag.KeepInResources);
	}

    public bool IsEncryptBundle()
	{
        return HasFlag(kBundleFlag.EncryptBundle);
	}

	public bool IsCompleteAssets()
	{
		return !HasFlag(kBundleFlag.UnCompleteAsset);
	}

	public void AddResourceInfo(ref AssetInfo_t resourceInfo)
	{
		m_resourceInfos.Add(resourceInfo);
	}

	public bool HasFlag(kBundleFlag flag)
	{
		return (m_flags & (int)flag) > 0;
	}

#if UNITY_EDITOR
    public void SetFlag(kBundleFlag flag)
    {
        m_flags = m_flags | (int) flag;
    }
#endif

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

    //public void CheckAlreadyContainResourceFileName(string fullPath)
    //{
    //    if (IsAllowDuplicateNames())
    //    {
    //        return;
    //    }
    //    string fullName = CFileManager.GetFullName(fullPath);
    //    for (int i = 0; i < m_resourceInfos.Count; i++)
    //    {
    //        string text = CFileManager.GetFullName(m_resourceInfos[i].m_pathName) + m_resourceInfos[i].m_extension;
    //        if (string.Equals(text, fullName))
    //        {
    //            string text2 = string.Concat(new string[]
    //            {
    //                "Duplicated File \"",
    //                fullPath,
    //                "\" and \"",
    //                m_resourceInfos[i].m_pathName,
    //                m_resourceInfos[i].m_extension,
    //                "\" in resource bundle:\"",
    //                m_pathInIFS,
    //                "\""
    //            });
    //            UnityEngine.Debug.LogError(text2);
    //            throw new Exception(text2);
    //        }
    //    }
    //}

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ifsExtractPath"></param>
	public void LoadAssetBundle(string ifsExtractPath)
	{
        if (m_pathInIFS == "AssetBundle/FloatText.assetbundle")
            Debug.LogError("Ab Load");
		if (m_isAssetBundle)
		{
		    if (IsAssetBundleLoaded())
		    {
		        miRefCnt++;
		    }
		    for (int s = 0; s < m_dependencies.Count; s++)
		    {
		        AssetGroupInfo_t dependency = AssetSystem.instance.GetAssetGroupInfo(m_dependencies[s]);
                if (dependency != null && dependency.m_isAssetBundle)
                {
                    dependency.LoadAssetBundle(ifsExtractPath);             
                }
		    }
			if (m_assetBundleState != kAssetBundleState.Unload)
			{
				return;
			}
			m_useASyncLoadingData = false;
			string text = CFileManager.CombinePath(ifsExtractPath, m_pathInIFS);
			if (CFileManager.IsFileExist(text))
			{
				{
					int num = 0;
					while (true)
					{
						try
						{
#if Profile_Mode
                            Debug.Log("===AssetGroupInfo_t Load Asset Bundle: "+text);
#endif
							m_assetBundle = AssetBundle.LoadFromFile(text);
						}
						catch (Exception)
						{
							m_assetBundle = null;
						}
						if (m_assetBundle != null)
						{
							break;
						}
                        num++;
						UnityEngine.Debug.Log(string.Concat(new object[]
						{
							"Create AssetBundle ",
							text,
							" From File Error! Try Count = ",
							num
						}));
						if (num >= 3)
						{
                            CFileManager.s_delegateOnOperateFileFail(text, kFileOperation.ReadFile);
						    break;
						}
					}
				}
				if (m_assetBundle == null)
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
					UnityEngine.Debug.LogError(text3);
				    return;
				}
			}
			else
			{
				UnityEngine.Debug.Log("File " + text + " can not be found!!!");
			    return;
			}
		    miRefCnt++;
			m_assetBundleState = kAssetBundleState.Loaded;
		}
	}

    [System.Diagnostics.DebuggerHidden]
	public IEnumerator ASyncLoadAssetBundle(string ifsExtractPath)
	{
        return null;// TODO;
	}

	public void UnloadAssetBundle(bool force = false)
	{
		if (!m_isAssetBundle)
		{
			return;
		}
		if (IsResident() && !force)
		{
			return;
		}
		if (m_assetBundleState == kAssetBundleState.Loaded)
		{
			if (m_assetBundle != null)
			{
			    if (miRefCnt > 0)
			    {
			        miRefCnt--;
			    }
			    else
			    {
                    Debug.LogError("Ref Count is 0: " + m_pathInIFS);
			        return;
			    }
			    if (miRefCnt == 0)
			    {
#if Profile_Mode
                    Debug.Log("===AssetGroupInfo_t UnLoad Asset Bundle: " + m_pathInIFS);
#endif
			        m_assetBundle.Unload(false);
			        m_assetBundle = null;
                    m_assetBundleState = kAssetBundleState.Unload;
			    }
			}
		}
		else if (m_assetBundleState == kAssetBundleState.Loading)
		{
			m_useASyncLoadingData = false;
		}
        for (int s = 0; s < m_dependencies.Count; s++)
        {
            AssetGroupInfo_t dependency = AssetSystem.instance.GetAssetGroupInfo(m_dependencies[s]);
            if (dependency != null)
            {
                dependency.UnloadAssetBundle(force);
            }
        }
	}

	public bool IsAssetBundleLoaded()
	{
		return m_isAssetBundle && m_assetBundleState == kAssetBundleState.Loaded;
	}

	public bool IsAssetBundleInLoading()
	{
		return m_isAssetBundle && m_assetBundleState == kAssetBundleState.Loading;
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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetManifest_t
{
    //类似于manifest的二进制文件，有多少ab，每个ab有多少子资源，分别被多少子ab依赖
	public static string s_assetGroupInfoSetFileName = "ManifestInfo.bytes";

	public string m_version;

	public string m_publish;

	public string m_pakPath;

    //all AssetGroupInfo_t s in this list no matter leafs or roots 
    public Dictionary<string, AssetGroupInfo_t> m_assetGroupInfosAll = new Dictionary<string, AssetGroupInfo_t>();
    //unload resourcepackerinfo by tag
    public Dictionary<int, List<AssetGroupInfo_t>> m_assetGroupTagMap = new Dictionary<int, List<AssetGroupInfo_t>>();
    //一个assetbundle里可能会有许多子资源，同样某个子资源可能在几个assetbundle中冗余
    //这个map的意义是当你要载一个子资源，只要从几个ab中找到那个已经加载完成的
	private CUtilDic<string, CUtilList<AssetGroupInfo_t>> m_assetMap = new CUtilDic<string, CUtilList<AssetGroupInfo_t>>();

	public void Dispose()
	{
	    foreach (string str in m_assetGroupInfosAll.Keys)
	    {
	        if (m_assetGroupInfosAll[str].IsAssetBundleLoaded())
	        {
                m_assetGroupInfosAll[str].UnloadAssetBundle(false);
	        }
	    }
		m_assetGroupInfosAll.Clear();
		m_assetMap.Clear();
	}

	public void AddAssetGroupInfo(AssetGroupInfo_t resourceInfo)
	{
	    if (m_assetGroupInfosAll.ContainsKey(resourceInfo.m_pathInIFS))
	    {
            Debug.LogError("Can't get same name: " + resourceInfo.m_pathInIFS);
	        return;
	    }
        m_assetGroupInfosAll.Add(resourceInfo.m_pathInIFS, resourceInfo);
	    if (!m_assetGroupTagMap.ContainsKey((int)(resourceInfo.m_tag)))
	    {
            m_assetGroupTagMap.Add((int)(resourceInfo.m_tag), new List<AssetGroupInfo_t>());
	    }
        m_assetGroupTagMap[(int)(resourceInfo.m_tag)].Add(resourceInfo);
	}

    public AssetGroupInfo_t GetAssetGroupInfo(string name)
    {
        return m_assetGroupInfosAll[name];
    }

	public void Write(byte[] data, ref int offset)
	{
		CMemoryManager.WriteString(m_version, data, ref offset);
		CMemoryManager.WriteString(m_publish, data, ref offset);
		CMemoryManager.WriteString(m_pakPath, data, ref offset);
        CMemoryManager.WriteShort((short)m_assetGroupInfosAll.Count, data, ref offset);
	    foreach (string res in m_assetGroupInfosAll.Keys)
	    {
            m_assetGroupInfosAll[res].Write(data, ref offset);
	    }
	}

	public void Read(byte[] data, ref int offset)
	{
		m_version = CMemoryManager.ReadString(data, ref offset);
		m_publish = CMemoryManager.ReadString(data, ref offset);
		m_pakPath = CMemoryManager.ReadString(data, ref offset);
		int num = CMemoryManager.ReadShort(data, ref offset);
		m_assetGroupInfosAll.Clear();
		for (int i = 0; i < num; i++)
		{
			AssetGroupInfo_t cAssetGroupInfo = new AssetGroupInfo_t(false);
			cAssetGroupInfo.Read(data, ref offset);
			AddAssetGroupInfo(cAssetGroupInfo);
		}
	}

    public void SaveToXML(byte[] data, ref int offset)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("root");
        XmlAttribute ver = doc.CreateAttribute("Ver");
        ver.Value = "1.0.0";
        root.Attributes.Append(ver);
        doc.AppendChild(root);

        m_version = CMemoryManager.ReadString(data, ref offset);
        m_publish = CMemoryManager.ReadString(data, ref offset);
        m_pakPath = CMemoryManager.ReadString(data, ref offset);
        int num = CMemoryManager.ReadShort(data, ref offset);
        m_assetGroupInfosAll.Clear();
        for (int i = 0; i < num; i++)
        {
            AssetGroupInfo_t cAssetGroupInfo = new AssetGroupInfo_t(false);
            cAssetGroupInfo.Read(data, ref offset, doc, root);
            AddAssetGroupInfo(cAssetGroupInfo);
        }
        doc.Save(Application.dataPath + "/PackerInfo.xml");
    }

	public void Write(StreamWriter streamWriter)
	{
		streamWriter.WriteLine("=========================================================================================================");
		streamWriter.WriteLine("AssetManifest_t : Version = " + m_version);
		streamWriter.WriteLine("=========================================================================================================");
	    foreach (string str in m_assetGroupInfosAll.Keys)
		{
            AssetGroupInfo_t cAssetGroupInfo = m_assetGroupInfosAll[str];
			streamWriter.WriteLine(string.Concat(new object[]
			{
				"    AssetGroupInfo_t : Path = ",
				cAssetGroupInfo.m_pathInIFS,
				", IsAssetBundle = ",
				cAssetGroupInfo.m_isAssetBundle
			}));
			for (int j = 0; j < cAssetGroupInfo.m_resourceInfos.Count; j++)
			{
				AssetInfo_t ai = cAssetGroupInfo.m_resourceInfos[j];
				streamWriter.WriteLine(string.Concat(new object[]
				{
					"        Resource : Path = ",
                    ai.m_pathName,
					", Extension = ",
                    ai.m_extension,
					", Flags = ",
                    ai.m_flags
				}));
			}
			if (cAssetGroupInfo.m_dependencies != null)
			{
                for (int k = 0; k < cAssetGroupInfo.m_dependencies.Count; k++)
				{
                    AssetGroupInfo_t cAssetGroupInfo2 = GetAssetGroupInfo(cAssetGroupInfo.m_dependencies[k]);
					streamWriter.WriteLine(string.Concat(new object[]
					{
						"        Child AssetGroupInfo_t : Path = ",
						cAssetGroupInfo2.m_pathInIFS,
						", IsAssetBundle = ",
						cAssetGroupInfo2.m_isAssetBundle
					}));
					for (int l = 0; l < cAssetGroupInfo2.m_resourceInfos.Count; l++)
					{
						AssetInfo_t ResourceInfo2_t = cAssetGroupInfo2.m_resourceInfos[l];
						streamWriter.WriteLine(string.Concat(new object[]
						{
							"            Resource : Path = ",
							ResourceInfo2_t.m_pathName,
							", Extension = ",
							ResourceInfo2_t.m_extension,
							", Flags = ",
							ResourceInfo2_t.m_flags
						}));
					}
				}
			}
			streamWriter.WriteLine(" ");
		}
		streamWriter.WriteLine("=========================================================================================================");
	}

	public void CreateAssetMap()
	{
		foreach(AssetGroupInfo_t info in m_assetGroupInfosAll.Values)
		{
            info.AddToAssetMap(m_assetMap);
		}
	}
    //通过文件名找到对应的ab，如果对应多个ab，优先找到已经加载完的那个ab，不然就用第一个ab
	public AssetGroupInfo_t GetAssetInGroup(string resourceKey)
	{
		CUtilList<AssetGroupInfo_t> listView = null;
		if (m_assetMap.TryGetValue(resourceKey, out listView) && listView != null && listView.Count > 0)
		{
			for (int i = 0; i < listView.Count; i++)
			{
				if (listView[i].IsAssetBundleLoaded())
				{
					return listView[i];
				}
			}
			return listView[0];
		}
		return null;
	}
}

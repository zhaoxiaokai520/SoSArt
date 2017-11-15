using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetManifest_t
{
    public static string s_assetGroupInfoSetFileName = "ManifestInfo.bytes";

    public string m_version;

    public string m_publish;

    public string m_pakPath;

    public byte[] m_hashStamp = new byte[8];

    public Dictionary<string, AssetGroupInfo_t> m_assetGroupInfosAll = new Dictionary<string, AssetGroupInfo_t>();

    public Dictionary<int, List<AssetGroupInfo_t>> m_assetGroupTagMap = new Dictionary<int, List<AssetGroupInfo_t>>();

    private CUtilDic<string, CUtilList<AssetGroupInfo_t>> m_assetMap = new CUtilDic<string, CUtilList<AssetGroupInfo_t>>();

    public AssetManifest_t()
    {
        DateTime now = DateTime.Now;
        int num = 0;
        CMemoryManager.WriteDateTime(ref now, this.m_hashStamp, ref num);
    }

    public void Dispose()
    {
        foreach (string current in this.m_assetGroupInfosAll.Keys)
        {
            if (this.m_assetGroupInfosAll[current].IsAssetBundleLoaded())
            {
                this.m_assetGroupInfosAll[current].UnloadAssetBundle(false,false);
            }
        }
        this.m_assetGroupInfosAll.Clear();
        this.m_assetMap.Clear();
    }

    public void AddAssetGroupInfo(AssetGroupInfo_t resourceInfo)
    {
        this.m_assetGroupInfosAll.Add(resourceInfo.m_pathInIFS, resourceInfo);
        if (!this.m_assetGroupTagMap.ContainsKey(resourceInfo.m_tag))
        {
            this.m_assetGroupTagMap.Add(resourceInfo.m_tag, new List<AssetGroupInfo_t>());
        }
        this.m_assetGroupTagMap[resourceInfo.m_tag].Add(resourceInfo);
    }

    public AssetGroupInfo_t GetAssetGroupInfo(string name)
    {
        return this.m_assetGroupInfosAll[name];
    }

    public AssetGroupInfo_t TryGetAssetGroupInfo(string name)
    {
        AssetGroupInfo_t res = null;
        m_assetGroupInfosAll.TryGetValue(name, out res);
        return res;
    }

    public void Write(byte[] data, ref int offset)
    {
        CMemoryManager.WriteString(this.m_version, data, ref offset);
        CMemoryManager.WriteString(this.m_publish, data, ref offset);
        CMemoryManager.WriteString(this.m_pakPath, data, ref offset);
        for (int i = 0; i < 8; i++)
        {
            CMemoryManager.WriteByte(this.m_hashStamp[i], data, ref offset);
        }
        CMemoryManager.WriteShort((short)this.m_assetGroupInfosAll.Count, data, ref offset);
        foreach (string current in this.m_assetGroupInfosAll.Keys)
        {
            this.m_assetGroupInfosAll[current].Write(data, ref offset);
        }
    }

    public void Read(byte[] data, ref int offset)
    {
        this.m_version = CMemoryManager.ReadString(data, ref offset);
        this.m_publish = CMemoryManager.ReadString(data, ref offset);
        this.m_pakPath = CMemoryManager.ReadString(data, ref offset);
        for (int i = 0; i < 8; i++)
        {
            this.m_hashStamp[i] = (byte)CMemoryManager.ReadByte(data, ref offset);
        }
        int num = CMemoryManager.ReadShort(data, ref offset);
        this.m_assetGroupInfosAll.Clear();
        for (int j = 0; j < num; j++)
        {
            AssetGroupInfo_t assetGroupInfo_t = new AssetGroupInfo_t(false);
            assetGroupInfo_t.Read(data, ref offset);
            this.AddAssetGroupInfo(assetGroupInfo_t);
        }
    }

    public void SaveToXML(byte[] data, ref int offset)
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement xmlElement = xmlDocument.CreateElement("root");
        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Ver");
        xmlAttribute.Value = "1.0.0";
        xmlElement.Attributes.Append(xmlAttribute);
        xmlDocument.AppendChild(xmlElement);
        this.m_version = CMemoryManager.ReadString(data, ref offset);
        this.m_publish = CMemoryManager.ReadString(data, ref offset);
        this.m_pakPath = CMemoryManager.ReadString(data, ref offset);
        int num = CMemoryManager.ReadShort(data, ref offset);
        this.m_assetGroupInfosAll.Clear();
        for (int i = 0; i < num; i++)
        {
            AssetGroupInfo_t assetGroupInfo_t = new AssetGroupInfo_t(false);
            assetGroupInfo_t.Read(data, ref offset, xmlDocument, xmlElement);
            this.AddAssetGroupInfo(assetGroupInfo_t);
        }
        xmlDocument.Save(Application.dataPath + "/PackerInfo.xml");
    }

    public void Write(StreamWriter streamWriter)
    {
        streamWriter.WriteLine("=========================================================================================================");
        streamWriter.WriteLine("AssetManifest_t : Version = " + this.m_version);
        streamWriter.WriteLine("=========================================================================================================");
        foreach (string current in this.m_assetGroupInfosAll.Keys)
        {
            AssetGroupInfo_t assetGroupInfo_t = this.m_assetGroupInfosAll[current];
            streamWriter.WriteLine(string.Concat(new object[]
            {
                "    AssetGroupInfo_t : Path = ",
                assetGroupInfo_t.m_pathInIFS,
                ", IsAssetBundle = ",
                assetGroupInfo_t.m_isAssetBundle
            }));
            for (int i = 0; i < assetGroupInfo_t.m_resourceInfos.Count; i++)
            {
                AssetInfo_t assetInfo_t = assetGroupInfo_t.m_resourceInfos[i];
                streamWriter.WriteLine(string.Concat(new object[]
                {
                    "        Resource : Path = ",
                    assetInfo_t.m_pathName,
                    ", Extension = ",
                    assetInfo_t.m_extension,
                    ", Flags = ",
                    assetInfo_t.m_flags
                }));
            }
            if (assetGroupInfo_t.m_dependencies != null)
            {
                for (int j = 0; j < assetGroupInfo_t.m_dependencies.Count; j++)
                {
                    AssetGroupInfo_t assetGroupInfo = this.GetAssetGroupInfo(assetGroupInfo_t.m_dependencies[j]);
                    streamWriter.WriteLine(string.Concat(new object[]
                    {
                        "        Child AssetGroupInfo_t : Path = ",
                        assetGroupInfo.m_pathInIFS,
                        ", IsAssetBundle = ",
                        assetGroupInfo.m_isAssetBundle
                    }));
                    for (int k = 0; k < assetGroupInfo.m_resourceInfos.Count; k++)
                    {
                        AssetInfo_t assetInfo_t2 = assetGroupInfo.m_resourceInfos[k];
                        streamWriter.WriteLine(string.Concat(new object[]
                        {
                            "            Resource : Path = ",
                            assetInfo_t2.m_pathName,
                            ", Extension = ",
                            assetInfo_t2.m_extension,
                            ", Flags = ",
                            assetInfo_t2.m_flags
                        }));
                    }
                }
            }
            streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("=========================================================================================================");
    }

    public void Prepare()
    {
        Dictionary<string, AssetGroupInfo_t>.ValueCollection.Enumerator enumerator = this.m_assetGroupInfosAll.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.AddToAssetMap(this.m_assetMap);
            enumerator.Current.PrepareParentBundle();
        }

        enumerator = this.m_assetGroupInfosAll.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.CheckDependencyLoop(null);
        }
    }

    public AssetGroupInfo_t GetAssetInGroup(string resourceKey)
    {
        CUtilList<AssetGroupInfo_t> cUtilList = null;
        if (this.m_assetMap.TryGetValue(resourceKey, out cUtilList) && cUtilList != null && cUtilList.Count > 0)
        {
            for (int i = 0; i < cUtilList.Count; i++)
            {
                if (cUtilList[i].IsAssetBundleLoaded())
                {
                    return cUtilList[i];
                }
            }
            return cUtilList[0];
        }
        return null;
    }
}

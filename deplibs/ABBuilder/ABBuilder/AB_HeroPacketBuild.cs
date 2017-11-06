using System;
using System.Collections.Generic;
using UnityEngine;

public class AB_HeroPacketBuild
{
	public AB_AssetBuildMgr.E_ABBUNLDE_TYPE meType = AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ILLEGAL;

	public string msBundleName = "";

	public AB_HeroBuildBase mHeroBuild;

	private List<AB_HeroCmdBase> mCmdList = new List<AB_HeroCmdBase>();

	private AssetGroupInfo_t mAssetGroupInfo;

	private AB_AssetBuildMgr.AssetBundleBuildEX mAssetBundle;

	public Dictionary<string, int> mAllABFileList = new Dictionary<string, int>();

	public Dictionary<string, int> mAllAssetGroupFileList = new Dictionary<string, int>();

	private int miFlag;

	public AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE eType, string bundlename, int flag = 0)
	{
		this.meType = eType;
		this.msBundleName = bundlename;
		this.miFlag = flag;
		if (!this.msBundleName.EndsWith(AB_Common.AB_EXT))
		{
			this.msBundleName += AB_Common.AB_EXT;
		}
	}

	public void AddCmd(AB_HeroCmdBase cmd)
	{
		cmd.mPacketBuild = this;
		this.mCmdList.Add(cmd);
	}

	public void BuildCmd()
	{
		for (int i = 0; i < this.mCmdList.Count; i++)
		{
			this.mCmdList[i].BuildCmd();
		}
	}

	public void BuildRes(Dictionary<string, int> excludeRes, bool bAuto = true)
	{
		for (int i = 0; i < this.mCmdList.Count; i++)
		{
			List<string> aBFileList = this.mCmdList[i].GetABFileList();
			if (aBFileList != null)
			{
				if (excludeRes != null && excludeRes.Count > 0)
				{
					for (int j = 0; j < aBFileList.Count; j++)
					{
						if (!this.mAllABFileList.ContainsKey(aBFileList[j]))
						{
							bool flag = false;
							using (Dictionary<string, int>.KeyCollection.Enumerator enumerator = excludeRes.Keys.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (enumerator.Current.Equals(aBFileList[j]))
									{
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								this.mAllABFileList.Add(aBFileList[j], 1);
							}
						}
					}
				}
				else
				{
					for (int k = 0; k < aBFileList.Count; k++)
					{
						if (!this.mAllABFileList.ContainsKey(aBFileList[k]))
						{
							this.mAllABFileList.Add(aBFileList[k], 1);
						}
					}
				}
			}
			List<string> assetGroupFileList = this.mCmdList[i].GetAssetGroupFileList();
			for (int l = 0; l < assetGroupFileList.Count; l++)
			{
				if (!this.mAllAssetGroupFileList.ContainsKey(assetGroupFileList[l]))
				{
					this.mAllAssetGroupFileList.Add(assetGroupFileList[l], 1);
				}
			}
		}
		if (bAuto)
		{
			this.BuildABandInfo();
		}
	}

	public void BuildABandInfo()
	{
		if (this.mAllABFileList.Count > 0)
		{
			this.BuildAssetBundle();
			this.BuildResInfo();
		}
	}

	private void BuildAssetBundle()
	{
		this.mAssetBundle = AB_AssetBuildMgr.GetAssetBundleBuild(this.meType, this.msBundleName);
		foreach (string current in this.mAllABFileList.Keys)
		{
			if (this.meType == AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SHADER)
			{
				AB_AssetBuildMgr.ForceAddAsset(this.mAssetBundle, current);
			}
			else
			{
				string extension = CFileManager.GetExtension(current);
				if (!extension.ToLower().Equals(".cs") && !extension.ToLower().Equals(".shader") && !extension.ToLower().Equals(".meta"))
				{
					if (ABSharedRes.mSharedResMap.ContainsKey(current))
					{
						Dictionary<string, int> arg_9C_0 = ABSharedRes.mSharedResMap;
						string key = current;
						int num = arg_9C_0[key];
						arg_9C_0[key] = num + 1;
					}
					else
					{
						AB_AssetBuildMgr.ForceAddAsset(this.mAssetBundle, current);
					}
				}
			}
		}
	}

	private void BuildResInfo()
	{
		this.mAssetGroupInfo = AB_GatherResInfo.CreateCResourcePackerInfo();
		this.mAssetGroupInfo.m_pathInIFS = AB_Common.AB_RES_INFO_PATH + this.msBundleName.ToLower();
		this.mAssetGroupInfo.m_tag = this.ABType2ResTag(this.meType);
		this.mAssetGroupInfo.m_flags = this.miFlag;
		if (AB_Encrypt.IsEncryptFile(this.miFlag))
		{
			AB_Encrypt.AddEncryptFile(this.mAssetGroupInfo);
		}
		foreach (string current in this.mAllAssetGroupFileList.Keys)
		{
			AssetInfo_t item = default(AssetInfo_t);
			item.m_pathName = CFileManager.EraseExtension(AB_Common.PathRemoveAssets(current));
			item.m_extension = CFileManager.GetExtension(current);
			this.mAssetGroupInfo.m_resourceInfos.Add(item);
		}
		AB_HeroBuildMgr.mHeroResPackers.Add(this.mAssetGroupInfo);
	}

	private int ABType2ResTag(AB_AssetBuildMgr.E_ABBUNLDE_TYPE eType)
	{
		switch (eType)
		{
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SHADER:
			return 10;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_TEXTURE:
			return 8;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_EFFECT:
			return 11;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_UI:
			return 3;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SCENE:
			return 3;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER:
			return 2;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTION:
			return 5;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTORINFO:
			return 6;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_GAMEDATA:
			return 3;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_FONT:
			return 13;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SOUND:
			return 12;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_WEAPON:
			return 15;
		case AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_GEM:
			return 16;
		}
		Debug.LogError("Can't Get Type: " + eType);
		return 17;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AB_HeroObBuild : AB_HeroBuildBase
{
	private AB_HeroPacketBuild mHeroBattleBase;

	private List<AB_HeroPacketBuild> mHeroSkinList = new List<AB_HeroPacketBuild>();

	public AB_HeroObBuild(DirectoryInfo info) : base(info)
	{
		this.CreatePacketBuild();
	}

	private void CreatePacketBuild()
	{
		string[] expr_1E = this.mFolderInfo.FullName.Split(new char[]
		{
			Path.DirectorySeparatorChar
		});
		string text = expr_1E[expr_1E.Length - 1];
		DirectoryInfo[] directories = this.mFolderInfo.GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo = directories[i];
			if (directoryInfo.Name.Equals(text))
			{
				this.mHeroBattleBase = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_ob", 0);
				this.mHeroBattleBase.mHeroBuild = this;
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo2 = directories2[j];
					if (directoryInfo2.Name.ToLower().Equals("skin"))
					{
						this.mHeroBattleBase.AddCmd(new AB_HeroCmdSkin(directoryInfo2));
					}
				}
			}
			else if (directoryInfo.Name.Contains(text))
			{
				AB_HeroPacketBuild aB_HeroPacketBuild = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, directoryInfo.Name + "_ob", 0);
				aB_HeroPacketBuild.mHeroBuild = this;
				this.mHeroSkinList.Add(aB_HeroPacketBuild);
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo3 = directories2[j];
					if (directoryInfo3.Name.ToLower().Equals("skin"))
					{
						aB_HeroPacketBuild.AddCmd(new AB_HeroCmdSkin(directoryInfo3));
					}
				}
			}
		}
	}

	public override void Build()
	{
		if (this.mHeroBattleBase == null)
		{
			Debug.LogError("Base Hero is Empty!! " + this.mFolderInfo.Name);
			return;
		}
		this.mHeroBattleBase.BuildCmd();
		for (int i = 0; i < this.mHeroSkinList.Count; i++)
		{
			this.mHeroSkinList[i].BuildCmd();
		}
		this.mHeroBattleBase.BuildRes(null, true);
		for (int j = 0; j < this.mHeroSkinList.Count; j++)
		{
			this.mHeroSkinList[j].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
	}
}

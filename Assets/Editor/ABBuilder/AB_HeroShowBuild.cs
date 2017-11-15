using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AB_HeroShowBuild : AB_HeroBuildBase
{
	private AB_HeroPacketBuild mHeroBattleBase;

	private List<AB_HeroPacketBuild> mHeroSkinList = new List<AB_HeroPacketBuild>();

	private List<AB_HeroPacketBuild> mHeroWeaponList = new List<AB_HeroPacketBuild>();

	public AB_HeroShowBuild(DirectoryInfo info) : base(info)
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
				this.mHeroBattleBase = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_show", 0);
				this.mHeroBattleBase.mHeroBuild = this;
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo2 = directories2[j];
					if (directoryInfo2.Name.ToLower().Equals("skin"))
					{
						this.mHeroBattleBase.AddCmd(new AB_HeroCmdSkin(directoryInfo2));
					}
					else if (directoryInfo2.Name.ToLower().Equals("effect"))
					{
						this.mHeroBattleBase.AddCmd(new AB_HeroCmdEffect(directoryInfo2));
					}
					else if (directoryInfo2.Name.ToLower().Equals("sound"))
					{
						this.mHeroBattleBase.AddCmd(new AB_HeroCmdSound(directoryInfo2));
					}
					else if (directoryInfo2.Name.ToLower().Equals("icon"))
					{
						this.mHeroBattleBase.AddCmd(new AB_HeroCmdIcon(directoryInfo2));
					}
				}
			}
			else if (directoryInfo.Name.Contains(text))
			{
				AB_HeroPacketBuild aB_HeroPacketBuild = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, directoryInfo.Name + "_show", 0);
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
					else if (directoryInfo3.Name.ToLower().Equals("effect"))
					{
						aB_HeroPacketBuild.AddCmd(new AB_HeroCmdEffect(directoryInfo3));
					}
					else if (directoryInfo3.Name.ToLower().Equals("sound"))
					{
						aB_HeroPacketBuild.AddCmd(new AB_HeroCmdSound(directoryInfo3));
					}
					else if (directoryInfo3.Name.ToLower().Equals("icon"))
					{
						aB_HeroPacketBuild.AddCmd(new AB_HeroCmdIcon(directoryInfo3));
					}
				}
			}
			else if (directoryInfo.Name.ToLower().Equals("weapon"))
			{
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo4 = directories2[j];
					AB_HeroPacketBuild aB_HeroPacketBuild2 = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_" + directoryInfo4.Name + "_show", 0);
					aB_HeroPacketBuild2.mHeroBuild = this;
					aB_HeroPacketBuild2.AddCmd(new AB_HeroCmdWeapon(directoryInfo4));
					this.mHeroWeaponList.Add(aB_HeroPacketBuild2);
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
		for (int j = 0; j < this.mHeroWeaponList.Count; j++)
		{
			this.mHeroWeaponList[j].BuildCmd();
		}
		this.mHeroBattleBase.BuildRes(null, true);
		for (int k = 0; k < this.mHeroSkinList.Count; k++)
		{
			this.mHeroSkinList[k].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
		for (int l = 0; l < this.mHeroWeaponList.Count; l++)
		{
			this.mHeroWeaponList[l].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
	}
}

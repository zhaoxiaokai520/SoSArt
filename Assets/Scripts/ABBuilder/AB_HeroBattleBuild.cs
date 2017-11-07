using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AB_HeroBattleBuild : AB_HeroBuildBase
{
	private AB_HeroPacketBuild mHeroBattleBase;

	private AB_HeroPacketBuild mHeroBattleEffectBase;

	private AB_HeroPacketBuild mHeroAction;

	private List<AB_HeroPacketBuild> mHeroSkinList = new List<AB_HeroPacketBuild>();

	private List<AB_HeroPacketBuild> mHeroWeaponList = new List<AB_HeroPacketBuild>();

	private List<AB_HeroPacketBuild> mHeroGemList = new List<AB_HeroPacketBuild>();

	public AB_HeroBattleBuild(DirectoryInfo info) : base(info)
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
				this.mHeroBattleBase = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_battle", 0);
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
			else if (directoryInfo.Name.ToLower().Equals("action") && this.mHeroBattleBase != null)
			{
				int num = 0;
				num |= 8;
				this.mHeroAction = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTION, text + "_skill", num);
				this.mHeroAction.AddCmd(new AB_HeroCmdAction(directoryInfo));
			}
			else if (directoryInfo.Name.Contains(text))
			{
				AB_HeroPacketBuild aB_HeroPacketBuild = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, directoryInfo.Name + "_battle", 0);
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
			else if (directoryInfo.Name.ToLower().Equals("actorinfo"))
			{
				ABRoleBuildMgr.mActorInfo.AddCmd(new AB_HeroCmdActorInfo(directoryInfo));
			}
			else if (directoryInfo.Name.ToLower().Equals("weapon"))
			{
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo4 = directories2[j];
					AB_HeroPacketBuild aB_HeroPacketBuild2 = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_" + directoryInfo4.Name + "_battle", 0);
					aB_HeroPacketBuild2.mHeroBuild = this;
					aB_HeroPacketBuild2.AddCmd(new AB_HeroCmdWeapon(directoryInfo4));
					this.mHeroWeaponList.Add(aB_HeroPacketBuild2);
				}
			}
			else if (directoryInfo.Name.ToLower().Equals("gem"))
			{
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				for (int j = 0; j < directories2.Length; j++)
				{
					DirectoryInfo directoryInfo5 = directories2[j];
					AB_HeroPacketBuild aB_HeroPacketBuild3 = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_gem_" + directoryInfo5.Name, 0);
					aB_HeroPacketBuild3.mHeroBuild = this;
					aB_HeroPacketBuild3.AddCmd(new AB_HeroCmdGem(directoryInfo5));
					this.mHeroGemList.Add(aB_HeroPacketBuild3);
				}
			}
			else if (directoryInfo.Name.ToLower().Equals("effect"))
			{
				this.mHeroBattleEffectBase = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CHARACTER, text + "_battle_effect", 0);
				this.mHeroBattleEffectBase.mHeroBuild = this;
				this.mHeroBattleEffectBase.AddCmd(new AB_HeroCmdEffect(directoryInfo));
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
		if (this.mHeroAction != null)
		{
			this.mHeroAction.BuildCmd();
		}
		else
		{
			Debug.LogError("Hero Action is Empty!! " + this.mFolderInfo.Name);
		}
		for (int i = 0; i < this.mHeroSkinList.Count; i++)
		{
			this.mHeroSkinList[i].BuildCmd();
		}
		for (int j = 0; j < this.mHeroWeaponList.Count; j++)
		{
			this.mHeroWeaponList[j].BuildCmd();
		}
		for (int k = 0; k < this.mHeroGemList.Count; k++)
		{
			this.mHeroGemList[k].BuildCmd();
		}
		if (this.mHeroBattleEffectBase != null)
		{
			this.mHeroBattleEffectBase.BuildCmd();
		}
		if (this.mHeroBattleBase != null)
		{
			this.mHeroBattleBase.BuildRes(null, true);
		}
		if (this.mHeroAction != null)
		{
			this.mHeroAction.BuildRes(null, true);
		}
		for (int l = 0; l < this.mHeroSkinList.Count; l++)
		{
			this.mHeroSkinList[l].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
		for (int m = 0; m < this.mHeroWeaponList.Count; m++)
		{
			this.mHeroWeaponList[m].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
		for (int n = 0; n < this.mHeroGemList.Count; n++)
		{
			this.mHeroGemList[n].BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
		if (this.mHeroBattleEffectBase != null)
		{
			this.mHeroBattleEffectBase.BuildRes(this.mHeroBattleBase.mAllABFileList, true);
		}
	}
}

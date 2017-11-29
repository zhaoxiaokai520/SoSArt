using System;
using System.Collections.Generic;
using System.IO;

public class ABRoleRes
{
	private static List<AB_HeroBuildBase> mHeroList = new List<AB_HeroBuildBase>();

	public static AB_HeroPacketBuild mActorInfo = null;

	public static string msHeroBattleLocation = "Assets/Exporter/Hero";

	public static string msHeroShowLocation = "Assets/Exporter/HeroShow";

	public static string msHeroBattleOb = "Assets/Exporter/HeroOb";

	public static List<AssetGroupInfo_t> mHeroResPackers = new List<AssetGroupInfo_t>();

	public static void BuildHero()
	{
		int num = 0;
		num |= 4;
		num |= 8;
		ABRoleRes.mActorInfo = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ACTORINFO, "hero_info", num);
		ABRoleRes.AddList();
		for (int i = 0; i < ABRoleRes.mHeroList.Count; i++)
		{
			ABRoleRes.mHeroList[i].Build();
		}
		ABRoleRes.mActorInfo.BuildCmd();
		ABRoleRes.mActorInfo.BuildRes(null, true);
	}

	private static void AddList()
	{
		DirectoryInfo[] directories = new DirectoryInfo(ABRoleRes.msHeroBattleLocation).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo = directories[i];
			int num = 0;
			if (int.TryParse(directoryInfo.Name, out num) && num > 100 && num < 1000)
			{
				AB_HeroBattleBuild item = new AB_HeroBattleBuild(directoryInfo);
				ABRoleRes.mHeroList.Add(item);
			}
		}
		directories = new DirectoryInfo(ABRoleRes.msHeroShowLocation).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo2 = directories[i];
			int num2 = 0;
			if (int.TryParse(directoryInfo2.Name, out num2) && num2 > 100 && num2 < 1000)
			{
				AB_HeroShowBuild item2 = new AB_HeroShowBuild(directoryInfo2);
				ABRoleRes.mHeroList.Add(item2);
			}
		}
		if (Directory.Exists(ABRoleRes.msHeroBattleOb))
		{
			directories = new DirectoryInfo(ABRoleRes.msHeroBattleOb).GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				DirectoryInfo directoryInfo3 = directories[i];
				int num3 = 0;
				if (int.TryParse(directoryInfo3.Name, out num3) && num3 > 100 && num3 < 1000)
				{
					AB_HeroObBuild item3 = new AB_HeroObBuild(directoryInfo3);
					ABRoleRes.mHeroList.Add(item3);
				}
			}
		}
	}

	public static void Init()
	{
		ABRoleRes.mActorInfo = null;
		ABRoleRes.mHeroList.Clear();
		ABRoleRes.mHeroResPackers.Clear();
	}
}

using System;
using System.Collections.Generic;
using System.IO;

public class ABRoleRes
{
	private static List<AB_HeroBuildBase> mHeroList = new List<AB_HeroBuildBase>();

	public static AB_HeroPacketBuild mActorInfo = null;

	public static string roleLocation = "Assets/RawRes/Role";

	public static string msHeroShowLocation = "Assets/Exporter/HeroShow";

	public static string msHeroBattleOb = "Assets/Exporter/HeroOb";

	public static List<AssetGroupInfo_t> mHeroResPackers = new List<AssetGroupInfo_t>();

	public static void BuildRoles()
	{
		int num = 0;
		num |= 4;
		num |= 8;

        //build rolebase, shared role res


		ABRoleRes.CreateRoleList();
		for (int i = 0; i < ABRoleRes.mHeroList.Count; i++)
		{
			ABRoleRes.mHeroList[i].Build();
		}
	}

    /// <summary>
    /// select roles that pack to ab
    /// </summary>
	private static void CreateRoleList()
	{
		DirectoryInfo[] directories = new DirectoryInfo(ABRoleRes.roleLocation).GetDirectories();
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
	}

	public static void Init()
	{
		ABRoleRes.mActorInfo = null;
		ABRoleRes.mHeroList.Clear();
		ABRoleRes.mHeroResPackers.Clear();
	}
}

using System;
using System.Collections.Generic;
using System.IO;

public class ABRoleRes
{
	private static List<AB_HeroBuildBase> mHeroList = new List<AB_HeroBuildBase>();

	public static AB_HeroPacketBuild mActorInfo = null;

	public static string roleLocation = "Assets/RawRes/Role*";

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
        List<DirectoryInfo> folders = new List<DirectoryInfo>();
        ABUtils.FindFolder(new DirectoryInfo(ABUtils.BaseFolder), "Role*", folders);
        //DirectoryInfo[] directories = new DirectoryInfo(ABRoleRes.roleLocation).GetDirectories();
        for (int i = 0; i < folders.Count; i++)
		{
			DirectoryInfo dirInfo = folders[i];
            if (dirInfo.Name.Contains("RolePublic"))//shared role assets
            {

            }
            else//heros
			{
				AB_HeroBattleBuild item = new AB_HeroBattleBuild(dirInfo);
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

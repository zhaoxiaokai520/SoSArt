using System;
using System.IO;

public class AB_GameDataBuild
{
	private static string msGameDataPath = "Assets/Exporter/GameData/";

	public static void BuildGameData()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(AB_GameDataBuild.msGameDataPath);
		if (directoryInfo.Exists)
		{
			int num = 0;
			num |= 8;
			AB_HeroPacketBuild arg_2D_0 = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_GAMEDATA, "gamedata", num);
			AB_GameDataCmd cmd = new AB_GameDataCmd(directoryInfo);
			arg_2D_0.AddCmd(cmd);
			arg_2D_0.BuildCmd();
			arg_2D_0.BuildRes(null, true);
		}
	}
}

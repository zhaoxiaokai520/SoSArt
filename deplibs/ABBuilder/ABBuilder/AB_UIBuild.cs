using System;
using System.IO;

public static class AB_UIBuild
{
	public static string msUIFolder = "Assets/Exporter/UI/";

	public static void BuildUI()
	{
		DirectoryInfo[] directories = new DirectoryInfo(AB_UIBuild.msUIFolder).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo = directories[i];
			AB_HeroPacketBuild arg_36_0 = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_UI, "ui" + directoryInfo.Name, 0);
			AB_UICmd cmd = new AB_UICmd(directoryInfo);
			arg_36_0.AddCmd(cmd);
			arg_36_0.BuildCmd();
			arg_36_0.BuildRes(null, true);
		}
	}
}

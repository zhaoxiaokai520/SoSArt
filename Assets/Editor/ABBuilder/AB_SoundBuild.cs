using System;
using System.IO;

public class AB_SoundBuild
{
	public static string msSoundFolder = "Assets/Exporter/sound/";

	public static void BuildSound()
	{
		DirectoryInfo[] directories = new DirectoryInfo(AB_SoundBuild.msSoundFolder).GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo = directories[i];
			AB_HeroPacketBuild arg_37_0 = new AB_HeroPacketBuild(ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_SOUND, "sound" + directoryInfo.Name, 0);
			AB_SoundCmd cmd = new AB_SoundCmd(directoryInfo);
			arg_37_0.AddCmd(cmd);
			arg_37_0.BuildCmd();
			arg_37_0.BuildRes(null, true);
		}
	}
}

using System;
using System.IO;

public static class AB_ShaderBuild
{
	private static string msShaderLocation = "Assets/Shaders/";

	private static AB_HeroPacketBuild mShaderPacker;

	public static void BuildShader()
	{
		int num = 0;
		num |= 4;
		num |= 8;
		AB_ShaderBuild.mShaderPacker = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SHADER, "shaderlist", num);
		DirectoryInfo directoryInfo = new DirectoryInfo(AB_ShaderBuild.msShaderLocation);
		if (directoryInfo.Exists)
		{
			AB_ShaderCmd cmd = new AB_ShaderCmd(directoryInfo);
			AB_ShaderBuild.mShaderPacker.AddCmd(cmd);
			AB_ShaderBuild.mShaderPacker.BuildCmd();
			AB_ShaderBuild.mShaderPacker.BuildRes(null, true);
		}
	}
}

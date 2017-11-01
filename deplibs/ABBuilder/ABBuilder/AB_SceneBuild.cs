using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AB_SceneBuild
{
	public static void BuildScene()
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.get_scenes();
		for (int i = 0; i < scenes.Length; i++)
		{
			EditorBuildSettingsScene expr_0D = scenes[i];
			FileInfo mFileInfo = new FileInfo(expr_0D.get_path());
			string[] expr_2E = expr_0D.get_path().Split(new char[]
			{
				'/'
			});
			string text = expr_2E[expr_2E.Length - 1];
			Debug.Log(text);
			AB_HeroPacketBuild arg_58_0 = new AB_HeroPacketBuild(AB_AssetBuildMgr.E_ABBUNLDE_TYPE.E_SCENE, CFileManager.EraseExtension(text), 0);
			arg_58_0.AddCmd(new AB_SceneCmd(null)
			{
				mFileInfo = mFileInfo
			});
			arg_58_0.BuildCmd();
			arg_58_0.BuildRes(null, true);
		}
	}
}

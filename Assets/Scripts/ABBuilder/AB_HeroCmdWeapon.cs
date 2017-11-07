using System;
using System.IO;
using UnityEditor;

public class AB_HeroCmdWeapon : ABCmdBase
{
	public AB_HeroCmdWeapon(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			string text = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mAssetGroupFileList.Add(text);
			string[] dependencies = AssetDatabase.GetDependencies(text);
			this.mABFileList.AddRange(dependencies);
		}
		base.BuildCmd();
	}
}

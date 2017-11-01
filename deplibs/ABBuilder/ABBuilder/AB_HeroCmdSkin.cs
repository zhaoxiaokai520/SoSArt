using System;
using System.IO;
using UnityEditor;

public class AB_HeroCmdSkin : AB_HeroCmdBase
{
	public AB_HeroCmdSkin(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.prefab");
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

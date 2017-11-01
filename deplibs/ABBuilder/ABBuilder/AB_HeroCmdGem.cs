using System;
using System.IO;
using UnityEditor;

public class AB_HeroCmdGem : AB_HeroCmdBase
{
	public AB_HeroCmdGem(DirectoryInfo info) : base(info)
	{
	}

	private void SearchRes(DirectoryInfo info)
	{
		FileInfo[] files = this.mInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			string text = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mAssetGroupFileList.Add(text);
			string[] dependencies = AssetDatabase.GetDependencies(text);
			this.mABFileList.AddRange(dependencies);
		}
	}

	public override void BuildCmd()
	{
		this.SearchRes(this.mInfo);
		base.BuildCmd();
	}
}

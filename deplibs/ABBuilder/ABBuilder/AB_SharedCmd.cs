using System;
using System.IO;
using UnityEditor;

public class AB_SharedCmd : AB_HeroCmdBase
{
	public AB_SharedCmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			if (!fileInfo.Extension.Equals(".meta"))
			{
				if (fileInfo.Extension.Equals(".prefab"))
				{
					string text = AB_Common.Absolute2RelativePath(fileInfo.FullName);
					this.mAssetGroupFileList.Add(text);
					string[] dependencies = AssetDatabase.GetDependencies(text);
					this.mABFileList.AddRange(dependencies);
				}
				else
				{
					string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
					this.mABFileList.Add(item);
					this.mAssetGroupFileList.Add(item);
				}
			}
		}
		base.BuildCmd();
	}
}

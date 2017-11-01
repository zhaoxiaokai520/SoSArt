using System;
using System.IO;

public class AB_SceneCmd : AB_HeroCmdBase
{
	public FileInfo mFileInfo;

	public AB_SceneCmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		if (this.mFileInfo.Exists)
		{
			string item = AB_Common.Absolute2RelativePath(this.mFileInfo.FullName);
			this.mABFileList.Add(item);
			this.mAssetGroupFileList.Add(item);
		}
		base.BuildCmd();
	}
}

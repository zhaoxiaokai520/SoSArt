using System;
using System.IO;

public class AB_GameDataCmd : ABCmdBase
{
	public AB_GameDataCmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.txt", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			string item = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mABFileList.Add(item);
			this.mAssetGroupFileList.Add(item);
		}
		base.BuildCmd();
	}
}

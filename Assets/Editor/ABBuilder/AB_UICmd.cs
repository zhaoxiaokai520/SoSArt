using System;
using System.IO;

public class AB_UICmd : ABCmdBase
{
	public AB_UICmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			if (!fileInfo.Extension.Equals(".meta"))
			{
				string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
				this.mABFileList.Add(item);
				this.mAssetGroupFileList.Add(item);
			}
		}
		base.BuildCmd();
	}
}

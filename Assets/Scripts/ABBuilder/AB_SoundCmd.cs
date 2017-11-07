using System;
using System.IO;

public class AB_SoundCmd : ABCmdBase
{
	public AB_SoundCmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			if (fileInfo.Extension.Equals(".ogg") || fileInfo.Extension.Equals(".bank") || fileInfo.Extension.Equals(".bytes"))
			{
				string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
				this.mABFileList.Add(item);
				this.mAssetGroupFileList.Add(item);
			}
		}
		base.BuildCmd();
	}
}

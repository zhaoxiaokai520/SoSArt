using System;
using System.IO;

public class AB_HeroCmdSound : ABCmdBase
{
	public AB_HeroCmdSound(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			if (fileInfo.Extension.Equals(".mp3") || fileInfo.Extension.Equals(".wav") || fileInfo.Extension.Equals(".ogg") || fileInfo.Extension.Equals(".bytes") || fileInfo.Extension.Equals(".asset"))
			{
				string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
				this.mABFileList.Add(item);
				this.mAssetGroupFileList.Add(item);
			}
		}
		base.BuildCmd();
	}
}

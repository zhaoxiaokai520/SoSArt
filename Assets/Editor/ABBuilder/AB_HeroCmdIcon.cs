using System;
using System.IO;

public class AB_HeroCmdIcon : ABCmdBase
{
	public AB_HeroCmdIcon(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			if (fileInfo.Extension.Equals(".prefab") || fileInfo.Extension.Equals(".png") || fileInfo.Extension.Equals(".jpg") || fileInfo.Extension.Equals(".tga"))
			{
				string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
				this.mABFileList.Add(item);
				if (fileInfo.Extension.Equals(".prefab"))
				{
					this.mAssetGroupFileList.Add(item);
				}
			}
		}
		base.BuildCmd();
	}
}

using System;
using System.IO;

public class AB_HeroCmdActorInfo : AB_HeroCmdBase
{
	public AB_HeroCmdActorInfo(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.asset");
		for (int i = 0; i < files.Length; i++)
		{
			string item = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mABFileList.Add(item);
			this.mAssetGroupFileList.Add(item);
		}
		base.BuildCmd();
	}
}

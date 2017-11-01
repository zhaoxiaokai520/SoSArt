using System;
using System.IO;

public class AB_ShaderCmd : AB_HeroCmdBase
{
	public AB_ShaderCmd(DirectoryInfo info) : base(info)
	{
	}

	public override void BuildCmd()
	{
		FileInfo[] files = this.mInfo.GetFiles("*.shader", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			string item = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mABFileList.Add(item);
			this.mAssetGroupFileList.Add(item);
		}
		files = this.mInfo.GetFiles("*.shadervariants", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			string item2 = AB_Common.Absolute2RelativePath(files[i].FullName);
			this.mABFileList.Add(item2);
			this.mAssetGroupFileList.Add(item2);
		}
		base.BuildCmd();
	}
}

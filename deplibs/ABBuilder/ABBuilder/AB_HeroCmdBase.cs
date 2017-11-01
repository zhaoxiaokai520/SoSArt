using System;
using System.Collections.Generic;
using System.IO;

public class AB_HeroCmdBase
{
	public AB_HeroPacketBuild mPacketBuild;

	protected List<string> mABFileList = new List<string>();

	protected List<string> mAssetGroupFileList = new List<string>();

	protected DirectoryInfo mInfo;

	public AB_HeroCmdBase(DirectoryInfo info)
	{
		this.mInfo = info;
	}

	public virtual void BuildCmd()
	{
	}

	public List<string> GetABFileList()
	{
		return this.mABFileList;
	}

	public List<string> GetAssetGroupFileList()
	{
		return this.mAssetGroupFileList;
	}
}

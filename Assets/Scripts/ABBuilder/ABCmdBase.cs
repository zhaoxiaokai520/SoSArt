using System;
using System.Collections.Generic;
using System.IO;

public class ABCmdBase
{
	public AB_HeroPacketBuild mPacketBuild;

	protected List<string> mABFileList = new List<string>();

	protected List<string> mAssetGroupFileList = new List<string>();

	protected DirectoryInfo mInfo;

	public ABCmdBase(DirectoryInfo info)
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

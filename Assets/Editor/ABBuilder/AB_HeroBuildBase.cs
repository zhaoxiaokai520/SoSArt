using System;
using System.IO;

public class AB_HeroBuildBase
{
	public DirectoryInfo mFolderInfo;

	public AB_HeroBuildBase(DirectoryInfo info)
	{
		this.mFolderInfo = info;
	}

	public virtual void Build()
	{
	}
}

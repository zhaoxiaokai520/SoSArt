using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABRoleCmd : ABCmdBase
{
    public ABRoleCmd(DirectoryInfo info) : base(info)
	{

    }

    public override void BuildCmd()
    {
        FileInfo[] files = mInfo.GetFiles();
        for (int j = 0; j < files.Length; j++)
        {
            FileInfo fileInfo = files[j];
            if (fileInfo.Exists && !fileInfo.Extension.Equals(".meta"))
            {
                string item = AB_Common.Absolute2RelativePath(fileInfo.FullName);
                this.mABFileList.Add(item);
                this.mAssetGroupFileList.Add(item);
            }
        }
        base.BuildCmd();
    }
}

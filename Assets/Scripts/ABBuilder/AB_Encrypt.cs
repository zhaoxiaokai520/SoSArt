using System;
using System.Collections.Generic;
using UnityEngine;

public static class AB_Encrypt
{
	private static List<string> msEncryptFileList = new List<string>();

	public static void Init()
	{
		AB_Encrypt.msEncryptFileList.Clear();
	}

	public static void AddEncryptFile(AssetGroupInfo_t resInfo)
	{
		if (!resInfo.HasFlag(eBundleFlag.EncryptBundle))
		{
			resInfo.SetFlag(eBundleFlag.EncryptBundle);
		}
		string item = "Assets/StreamingAssets/" + resInfo.m_pathInIFS;
		AB_Encrypt.msEncryptFileList.Add(item);
	}

	public static void DeleteEncryptBundleFile()
	{
		for (int i = 0; i < AB_Encrypt.msEncryptFileList.Count; i++)
		{
			string text = AB_Encrypt.msEncryptFileList[i];
			if (CFileManager.IsFileExist(text))
			{
				Debug.Log("===Encrypt Delete File: " + text);
				CFileManager.DeleteFile(text);
			}
			text += ".manifest";
			if (CFileManager.IsFileExist(text))
			{
				Debug.Log("===Encrypt Delete File: " + text);
				CFileManager.DeleteFile(text);
			}
		}
	}

	public static void EncryptFile()
	{
		for (int i = 0; i < AB_Encrypt.msEncryptFileList.Count; i++)
		{
			if (CFileManager.IsFileExist(AB_Encrypt.msEncryptFileList[i]))
			{
				byte[] rawBytes = CFileManager.ReadFile(AB_Encrypt.msEncryptFileList[i]);
				try
				{
					Debug.Log("===Encrypt Wrtie File: " + AB_Encrypt.msEncryptFileList[i]);
					CFileManager.WriteFile(AB_Encrypt.msEncryptFileList[i], CFileManager.AESEncrypt(rawBytes, "forthelichking!!"));
				}
				catch (Exception ex)
				{
					Debug.LogError("===Encrypt Error: " + ex.Message);
				}
			}
		}
	}

	public static bool IsEncryptFile(int iFlag)
	{
		return (iFlag & 8) > 0;
	}
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ABUtils {
    public const string BaseFolder = "Assets/RawRes/";
    public const string msSharedFolder = "Assets/RawRes/Public";

    public static void FindFolder(DirectoryInfo path, List<DirectoryInfo> folders)
    {
        FileInfo[] files = path.GetFiles("Public*");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension != ".meta")
            {
                folders.Add(path);
                break;
            }
        }
        DirectoryInfo[] directories = path.GetDirectories();
        for (int j = 0; j < directories.Length; j++)
        {
            FindFolder(directories[j], folders);
        }
    }

    public static void FindFolder(DirectoryInfo path, string searchPattern, List<DirectoryInfo> folders)
    {
        DirectoryInfo[] dirs = path.GetDirectories(searchPattern);
        for (int i = 0; i < dirs.Length; i++)
        {
            if (null != dirs[i])
            {
                folders.Add(dirs[i]);
            }
        }
    }

    public static bool GetFolder(string path, ref string folder1, ref string folder2)
    {
        path = path.Replace("\\", "/");
        //msSharedFolder = "Assets/Exporter/Common/"
        //int num = path.IndexOf(msSharedFolder);
        int num = path.LastIndexOf("/");
        if (num == -1)
        {
            return false;
        }
        string text = path.Substring(num);
        if (text.Contains("/"))
        {
            string[] array = text.Split(new char[]
            {
                '/'
            });
            folder1 = array[0];
            folder2 = array[array.Length - 1];
        }
        else
        {
            folder1 = text;
            folder2 = string.Empty;
        }
        return true;
    }

    public static string GetAssetBundleName(string folder1, string folder2)
    {
        folder1 = folder1.Replace('.', '_');
        folder2 = folder2.Replace('.', '_');
        return (folder1 + folder2).ToLower() + AB_Common.AB_EXT;
    }

    public static ABAssetBuildMgr.E_ABBUNLDE_TYPE GetType(string folder1)
    {
        if (folder1.ToLower().Contains("anim"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ANIM;
        }
        else if (folder1.ToLower().Contains("texture"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_TEXTURE;
        }
        else if (folder1.ToLower().Contains("controller"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_CONTROLLER;
        }
        else if (folder1.ToLower().Contains("material"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_MATERIAL;
        }
        else if (folder1.ToLower().Contains("model"))
        {
            return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_MODEL;
        }

        Debug.LogError("Error Type: " + folder1);
        return ABAssetBuildMgr.E_ABBUNLDE_TYPE.E_ILLEGAL;
    }
}
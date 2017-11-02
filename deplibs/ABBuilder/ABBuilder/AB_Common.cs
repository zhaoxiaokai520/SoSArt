using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AB_Common
{
	public static string AB_VERSION = "0.0.1";

	public static string AB_LOCATION = Application.streamingAssetsPath + "/AssetBundle/";

	public static string AB_RESINFO_PATH = Application.streamingAssetsPath + "/";

	public static string AB_EXT = ".unity3d";

	public static string AB_PATH = "Assets/StreamingAssets/AssetBundle/";

	public static string AB_RES_INFO_PATH = "AssetBundle/";

	public static string AB_IFS_PATH = Application.streamingAssetsPath + "/";

	public static bool AB_SKIP_BUILD = false;

	public static bool AB_SKIP_COOK_AGE = false;

	public static BuildTarget BUILD_TARGET = EditorUserBuildSettings.activeBuildTarget;

	private static bool mbUnity5 = true;

	public static BuildAssetBundleOptions AB_OPTION = (BuildAssetBundleOptions)22;

	private static bool mbOldBuildMethod = true;

	private static string[] mExtNeed = new string[]
	{
		".prefab",
		".png",
		".tga",
		".jpg",
		".exr",
		".fbx",
		".shader",
		".anim"
	};

	public static bool bOldBuildMethod
	{
		get
		{
			return AB_Common.mbOldBuildMethod;
		}
		set
		{
			if (AB_Common.mbUnity5 && !value)
			{
				AB_Common.mbOldBuildMethod = false;
				return;
			}
			AB_Common.mbOldBuildMethod = true;
		}
	}

	public static string assetVersion
	{
		get
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				if (text.StartsWith("assetVersion-"))
				{
					return text.Split(new char[]
					{
						"-"[0]
					})[1];
				}
			}
			return "0.0.1";
		}
	}

	private static string BuildCmd()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			string text = commandLineArgs[i];
			if (text.StartsWith("buildoption-"))
			{
				return AB_Common.BuildCmd(text);
			}
		}
		return "Invalid";
	}

	private static string BuildCmd(string arg)
	{
		string buildType = "lz4";
		string version = "0.01";
		bool bClear = false;
		List<string> list = new List<string>(arg.Split(new char[]
		{
			"-"[0]
		}));
		if (list.Contains("lzma"))
		{
			buildType = "lzma";
		}
		else if (list.Contains("uncompressed"))
		{
			buildType = "uncompressed";
		}
		if (list.Contains("clearab"))
		{
			bClear = true;
		}
		foreach (string current in list)
		{
			if (current.StartsWith("v"))
			{
				version = current.Substring(1);
				break;
			}
		}
		return AB_Common.BuildCmd(buildType, version, bClear);
	}

	private static string BuildCmd(string buildType, string version, bool bClear = true)
	{
		if (bClear)
		{
			AB_Common.ClearAB();
		}
		string text = "";
		if (buildType == "lz4")
		{
			text = AB_Common.BuildAll5LZ4();
		}
		else if (buildType == "lzma")
		{
			text = AB_Common.BuildAll5();
		}
		else if (buildType == "uncompressed")
		{
			text = AB_Common.BuildAll5UnCompress();
		}
		if (string.IsNullOrEmpty(text))
		{
			return "Invalid";
		}
		AB_Common.SetVersion(version);
		return text;
	}

	public static bool SetVersion(string version)
	{
		AB_Common.AB_VERSION = version;
		if (!AB_GatherResInfo.LoadAssetGroupSet())
		{
			return false;
		}
		AB_GatherResInfo.SaveAssetGroupSet();
		return true;
	}

	public static void SetAssetVersion()
	{
		string assetVersion = AB_Common.assetVersion;
		UnityEngine.Debug.Log("SetAssetVersion => " + assetVersion);
		try
		{
			if (!AB_Common.SetVersion(assetVersion))
			{
                UnityEngine.Debug.LogError("build failed: SetAssetVersion => " + assetVersion);
			}
		}
		catch (Exception ex)
		{
            UnityEngine.Debug.LogError(string.Format("build failed: SetAssetVersion => {0}, error: {1} ", AB_Common.assetVersion, ex.Message));
		}
	}

	[MenuItem("AssetsBundle/Build All UnCompress(Unity 5)")]
	private static string BuildAll5UnCompress()
	{
		return AB_Common.BuildAll5WithOption((BuildAssetBundleOptions)17);
	}

	[MenuItem("AssetsBundle/Build All LZ4(Unity 5)")]
	private static string BuildAll5LZ4()
	{
		return AB_Common.BuildAll5WithOption((BuildAssetBundleOptions)272);
	}

	[MenuItem("AssetsBundle/Build All(Unity 5)")]
	private static string BuildAll5()
	{
		return AB_Common.BuildAll5WithOption((BuildAssetBundleOptions)16);
	}

	[MenuItem("AssetsBundle/Clear")]
	private static void ClearAB()
	{
		AssetDatabase.Refresh();
		CFileManager.DeleteDirectory(AB_Common.AB_LOCATION);
		CFileManager.DeleteFile(AB_GatherResInfo.GetAssetGroupInfoSetPath());
	}

	private static string BuildAll5WithOption(BuildAssetBundleOptions opt)
	{
		EditorUtility.DisplayProgressBar("Build All", "Init All New Hero.....", 0f);
		AB_Common.bOldBuildMethod = false;
		AB_Common.AB_SKIP_BUILD = false;
		AB_Common.AB_SKIP_COOK_AGE = false;
		AB_Common.InitAll();
		EditorUtility.DisplayProgressBar("Build All", "Build Shader Shared GameData.....", 0.15f);
		Stopwatch stopwatch = Stopwatch.StartNew();
		AB_AssetBuildMgr.Clear();
		AB_ShaderBuild.BuildShader();
		AB_SharedRes.BuildSharedRes();
		AB_GameDataBuild.BuildGameData();
		stopwatch.Stop();
        UnityEngine.Debug.Log("Pass Time Shader Shared GameData: " + stopwatch.ElapsedMilliseconds);
		EditorUtility.DisplayProgressBar("Build All", "Build Hero.....", 0.3f);
		stopwatch.Reset();
		stopwatch.Start();
		AB_HeroBuildMgr.BuildHero();
		stopwatch.Stop();
        UnityEngine.Debug.Log("Pass Time Hero: " + stopwatch.ElapsedMilliseconds);
		EditorUtility.DisplayProgressBar("Build All", "Build Scene.....", 0.45f);
		stopwatch.Reset();
		stopwatch.Start();
		AB_SceneBuild.BuildScene();
		stopwatch.Stop();
        UnityEngine.Debug.Log("Pass Time Scene: " + stopwatch.ElapsedMilliseconds);
		EditorUtility.DisplayProgressBar("Build All", "Build UI Sound.....", 0.6f);
		stopwatch.Reset();
		stopwatch.Start();
		AB_UIBuild.BuildUI();
		stopwatch.Stop();
        UnityEngine.Debug.Log("Pass Time UI: " + stopwatch.ElapsedMilliseconds);
		EditorUtility.DisplayProgressBar("Build All", "Gather Info.....", 0.75f);
		stopwatch.Reset();
		stopwatch.Start();
		AB_GatherResInfo.GatherResInfo();
		stopwatch.Stop();
        UnityEngine.Debug.Log("Pass Time GatherResInfo: " + stopwatch.ElapsedMilliseconds);
		AB_Encrypt.DeleteEncryptBundleFile();
		AssetDatabase.Refresh();
		BuildPipeline.BuildAssetBundles(AB_Common.AB_PATH, AB_AssetBuildMgr.Parse(), opt, AB_Common.BUILD_TARGET);
		AB_GatherResInfo.PostBuild();
		AB_AssetBuildMgr.Write();
		AB_Encrypt.EncryptFile();
		EditorUtility.ClearProgressBar();
		AB_Analyze.CheckAB();
		if (AB_Analyze.mErrorMessageList.Count > 0)
		{
			EditorUtility.DisplayDialog("Error", "AB存在相互依赖", "OK");
			return "Error";
		}
		EditorUtility.DisplayDialog("Build All", "Build Complete!!!!" + AB_GatherResInfo.GetPublish(), "OK");
		return AB_GatherResInfo.GetPublish();
	}

	public static void InitAll()
	{
		if (!CFileManager.IsDirectoryExist(AB_Common.AB_LOCATION))
		{
			CFileManager.CreateDirectory(AB_Common.AB_LOCATION);
		}
		AB_SharedRes.Init();
		AB_HeroBuildMgr.Init();
		AB_GatherResInfo.Init();
		AB_Encrypt.Init();
		EditorSceneManager.OpenScene("Assets/Scenes/packScene.unity");
	}

	public static List<string> GetDependencies(string path)
	{
		string[] dependencies = AssetDatabase.GetDependencies(path);
		List<string> list = new List<string>();
		for (int i = 0; i < dependencies.Length; i++)
		{
			string extension = CFileManager.GetExtension(dependencies[i]);
			bool flag = false;
			for (int j = 0; j < AB_Common.mExtNeed.Length; j++)
			{
				if (extension.ToLower().Equals(AB_Common.mExtNeed[j]))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				list.Add(dependencies[i]);
			}
		}
		return list;
	}

	public static bool CheckDirectory(string str)
	{
		return CFileManager.IsDirectoryExist(str);
	}

	public static bool CreateDirectory(string str)
	{
		return CFileManager.CreateDirectory(str);
	}

	public static string Absolute2RelativePath(string path)
	{
		path = path.Replace("\\", "/");
		string value = "Assets";
		int num = path.IndexOf(value);
		if (num == -1)
		{
			return path;
		}
		return path.Substring(num, path.Length - num);
	}

	public static string Relative2AbsolutePath(string path)
	{
		path = path.Replace("\\", "/");
		string text = "Assets";
		int num = path.IndexOf(text);
		if (num == -1)
		{
			return path;
		}
		num += text.Length;
		return Application.dataPath + path.Substring(num, path.Length - num);
	}

	public static string PathRemoveAssets(string path)
	{
		path = path.Replace("\\", "/");
		string text = "Assets/";
		int num = path.IndexOf(text);
		if (num == -1)
		{
			return path;
		}
		num += text.Length;
		return path.Substring(num, path.Length - num);
	}
}

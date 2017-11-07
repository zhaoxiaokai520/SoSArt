using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ABBuilder {
    public static string AB_VERSION = "0.0.1";
    //public static string AB_PUBLISH = "ILLGEL";
    //文件名过于种类繁多所以定下以下规定：
    //1，File函数调用的比如CreateFromFile用绝对路径 D:/MobaGo/Assets/xxx.png
    //2，unity系统函数调用比如BuildPipeLine和AssetDataBase使用包含Assets的相对路径 Assets/xxx.png
    //3，ab资源考虑到会移动，所以IFS Path + 路径(AssetBundle/hero123.unity3d) AssetGroupInfo_t的m_pathInIFS使用
    //4，manifest使用剥离Assets的相对路径 Prefab/xxx.unity3d   AssetInfo_t中使用
    //5，统一使用 /。注意在windows下File函数返回路径会是 \
    public static string AB_LOCATION = Application.streamingAssetsPath + "/AssetBundle/";
    public static string AB_RESINFO_PATH = Application.streamingAssetsPath + "/";
    public static string AB_EXT = ".unity3d";
    public static string AB_PATH = "Assets/StreamingAssets/AssetBundle/";
    public static string AB_RES_INFO_PATH = "AssetBundle/";
    public static string AB_IFS_PATH = Application.streamingAssetsPath + "/";
    //考虑打包时间太慢有时需要只更新ResourceInfoPackerSet.byte
    //不cook age打包
    public static bool AB_SKIP_COOK_AGE = false;
    public static BuildTarget BUILD_TARGET = EditorUserBuildSettings.activeBuildTarget;

    [MenuItem("AssetsBundle/Build All LZ4(Unity 5)")]
    static string BuildAll5LZ4()
    {
        return BuildAll5WithOption(BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression);
    }

    [MenuItem("AssetsBundle/Clear")]
    static void ClearAB()
    {
        AssetDatabase.Refresh();
        CFileManager.DeleteDirectory(AB_LOCATION);
        CFileManager.DeleteFile(AB_GatherResInfo.GetAssetGroupInfoSetPath());
    }

    static string BuildAll5WithOption(BuildAssetBundleOptions opt)
    {
        EditorUtility.DisplayProgressBar("Build All", "Init All New Hero.....", 0f);
        InitAll();
        EditorUtility.DisplayProgressBar("Build All", "Build Shader Shared GameData.....", 0.15f);
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        ABAssetBuildMgr.Clear();
        AB_ShaderBuild.BuildShader();
        ABSharedRes.BuildSharedRes();
        AB_GameDataBuild.BuildGameData();
        stopwatch.Stop();
        EditorUtility.ClearProgressBar();
        //UnityEngine.Debug.Log("Pass Time Shader Shared GameData: " + stopwatch.ElapsedMilliseconds);
        //EditorUtility.DisplayProgressBar("Build All", "Build Hero.....", 0.3f);
        //stopwatch.Reset();
        //stopwatch.Start();
        //ABRoleBuildMgr.BuildHero();
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Pass Time Hero: " + stopwatch.ElapsedMilliseconds);
        //EditorUtility.DisplayProgressBar("Build All", "Build Scene.....", 0.45f);
        //stopwatch.Reset();
        //stopwatch.Start();
        //AB_SceneBuild.BuildScene();
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Pass Time Scene: " + stopwatch.ElapsedMilliseconds);
        //EditorUtility.DisplayProgressBar("Build All", "Build UI Sound.....", 0.6f);
        //stopwatch.Reset();
        //stopwatch.Start();
        //AB_UIBuild.BuildUI();
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Pass Time UI: " + stopwatch.ElapsedMilliseconds);
        //EditorUtility.DisplayProgressBar("Build All", "Gather Info.....", 0.75f);
        //stopwatch.Reset();
        //stopwatch.Start();
        //AB_GatherResInfo.GatherResInfo();
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Pass Time GatherResInfo: " + stopwatch.ElapsedMilliseconds);
        //AB_Encrypt.DeleteEncryptBundleFile();
        //AssetDatabase.Refresh();
        //BuildPipeline.BuildAssetBundles(AB_Common.AB_PATH, ABAssetBuildMgr.Parse(), opt, AB_Common.BUILD_TARGET);
        //AB_GatherResInfo.PostBuild();
        //ABAssetBuildMgr.Write();
        //AB_Encrypt.EncryptFile();
        //EditorUtility.ClearProgressBar();
        //AB_Analyze.CheckAB();
        //if (AB_Analyze.mErrorMessageList.Count > 0)
        //{
        //    EditorUtility.DisplayDialog("Error", "AB存在相互依赖", "OK");
        //    return "Error";
        //}
        //EditorUtility.DisplayDialog("Build All", "Build Complete!!!!" + AB_GatherResInfo.GetPublish(), "OK");
        //return AB_GatherResInfo.GetPublish();
        return "";
    }

    public static void InitAll()
    {
        if (!CFileManager.IsDirectoryExist(AB_LOCATION))
        {
            CFileManager.CreateDirectory(AB_LOCATION);
        }
        ABSharedRes.Init();
        ABRoleBuildMgr.Init();
        EditorSceneManager.OpenScene("Assets/Scenes/pack.unity");
    }
}

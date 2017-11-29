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
        //AB_ShaderBuild.BuildShader();
        ABSharedRes.BuildSharedRes();
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(AB_PATH, ABAssetBuildMgr.Parse(), opt, BUILD_TARGET);
        stopwatch.Stop();
        EditorUtility.ClearProgressBar();
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
        ABRoleRes.Init();
        EditorSceneManager.OpenScene("Assets/Scenes/pack.unity");
    }
}
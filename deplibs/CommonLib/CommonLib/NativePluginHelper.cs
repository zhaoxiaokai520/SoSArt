using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class NativePluginHelper
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static IntPtr nativeLibraryPtr;

    delegate int MultiplyFloat(float number, float multiplyBy);
    delegate void DoSomething(string words);
#endif

    public static T Invoke<T, T2>(IntPtr library, params object[] pars)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        IntPtr funcPtr = GetProcAddress(library, typeof(T2).Name);
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogError("Could not gain reference to method address.");
            return default(T);
        }

        var func = Marshal.GetDelegateForFunctionPointer(GetProcAddress(library, typeof(T2).Name), typeof(T2));
        return (T)func.DynamicInvoke(pars);
#else
        T t;
        return t;
#endif
    }

    public static void Invoke<T>(IntPtr library, params object[] pars)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        IntPtr funcPtr = GetProcAddress(library, typeof(T).Name);
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogError("Could not gain reference to method address.");
            return;
        }

        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
        func.DynamicInvoke(pars);
#endif
    }

    public static void LoadNativeDll()
    {
        Debug.Log("LoadNativeDll==========");
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (nativeLibraryPtr != IntPtr.Zero) return;

        nativeLibraryPtr = NativePluginHelper.LoadLibrary("ReloadableNativeDll/GameCoreCpp");
        if (nativeLibraryPtr == IntPtr.Zero)
        {
            Debug.LogError("Failed to load native library");
        }
#endif
    }

    public static void OnAppQuit()
    {
        Debug.Log("OnAppQuit==========");
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (nativeLibraryPtr == IntPtr.Zero) return;

        bool free = NativePluginHelper.FreeLibrary(nativeLibraryPtr);
        Debug.Log(free
                      ? "Native library successfully unloaded."
                      : "Native library could not be unloaded.");
#endif
    }

    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
}

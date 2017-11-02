using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class GlobalPool
{
	public static List<int> listInt = new List<int>();

	public static List<object> listPoolType = new List<object>();

	public static List<int> getListInt_Clear()
	{
		GlobalPool.listInt.Clear();
		return GlobalPool.listInt;
	}

	public static void SaveAllPoolType(string sss)
	{
		bool flag = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
		if (flag)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + "/Log");
			bool flag2 = !directoryInfo.Exists;
			if (flag2)
			{
				directoryInfo.Create();
			}
			DateTime now = DateTime.Now;
			string str = "TypeLog_" + now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + "_" + now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString();
			File.AppendAllText(Application.dataPath + "/Log/" + str + ".txt", sss);
			File.AppendAllText(Application.dataPath + "/Log/" + str + ".txt", GlobalPool.DumpAllPool());
		}
	}

	public static string DumpAllPool()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < GlobalPool.listPoolType.Count; i++)
		{
			Type type = GlobalPool.listPoolType[i].GetType();
			object obj = type.InvokeMember("Dump", BindingFlags.InvokeMethod, null, GlobalPool.listPoolType[i], null);
			bool flag = obj != null;
			if (flag)
			{
				stringBuilder.AppendFormat("{0}\n", obj);
			}
		}
		return stringBuilder.ToString();
	}

	public static void AddPoolType(object pp)
	{
		GlobalPool.listPoolType.Add(pp);
	}

	public static void ResetPoolType()
	{
	}
}

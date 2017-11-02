using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AB_Analyze
{
	public class AB_Node
	{
		public class AB_Node_SizeWrapper
		{
			public AB_Analyze.AB_Node node;

			public long size;

			public List<string> nodeNames = new List<string>();
		}

		public class AB_Node_SizeSort : IComparer<AB_Analyze.AB_Node.AB_Node_SizeWrapper>
		{
			public int Compare(AB_Analyze.AB_Node.AB_Node_SizeWrapper x, AB_Analyze.AB_Node.AB_Node_SizeWrapper y)
			{
				if (x.size > y.size)
				{
					return -1;
				}
				if (x.size < y.size)
				{
					return 1;
				}
				return 0;
			}
		}

		public string msName;

		public FileInfo mFileInfo;

		public List<AB_Analyze.AB_Node> mDependencies = new List<AB_Analyze.AB_Node>();

		public AB_Node(FileInfo ff)
		{
			this.msName = ff.Name;
			this.mFileInfo = ff;
		}

		public void Write(StreamWriter wr, AB_Analyze.AB_Node.AB_Node_SizeWrapper wrapper)
		{
			string text = wrapper.size / 1000L + "K";
			string text2 = this.mFileInfo.Length / 1000L + "K";
			string text3 = "";
			for (int i = 0; i < wrapper.nodeNames.Count; i++)
			{
				if (!wrapper.nodeNames[i].Equals(this.msName))
				{
					text3 += wrapper.nodeNames[i];
					if (i < wrapper.nodeNames.Count - 1)
					{
						text3 += ",";
					}
				}
			}
			wr.WriteLine(string.Concat(new string[]
			{
				this.msName,
				",",
				text2,
				",",
				text,
				",",
				text3
			}));
		}
	}

	private static Dictionary<string, List<string>> mCheckDictionary = new Dictionary<string, List<string>>();

	public static List<string> mErrorMessageList = new List<string>();

	[MenuItem("AssetsBundle/Analyze/Analyze AB(Details)")]
	private static void AnalyzeAB1()
	{
		AB_Analyze.AnalyzeAB(true);
	}

	[MenuItem("AssetsBundle/Analyze/Analyze AB")]
	private static void AnalyzeAB2()
	{
		AB_Analyze.AnalyzeAB(false);
	}

	private static void AnalyzeAB(bool bShowDetail = false)
	{
		string text = Application.streamingAssetsPath + "/analyze";
		if (!CFileManager.IsDirectoryExist(text))
		{
			CFileManager.CreateDirectory(text);
		}
		FileStream fileStream = new FileStream(text + "/analyze.csv", FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		AssetBundle expr_4C = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/AssetBundle");
		AssetBundleManifest assetBundleManifest = expr_4C.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		expr_4C.Unload(false);
		streamWriter.WriteLine("文件名,大小,总共大小,依赖资源包");
		string[] allAssetBundles = assetBundleManifest.GetAllAssetBundles();
		List<AB_Analyze.AB_Node> list = new List<AB_Analyze.AB_Node>();
		for (int i = 0; i < allAssetBundles.Length; i++)
		{
			FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + "/AssetBundle/" + allAssetBundles[i]);
			if (fileInfo != null)
			{
				AB_Analyze.AB_Node item = new AB_Analyze.AB_Node(fileInfo);
				list.Add(item);
			}
		}
		foreach (AB_Analyze.AB_Node current in list)
		{
			if (current != null)
			{
				string[] directDependencies = assetBundleManifest.GetDirectDependencies(current.msName);
				for (int j = 0; j < directDependencies.Length; j++)
				{
					string text2 = directDependencies[j];
					if (!text2.ToLower().Contains("shaderlist") && !text2.ToLower().Contains("sharedfont"))
					{
						AB_Analyze.AB_Node aB_Node = AB_Analyze.FindNode(list, text2);
						if (aB_Node != null)
						{
							current.mDependencies.Add(aB_Node);
						}
					}
				}
			}
		}
		List<AB_Analyze.AB_Node.AB_Node_SizeWrapper> list2 = new List<AB_Analyze.AB_Node.AB_Node_SizeWrapper>();
		foreach (AB_Analyze.AB_Node current2 in list)
		{
			AB_Analyze.AB_Node.AB_Node_SizeWrapper aB_Node_SizeWrapper = new AB_Analyze.AB_Node.AB_Node_SizeWrapper();
			aB_Node_SizeWrapper.size = AB_Analyze.GetABNodeInfo(current2, ref aB_Node_SizeWrapper.nodeNames);
			List<string> list3 = new List<string>();
			if (!bShowDetail)
			{
				string[] array = current2.msName.Split(new char[]
				{
					'_'
				});
				if (array.Length > 1)
				{
					string arg_1C4_0 = array[0];
				}
				foreach (string current3 in aB_Node_SizeWrapper.nodeNames)
				{
					if (!current3.Contains("shared") && !current3.Contains("e_charactere_level"))
					{
						list3.Add(current3);
					}
				}
			}
			aB_Node_SizeWrapper.nodeNames = list3;
			aB_Node_SizeWrapper.node = current2;
			list2.Add(aB_Node_SizeWrapper);
		}
		list2.Sort(new AB_Analyze.AB_Node.AB_Node_SizeSort());
		foreach (AB_Analyze.AB_Node.AB_Node_SizeWrapper current4 in list2)
		{
			current4.node.Write(streamWriter, current4);
		}
		streamWriter.Close();
		fileStream.Close();
	}

	private static long GetABNodeInfo(AB_Analyze.AB_Node root, ref List<string> nodeNames)
	{
		long num = 0L;
		List<AB_Analyze.AB_Node> list = new List<AB_Analyze.AB_Node>();
		list.Add(root);
		List<AB_Analyze.AB_Node> list2 = new List<AB_Analyze.AB_Node>();
		while (list.Count > 0)
		{
			AB_Analyze.AB_Node node = list[0];
			list.RemoveAt(0);
			AB_Analyze.AB_Node aB_Node = list2.Find((AB_Analyze.AB_Node raw) => raw.msName == node.msName);
			if (aB_Node != null && aB_Node.mDependencies.Count > 0)
			{
				Debug.LogError(string.Format("root bundle {0} - Duplicated: {1}", root.mFileInfo.Name, aB_Node.mFileInfo.Name));
			}
			else
			{
				if (!nodeNames.Contains(node.mFileInfo.Name))
				{
					nodeNames.Add(node.mFileInfo.Name);
					num += node.mFileInfo.Length;
				}
				foreach (AB_Analyze.AB_Node current in node.mDependencies)
				{
					if (current.mFileInfo != null)
					{
						list.Add(current);
					}
				}
				list2.Add(node);
			}
		}
		return root.mFileInfo.Length;
	}

	private static AB_Analyze.AB_Node FindNode(List<AB_Analyze.AB_Node> ll, AB_Analyze.AB_Node node)
	{
		foreach (AB_Analyze.AB_Node current in ll)
		{
			if (current.msName.Equals(node.msName))
			{
				return current;
			}
		}
		return null;
	}

	private static AB_Analyze.AB_Node FindNode(List<AB_Analyze.AB_Node> ll, string node)
	{
		foreach (AB_Analyze.AB_Node current in ll)
		{
			if (current.msName.Equals(node))
			{
				return current;
			}
		}
		return null;
	}

	[MenuItem("AssetsBundle/Analyze/Check AB Deps")]
	public static void CheckAB()
	{
		AB_Analyze.mCheckDictionary.Clear();
		AB_Analyze.mErrorMessageList.Clear();
		if (AB_AssetBuildMgr.mManifest == null)
		{
			AB_AssetBuildMgr.LoadManifest();
		}
		string[] allAssetBundles = AB_AssetBuildMgr.mManifest.GetAllAssetBundles();
		for (int i = 0; i < allAssetBundles.Length; i++)
		{
			string text = allAssetBundles[i];
			AB_Analyze.mCheckDictionary.Add(text, new List<string>());
			AB_Analyze.CheckDeps(AB_AssetBuildMgr.mManifest, text, text);
		}
		string text2 = Application.streamingAssetsPath + "/analyze";
		if (!CFileManager.IsDirectoryExist(text2))
		{
			CFileManager.CreateDirectory(text2);
		}
		StreamWriter streamWriter = new StreamWriter(new FileStream(text2 + "/CheckDeps.csv", FileMode.Create));
		foreach (string current in AB_Analyze.mCheckDictionary.Keys)
		{
			string text3 = current + ",";
			foreach (string current2 in AB_Analyze.mCheckDictionary[current])
			{
				text3 = text3 + current2 + ",";
			}
			streamWriter.WriteLine(text3);
		}
		streamWriter.Close();
	}

	private static void CheckDeps(AssetBundleManifest manifest, string rootFile, string currentFile)
	{
		string[] directDependencies = manifest.GetDirectDependencies(currentFile);
		for (int i = 0; i < directDependencies.Length; i++)
		{
			if (directDependencies[i].Equals(rootFile))
			{
				string text = rootFile + " : " + currentFile;
				Debug.LogError("Check deps Error: " + text);
				AB_Analyze.mErrorMessageList.Add(text);
			}
			else if (!AB_Analyze.mCheckDictionary[rootFile].Contains(directDependencies[i]))
			{
				AB_Analyze.mCheckDictionary[rootFile].Add(directDependencies[i]);
				AB_Analyze.CheckDeps(manifest, rootFile, directDependencies[i]);
			}
		}
	}
}

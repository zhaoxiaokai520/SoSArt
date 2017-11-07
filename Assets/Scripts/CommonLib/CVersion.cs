using System;
using UnityEngine;

public class CVersion
{
	public static string s_emptyResourceVersion = "0.0.0.0";

	private static string s_versionTxtPathInResources = "Config/Version";

	private static string s_publishKey = "Publish";

	private static string s_publish;

	private static string s_codeVersionKey = "CodeVersion";

	private static string s_codeVersion;

	private static string s_resourceVersionKey = "ResourceVersion";

	private static string s_resourceVersion;

	private static string s_appVersion;

	private static string s_usedResourceVersion;

	private static string s_buildNumberKey = "Build";

	private static string s_buildNumber;

	private static string s_androidVersionCodeKey = "AndroidVersionCode";

	private static string s_androidVersionCode;

	private static string s_iOSBundleVersionKey = "iOSBundleVersion";

	private static string s_iOSBundleVersion;

	private static string s_revisionNumber;

	public static string GetPublish()
	{
		bool flag = CVersion.s_publish == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_publish;
	}

	public static string GetAppVersion()
	{
		bool flag = CVersion.s_appVersion == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_appVersion;
	}

	public static string GetCodeVersion()
	{
		bool flag = CVersion.s_codeVersion == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_codeVersion;
	}

	public static string GetiOSBundleVersion()
	{
		bool flag = CVersion.s_iOSBundleVersion == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_iOSBundleVersion;
	}

	public static string GetResourceVersion()
	{
		bool flag = CVersion.s_resourceVersion == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_resourceVersion;
	}

	public static int GetAndroidVersionCode()
	{
		bool flag = CVersion.s_androidVersionCode == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return int.Parse(CVersion.s_androidVersionCode.Trim());
	}

	public static uint GetVersionNumber(string versionStr)
	{
		uint num = 0u;
		string[] array = versionStr.Split(new char[]
		{
			'.'
		});
		for (int i = 0; i < array.Length; i++)
		{
			num += uint.Parse(array[i].Trim()) * (uint)Mathf.Pow(10f, (float)((array.Length - i - 1) * 2));
		}
		return num;
	}

	public static string GetBuildNumber()
	{
		bool flag = CVersion.s_buildNumber == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_buildNumber;
	}

	public static string GetRevisonNumber()
	{
		bool flag = CVersion.s_revisionNumber == null;
		if (flag)
		{
			CVersion.Initialize();
		}
		return CVersion.s_revisionNumber;
	}

	public static void SetUsedResourceVersion(string usedResourceVersion)
	{
		CVersion.s_usedResourceVersion = usedResourceVersion;
	}

	public static string GetUsedResourceVersion()
	{
		return CVersion.s_usedResourceVersion;
	}

	public static bool IsSynchronizedVersion(string appVersion, string usedResourceVersion)
	{
		bool flag = string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(usedResourceVersion);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !string.Equals(usedResourceVersion, CVersion.s_usedResourceVersion);
			if (flag2)
			{
				result = false;
			}
			else
			{
				int num = appVersion.LastIndexOf(".");
				bool flag3 = num < 0;
				if (flag3)
				{
					result = false;
				}
				else
				{
					string a = appVersion.Substring(0, num);
					result = string.Equals(a, CVersion.s_codeVersion);
				}
			}
		}
		return result;
	}

	private static void Initialize()
	{
		CVersion.s_publish = string.Empty;
		CVersion.s_codeVersion = string.Empty;
		CVersion.s_resourceVersion = string.Empty;
		CVersion.s_appVersion = string.Empty;
		CVersion.s_usedResourceVersion = string.Empty;
		CVersion.s_buildNumber = string.Empty;
		CVersion.s_revisionNumber = string.Empty;
		CVersion.s_androidVersionCode = string.Empty;
		CVersion.s_iOSBundleVersion = string.Empty;
		TextAsset textAsset = Resources.Load(CVersion.s_versionTxtPathInResources, typeof(TextAsset)) as TextAsset;
		bool flag = textAsset != null;
		if (flag)
		{
			string text = textAsset.text;
			string[] array = text.Split(new char[]
			{
				'[',
				']'
			}, 1);
			bool flag2 = array != null;
			if (flag2)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'='
					}, 1);
					bool flag3 = array2 != null && array2.Length == 2;
					if (flag3)
					{
						for (int j = 0; j < 2; j++)
						{
							array2[j] = array2[j].Trim();
						}
						bool flag4 = string.Equals(array2[0], CVersion.s_publishKey, StringComparison.OrdinalIgnoreCase);
						if (flag4)
						{
							CVersion.s_publish = array2[1];
						}
						else
						{
							bool flag5 = string.Equals(array2[0], CVersion.s_codeVersionKey, StringComparison.OrdinalIgnoreCase);
							if (flag5)
							{
								CVersion.s_codeVersion = array2[1];
							}
							else
							{
								bool flag6 = string.Equals(array2[0], CVersion.s_resourceVersionKey, StringComparison.OrdinalIgnoreCase);
								if (flag6)
								{
									CVersion.s_resourceVersion = array2[1];
								}
								else
								{
									bool flag7 = string.Equals(array2[0], CVersion.s_buildNumberKey, StringComparison.OrdinalIgnoreCase);
									if (flag7)
									{
										CVersion.s_buildNumber = array2[1];
									}
									else
									{
										bool flag8 = string.Equals(array2[0], CVersion.s_androidVersionCodeKey, StringComparison.OrdinalIgnoreCase);
										if (flag8)
										{
											CVersion.s_androidVersionCode = array2[1];
										}
										else
										{
											bool flag9 = string.Equals(array2[0], CVersion.s_iOSBundleVersionKey, StringComparison.OrdinalIgnoreCase);
											if (flag9)
											{
												CVersion.s_iOSBundleVersion = array2[1];
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		bool flag10 = string.IsNullOrEmpty(CVersion.s_iOSBundleVersion);
		if (flag10)
		{
			CVersion.s_iOSBundleVersion = CVersion.s_codeVersion;
		}
		textAsset = (Resources.Load("Revision", typeof(TextAsset)) as TextAsset);
		bool flag11 = textAsset != null;
		if (flag11)
		{
			CVersion.s_revisionNumber = textAsset.text;
		}
		CVersion.s_appVersion = string.Format("{0}.{1}", CVersion.s_codeVersion, CVersion.s_resourceVersion);
		CVersion.s_usedResourceVersion = CVersion.s_appVersion;
	}
}

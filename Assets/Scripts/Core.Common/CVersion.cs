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
		if (CVersion.s_publish == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_publish;
	}

	public static string GetAppVersion()
	{
		if (CVersion.s_appVersion == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_appVersion;
	}

	public static string GetCodeVersion()
	{
		if (CVersion.s_codeVersion == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_codeVersion;
	}

	public static string GetiOSBundleVersion()
	{
		if (CVersion.s_iOSBundleVersion == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_iOSBundleVersion;
	}

	public static string GetResourceVersion()
	{
		if (CVersion.s_resourceVersion == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_resourceVersion;
	}

	public static int GetAndroidVersionCode()
	{
		if (CVersion.s_androidVersionCode == null)
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
		if (CVersion.s_buildNumber == null)
		{
			CVersion.Initialize();
		}
		return CVersion.s_buildNumber;
	}

	public static string GetRevisonNumber()
	{
		if (CVersion.s_revisionNumber == null)
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
		if (string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(usedResourceVersion))
		{
			return false;
		}
		if (!string.Equals(usedResourceVersion, CVersion.s_usedResourceVersion))
		{
			return false;
		}
		int num = appVersion.LastIndexOf(".");
		if (num < 0)
		{
			return false;
		}
		string text = appVersion.Substring(0, num);
		return string.Equals(text, CVersion.s_codeVersion);
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
		if (textAsset != null)
		{
			string text = textAsset.text;
			string[] array = text.Split(new char[]
			{
				'[',
				']'
			}, 1);
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'='
					}, 1);
					if (array2 != null && array2.Length == 2)
					{
						for (int j = 0; j < 2; j++)
						{
							array2[j] = array2[j].Trim();
						}
						if (string.Equals(array2[0], CVersion.s_publishKey, (StringComparison)5))
						{
							CVersion.s_publish = array2[1];
						}
						else if (string.Equals(array2[0], CVersion.s_codeVersionKey, (StringComparison)5))
						{
							CVersion.s_codeVersion = array2[1];
						}
						else if (string.Equals(array2[0], CVersion.s_resourceVersionKey, (StringComparison)5))
						{
							CVersion.s_resourceVersion = array2[1];
						}
						else if (string.Equals(array2[0], CVersion.s_buildNumberKey, (StringComparison)5))
						{
							CVersion.s_buildNumber = array2[1];
						}
						else if (string.Equals(array2[0], CVersion.s_androidVersionCodeKey, (StringComparison)5))
						{
							CVersion.s_androidVersionCode = array2[1];
						}
						else if (string.Equals(array2[0], CVersion.s_iOSBundleVersionKey, (StringComparison)5))
						{
							CVersion.s_iOSBundleVersion = array2[1];
						}
					}
				}
			}
		}
		if (string.IsNullOrEmpty(CVersion.s_iOSBundleVersion))
		{
			CVersion.s_iOSBundleVersion = CVersion.s_codeVersion;
		}
		textAsset = (Resources.Load("Revision", typeof(TextAsset)) as TextAsset);
		if (textAsset != null)
		{
			CVersion.s_revisionNumber = textAsset.text;
		}
		CVersion.s_appVersion = string.Format("{0}.{1}", CVersion.s_codeVersion, CVersion.s_resourceVersion);
		CVersion.s_usedResourceVersion = CVersion.s_appVersion;
	}
}

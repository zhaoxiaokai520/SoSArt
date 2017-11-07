using System;
using System.Text;
using UnityEngine;

public class StringHelper
{
	public static StringBuilder Formater = new StringBuilder(1024);

	public static void ClearFormater()
	{
		StringHelper.Formater.Remove(0, StringHelper.Formater.Length);
	}

	public static string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes).TrimEnd(new char[1]);
	}

	public static string BytesToString(string str)
	{
		return str;
	}

	public static string UTF8BytesToString(ref byte[] str)
	{
		string result;
		try
		{
			result = ((str == null) ? null : Encoding.UTF8.GetString(str).TrimEnd(new char[1]));
		}
		catch
		{
			result = null;
		}
		return result;
	}

	public static string ASCIIBytesToString(byte[] data)
	{
		bool flag = data == null;
		string result;
		if (flag)
		{
			result = null;
		}
		else
		{
			string text;
			try
			{
				text = Encoding.ASCII.GetString(data).TrimEnd(new char[1]);
			}
			catch (Exception)
			{
				text = null;
			}
			result = text;
		}
		return result;
	}

	public static void StringToUTF8Bytes(string str, ref byte[] buffer)
	{
		bool flag = str == null || buffer == null;
		if (!flag)
		{
			bool flag2 = !str.EndsWith("\0");
			if (flag2)
			{
				str += "\0";
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int count = (bytes.Length <= buffer.Length) ? bytes.Length : buffer.Length;
			Buffer.BlockCopy(bytes, 0, buffer, 0, count);
			buffer[buffer.Length - 1] = 0;
		}
	}

	public static bool IsAvailableString(string str)
	{
		int num = 0;
		int i = 0;
		bool flag = false;
		int length = str.Length;
		bool result;
		while (i < length)
		{
			char c = str[i];
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = c < '\udc00' || c > '\udfff';
				if (flag3)
				{
					Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate tail)", i));
					result = false;
					return result;
				}
				num += 4;
				flag = false;
			}
			else
			{
				bool flag4 = c < '\u0080';
				if (flag4)
				{
					while (i < length)
					{
						bool flag5 = str[i] >= '\u0080';
						if (flag5)
						{
							break;
						}
						num++;
						i++;
					}
					continue;
				}
				bool flag6 = c < 'ࠀ';
				if (flag6)
				{
					num += 2;
				}
				else
				{
					bool flag7 = c >= '\ud800' && c <= '\udbff';
					if (flag7)
					{
						flag = true;
					}
					else
					{
						bool flag8 = c >= '\udc00' && c <= '\udfff';
						if (flag8)
						{
							Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate head)", i));
							result = false;
							return result;
						}
						num += 3;
					}
				}
			}
			i++;
		}
		result = true;
		return result;
	}

	public static void StringToUTF8Bytes(string str, ref sbyte[] buffer)
	{
		bool flag = str == null || buffer == null;
		if (!flag)
		{
			bool flag2 = !str.EndsWith("\0");
			if (flag2)
			{
				str += "\0";
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int count = (bytes.Length <= buffer.Length) ? bytes.Length : buffer.Length;
			Buffer.BlockCopy(bytes, 0, buffer, 0, count);
			buffer[buffer.Length - 1] = 0;
		}
	}

	public static string AutoAppendString(params object[] strs)
	{
		StringHelper.ClearFormater();
		for (int i = 0; i < strs.Length; i++)
		{
			StringHelper.Formater.Append(strs[i]);
		}
		return StringHelper.Formater.ToString();
	}

	public static void AutoAppendString(string str1, char str2)
	{
		StringHelper.ClearFormater();
		StringHelper.Formater.Append(str1);
		StringHelper.Formater.Append(str2);
	}

	public static void AutoAppendString(string str1, string str2)
	{
		StringHelper.ClearFormater();
		StringHelper.Formater.Append(str1);
		StringHelper.Formater.Append(str2);
	}

	public static void AutoAppendString(string str1, int str2)
	{
		StringHelper.ClearFormater();
		StringHelper.Formater.Append(str1);
		StringHelper.Formater.Append(str2);
	}
}


using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CFileManager
{
	public delegate void DelegateOnOperateFileFail(string fullPath, kFileOperation fileOperation);

	private static string s_cachePath = null;

	public static string s_ifsExtractFolder = "Resources";
	public static string s_ifsExtractPath = Application.dataPath+"/Resources";
    public static string s_localPathUrlHeader = "file://";

	private static readonly MD5CryptoServiceProvider s_md5Provider = new MD5CryptoServiceProvider();

    public static CFileManager.DelegateOnOperateFileFail s_delegateOnOperateFileFail = delegate {};

	public static bool IsFileExist(string filePath)
	{
		return File.Exists(filePath);
	}

	public static bool IsDirectoryExist(string directory)
	{
		return Directory.Exists(directory);
	}

	public static bool CreateDirectory(string directory)
	{
		if (CFileManager.IsDirectoryExist(directory))
		{
			return true;
		}

        int tryCount = 0;
		bool result;
		while (true)
		{
			try
			{
				Directory.CreateDirectory(directory);
				result = true;
				break;
			}
			catch (Exception e)
			{
                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Create Directory " + directory + " Error! Exception = " + e.Message);
					CFileManager.s_delegateOnOperateFileFail(directory, kFileOperation.CreateDirectory);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool DeleteDirectory(string directory)
	{
		if (!CFileManager.IsDirectoryExist(directory))
		{
			return true;
		}
		int tryCount = 0;
		bool result;
		while (true)
		{
			try
			{
				Directory.Delete(directory, true);
				result = true;
				break;
			}
			catch (Exception e)
			{
                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Delete Directory " + directory + " Error! Exception = " + e.Message);
					CFileManager.s_delegateOnOperateFileFail(directory, kFileOperation.DeleteDirectory);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static int GetFileLength(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return 0;
		}

		int tryCount = 0;
		int result;
		while (true)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(filePath);
				result = (int)fileInfo.Length;
				break;
			}
			catch (Exception e)
			{
                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Get FileLength of " + filePath + " Error! Exception = " + e.Message);
					result = 0;
					break;
				}
			}
		}
		return result;
	}

	public static byte[] ReadFile(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return null;
		}

		byte[] aData = null;
		int tryCount = 0;
		
        do
		{
			try
			{
                aData = File.ReadAllBytes(filePath);
			}
			catch (Exception e)
			{
                Debug.LogError("Get FileLength of " + filePath + " Error! Exception = " + e.Message);
                aData = null;
			}

            if (aData != null && aData.Length > 0)
			{
                return aData;
			}
            tryCount++;
		}
        while (tryCount < 3);

		CFileManager.s_delegateOnOperateFileFail(filePath, kFileOperation.ReadFile);

		return null;
	}

	public static bool WriteFile(string filePath, byte[] data)
	{
		int tryCount = 0;
		bool result;
		while (true)
		{
			try
			{
				File.WriteAllBytes(filePath, data);
				result = true;
				break;
			}
			catch (Exception e)
			{
                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Write File " + filePath + " Error! Exception = " + e.Message);
					CFileManager.DeleteFile(filePath);
					CFileManager.s_delegateOnOperateFileFail(filePath, kFileOperation.WriteFile);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool WriteFile(string filePath, byte[] data, int offset, int length)
	{
		FileStream fileStream = null;
		int tryCount = 0;
		bool result;
		while (true)
		{
			try
			{
                fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);   // 4, 2, 3
				fileStream.Write(data, offset, length);
				fileStream.Close();
				result = true;
				break;
			}
			catch (Exception e)
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}

                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Write File " + filePath + " Error! Exception = " + e.Message);
					CFileManager.DeleteFile(filePath);
					CFileManager.s_delegateOnOperateFileFail(filePath, kFileOperation.WriteFile);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static bool DeleteFile(string filePath)
	{
		if (!CFileManager.IsFileExist(filePath))
		{
			return true;
		}

		int tryCount = 0;
		bool result;
		while (true)
		{
			try
			{
				File.Delete(filePath);
				result = true;
				break;
			}
			catch (Exception e)
			{
                tryCount++;
                if (tryCount >= 3)
				{
					Debug.LogError("Delete File " + filePath + " Error! Exception = " + e.Message);
					CFileManager.s_delegateOnOperateFileFail(filePath, kFileOperation.DeleteFile);
					result = false;
					break;
				}
			}
		}
		return result;
	}

	public static void CopyFile(string srcFile, string dstFile)
	{
		File.Copy(srcFile, dstFile, true);
	}

	public static string GetFileMd5(string filePath)
	{
	    if (string.IsNullOrEmpty(filePath))
	    {
	        return string.Empty;
	    }
		if (!CFileManager.IsFileExist(filePath))
		{
			return string.Empty;
		}
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(CFileManager.ReadFile(filePath))).Replace("-", string.Empty);
	}

	public static string GetMd5(byte[] data)
	{
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(data)).Replace("-", string.Empty);
	}

	public static string GetMd5(string str)
	{
		return BitConverter.ToString(CFileManager.s_md5Provider.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", string.Empty);
	}

	public static string CombinePath(string firstPath, string secondPath)
	{
        if (firstPath.LastIndexOf('/') != firstPath.Length - 1)
		{
            firstPath += "/";
		}

        if (secondPath.IndexOf('/') == 0)
		{
            secondPath = secondPath.Substring(1);
		}
        return firstPath + secondPath;
	}

	public static string CombinePaths(params string[] values)
	{
		if (values.Length <= 0)
		{
			return string.Empty;
		}
		
        if (values.Length == 1)
		{
			return CFileManager.CombinePath(values[0], string.Empty);
		}
		
        if (values.Length > 1)
		{
			string path = CFileManager.CombinePath(values[0], values[1]);
			for (int i = 2; i < values.Length; i++)
			{
                path = CFileManager.CombinePath(path, values[i]);
			}
			return path;
		}

		return string.Empty;
	}

	public static string GetStreamingAssetsPathWithHeader(string fileName)
	{
		return Path.Combine(Application.streamingAssetsPath, fileName);
	}

	public static string GetCachePath()
	{
		if (string.IsNullOrEmpty(CFileManager.s_cachePath))
		{
			CFileManager.s_cachePath = Application.persistentDataPath;
		}
		return CFileManager.s_cachePath;
	}

	public static string GetCachePath(string fileName)
	{
		return CFileManager.CombinePath(CFileManager.GetCachePath(), fileName);
	}

	public static string GetCachePathWithHeader(string fileName)
	{
		return s_localPathUrlHeader + CFileManager.GetCachePath(fileName);
	}

	public static string GetIFSExtractPath()
	{
		if (string.IsNullOrEmpty(CFileManager.s_ifsExtractPath))
		{
			CFileManager.s_ifsExtractPath = CFileManager.CombinePath(CFileManager.GetCachePath(), CFileManager.s_ifsExtractFolder);
		}
		return CFileManager.s_ifsExtractPath;
	}

	public static string GetFullName(string fullPath)
	{
		if (string.IsNullOrEmpty(fullPath))
		{
            return string.Empty;
		}

		int pos = fullPath.LastIndexOf("/");
        if (pos > 0)
		{
            return fullPath.Substring(pos + 1, fullPath.Length - pos - 1);
		}
		return fullPath;
	}

	public static string EraseExtension(string fullName)
	{
		if (string.IsNullOrEmpty(fullName))
		{
            return string.Empty;
		}

        int pos = fullName.LastIndexOf('.');
        if (pos > 0)
		{
            return fullName.Substring(0, pos);
		}
		return fullName;
	}

	public static string GetExtension(string fullName)
	{
	    if (string.IsNullOrEmpty(fullName))
	    {
	        return string.Empty;
	    }
        int pos = fullName.LastIndexOf('.');
        if (pos > 0 && pos + 1 < fullName.Length)
		{
            return fullName.Substring(pos);
		}
		return string.Empty;
	}

	public static string GetFullDirectory(string fullPath)
	{
        if (string.IsNullOrEmpty(fullPath))
        {
            return string.Empty;
        }
		return Path.GetDirectoryName(fullPath);
	}

	public static bool ClearDirectory(string fullPath)
	{
		bool result;
		try
		{
			string[] files = Directory.GetFiles(fullPath);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
            string[] directories = Directory.GetDirectories(fullPath);
			for (int j = 0; j < directories.Length; j++)
			{
				Directory.Delete(directories[j], true);
			}
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool ClearDirectory(string fullPath, string[] fileExtensionFilter, string[] folderFilter)
	{
		bool result;
		try
		{
			if (fileExtensionFilter != null)
			{
				string[] files = Directory.GetFiles(fullPath);
				for (int i = 0; i < files.Length; i++)
				{
					if (fileExtensionFilter.Length > 0)
					{
						for (int j = 0; j < fileExtensionFilter.Length; j++)
						{
							if (files[i].Contains(fileExtensionFilter[j]))
							{
								CFileManager.DeleteFile(files[i]);
								break;
							}
						}
					}
				}
			}

			if (folderFilter != null)
			{
				string[] directories = Directory.GetDirectories(fullPath);
				for (int k = 0; k < directories.Length; k++)
				{
					if (folderFilter != null && folderFilter.Length > 0)
					{
						for (int l = 0; l < folderFilter.Length; l++)
						{
							if (directories[k].Contains(folderFilter[l]))
							{
								CFileManager.DeleteDirectory(directories[k]);
								break;
							}
						}
					}
				}
			}
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}

    private static readonly byte sFactor = 123;
    static public byte Encode(byte inbyte)
    {
        inbyte = (byte)(0xff & ((inbyte >> 6) | (inbyte << 2)));
        return (byte)(inbyte ^ sFactor);
    }

    static public byte Decode(byte inbyte)
    {
        inbyte ^= sFactor;
        return (byte)(0xff & ((inbyte << 6) | (inbyte >> 2)));
    }

    public static byte[] AESEncrypt(byte[] rawBytes, string Key)
    {
        // Check arguments.
        if (rawBytes == null || rawBytes.Length <= 0)
            throw new ArgumentNullException("RawBytes");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");

        byte[] encrypted;
        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.Default.GetBytes(Key);
            aesAlg.IV = Encoding.Default.GetBytes("yesmylord1234567");

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(rawBytes, 0, rawBytes.Length);
                    csEncrypt.FlushFinalBlock();
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    public static byte[] AESDecrypt(byte[] rawBytes, string Key)
    {
        // Check arguments.
        if (rawBytes == null || rawBytes.Length <= 0)
            throw new ArgumentNullException("RawBytes");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");

        byte[] decrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.Default.GetBytes(Key);
            aesAlg.IV = Encoding.Default.GetBytes("yesmylord1234567");
            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(rawBytes, 0, rawBytes.Length);
                    csDecrypt.FlushFinalBlock();
                    decrypted = msDecrypt.ToArray();
                }
            }
        }
        return decrypted;
    }

#if false
    public static byte[] AESEncrypt(byte[] EncryptByte, string EncryptKey)
    {
        if (EncryptByte.Length == 0) { throw (new Exception("明文不得为空")); }
        if (string.IsNullOrEmpty(EncryptKey)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strEncrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptKey, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(EncryptByte, 0, EncryptByte.Length);
            m_csstream.FlushFinalBlock();
            m_strEncrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strEncrypt;
    }

    public static byte[] AESDecrypt(byte[] DecryptByte, string DecryptKey)
    {
        if (DecryptByte.Length == 0) { throw (new Exception("密文不得为空")); }
        if (string.IsNullOrEmpty(DecryptKey)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strDecrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(DecryptKey, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(DecryptByte, 0, DecryptByte.Length);
            m_csstream.FlushFinalBlock();
            m_strDecrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strDecrypt;
    }
#endif

}

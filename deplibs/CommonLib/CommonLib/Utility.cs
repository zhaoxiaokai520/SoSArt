using MobaGo.EventBus;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.IO.Compression;
using UnityEngine;

public static class Utility
{
	public static class CRC32
	{
		private static uint[] crc32_table = new uint[]
		{
			0u,
			1996959894u,
			3993919788u,
			2567524794u,
			124634137u,
			1886057615u,
			3915621685u,
			2657392035u,
			249268274u,
			2044508324u,
			3772115230u,
			2547177864u,
			162941995u,
			2125561021u,
			3887607047u,
			2428444049u,
			498536548u,
			1789927666u,
			4089016648u,
			2227061214u,
			450548861u,
			1843258603u,
			4107580753u,
			2211677639u,
			325883990u,
			1684777152u,
			4251122042u,
			2321926636u,
			335633487u,
			1661365465u,
			4195302755u,
			2366115317u,
			997073096u,
			1281953886u,
			3579855332u,
			2724688242u,
			1006888145u,
			1258607687u,
			3524101629u,
			2768942443u,
			901097722u,
			1119000684u,
			3686517206u,
			2898065728u,
			853044451u,
			1172266101u,
			3705015759u,
			2882616665u,
			651767980u,
			1373503546u,
			3369554304u,
			3218104598u,
			565507253u,
			1454621731u,
			3485111705u,
			3099436303u,
			671266974u,
			1594198024u,
			3322730930u,
			2970347812u,
			795835527u,
			1483230225u,
			3244367275u,
			3060149565u,
			1994146192u,
			31158534u,
			2563907772u,
			4023717930u,
			1907459465u,
			112637215u,
			2680153253u,
			3904427059u,
			2013776290u,
			251722036u,
			2517215374u,
			3775830040u,
			2137656763u,
			141376813u,
			2439277719u,
			3865271297u,
			1802195444u,
			476864866u,
			2238001368u,
			4066508878u,
			1812370925u,
			453092731u,
			2181625025u,
			4111451223u,
			1706088902u,
			314042704u,
			2344532202u,
			4240017532u,
			1658658271u,
			366619977u,
			2362670323u,
			4224994405u,
			1303535960u,
			984961486u,
			2747007092u,
			3569037538u,
			1256170817u,
			1037604311u,
			2765210733u,
			3554079995u,
			1131014506u,
			879679996u,
			2909243462u,
			3663771856u,
			1141124467u,
			855842277u,
			2852801631u,
			3708648649u,
			1342533948u,
			654459306u,
			3188396048u,
			3373015174u,
			1466479909u,
			544179635u,
			3110523913u,
			3462522015u,
			1591671054u,
			702138776u,
			2966460450u,
			3352799412u,
			1504918807u,
			783551873u,
			3082640443u,
			3233442989u,
			3988292384u,
			2596254646u,
			62317068u,
			1957810842u,
			3939845945u,
			2647816111u,
			81470997u,
			1943803523u,
			3814918930u,
			2489596804u,
			225274430u,
			2053790376u,
			3826175755u,
			2466906013u,
			167816743u,
			2097651377u,
			4027552580u,
			2265490386u,
			503444072u,
			1762050814u,
			4150417245u,
			2154129355u,
			426522225u,
			1852507879u,
			4275313526u,
			2312317920u,
			282753626u,
			1742555852u,
			4189708143u,
			2394877945u,
			397917763u,
			1622183637u,
			3604390888u,
			2714866558u,
			953729732u,
			1340076626u,
			3518719985u,
			2797360999u,
			1068828381u,
			1219638859u,
			3624741850u,
			2936675148u,
			906185462u,
			1090812512u,
			3747672003u,
			2825379669u,
			829329135u,
			1181335161u,
			3412177804u,
			3160834842u,
			628085408u,
			1382605366u,
			3423369109u,
			3138078467u,
			570562233u,
			1426400815u,
			3317316542u,
			2998733608u,
			733239954u,
			1555261956u,
			3268935591u,
			3050360625u,
			752459403u,
			1541320221u,
			2607071920u,
			3965973030u,
			1969922972u,
			40735498u,
			2617837225u,
			3943577151u,
			1913087877u,
			83908371u,
			2512341634u,
			3803740692u,
			2075208622u,
			213261112u,
			2463272603u,
			3855990285u,
			2094854071u,
			198958881u,
			2262029012u,
			4057260610u,
			1759359992u,
			534414190u,
			2176718541u,
			4139329115u,
			1873836001u,
			414664567u,
			2282248934u,
			4279200368u,
			1711684554u,
			285281116u,
			2405801727u,
			4167216745u,
			1634467795u,
			376229701u,
			2685067896u,
			3608007406u,
			1308918612u,
			956543938u,
			2808555105u,
			3495958263u,
			1231636301u,
			1047427035u,
			2932959818u,
			3654703836u,
			1088359270u,
			936918000u,
			2847714899u,
			3736837829u,
			1202900863u,
			817233897u,
			3183342108u,
			3401237130u,
			1404277552u,
			615818150u,
			3134207493u,
			3453421203u,
			1423857449u,
			601450431u,
			3009837614u,
			3294710456u,
			1567103746u,
			711928724u,
			3020668471u,
			3272380065u,
			1510334235u,
			755167117u
		};

		public static uint Crc32(byte[] data, int length)
		{
			uint num = 4294967295u;
			for (int i = 0; i < length; i++)
			{
				num = ((num >> 8 & 16777215u) ^ Utility.CRC32.crc32_table[(int)((num ^ (uint)data[i]) & 255u)]);
			}
			return num ^ 4294967295u;
		}

		public static uint Crc32(byte[] data)
		{
			return Utility.CRC32.Crc32(data, data.Length);
		}
	}

	public delegate int InsertComparsionFunc<T>(T atom, T curr, T last);

	public const int kSharedBufferSize = 4096;

	public static bool LinkedListInsert<T>(LinkedList<T> list, T atom, Utility.InsertComparsionFunc<T> func)
	{
		if (list != null && atom != null)
		{
			if (list.Count == 0)
			{
				list.AddFirst(atom);
				return true;
			}
			LinkedListNode<T> linkedListNode = list.First;
			LinkedListNode<T> linkedListNode2 = null;
			while (linkedListNode != null && linkedListNode.Value != null)
			{
				T value = linkedListNode.Value;
				T last = (linkedListNode2 != null) ? linkedListNode2.Value : default(T);
				int num = func(atom, value, last);
				if (num == -1)
				{
					list.AddBefore(linkedListNode, atom);
					return true;
				}
				if (num == 1)
				{
					list.AddAfter(linkedListNode2, atom);
					return true;
				}
				if (num == 2)
				{
					return false;
				}
				linkedListNode2 = linkedListNode;
				linkedListNode = linkedListNode.Next;
			}
			if (func(atom, linkedListNode2.Value, default(T)) != 2)
			{
				list.AddLast(atom);
				return true;
			}
		}
		return false;
	}

	public static int SparseArray_Compress(byte[] indata, int size, ref byte[] outdata)
	{
		int num = 1;
		byte b = (byte)(size % 8);
		int i = 0;
		while (i < size)
		{
			byte b2 = 0;
			int num2 = num++;
			int num3 = Math.Min(8, size - i);
			for (int j = 0; j < num3; j++)
			{
				if (indata[i + j] != 0)
				{
					b2 |= (byte)(1 << j);
					outdata[num++] = indata[i + j];
				}
			}
			outdata[num2] = b2;
			i += num3;
			if (i >= size)
			{
				outdata[0] = (byte)((int)b << 4 | (int)((byte)(num - num2)));
			}
		}
		return num;
	}

	public static int SparseArray_Decompress(byte[] indata, int size, ref byte[] outdata)
	{
		int result = 0;
		byte b = (byte)((indata[0] & 240) >> 4);
		byte b2 = System.Convert.ToByte(System.Convert.ToInt32(indata[0]) & 15);
		int num = 8;
		int i = 1;
		while (i < size)
		{
			byte b3 = indata[i++];
			for (int j = 0; j < num; j++)
			{
				if ((byte)((1 << j & (int)b3) >> j) == 1)
				{
					outdata[result++] = indata[i++];
				}
				else
				{
					outdata[result++] = 0;
				}
			}
			if (i == size - (int)b2)
			{
				num = (int)((b > 0) ? b : 8);
			}
		}
		return result;
	}

	public static int NextPowerOfTwo(int n)
	{
		int i;
		for (i = 1; i < n; i *= 2)
		{
		}
		return i;
	}

	public static void NotifyEvents(object sender, int channel, int op)
	{
		MEObjDeliver mEObjDeliver = Singleton<SmartReferencePool>.instance.Fetch<MEObjDeliver>(32, 4);
		mEObjDeliver.opcode = op;
		Singleton<Mercury>.instance.Broadcast(channel, sender, mEObjDeliver);
	}

	public static int Gzip_DecompressData(byte[] indata, out byte[] outdata)
	{
		int result = 0;
		using (Stream stream = new MemoryStream(indata))
		{
			result = Utility.Zlib_Internal_DecompressBuffer(new GZipStream(stream, CompressionMode.Decompress), out outdata);
		}
		return result;
	}

	public static int Gzip_CompressData(byte[] indata, out byte[] outdata)
	{
		SharedBuffer sharedBuffer = Singleton<SmartReferencePool>.instance.Fetch<SharedBuffer>(32, 4);
		int num = indata.Length * 2 + 512;
		if (sharedBuffer.buffer.Length < num)
		{
			sharedBuffer.buffer = new byte[Utility.NextPowerOfTwo(num)];
		}
		outdata = null;
		Stream stream = new MemoryStream(sharedBuffer.buffer, 0, sharedBuffer.buffer.Length, true, true);
		Stream compressor = new GZipStream(stream, CompressionMode.Compress, true);
		int arg_72_0 = Utility.Zlib_Internal_CompressBuffer(indata, indata.Length, compressor, out outdata);
		stream.Close();
		sharedBuffer.Release();
		return arg_72_0;
	}

	private static int Zlib_Internal_CompressBuffer(byte[] rawdata, int rawdataLength, Stream compressor, out byte[] outdata)
	{
		outdata = null;
		if (rawdata == null || rawdataLength <= 0 || rawdata.Length < rawdataLength)
		{
			return 0;
		}
		bool flag = false;
		MemoryStream memoryStream = (MemoryStream)((GZipStream)compressor).BaseStream;
		try
		{
			compressor.Write(rawdata, 0, rawdataLength);
			compressor.Close();
		}
		catch (Exception ex)
		{
			flag = true;
			Debug.LogError(string.Concat(new object[]
			{
				"FAILURE: Zlib_Internal_CompressBuffer - data length = ",
				rawdataLength,
				", ",
				ex.Message
			}));
		}
		int num;
		if (flag)
		{
			num = 0;
		}
		else
		{
			num = (int)memoryStream.Position;
			SharedBuffer sharedBuffer = Singleton<SmartReferencePool>.instance.Fetch<SharedBuffer>(32, 4);
			if (sharedBuffer.buffer.Length < num)
			{
				sharedBuffer.buffer = new byte[Utility.NextPowerOfTwo(num)];
			}
			outdata = sharedBuffer.buffer;
			Buffer.BlockCopy(memoryStream.GetBuffer(), 0, outdata, 0, num);
			sharedBuffer.Release();
		}
		return num;
	}

	private static int Zlib_Internal_DecompressBuffer(Stream decompressor, out byte[] outdata)
	{
		SharedBuffer sharedBuffer = Singleton<SmartReferencePool>.instance.Fetch<SharedBuffer>(32, 4);
		SharedBuffer sharedBuffer2 = Singleton<SmartReferencePool>.instance.Fetch<SharedBuffer>(32, 4);
		byte[] buffer = sharedBuffer.buffer;
		int num = 0;
		int num2 = 0;
		try
		{
			int num3;
			while ((num3 = decompressor.Read(buffer, 0, buffer.Length)) != 0)
			{
				num += num3;
				if (num2 + num3 >= sharedBuffer2.buffer.Length)
				{
					byte[] array = new byte[Utility.NextPowerOfTwo(sharedBuffer2.buffer.Length + num3)];
					Buffer.BlockCopy(sharedBuffer2.buffer, 0, array, 0, num2);
					sharedBuffer2.buffer = array;
				}
				Buffer.BlockCopy(buffer, 0, sharedBuffer2.buffer, num2, num3);
				num2 += num3;
			}
		}
		finally
		{
			if (decompressor != null)
			{
				((IDisposable)decompressor).Dispose();
			}
		}
		outdata = sharedBuffer2.buffer;
		sharedBuffer.Release();
		sharedBuffer2.Release();
		return num;
	}
}

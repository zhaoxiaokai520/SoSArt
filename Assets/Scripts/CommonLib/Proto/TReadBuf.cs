using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Proto
{
	public class TReadBuf : TBuf
	{
		public byte[] beginPtr;

		public int position;

		public int length;

		public bool IsNetEndian;

		public TReadBuf()
		{
			this.length = 0;
			this.position = 0;
			this.beginPtr = null;
			this.IsNetEndian = true;
		}

		public TReadBuf(ref TWriteBuf writeBuf)
		{
			byte[] array = writeBuf.getBeginPtr();
			this.set(ref array, writeBuf.getUsedSize(), true);
		}

		public TReadBuf(ref byte[] ptr, int len)
		{
			this.set(ref ptr, len, true);
		}

		public override void OnRelease()
		{
			this.reset();
		}

		public void reset()
		{
			this.length = 0;
			this.position = 0;
			this.beginPtr = null;
			this.IsNetEndian = true;
		}

		public void set(ref byte[] ptr, int len, bool _IsNetEndian)
		{
			this.beginPtr = ptr;
			this.position = 0;
			this.length = 0;
			this.IsNetEndian = _IsNetEndian;
			bool flag = this.beginPtr != null;
			if (flag)
			{
				this.length = len;
			}
		}

		public T readStruct<T>()
		{
			int num = Marshal.SizeOf(typeof(T));
			bool flag = this.length - this.position < num;
			if (flag)
			{
				throw new Exception();
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			T result;
			try
			{
				Marshal.Copy(this.beginPtr, this.position, intPtr, num);
				result = (T)((object)Marshal.PtrToStructure(intPtr, typeof(T)));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		public int getUsedSize()
		{
			return this.position;
		}

		public int getTotalSize()
		{
			return this.length;
		}

		public int getLeftSize()
		{
			return this.length - this.position;
		}

		public void disableEndian()
		{
			this.IsNetEndian = false;
		}

		public TError.ErrorType readInt8(ref sbyte dest)
		{
			byte b = 0;
			TError.ErrorType result = this.readUInt8(ref b);
			dest = (sbyte)b;
			return result;
		}

		public TError.ErrorType readUInt8(ref byte dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 1 > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					byte[] arg_46_0 = this.beginPtr;
					int num = this.position;
					this.position = num + 1;
					dest = arg_46_0[num];
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readInt16(ref short dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 2 > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt16(this.beginPtr, this.position);
					this.position += 2;
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt16(ref ushort dest)
		{
			short num = 0;
			TError.ErrorType result = this.readInt16(ref num);
			dest = (ushort)num;
			return result;
		}

		public TError.ErrorType readInt32(ref int dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 4 > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt32(this.beginPtr, this.position);
					this.position += 4;
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt32(ref uint dest)
		{
			int num = 0;
			TError.ErrorType result = this.readInt32(ref num);
			dest = (uint)num;
			return result;
		}

		public TError.ErrorType readInt64(ref long dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 8 > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt64(this.beginPtr, this.position);
					this.position += 8;
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt64(ref ulong dest)
		{
			long num = 0L;
			TError.ErrorType result = this.readInt64(ref num);
			dest = (ulong)num;
			return result;
		}

		public TError.ErrorType readFloat(ref float dest)
		{
			int value = 0;
			TError.ErrorType result = this.readInt32(ref value);
			dest = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
			return result;
		}

		public TError.ErrorType readDouble(ref double dest)
		{
			long value = 0L;
			TError.ErrorType result = this.readInt64(ref value);
			dest = BitConverter.Int64BitsToDouble(value);
			return result;
		}

		public TError.ErrorType readVarUInt16(ref ushort dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_39_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_39_0[num2];
					dest |= (ushort)(((ulong)b & 127uL) << 7 * num);
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = !BitConverter.IsLittleEndian;
						if (flag3)
						{
							dest = (ushort)IPAddress.NetworkToHostOrder((short)dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType readVarInt16(ref short dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_3C_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_3C_0[num2];
					dest |= (short)(((ulong)b & 127uL) << 7 * num);
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = (dest & 1) != 0;
						if (flag3)
						{
							dest = (short)((((int)dest ^ 65535) >> 1 & -32769) | (int)(dest & 1) << 15);
						}
						else
						{
							dest = (short)((dest >> 1 & -32769) | (int)(dest & 1) << 15);
						}
						bool flag4 = !BitConverter.IsLittleEndian;
						if (flag4)
						{
							dest = IPAddress.NetworkToHostOrder(dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType readVarUInt32(ref uint dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0u;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_39_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_39_0[num2];
					dest |= (uint)((ulong)b & 127uL) << 7 * num;
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = !BitConverter.IsLittleEndian;
						if (flag3)
						{
							dest = (uint)IPAddress.NetworkToHostOrder((int)dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType readVarInt32(ref int dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_3C_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_3C_0[num2];
					dest |= (int)((ulong)b & 127uL) << 7 * num;
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = (dest & 1) != 0;
						if (flag3)
						{
							uint num3 = (uint)(dest ^ -1) >> 1;
							num3 &= 2147483647u;
							dest = (int)(num3 | (uint)((uint)(dest & 1) << 31));
						}
						else
						{
							uint num4 = (uint)(dest >> 1 & 2147483647);
							dest = (int)(num4 | (uint)((uint)(dest & 1) << 31));
						}
						bool flag4 = !BitConverter.IsLittleEndian;
						if (flag4)
						{
							dest = IPAddress.NetworkToHostOrder(dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType readVarUInt64(ref ulong dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0uL;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_3A_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_3A_0[num2];
					dest |= ((ulong)b & 127uL) << 7 * num;
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = !BitConverter.IsLittleEndian;
						if (flag3)
						{
							dest = (ulong)IPAddress.NetworkToHostOrder((long)dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType readVarInt64(ref long dest)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				dest = 0L;
				int num = 0;
				while (this.length - this.position > 0)
				{
					byte[] arg_3D_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					byte b = arg_3D_0[num2];
					dest |= (long)((long)((ulong)b & 127uL) << 7 * num);
					num++;
					bool flag2 = (b & 128) == 0;
					if (flag2)
					{
						bool flag3 = (dest & 1L) != 0L;
						if (flag3)
						{
							ulong num3 = (ulong)(dest ^ -1L) >> 1;
							num3 &= 9223372036854775807uL;
							dest = (long)(num3 | (ulong)((ulong)(dest & 1L) << 63));
						}
						else
						{
							ulong num4 = (ulong)(dest >> 1 & 9223372036854775807L);
							dest = (long)(num4 | (ulong)((ulong)(dest & 1L) << 63));
						}
						bool flag4 = !BitConverter.IsLittleEndian;
						if (flag4)
						{
							dest = IPAddress.NetworkToHostOrder(dest);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			return result;
		}

		public TError.ErrorType skipForward(int step)
		{
			bool flag = this.length - this.position < step;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			else
			{
				this.position += step;
				result = TError.ErrorType.TDR_NO_ERROR;
			}
			return result;
		}

		public TError.ErrorType readCString(ref byte[] dest, int count)
		{
			bool flag = this.beginPtr == null || count > dest.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = dest == null || dest.GetLength(0) == 0;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
				}
				else
				{
					bool flag3 = count > this.length - this.position;
					if (flag3)
					{
						result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							byte[] arg_7C_0 = dest;
							int arg_7C_1 = i;
							byte[] arg_7B_0 = this.beginPtr;
							int num = this.position;
							this.position = num + 1;
							arg_7C_0[arg_7C_1] = arg_7B_0[num];
						}
						result = TError.ErrorType.TDR_NO_ERROR;
					}
				}
			}
			return result;
		}

		public TError.ErrorType readWString(ref short[] dest, int count)
		{
			bool flag = this.beginPtr == null || count > dest.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = dest == null || dest.GetLength(0) == 0;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
				}
				else
				{
					bool flag3 = 2 * count > this.length - this.position;
					if (flag3)
					{
						result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							dest[i] = BitConverter.ToInt16(this.beginPtr, this.position);
							this.position += 2;
						}
						result = TError.ErrorType.TDR_NO_ERROR;
					}
				}
			}
			return result;
		}

		public TError.ErrorType readInt8(ref sbyte dest, int pos)
		{
			byte b = 0;
			TError.ErrorType result = this.readUInt8(ref b, pos);
			dest = (sbyte)b;
			return result;
		}

		public TError.ErrorType readUInt8(ref byte dest, int pos)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 1 > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = this.beginPtr[pos];
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readInt16(ref short dest, int pos)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 2 > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt16(this.beginPtr, pos);
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt16(ref ushort dest, int pos)
		{
			short num = 0;
			TError.ErrorType result = this.readInt16(ref num, pos);
			dest = (ushort)num;
			return result;
		}

		public TError.ErrorType readInt32(ref int dest, int pos)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 4 > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt32(this.beginPtr, pos);
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt32(ref uint dest, int pos)
		{
			int num = 0;
			TError.ErrorType result = this.readInt32(ref num, pos);
			dest = (uint)num;
			return result;
		}

		public TError.ErrorType readInt64(ref long dest, int pos)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 8 > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
				}
				else
				{
					dest = BitConverter.ToInt64(this.beginPtr, pos);
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType readUInt64(ref ulong dest, int pos)
		{
			long num = 0L;
			TError.ErrorType result = this.readInt64(ref num, pos);
			dest = (ulong)num;
			return result;
		}

		public TError.ErrorType readFloat(ref float dest, int pos)
		{
			int value = 0;
			TError.ErrorType result = this.readInt32(ref value, pos);
			dest = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
			return result;
		}

		public TError.ErrorType readDouble(ref double dest, int pos)
		{
			long value = 0L;
			TError.ErrorType result = this.readInt64(ref value, pos);
			dest = BitConverter.Int64BitsToDouble(value);
			return result;
		}

		public TError.ErrorType readCString(ref byte[] dest, int count, int pos)
		{
			bool flag = this.beginPtr == null || count > dest.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = dest == null || dest.GetLength(0) == 0;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
				}
				else
				{
					bool flag3 = count > this.length - pos;
					if (flag3)
					{
						result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							dest[i] = this.beginPtr[pos + count];
						}
						result = TError.ErrorType.TDR_NO_ERROR;
					}
				}
			}
			return result;
		}

		public TError.ErrorType readWString(ref short[] dest, int count, int pos)
		{
			bool flag = this.beginPtr == null || count > dest.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = dest == null || dest.GetLength(0) == 0;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
				}
				else
				{
					bool flag3 = 2 * count > this.length - pos;
					if (flag3)
					{
						result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							dest[i] = BitConverter.ToInt16(this.beginPtr, pos + 2 * count);
						}
						result = TError.ErrorType.TDR_NO_ERROR;
					}
				}
			}
			return result;
		}

		public TError.ErrorType toHexStr(ref char[] buffer, out int usedsize)
		{
			TError.ErrorType errorType = TError.ErrorType.TDR_NO_ERROR;
			int num = this.length - this.position;
			int num2 = num * 2 + 1;
			bool flag = buffer.GetLength(0) < num2;
			TError.ErrorType result;
			if (flag)
			{
				usedsize = 0;
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			else
			{
				string text = string.Empty;
				byte[] array = new byte[this.length - this.position];
				for (int i = 0; i < this.length - this.position; i++)
				{
					errorType = this.readUInt8(ref array[i], this.position + i);
					bool flag2 = errorType > TError.ErrorType.TDR_NO_ERROR;
					if (flag2)
					{
						usedsize = 0;
						result = errorType;
						return result;
					}
					text += string.Format("{0:x2}", array[i]);
				}
				text += string.Format("{0:x}", 0);
				buffer = text.ToCharArray();
				usedsize = num2;
				result = errorType;
			}
			return result;
		}

		public TError.ErrorType toHexStr(ref string buffer)
		{
			TError.ErrorType errorType = TError.ErrorType.TDR_NO_ERROR;
			byte[] array = new byte[this.length - this.position];
			TError.ErrorType result;
			for (int i = 0; i < this.length - this.position; i++)
			{
				errorType = this.readUInt8(ref array[i], this.position + i);
				bool flag = errorType > TError.ErrorType.TDR_NO_ERROR;
				if (flag)
				{
					result = errorType;
					return result;
				}
				buffer += string.Format("{0:x2}", array[i]);
			}
			buffer += string.Format("{0:x}", 0);
			result = errorType;
			return result;
		}
	}
}

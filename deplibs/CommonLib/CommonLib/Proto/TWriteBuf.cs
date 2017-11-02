using System;
using System.Net;

namespace Proto
{
	public class TWriteBuf : TBuf
	{
		public byte[] beginPtr;

		public int position;

		public int length;

		public byte[] _shortBytes = new byte[2];

		public byte[] _intBytes = new byte[4];

		public byte[] _longBytes = new byte[8];

		public bool IsNetEndian;

		public TWriteBuf()
		{
			this.beginPtr = null;
			this.position = 0;
			this.length = 0;
			this.IsNetEndian = true;
		}

		public TWriteBuf(ref byte[] ptr, int len)
		{
			this._set(ref ptr, len);
		}

		public TWriteBuf(int len)
		{
			this.beginPtr = new byte[len];
			this.position = 0;
			this.length = 0;
			bool flag = this.beginPtr != null;
			if (flag)
			{
				this.length = len;
			}
		}

		public override void OnRelease()
		{
			this._reset();
		}

		private void _set(ref byte[] ptr, int len)
		{
			this.beginPtr = ptr;
			this.position = 0;
			this.length = 0;
			bool flag = this.beginPtr != null;
			if (flag)
			{
				this.length = len;
			}
		}

		private void _reset()
		{
			this.position = 0;
			this.length = 0;
			this.beginPtr = null;
		}

		public void resetPos()
		{
			this.position = 0;
		}

		public void reset()
		{
			this._reset();
		}

		public void set(ref byte[] ptr, int len)
		{
			this._set(ref ptr, len);
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

		public byte[] getBeginPtr()
		{
			return this.beginPtr;
		}

		public TError.ErrorType reserve(int gap)
		{
			bool flag = this.position > this.length;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			else
			{
				bool flag2 = gap > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					this.position += gap;
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeInt8(sbyte src)
		{
			return this.writeUInt8((byte)src);
		}

		public TError.ErrorType writeUInt8(byte src)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					byte[] arg_45_0 = this.beginPtr;
					int num = this.position;
					this.position = num + 1;
					arg_45_0[num] = src;
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeUInt16(ushort src)
		{
			return this.writeInt16((short)src);
		}

		public TError.ErrorType writeInt16(short src)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						byte[] arg_7C_0 = this.beginPtr;
						int num = this.position;
						this.position = num + 1;
						arg_7C_0[num] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		private byte[] GetUnsafeBytes(short src)
		{
            //fixed (byte* ptr = (this._shortBytes != null && this._shortBytes.Length != 0) ? this._shortBytes : null)
            //{
            //	*(short*)ptr = src;
            //}
            //return this._shortBytes;
            return null;
		}

		public TError.ErrorType writeUInt32(uint src)
		{
			return this.writeInt32((int)src);
		}

		public TError.ErrorType writeInt32(int src)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						byte[] arg_7C_0 = this.beginPtr;
						int num = this.position;
						this.position = num + 1;
						arg_7C_0[num] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		private byte[] GetUnsafeBytes(int src)
		{
            //fixed (byte* ptr = (this._intBytes != null && this._intBytes.Length != 0) ? this._intBytes : null)
            //{
            //	*(int*)ptr = src;
            //}
            //return this._intBytes;
            return null;
		}

		public TError.ErrorType writeUInt64(ulong src)
		{
			return this.writeInt64((long)src);
		}

		public TError.ErrorType writeInt64(long src)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						byte[] arg_7C_0 = this.beginPtr;
						int num = this.position;
						this.position = num + 1;
						arg_7C_0[num] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		private byte[] GetUnsafeBytes(long src)
		{
            //fixed (byte* ptr = (this._longBytes != null && this._longBytes.Length != 0) ? this._longBytes : null)
            //{
            //	*(long*)ptr = src;
            //}
            //return this._longBytes;
            return null;
		}

		public TError.ErrorType writeFloat(float src)
		{
			int src2 = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
			return this.writeInt32(src2);
		}

		public TError.ErrorType writeDouble(double src)
		{
			long src2 = BitConverter.DoubleToInt64Bits(src);
			return this.writeInt64(src2);
		}

		public TError.ErrorType writeVarUInt16(ushort src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = (ushort)IPAddress.HostToNetworkOrder((short)src);
				}
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(src);
					byte b = bytes[0];
					src = (ushort)(src >> 7);
					bool flag3 = src > 0;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_82_0 = this.beginPtr;
					int num = this.position;
					this.position = num + 1;
					arg_82_0[num] = b;
					bool flag4 = src == 0;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeVarInt16(short src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = IPAddress.HostToNetworkOrder(src);
				}
				src = (short)((int)src << 1 ^ src >> 15);
				ushort num = (ushort)src;
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(num);
					byte b = bytes[0];
					num = (ushort)(num >> 7);
					bool flag3 = num > 0;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_8F_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					arg_8F_0[num2] = b;
					bool flag4 = num == 0;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeVarUInt32(uint src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = (uint)IPAddress.HostToNetworkOrder((int)src);
				}
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(src);
					byte b = bytes[0];
					src >>= 7;
					bool flag3 = src > 0u;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_7F_0 = this.beginPtr;
					int num = this.position;
					this.position = num + 1;
					arg_7F_0[num] = b;
					bool flag4 = src == 0u;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeVarInt32(int src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = IPAddress.HostToNetworkOrder(src);
				}
				src = (src << 1 ^ src >> 31);
				uint num = (uint)src;
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(num);
					byte b = bytes[0];
					num >>= 7;
					bool flag3 = num > 0u;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_8C_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					arg_8C_0[num2] = b;
					bool flag4 = num == 0u;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeVarUInt64(ulong src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = (ulong)IPAddress.HostToNetworkOrder((long)src);
				}
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(src);
					byte b = bytes[0];
					src >>= 7;
					bool flag3 = src > 0uL;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_80_0 = this.beginPtr;
					int num = this.position;
					this.position = num + 1;
					arg_80_0[num] = b;
					bool flag4 = src == 0uL;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeVarInt64(long src)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = !this.IsNetEndian || !BitConverter.IsLittleEndian;
				if (flag2)
				{
					src = IPAddress.HostToNetworkOrder(src);
				}
				src = (src << 1 ^ src >> 63);
				ulong num = (ulong)src;
				while (this.length - this.position > 0)
				{
					byte[] bytes = BitConverter.GetBytes(num);
					byte b = bytes[0];
					num >>= 7;
					bool flag3 = num > 0uL;
					if (flag3)
					{
						b |= 128;
					}
					byte[] arg_8D_0 = this.beginPtr;
					int num2 = this.position;
					this.position = num2 + 1;
					arg_8D_0[num2] = b;
					bool flag4 = num == 0uL;
					if (flag4)
					{
						result = TError.ErrorType.TDR_NO_ERROR;
						return result;
					}
				}
				result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			return result;
		}

		public TError.ErrorType writeCString(byte[] src, int count)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = count > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						byte[] arg_4F_0 = this.beginPtr;
						int num = this.position;
						this.position = num + 1;
						arg_4F_0[num] = src[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeWString(short[] src, int count)
		{
			bool flag = this.beginPtr == null;
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 2 * count > this.length - this.position;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						byte[] bytes = BitConverter.GetBytes(src[i]);
						for (int j = 0; j < bytes.GetLength(0); j++)
						{
							byte[] arg_63_0 = this.beginPtr;
							int num = this.position;
							this.position = num + 1;
							arg_63_0[num] = bytes[j];
						}
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeInt8(sbyte src, int pos)
		{
			return this.writeUInt8((byte)src, pos);
		}

		public TError.ErrorType writeUInt8(byte src, int pos)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					this.beginPtr[pos] = src;
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeUInt16(ushort src, int pos)
		{
			return this.writeInt16((short)src, pos);
		}

		public TError.ErrorType writeInt16(short src, int pos)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						this.beginPtr[pos + i] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeUInt32(uint src, int pos)
		{
			return this.writeInt32((int)src, pos);
		}

		public TError.ErrorType writeInt32(int src, int pos)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						this.beginPtr[pos + i] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeUInt64(ulong src, int pos)
		{
			return this.writeInt64((long)src, pos);
		}

		public TError.ErrorType writeInt64(long src, int pos)
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
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					bool flag3 = this.IsNetEndian && BitConverter.IsLittleEndian;
					if (flag3)
					{
						src = IPAddress.HostToNetworkOrder(src);
					}
					byte[] unsafeBytes = this.GetUnsafeBytes(src);
					for (int i = 0; i < unsafeBytes.GetLength(0); i++)
					{
						this.beginPtr[pos + i] = unsafeBytes[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeFloat(float src, int pos)
		{
			int src2 = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
			return this.writeInt32(src2, pos);
		}

		public TError.ErrorType writeDouble(double src, int pos)
		{
			long src2 = BitConverter.DoubleToInt64Bits(src);
			return this.writeInt64(src2, pos);
		}

		public TError.ErrorType writeCString(byte[] src, int count, int pos)
		{
			bool flag = this.beginPtr == null || count > src.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = count > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						this.beginPtr[pos + i] = src[i];
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}

		public TError.ErrorType writeWString(short[] src, int count, int pos)
		{
			bool flag = this.beginPtr == null || count > src.GetLength(0);
			TError.ErrorType result;
			if (flag)
			{
				result = TError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			else
			{
				bool flag2 = 2 * count > this.length - pos;
				if (flag2)
				{
					result = TError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						byte[] bytes = BitConverter.GetBytes(src[i]);
						for (int j = 0; j < bytes.GetLength(0); j++)
						{
							this.beginPtr[pos + (2 * i + j)] = bytes[j];
						}
					}
					result = TError.ErrorType.TDR_NO_ERROR;
				}
			}
			return result;
		}
	}
}

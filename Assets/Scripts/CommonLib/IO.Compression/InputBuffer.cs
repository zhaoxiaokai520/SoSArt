using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class InputBuffer
	{
		private byte[] buffer;

		private int start;

		private int end;

		private uint bitBuffer = 0u;

		private int bitsInBuffer = 0;

		public int AvailableBits
		{
			get
			{
				return this.bitsInBuffer;
			}
		}

		public int AvailableBytes
		{
			get
			{
				return this.end - this.start + this.bitsInBuffer / 8;
			}
		}

		public bool EnsureBitsAvailable(int count)
		{
			Debug.Assert(0 < count && count <= 16, "count is invalid.");
			bool flag = this.bitsInBuffer < count;
			bool result;
			if (flag)
			{
				bool flag2 = this.NeedsInput();
				if (flag2)
				{
					result = false;
					return result;
				}
				uint arg_67_0 = this.bitBuffer;
				byte[] arg_5C_0 = this.buffer;
				int num = this.start;
				this.start = num + 1;
				this.bitBuffer = System.Convert.ToUInt32((arg_67_0 | arg_5C_0[num] << (this.bitsInBuffer & 31)));
				this.bitsInBuffer += 8;
				bool flag3 = this.bitsInBuffer < count;
				if (flag3)
				{
					bool flag4 = this.NeedsInput();
					if (flag4)
					{
						result = false;
						return result;
					}
					uint arg_C5_0 = this.bitBuffer;
					byte[] arg_BA_0 = this.buffer;
					num = this.start;
					this.start = num + 1;
					this.bitBuffer = System.Convert.ToUInt32((arg_C5_0 | arg_BA_0[num] << (this.bitsInBuffer & 31)));
					this.bitsInBuffer += 8;
				}
			}
			result = true;
			return result;
		}

		public uint TryLoad16Bits()
		{
			bool flag = this.bitsInBuffer < 8;
			if (flag)
			{
				bool flag2 = this.start < this.end;
				if (flag2)
				{
					uint arg_4E_0 = this.bitBuffer;
					byte[] arg_43_0 = this.buffer;
					int num = this.start;
					this.start = num + 1;
					this.bitBuffer = System.Convert.ToUInt32((arg_4E_0 | arg_43_0[num] << (this.bitsInBuffer & 31)));
					this.bitsInBuffer += 8;
				}
				bool flag3 = this.start < this.end;
				if (flag3)
				{
					uint arg_9F_0 = this.bitBuffer;
					byte[] arg_94_0 = this.buffer;
					int num = this.start;
					this.start = num + 1;
					this.bitBuffer = System.Convert.ToUInt32((arg_9F_0 | arg_94_0[num] << (this.bitsInBuffer & 31)));
					this.bitsInBuffer += 8;
				}
			}
			else
			{
				bool flag4 = this.bitsInBuffer < 16;
				if (flag4)
				{
					bool flag5 = this.start < this.end;
					if (flag5)
					{
						uint arg_106_0 = this.bitBuffer;
						byte[] arg_FB_0 = this.buffer;
						int num = this.start;
						this.start = num + 1;
						this.bitBuffer = System.Convert.ToUInt32((arg_106_0 | arg_FB_0[num] << (this.bitsInBuffer & 31)));
						this.bitsInBuffer += 8;
					}
				}
			}
			return this.bitBuffer;
		}

		private uint GetBitMask(int count)
		{
			return (1u << count) - 1u;
		}

		public int GetBits(int count)
		{
			Debug.Assert(0 < count && count <= 16, "count is invalid.");
			bool flag = !this.EnsureBitsAvailable(count);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int num = (int)(this.bitBuffer & this.GetBitMask(count));
				this.bitBuffer >>= count;
				this.bitsInBuffer -= count;
				result = num;
			}
			return result;
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			Debug.Assert(output != null, "");
			Debug.Assert(offset >= 0, "");
			Debug.Assert(length >= 0, "");
			Debug.Assert(offset <= output.Length - length, "");
			Debug.Assert(this.bitsInBuffer % 8 == 0, "");
			int num = 0;
			while (this.bitsInBuffer > 0 && length > 0)
			{
				output[offset++] = (byte)this.bitBuffer;
				this.bitBuffer >>= 8;
				this.bitsInBuffer -= 8;
				length--;
				num++;
			}
			bool flag = length == 0;
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				int num2 = this.end - this.start;
				bool flag2 = length > num2;
				if (flag2)
				{
					length = num2;
				}
				Array.Copy(this.buffer, this.start, output, offset, length);
				this.start += length;
				result = num + length;
			}
			return result;
		}

		public bool NeedsInput()
		{
			return this.start == this.end;
		}

		public void SetInput(byte[] buffer, int offset, int length)
		{
			Debug.Assert(buffer != null, "");
			Debug.Assert(offset >= 0, "");
			Debug.Assert(length >= 0, "");
			Debug.Assert(offset <= buffer.Length - length, "");
			Debug.Assert(this.start == this.end, "");
			this.buffer = buffer;
			this.start = offset;
			this.end = offset + length;
		}

		public void SkipBits(int n)
		{
			Debug.Assert(this.bitsInBuffer >= n, "No enough bits in the buffer, Did you call EnsureBitsAvailable?");
			this.bitBuffer >>= n;
			this.bitsInBuffer -= n;
		}

		public void SkipToByteBoundary()
		{
			this.bitBuffer >>= this.bitsInBuffer % 8;
			this.bitsInBuffer -= this.bitsInBuffer % 8;
		}
	}
}

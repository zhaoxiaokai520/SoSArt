using System;

namespace Unity.IO.Compression
{
	internal class OutputWindow
	{
		private const int WindowSize = 32768;

		private const int WindowMask = 32767;

		private byte[] window = new byte[32768];

		private int end;

		private int bytesUsed;

		public int FreeBytes
		{
			get
			{
				return 32768 - this.bytesUsed;
			}
		}

		public int AvailableBytes
		{
			get
			{
				return this.bytesUsed;
			}
		}

		public void Reset()
		{
			this.end = 0;
			this.bytesUsed = 0;
		}

		public void Write(byte b)
		{
			byte[] arg_19_0 = this.window;
			int num = this.end;
			this.end = num + 1;
			arg_19_0[num] = b;
			this.end &= 32767;
			this.bytesUsed++;
		}

		public void WriteLengthDistance(int length, int distance)
		{
			this.bytesUsed += length;
			int num = this.end - distance & 32767;
			int num2 = 32768 - length;
			bool flag = num <= num2 && this.end < num2;
			if (flag)
			{
				bool flag2 = length <= distance;
				if (flag2)
				{
					Array.Copy(this.window, num, this.window, this.end, length);
					this.end += length;
				}
				else
				{
					while (length-- > 0)
					{
						byte[] arg_9C_0 = this.window;
						int num3 = this.end;
						this.end = num3 + 1;
						arg_9C_0[num3] = this.window[num++];
					}
				}
			}
			else
			{
				while (length-- > 0)
				{
					byte[] arg_DB_0 = this.window;
					int num3 = this.end;
					this.end = num3 + 1;
					arg_DB_0[num3] = this.window[num++];
					this.end &= 32767;
					num &= 32767;
				}
			}
		}

		public int CopyFrom(InputBuffer input, int length)
		{
			length = Math.Min(Math.Min(length, 32768 - this.bytesUsed), input.AvailableBytes);
			int num = 32768 - this.end;
			bool flag = length > num;
			int num2;
			if (flag)
			{
				num2 = input.CopyTo(this.window, this.end, num);
				bool flag2 = num2 == num;
				if (flag2)
				{
					num2 += input.CopyTo(this.window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyTo(this.window, this.end, length);
			}
			this.end = (this.end + num2 & 32767);
			this.bytesUsed += num2;
			return num2;
		}

		public int CopyTo(byte[] output, int offset, int length)
		{
			bool flag = length > this.bytesUsed;
			int num;
			if (flag)
			{
				num = this.end;
				length = this.bytesUsed;
			}
			else
			{
				num = (this.end - this.bytesUsed + length & 32767);
			}
			int num2 = length;
			int num3 = length - num;
			bool flag2 = num3 > 0;
			if (flag2)
			{
				Array.Copy(this.window, 32768 - num3, output, offset, num3);
				offset += num3;
				length = num;
			}
			Array.Copy(this.window, num - length, output, offset, length);
			this.bytesUsed -= num2;
			return num2;
		}
	}
}

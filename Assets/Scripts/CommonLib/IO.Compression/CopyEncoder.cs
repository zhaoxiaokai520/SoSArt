using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class CopyEncoder
	{
		private const int PaddingSize = 5;

		private const int MaxUncompressedBlockSize = 65536;

		public void GetBlock(DeflateInput input, OutputBuffer output, bool isFinal)
		{
			Debug.Assert(output != null);
			Debug.Assert(output.FreeBytes >= 5);
			int num = 0;
			bool flag = input != null;
			if (flag)
			{
				num = Math.Min(input.Count, output.FreeBytes - 5 - output.BitsInBuffer);
				bool flag2 = num > 65531;
				if (flag2)
				{
					num = 65531;
				}
			}
			if (isFinal)
			{
				output.WriteBits(3, 1u);
			}
			else
			{
				output.WriteBits(3, 0u);
			}
			output.FlushBits();
			this.WriteLenNLen((ushort)num, output);
			bool flag3 = input != null && num > 0;
			if (flag3)
			{
				output.WriteBytes(input.Buffer, input.StartIndex, num);
				input.ConsumeBytes(num);
			}
		}

		private void WriteLenNLen(ushort len, OutputBuffer output)
		{
			output.WriteUInt16(len);
			ushort value = System.Convert.ToUInt16(~len);
			output.WriteUInt16(value);
		}
	}
}

using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class FastEncoder
	{
		private FastEncoderWindow inputWindow;

		private Match currentMatch;

		private double lastCompressionRatio;

		internal int BytesInHistory
		{
			get
			{
				return this.inputWindow.BytesAvailable;
			}
		}

		internal DeflateInput UnprocessedInput
		{
			get
			{
				return this.inputWindow.UnprocessedInput;
			}
		}

		internal double LastCompressionRatio
		{
			get
			{
				return this.lastCompressionRatio;
			}
		}

		public FastEncoder()
		{
			this.inputWindow = new FastEncoderWindow();
			this.currentMatch = new Match();
		}

		internal void FlushInput()
		{
			this.inputWindow.FlushWindow();
		}

		internal void GetBlock(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
			Debug.Assert(this.InputAvailable(input), "call SetInput before trying to compress!");
			FastEncoder.WriteDeflatePreamble(output);
			this.GetCompressedOutput(input, output, maxBytesToCopy);
			this.WriteEndOfBlock(output);
		}

		internal void GetCompressedData(DeflateInput input, OutputBuffer output)
		{
			this.GetCompressedOutput(input, output, -1);
		}

		internal void GetBlockHeader(OutputBuffer output)
		{
			FastEncoder.WriteDeflatePreamble(output);
		}

		internal void GetBlockFooter(OutputBuffer output)
		{
			this.WriteEndOfBlock(output);
		}

		private void GetCompressedOutput(DeflateInput input, OutputBuffer output, int maxBytesToCopy)
		{
			int bytesWritten = output.BytesWritten;
			int num = 0;
			int num2 = this.BytesInHistory + input.Count;
			do
			{
				int num3 = (input.Count < this.inputWindow.FreeWindowSpace) ? input.Count : this.inputWindow.FreeWindowSpace;
				bool flag = maxBytesToCopy >= 1;
				if (flag)
				{
					num3 = Math.Min(num3, maxBytesToCopy - num);
				}
				bool flag2 = num3 > 0;
				if (flag2)
				{
					this.inputWindow.CopyBytes(input.Buffer, input.StartIndex, num3);
					input.ConsumeBytes(num3);
					num += num3;
				}
				this.GetCompressedOutput(output);
			}
			while (this.SafeToWriteTo(output) && this.InputAvailable(input) && (maxBytesToCopy < 1 || num < maxBytesToCopy));
			int bytesWritten2 = output.BytesWritten;
			int num4 = bytesWritten2 - bytesWritten;
			int num5 = this.BytesInHistory + input.Count;
			int num6 = num2 - num5;
			bool flag3 = num4 != 0;
			if (flag3)
			{
				this.lastCompressionRatio = (double)num4 / (double)num6;
			}
		}

		private void GetCompressedOutput(OutputBuffer output)
		{
			while (this.inputWindow.BytesAvailable > 0 && this.SafeToWriteTo(output))
			{
				this.inputWindow.GetNextSymbolOrMatch(this.currentMatch);
				bool flag = this.currentMatch.State == MatchState.HasSymbol;
				if (flag)
				{
					FastEncoder.WriteChar(this.currentMatch.Symbol, output);
				}
				else
				{
					bool flag2 = this.currentMatch.State == MatchState.HasMatch;
					if (flag2)
					{
						FastEncoder.WriteMatch(this.currentMatch.Length, this.currentMatch.Position, output);
					}
					else
					{
						FastEncoder.WriteChar(this.currentMatch.Symbol, output);
						FastEncoder.WriteMatch(this.currentMatch.Length, this.currentMatch.Position, output);
					}
				}
			}
		}

		private bool InputAvailable(DeflateInput input)
		{
			return input.Count > 0 || this.BytesInHistory > 0;
		}

		private bool SafeToWriteTo(OutputBuffer output)
		{
			return output.FreeBytes > 16;
		}

		private void WriteEndOfBlock(OutputBuffer output)
		{
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[256];
			int n = (int)(num & 31u);
			output.WriteBits(n, num >> 5);
		}

		internal static void WriteMatch(int matchLen, int matchPos, OutputBuffer output)
		{
			Debug.Assert(matchLen >= 3 && matchLen <= 258, "Illegal currentMatch length!");
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[254 + matchLen];
			int num2 = (int)(num & 31u);
			Debug.Assert(num2 != 0, "Invalid Match Length!");
			bool flag = num2 <= 16;
			if (flag)
			{
				output.WriteBits(num2, num >> 5);
			}
			else
			{
				output.WriteBits(16, num >> 5 & 65535u);
				output.WriteBits(num2 - 16, num >> 21);
			}
			num = FastEncoderStatics.FastEncoderDistanceCodeInfo[FastEncoderStatics.GetSlot(matchPos)];
			output.WriteBits((int)(num & 15u), num >> 8);
			int num3 = (int)(num >> 4 & 15u);
			bool flag2 = num3 != 0;
			if (flag2)
			{
				output.WriteBits(num3, (uint)(matchPos & (int)FastEncoderStatics.BitMask[num3]));
			}
		}

		internal static void WriteChar(byte b, OutputBuffer output)
		{
			uint num = FastEncoderStatics.FastEncoderLiteralCodeInfo[(int)b];
			output.WriteBits((int)(num & 31u), num >> 5);
		}

		internal static void WriteDeflatePreamble(OutputBuffer output)
		{
			output.WriteBytes(FastEncoderStatics.FastEncoderTreeStructureData, 0, FastEncoderStatics.FastEncoderTreeStructureData.Length);
			output.WriteBits(9, 34u);
		}
	}
}

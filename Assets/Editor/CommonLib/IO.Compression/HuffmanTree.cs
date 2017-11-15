using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class HuffmanTree
	{
		internal const int MaxLiteralTreeElements = 288;

		internal const int MaxDistTreeElements = 32;

		internal const int EndOfBlockCode = 256;

		internal const int NumberOfCodeLengthTreeElements = 19;

		private int tableBits;

		private short[] table;

		private short[] left;

		private short[] right;

		private byte[] codeLengthArray;

		private uint[] codeArrayDebug;

		private int tableMask;

		private static HuffmanTree staticLiteralLengthTree;

		private static HuffmanTree staticDistanceTree;

		public static HuffmanTree StaticLiteralLengthTree
		{
			get
			{
				return HuffmanTree.staticLiteralLengthTree;
			}
		}

		public static HuffmanTree StaticDistanceTree
		{
			get
			{
				return HuffmanTree.staticDistanceTree;
			}
		}

		static HuffmanTree()
		{
			HuffmanTree.staticLiteralLengthTree = new HuffmanTree(HuffmanTree.GetStaticLiteralTreeLength());
			HuffmanTree.staticDistanceTree = new HuffmanTree(HuffmanTree.GetStaticDistanceTreeLength());
		}

		public HuffmanTree(byte[] codeLengths)
		{
			Debug.Assert(codeLengths.Length == 288 || codeLengths.Length == 32 || codeLengths.Length == 19, "we only expect three kinds of Length here");
			this.codeLengthArray = codeLengths;
			bool flag = this.codeLengthArray.Length == 288;
			if (flag)
			{
				this.tableBits = 9;
			}
			else
			{
				this.tableBits = 7;
			}
			this.tableMask = (1 << this.tableBits) - 1;
			this.CreateTable();
		}

		private static byte[] GetStaticLiteralTreeLength()
		{
			byte[] array = new byte[288];
			for (int i = 0; i <= 143; i++)
			{
				array[i] = 8;
			}
			for (int j = 144; j <= 255; j++)
			{
				array[j] = 9;
			}
			for (int k = 256; k <= 279; k++)
			{
				array[k] = 7;
			}
			for (int l = 280; l <= 287; l++)
			{
				array[l] = 8;
			}
			return array;
		}

		private static byte[] GetStaticDistanceTreeLength()
		{
			byte[] array = new byte[32];
			for (int i = 0; i < 32; i++)
			{
				array[i] = 5;
			}
			return array;
		}

		private uint[] CalculateHuffmanCode()
		{
			uint[] array = new uint[17];
			byte[] array2 = this.codeLengthArray;
			for (int i = 0; i < array2.Length; i++)
			{
				int num = (int)array2[i];
				array[num] += 1u;
			}
			array[0] = 0u;
			uint[] array3 = new uint[17];
			uint num2 = 0u;
			for (int j = 1; j <= 16; j++)
			{
				num2 = num2 + array[j - 1] << 1;
				array3[j] = num2;
			}
			uint[] array4 = new uint[288];
			for (int k = 0; k < this.codeLengthArray.Length; k++)
			{
				int num3 = (int)this.codeLengthArray[k];
				bool flag = num3 > 0;
				if (flag)
				{
					array4[k] = FastEncoderStatics.BitReverse(array3[num3], num3);
					array3[num3] += 1u;
				}
			}
			return array4;
		}

		private void CreateTable()
		{
			uint[] array = this.CalculateHuffmanCode();
			this.table = new short[1 << this.tableBits];
			this.codeArrayDebug = array;
			this.left = new short[2 * this.codeLengthArray.Length];
			this.right = new short[2 * this.codeLengthArray.Length];
			short num = (short)this.codeLengthArray.Length;
			for (int i = 0; i < this.codeLengthArray.Length; i++)
			{
				int num2 = (int)this.codeLengthArray[i];
				bool flag = num2 > 0;
				if (flag)
				{
					int num3 = (int)array[i];
					bool flag2 = num2 <= this.tableBits;
					if (!flag2)
					{
						int num4 = num2 - this.tableBits;
						int num5 = 1 << this.tableBits;
						int num6 = num3 & (1 << this.tableBits) - 1;
						short[] array2 = this.table;
						do
						{
							short num7 = array2[num6];
							bool flag3 = num7 == 0;
							if (flag3)
							{
								array2[num6] = System.Convert.ToInt16(- num);
								num7 = System.Convert.ToInt16(- num);
								num += 1;
							}
							bool flag4 = num7 > 0;
							if (flag4)
							{
								goto Block_6;
							}
							Debug.Assert(num7 < 0, "CreateTable: Only negative numbers are used for tree pointers!");
							bool flag5 = (num3 & num5) == 0;
							if (flag5)
							{
								array2 = this.left;
							}
							else
							{
								array2 = this.right;
							}
							num6 = (int)(-(int)num7);
							num5 <<= 1;
							num4--;
						}
						while (num4 != 0);
						array2[num6] = (short)i;
						goto IL_1C9;
						Block_6:
						throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
					}
					int num8 = 1 << num2;
					bool flag6 = num3 >= num8;
					if (flag6)
					{
						throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
					}
					int num9 = 1 << this.tableBits - num2;
					for (int j = 0; j < num9; j++)
					{
						this.table[num3] = (short)i;
						num3 += num8;
					}
					IL_1C9:;
				}
			}
		}

		public int GetNextSymbol(InputBuffer input)
		{
			uint num = input.TryLoad16Bits();
			bool flag = input.AvailableBits == 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int num2 = (int)this.table[(int)(checked((IntPtr)(unchecked((ulong)num & (ulong)((long)this.tableMask)))))];
				bool flag2 = num2 < 0;
				if (flag2)
				{
					uint num3 = 1u << this.tableBits;
					do
					{
						num2 = -num2;
						bool flag3 = (num & num3) == 0u;
						if (flag3)
						{
							num2 = (int)this.left[num2];
						}
						else
						{
							num2 = (int)this.right[num2];
						}
						num3 <<= 1;
					}
					while (num2 < 0);
				}
				int num4 = (int)this.codeLengthArray[num2];
				bool flag4 = num4 <= 0;
				if (flag4)
				{
					throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
				}
				bool flag5 = num4 > input.AvailableBits;
				if (flag5)
				{
					result = -1;
				}
				else
				{
					input.SkipBits(num4);
					result = num2;
				}
			}
			return result;
		}
	}
}

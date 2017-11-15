using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class Inflater
	{
		private class PooledInflater
		{
			private OutputWindow _window = null;

			public byte[] codeList = new byte[320];

			public byte[] codeLengthTreeCodeLength = new byte[19];

			public OutputWindow window
			{
				get
				{
					bool flag = this._window == null;
					if (flag)
					{
						this._window = new OutputWindow();
					}
					return this._window;
				}
			}

			public void Reset()
			{
				this.window.Reset();
			}
		}

		private static readonly byte[] extraLengthBits = new byte[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			0
		};

		private static readonly int[] lengthBase = new int[]
		{
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			13,
			15,
			17,
			19,
			23,
			27,
			31,
			35,
			43,
			51,
			59,
			67,
			83,
			99,
			115,
			131,
			163,
			195,
			227,
			258
		};

		private static readonly int[] distanceBasePosition = new int[]
		{
			1,
			2,
			3,
			4,
			5,
			7,
			9,
			13,
			17,
			25,
			33,
			49,
			65,
			97,
			129,
			193,
			257,
			385,
			513,
			769,
			1025,
			1537,
			2049,
			3073,
			4097,
			6145,
			8193,
			12289,
			16385,
			24577,
			0,
			0
		};

		private static readonly byte[] codeOrder = new byte[]
		{
			16,
			17,
			18,
			0,
			8,
			7,
			9,
			6,
			10,
			5,
			11,
			4,
			12,
			3,
			13,
			2,
			14,
			1,
			15
		};

		private static readonly byte[] staticDistanceTreeTable = new byte[]
		{
			0,
			16,
			8,
			24,
			4,
			20,
			12,
			28,
			2,
			18,
			10,
			26,
			6,
			22,
			14,
			30,
			1,
			17,
			9,
			25,
			5,
			21,
			13,
			29,
			3,
			19,
			11,
			27,
			7,
			23,
			15,
			31
		};

		private OutputWindow output;

		private InputBuffer input;

		private HuffmanTree literalLengthTree;

		private HuffmanTree distanceTree;

		private InflaterState state;

		private bool hasFormatReader;

		private int bfinal;

		private BlockType blockType;

		private byte[] blockLengthBuffer = new byte[4];

		private int blockLength;

		private int length;

		private int distanceCode;

		private int extraBits;

		private int loopCounter;

		private int literalLengthCodeCount;

		private int distanceCodeCount;

		private int codeLengthCodeCount;

		private int codeArraySize;

		private int lengthCode;

		private byte[] codeList;

		private byte[] codeLengthTreeCodeLength;

		private HuffmanTree codeLengthTree;

		private IFileFormatReader formatReader;

		private const int kMaxInflaterPoolSize = 4;

		private static int s_fetchIndex = 0;

		private static Inflater.PooledInflater[] s_inflaterPool = new Inflater.PooledInflater[4];

		public int AvailableOutput
		{
			get
			{
				return this.output.AvailableBytes;
			}
		}

		private static Inflater.PooledInflater FetchPooledInflater()
		{
			bool flag = Inflater.s_inflaterPool[Inflater.s_fetchIndex] == null;
			Inflater.PooledInflater pooledInflater;
			if (flag)
			{
				pooledInflater = new Inflater.PooledInflater();
				Inflater.s_inflaterPool[Inflater.s_fetchIndex] = pooledInflater;
			}
			else
			{
				pooledInflater = Inflater.s_inflaterPool[Inflater.s_fetchIndex];
			}
			Inflater.s_fetchIndex = (Inflater.s_fetchIndex + 1) % 4;
			pooledInflater.Reset();
			return pooledInflater;
		}

		public Inflater()
		{
			Inflater.PooledInflater pooledInflater = Inflater.FetchPooledInflater();
			this.output = pooledInflater.window;
			this.input = new InputBuffer();
			this.codeList = pooledInflater.codeList;
			this.codeLengthTreeCodeLength = pooledInflater.codeLengthTreeCodeLength;
			this.Reset();
		}

		internal void SetFileFormatReader(IFileFormatReader reader)
		{
			this.formatReader = reader;
			this.hasFormatReader = true;
			this.Reset();
		}

		private void Reset()
		{
			bool flag = this.hasFormatReader;
			if (flag)
			{
				this.state = InflaterState.ReadingHeader;
			}
			else
			{
				this.state = InflaterState.ReadingBFinal;
			}
		}

		public void SetInput(byte[] inputBytes, int offset, int length)
		{
			this.input.SetInput(inputBytes, offset, length);
		}

		public bool Finished()
		{
			return this.state == InflaterState.Done || this.state == InflaterState.VerifyingFooter;
		}

		public bool NeedsInput()
		{
			return this.input.NeedsInput();
		}

		public int Inflate(byte[] bytes, int offset, int length)
		{
			int num = 0;
			do
			{
				int num2 = this.output.CopyTo(bytes, offset, length);
				bool flag = num2 > 0;
				if (flag)
				{
					bool flag2 = this.hasFormatReader;
					if (flag2)
					{
						this.formatReader.UpdateWithBytesRead(bytes, offset, num2);
					}
					offset += num2;
					num += num2;
					length -= num2;
				}
				bool flag3 = length == 0;
				if (flag3)
				{
					break;
				}
			}
			while (!this.Finished() && this.Decode());
			bool flag4 = this.state == InflaterState.VerifyingFooter;
			if (flag4)
			{
				bool flag5 = this.output.AvailableBytes == 0;
				if (flag5)
				{
					this.formatReader.Validate();
				}
			}
			return num;
		}

		private bool Decode()
		{
			bool flag = false;
			bool flag2 = this.Finished();
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = this.hasFormatReader;
				if (flag3)
				{
					bool flag4 = this.state == InflaterState.ReadingHeader;
					if (flag4)
					{
						bool flag5 = !this.formatReader.ReadHeader(this.input);
						if (flag5)
						{
							result = false;
							return result;
						}
						this.state = InflaterState.ReadingBFinal;
					}
					else
					{
						bool flag6 = this.state == InflaterState.StartReadingFooter || this.state == InflaterState.ReadingFooter;
						if (flag6)
						{
							bool flag7 = !this.formatReader.ReadFooter(this.input);
							if (flag7)
							{
								result = false;
								return result;
							}
							this.state = InflaterState.VerifyingFooter;
							result = true;
							return result;
						}
					}
				}
				bool flag8 = this.state == InflaterState.ReadingBFinal;
				if (flag8)
				{
					bool flag9 = !this.input.EnsureBitsAvailable(1);
					if (flag9)
					{
						result = false;
						return result;
					}
					this.bfinal = this.input.GetBits(1);
					this.state = InflaterState.ReadingBType;
				}
				bool flag10 = this.state == InflaterState.ReadingBType;
				if (flag10)
				{
					bool flag11 = !this.input.EnsureBitsAvailable(2);
					if (flag11)
					{
						this.state = InflaterState.ReadingBType;
						result = false;
						return result;
					}
					this.blockType = (BlockType)this.input.GetBits(2);
					bool flag12 = this.blockType == BlockType.Dynamic;
					if (flag12)
					{
						this.state = InflaterState.ReadingNumLitCodes;
					}
					else
					{
						bool flag13 = this.blockType == BlockType.Static;
						if (flag13)
						{
							this.literalLengthTree = HuffmanTree.StaticLiteralLengthTree;
							this.distanceTree = HuffmanTree.StaticDistanceTree;
							this.state = InflaterState.DecodeTop;
						}
						else
						{
							bool flag14 = this.blockType == BlockType.Uncompressed;
							if (!flag14)
							{
								throw new InvalidDataException(SR.GetString("Unknown block type"));
							}
							this.state = InflaterState.UncompressedAligning;
						}
					}
				}
				bool flag15 = this.blockType == BlockType.Dynamic;
				bool flag17;
				if (flag15)
				{
					bool flag16 = this.state < InflaterState.DecodeTop;
					if (flag16)
					{
						flag17 = this.DecodeDynamicBlockHeader();
					}
					else
					{
						flag17 = this.DecodeBlock(out flag);
					}
				}
				else
				{
					bool flag18 = this.blockType == BlockType.Static;
					if (flag18)
					{
						flag17 = this.DecodeBlock(out flag);
					}
					else
					{
						bool flag19 = this.blockType == BlockType.Uncompressed;
						if (!flag19)
						{
							throw new InvalidDataException(SR.GetString("Unknown block type"));
						}
						flag17 = this.DecodeUncompressedBlock(out flag);
					}
				}
				bool flag20 = flag && this.bfinal != 0;
				if (flag20)
				{
					bool flag21 = this.hasFormatReader;
					if (flag21)
					{
						this.state = InflaterState.StartReadingFooter;
					}
					else
					{
						this.state = InflaterState.Done;
					}
				}
				result = flag17;
			}
			return result;
		}

		private bool DecodeUncompressedBlock(out bool end_of_block)
		{
			end_of_block = false;
			while (true)
			{
				switch (this.state)
				{
				case InflaterState.UncompressedAligning:
					this.input.SkipToByteBoundary();
					this.state = InflaterState.UncompressedByte1;
					goto IL_4D;
				case InflaterState.UncompressedByte1:
				case InflaterState.UncompressedByte2:
				case InflaterState.UncompressedByte3:
				case InflaterState.UncompressedByte4:
					goto IL_4D;
				case InflaterState.DecodingUncompressed:
					goto IL_FF;
				}
				break;
				IL_4D:
				int bits = this.input.GetBits(8);
				bool flag = bits < 0;
				if (flag)
				{
					goto Block_2;
				}
				this.blockLengthBuffer[this.state - InflaterState.UncompressedByte1] = (byte)bits;
				bool flag2 = this.state == InflaterState.UncompressedByte4;
				if (flag2)
				{
					this.blockLength = (int)this.blockLengthBuffer[0] + (int)this.blockLengthBuffer[1] * 256;
					int num = (int)this.blockLengthBuffer[2] + (int)this.blockLengthBuffer[3] * 256;
					bool flag3 = (ushort)this.blockLength != (ushort)(~(ushort)num);
					if (flag3)
					{
						goto Block_4;
					}
				}
				this.state++;
			}
			Debug.Assert(false, "check why we are here!");
			throw new InvalidDataException(SR.GetString("Unknown state"));
			Block_2:
			bool result = false;
			return result;
			Block_4:
			throw new InvalidDataException(SR.GetString("Invalid block length"));
			IL_FF:
			int num2 = this.output.CopyFrom(this.input, this.blockLength);
			this.blockLength -= num2;
			bool flag4 = this.blockLength == 0;
			if (flag4)
			{
				this.state = InflaterState.ReadingBFinal;
				end_of_block = true;
				result = true;
			}
			else
			{
				bool flag5 = this.output.FreeBytes == 0;
				result = flag5;
			}
			return result;
		}

		private bool DecodeBlock(out bool end_of_block_code_seen)
		{
			end_of_block_code_seen = false;
			int i = this.output.FreeBytes;
			bool result;
			while (i > 258)
			{
				switch (this.state)
				{
				case InflaterState.DecodeTop:
				{
					int num = this.literalLengthTree.GetNextSymbol(this.input);
					bool flag = num < 0;
					if (flag)
					{
						result = false;
						return result;
					}
					bool flag2 = num < 256;
					if (flag2)
					{
						this.output.Write((byte)num);
						i--;
					}
					else
					{
						bool flag3 = num == 256;
						if (flag3)
						{
							end_of_block_code_seen = true;
							this.state = InflaterState.ReadingBFinal;
							result = true;
							return result;
						}
						num -= 257;
						bool flag4 = num < 8;
						if (flag4)
						{
							num += 3;
							this.extraBits = 0;
						}
						else
						{
							bool flag5 = num == 28;
							if (flag5)
							{
								num = 258;
								this.extraBits = 0;
							}
							else
							{
								bool flag6 = num < 0 || num >= Inflater.extraLengthBits.Length;
								if (flag6)
								{
									throw new InvalidDataException(SR.GetString("Invalid data"));
								}
								this.extraBits = (int)Inflater.extraLengthBits[num];
								Debug.Assert(this.extraBits != 0, "We handle other cases seperately!");
							}
						}
						this.length = num;
						goto IL_142;
					}
					break;
				}
				case InflaterState.HaveInitialLength:
					goto IL_142;
				case InflaterState.HaveFullLength:
					goto IL_1D6;
				case InflaterState.HaveDistCode:
					goto IL_25C;
				default:
					Debug.Assert(false, "check why we are here!");
					throw new InvalidDataException(SR.GetString("Unknown state"));
				}
				continue;
				IL_25C:
				bool flag7 = this.distanceCode > 3;
				int distance;
				if (flag7)
				{
					this.extraBits = this.distanceCode - 2 >> 1;
					int bits = this.input.GetBits(this.extraBits);
					bool flag8 = bits < 0;
					if (flag8)
					{
						result = false;
						return result;
					}
					distance = Inflater.distanceBasePosition[this.distanceCode] + bits;
				}
				else
				{
					distance = this.distanceCode + 1;
				}
				Debug.Assert(i >= 258, "following operation is not safe!");
				this.output.WriteLengthDistance(this.length, distance);
				i -= this.length;
				this.state = InflaterState.DecodeTop;
				continue;
				IL_1D6:
				bool flag9 = this.blockType == BlockType.Dynamic;
				if (flag9)
				{
					this.distanceCode = this.distanceTree.GetNextSymbol(this.input);
				}
				else
				{
					this.distanceCode = this.input.GetBits(5);
					bool flag10 = this.distanceCode >= 0;
					if (flag10)
					{
						this.distanceCode = (int)Inflater.staticDistanceTreeTable[this.distanceCode];
					}
				}
				bool flag11 = this.distanceCode < 0;
				if (flag11)
				{
					result = false;
					return result;
				}
				this.state = InflaterState.HaveDistCode;
				goto IL_25C;
				IL_142:
				bool flag12 = this.extraBits > 0;
				if (flag12)
				{
					this.state = InflaterState.HaveInitialLength;
					int bits2 = this.input.GetBits(this.extraBits);
					bool flag13 = bits2 < 0;
					if (flag13)
					{
						result = false;
						return result;
					}
					bool flag14 = this.length < 0 || this.length >= Inflater.lengthBase.Length;
					if (flag14)
					{
						throw new InvalidDataException(SR.GetString("Invalid data"));
					}
					this.length = Inflater.lengthBase[this.length] + bits2;
				}
				this.state = InflaterState.HaveFullLength;
				goto IL_1D6;
			}
			result = true;
			return result;
		}

		private bool DecodeDynamicBlockHeader()
		{
			bool result;
			switch (this.state)
			{
			case InflaterState.ReadingNumLitCodes:
			{
				this.literalLengthCodeCount = this.input.GetBits(5);
				bool flag = this.literalLengthCodeCount < 0;
				if (flag)
				{
					result = false;
					return result;
				}
				this.literalLengthCodeCount += 257;
				this.state = InflaterState.ReadingNumDistCodes;
				break;
			}
			case InflaterState.ReadingNumDistCodes:
				break;
			case InflaterState.ReadingNumCodeLengthCodes:
				goto IL_B1;
			case InflaterState.ReadingCodeLengthCodes:
				goto IL_F9;
			case InflaterState.ReadingTreeCodesBefore:
			case InflaterState.ReadingTreeCodesAfter:
				goto IL_1BE;
			default:
				Debug.Assert(false, "check why we are here!");
				throw new InvalidDataException(SR.GetString("Unknown state"));
			}
			this.distanceCodeCount = this.input.GetBits(5);
			bool flag2 = this.distanceCodeCount < 0;
			if (flag2)
			{
				result = false;
				return result;
			}
			this.distanceCodeCount++;
			this.state = InflaterState.ReadingNumCodeLengthCodes;
			IL_B1:
			this.codeLengthCodeCount = this.input.GetBits(4);
			bool flag3 = this.codeLengthCodeCount < 0;
			if (flag3)
			{
				result = false;
				return result;
			}
			this.codeLengthCodeCount += 4;
			this.loopCounter = 0;
			this.state = InflaterState.ReadingCodeLengthCodes;
			IL_F9:
			while (this.loopCounter < this.codeLengthCodeCount)
			{
				int bits = this.input.GetBits(3);
				bool flag4 = bits < 0;
				if (flag4)
				{
					result = false;
					return result;
				}
				this.codeLengthTreeCodeLength[(int)Inflater.codeOrder[this.loopCounter]] = (byte)bits;
				this.loopCounter++;
			}
			for (int i = this.codeLengthCodeCount; i < Inflater.codeOrder.Length; i++)
			{
				this.codeLengthTreeCodeLength[(int)Inflater.codeOrder[i]] = 0;
			}
			this.codeLengthTree = new HuffmanTree(this.codeLengthTreeCodeLength);
			this.codeArraySize = this.literalLengthCodeCount + this.distanceCodeCount;
			this.loopCounter = 0;
			this.state = InflaterState.ReadingTreeCodesBefore;
			IL_1BE:
			while (this.loopCounter < this.codeArraySize)
			{
				bool flag5 = this.state == InflaterState.ReadingTreeCodesBefore;
				if (flag5)
				{
					bool flag6 = (this.lengthCode = this.codeLengthTree.GetNextSymbol(this.input)) < 0;
					if (flag6)
					{
						result = false;
						return result;
					}
				}
				bool flag7 = this.lengthCode <= 15;
				if (flag7)
				{
					byte[] arg_238_0 = this.codeList;
					int num = this.loopCounter;
					this.loopCounter = num + 1;
					arg_238_0[num] = (byte)this.lengthCode;
				}
				else
				{
					bool flag8 = !this.input.EnsureBitsAvailable(7);
					if (flag8)
					{
						this.state = InflaterState.ReadingTreeCodesAfter;
						result = false;
						return result;
					}
					bool flag9 = this.lengthCode == 16;
					if (flag9)
					{
						bool flag10 = this.loopCounter == 0;
						if (flag10)
						{
							throw new InvalidDataException();
						}
						byte b = this.codeList[this.loopCounter - 1];
						int num2 = this.input.GetBits(2) + 3;
						bool flag11 = this.loopCounter + num2 > this.codeArraySize;
						if (flag11)
						{
							throw new InvalidDataException();
						}
						for (int j = 0; j < num2; j++)
						{
							byte[] arg_2F1_0 = this.codeList;
							int num = this.loopCounter;
							this.loopCounter = num + 1;
							arg_2F1_0[num] = b;
						}
					}
					else
					{
						bool flag12 = this.lengthCode == 17;
						if (flag12)
						{
							int num2 = this.input.GetBits(3) + 3;
							bool flag13 = this.loopCounter + num2 > this.codeArraySize;
							if (flag13)
							{
								throw new InvalidDataException();
							}
							for (int k = 0; k < num2; k++)
							{
								byte[] arg_36B_0 = this.codeList;
								int num = this.loopCounter;
								this.loopCounter = num + 1;
								arg_36B_0[num] = 0;
							}
						}
						else
						{
							int num2 = this.input.GetBits(7) + 11;
							bool flag14 = this.loopCounter + num2 > this.codeArraySize;
							if (flag14)
							{
								throw new InvalidDataException();
							}
							for (int l = 0; l < num2; l++)
							{
								byte[] arg_3D3_0 = this.codeList;
								int num = this.loopCounter;
								this.loopCounter = num + 1;
								arg_3D3_0[num] = 0;
							}
						}
					}
				}
				this.state = InflaterState.ReadingTreeCodesBefore;
			}
			byte[] array = new byte[288];
			byte[] array2 = new byte[32];
			Array.Copy(this.codeList, array, this.literalLengthCodeCount);
			Array.Copy(this.codeList, this.literalLengthCodeCount, array2, 0, this.distanceCodeCount);
			bool flag15 = array[256] == 0;
			if (flag15)
			{
				throw new InvalidDataException();
			}
			this.literalLengthTree = new HuffmanTree(array);
			this.distanceTree = new HuffmanTree(array2);
			this.state = InflaterState.DecodeTop;
			result = true;
			return result;
		}
	}
}

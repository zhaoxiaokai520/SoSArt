using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class GZipDecoder : IFileFormatReader
	{
		internal enum GzipHeaderState
		{
			ReadingID1,
			ReadingID2,
			ReadingCM,
			ReadingFLG,
			ReadingMMTime,
			ReadingXFL,
			ReadingOS,
			ReadingXLen1,
			ReadingXLen2,
			ReadingXLenData,
			ReadingFileName,
			ReadingComment,
			ReadingCRC16Part1,
			ReadingCRC16Part2,
			Done,
			ReadingCRC,
			ReadingFileSize
		}

		[Flags]
		internal enum GZipOptionalHeaderFlags
		{
			CRCFlag = 2,
			ExtraFieldsFlag = 4,
			FileNameFlag = 8,
			CommentFlag = 16
		}

		private GZipDecoder.GzipHeaderState gzipHeaderSubstate;

		private GZipDecoder.GzipHeaderState gzipFooterSubstate;

		private int gzip_header_flag;

		private int gzip_header_xlen;

		private uint expectedCrc32;

		private uint expectedOutputStreamSizeModulo;

		private int loopCounter;

		private uint actualCrc32;

		private long actualStreamSizeModulo;

		public GZipDecoder()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingID1;
			this.gzipFooterSubstate = GZipDecoder.GzipHeaderState.ReadingCRC;
			this.expectedCrc32 = 0u;
			this.expectedOutputStreamSizeModulo = 0u;
		}

		public bool ReadHeader(InputBuffer input)
		{
			int bits;
			bool result;
			switch (this.gzipHeaderSubstate)
			{
			case GZipDecoder.GzipHeaderState.ReadingID1:
			{
				bits = input.GetBits(8);
				bool flag = bits < 0;
				if (flag)
				{
					result = false;
					return result;
				}
				bool flag2 = bits != 31;
				if (flag2)
				{
					throw new InvalidDataException(SR.GetString("Corrupted gzip header"));
				}
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingID2;
				break;
			}
			case GZipDecoder.GzipHeaderState.ReadingID2:
				break;
			case GZipDecoder.GzipHeaderState.ReadingCM:
				goto IL_DA;
			case GZipDecoder.GzipHeaderState.ReadingFLG:
				goto IL_11C;
			case GZipDecoder.GzipHeaderState.ReadingMMTime:
				goto IL_14D;
			case GZipDecoder.GzipHeaderState.ReadingXFL:
				goto IL_19A;
			case GZipDecoder.GzipHeaderState.ReadingOS:
				goto IL_1BD;
			case GZipDecoder.GzipHeaderState.ReadingXLen1:
				goto IL_1E0;
			case GZipDecoder.GzipHeaderState.ReadingXLen2:
				goto IL_221;
			case GZipDecoder.GzipHeaderState.ReadingXLenData:
				goto IL_25C;
			case GZipDecoder.GzipHeaderState.ReadingFileName:
				goto IL_2AF;
			case GZipDecoder.GzipHeaderState.ReadingComment:
				goto IL_303;
			case GZipDecoder.GzipHeaderState.ReadingCRC16Part1:
				goto IL_358;
			case GZipDecoder.GzipHeaderState.ReadingCRC16Part2:
				goto IL_395;
			case GZipDecoder.GzipHeaderState.Done:
				goto IL_3B6;
			default:
				Debug.Assert(false, "We should not reach unknown state!");
				throw new InvalidDataException(SR.GetString("Unknown state"));
			}
			bits = input.GetBits(8);
			bool flag3 = bits < 0;
			if (flag3)
			{
				result = false;
				return result;
			}
			bool flag4 = bits != 139;
			if (flag4)
			{
				throw new InvalidDataException(SR.GetString("Corrupted gzip header"));
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingCM;
			IL_DA:
			bits = input.GetBits(8);
			bool flag5 = bits < 0;
			if (flag5)
			{
				result = false;
				return result;
			}
			bool flag6 = bits != 8;
			if (flag6)
			{
				throw new InvalidDataException(SR.GetString("Unknown compression mode"));
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingFLG;
			IL_11C:
			bits = input.GetBits(8);
			bool flag7 = bits < 0;
			if (flag7)
			{
				result = false;
				return result;
			}
			this.gzip_header_flag = bits;
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingMMTime;
			this.loopCounter = 0;
			IL_14D:
			while (this.loopCounter < 4)
			{
				bits = input.GetBits(8);
				bool flag8 = bits < 0;
				if (flag8)
				{
					result = false;
					return result;
				}
				this.loopCounter++;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingXFL;
			this.loopCounter = 0;
			IL_19A:
			bits = input.GetBits(8);
			bool flag9 = bits < 0;
			if (flag9)
			{
				result = false;
				return result;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingOS;
			IL_1BD:
			bits = input.GetBits(8);
			bool flag10 = bits < 0;
			if (flag10)
			{
				result = false;
				return result;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingXLen1;
			IL_1E0:
			bool flag11 = (this.gzip_header_flag & 4) == 0;
			if (flag11)
			{
				goto IL_2AF;
			}
			bits = input.GetBits(8);
			bool flag12 = bits < 0;
			if (flag12)
			{
				result = false;
				return result;
			}
			this.gzip_header_xlen = bits;
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingXLen2;
			IL_221:
			bits = input.GetBits(8);
			bool flag13 = bits < 0;
			if (flag13)
			{
				result = false;
				return result;
			}
			this.gzip_header_xlen |= bits << 8;
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingXLenData;
			this.loopCounter = 0;
			IL_25C:
			while (this.loopCounter < this.gzip_header_xlen)
			{
				bits = input.GetBits(8);
				bool flag14 = bits < 0;
				if (flag14)
				{
					result = false;
					return result;
				}
				this.loopCounter++;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingFileName;
			this.loopCounter = 0;
			IL_2AF:
			bool flag15 = (this.gzip_header_flag & 8) == 0;
			if (flag15)
			{
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingComment;
			}
			else
			{
				while (true)
				{
					bits = input.GetBits(8);
					bool flag16 = bits < 0;
					if (flag16)
					{
						break;
					}
					bool flag17 = bits == 0;
					if (flag17)
					{
						goto Block_20;
					}
				}
				result = false;
				return result;
				Block_20:
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingComment;
			}
			IL_303:
			bool flag18 = (this.gzip_header_flag & 16) == 0;
			if (flag18)
			{
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingCRC16Part1;
			}
			else
			{
				while (true)
				{
					bits = input.GetBits(8);
					bool flag19 = bits < 0;
					if (flag19)
					{
						break;
					}
					bool flag20 = bits == 0;
					if (flag20)
					{
						goto Block_23;
					}
				}
				result = false;
				return result;
				Block_23:
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingCRC16Part1;
			}
			IL_358:
			bool flag21 = (this.gzip_header_flag & 2) == 0;
			if (flag21)
			{
				this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.Done;
				goto IL_3B6;
			}
			bits = input.GetBits(8);
			bool flag22 = bits < 0;
			if (flag22)
			{
				result = false;
				return result;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.ReadingCRC16Part2;
			IL_395:
			bits = input.GetBits(8);
			bool flag23 = bits < 0;
			if (flag23)
			{
				result = false;
				return result;
			}
			this.gzipHeaderSubstate = GZipDecoder.GzipHeaderState.Done;
			IL_3B6:
			result = true;
			return result;
		}

		public bool ReadFooter(InputBuffer input)
		{
			input.SkipToByteBoundary();
			bool flag = this.gzipFooterSubstate == GZipDecoder.GzipHeaderState.ReadingCRC;
			bool result;
			if (flag)
			{
				while (this.loopCounter < 4)
				{
					int bits = input.GetBits(8);
					bool flag2 = bits < 0;
					if (flag2)
					{
						result = false;
						return result;
					}
					this.expectedCrc32 |= (uint)((uint)bits << 8 * this.loopCounter);
					this.loopCounter++;
				}
				this.gzipFooterSubstate = GZipDecoder.GzipHeaderState.ReadingFileSize;
				this.loopCounter = 0;
			}
			bool flag3 = this.gzipFooterSubstate == GZipDecoder.GzipHeaderState.ReadingFileSize;
			if (flag3)
			{
				bool flag4 = this.loopCounter == 0;
				if (flag4)
				{
					this.expectedOutputStreamSizeModulo = 0u;
				}
				while (this.loopCounter < 4)
				{
					int bits2 = input.GetBits(8);
					bool flag5 = bits2 < 0;
					if (flag5)
					{
						result = false;
						return result;
					}
					this.expectedOutputStreamSizeModulo |= (uint)((uint)bits2 << 8 * this.loopCounter);
					this.loopCounter++;
				}
			}
			result = true;
			return result;
		}

		public void UpdateWithBytesRead(byte[] buffer, int offset, int copied)
		{
			this.actualCrc32 = Crc32Helper.UpdateCrc32(this.actualCrc32, buffer, offset, copied);
			long num = this.actualStreamSizeModulo + (long)((ulong)copied);
			bool flag = num >= 4294967296L;
			if (flag)
			{
				num %= 4294967296L;
			}
			this.actualStreamSizeModulo = num;
		}

		public void Validate()
		{
			bool flag = this.expectedCrc32 != this.actualCrc32;
			if (flag)
			{
				throw new InvalidDataException(SR.GetString("Invalid CRC"));
			}
			bool flag2 = this.actualStreamSizeModulo != (long)((ulong)this.expectedOutputStreamSizeModulo);
			if (flag2)
			{
				throw new InvalidDataException(SR.GetString("Invalid stream size"));
			}
		}
	}
}

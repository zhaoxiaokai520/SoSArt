using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal class FastEncoderWindow
	{
		private class WindowData
		{
			public byte[] window = new byte[16646];

			public ushort[] prev = new ushort[8450];

			public ushort[] lookup = new ushort[2048];
		}

		private byte[] window;

		private int bufPos;

		private int bufEnd;

		private const int FastEncoderHashShift = 4;

		private const int FastEncoderHashtableSize = 2048;

		private const int FastEncoderHashMask = 2047;

		private const int FastEncoderWindowSize = 8192;

		private const int FastEncoderWindowMask = 8191;

		private const int FastEncoderMatch3DistThreshold = 16384;

		internal const int MaxMatch = 258;

		internal const int MinMatch = 3;

		private const int SearchDepth = 32;

		private const int GoodLength = 4;

		private const int NiceLength = 32;

		private const int LazyMatchThreshold = 6;

		private ushort[] prev;

		private ushort[] lookup;

		internal const int kWindowPoolSize = 4;

		private static FastEncoderWindow.WindowData[] s_windowDataPool = new FastEncoderWindow.WindowData[4];

		private static int s_fetchIndex = 0;

		public int BytesAvailable
		{
			get
			{
				Debug.Assert(this.bufEnd - this.bufPos >= 0, "Ending pointer can't be in front of starting pointer!");
				return this.bufEnd - this.bufPos;
			}
		}

		public DeflateInput UnprocessedInput
		{
			get
			{
				return new DeflateInput
				{
					Buffer = this.window,
					StartIndex = this.bufPos,
					Count = this.bufEnd - this.bufPos
				};
			}
		}

		public int FreeWindowSpace
		{
			get
			{
				return 16384 - this.bufEnd;
			}
		}

		private static FastEncoderWindow.WindowData Fetch()
		{
			bool flag = FastEncoderWindow.s_windowDataPool[FastEncoderWindow.s_fetchIndex] == null;
			FastEncoderWindow.WindowData windowData;
			if (flag)
			{
				windowData = new FastEncoderWindow.WindowData();
				FastEncoderWindow.s_windowDataPool[FastEncoderWindow.s_fetchIndex] = windowData;
			}
			else
			{
				windowData = FastEncoderWindow.s_windowDataPool[FastEncoderWindow.s_fetchIndex];
			}
			FastEncoderWindow.s_fetchIndex = (FastEncoderWindow.s_fetchIndex + 1) % 4;
			return windowData;
		}

		public FastEncoderWindow()
		{
			this.ResetWindow();
		}

		public void FlushWindow()
		{
			this.ResetWindow();
		}

		private void ResetWindow()
		{
			FastEncoderWindow.WindowData windowData = FastEncoderWindow.Fetch();
			this.window = windowData.window;
			this.prev = windowData.prev;
			this.lookup = windowData.lookup;
			this.bufPos = 8192;
			this.bufEnd = this.bufPos;
		}

		public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
		{
			Array.Copy(inputBuffer, startIndex, this.window, this.bufEnd, count);
			this.bufEnd += count;
		}

		public void MoveWindows()
		{
			Debug.Assert(this.bufPos == 16384, "only call this at the end of the window");
			this.VerifyHashes();
			Array.Copy(this.window, this.bufPos - 8192, this.window, 0, 8192);
			for (int i = 0; i < 2048; i++)
			{
				int num = (int)(this.lookup[i] - 8192);
				bool flag = num <= 0;
				if (flag)
				{
					this.lookup[i] = 0;
				}
				else
				{
					this.lookup[i] = (ushort)num;
				}
			}
			for (int i = 0; i < 8192; i++)
			{
				long num2 = (long)((ulong)this.prev[i] - 8192uL);
				bool flag2 = num2 <= 0L;
				if (flag2)
				{
					this.prev[i] = 0;
				}
				else
				{
					this.prev[i] = (ushort)num2;
				}
			}
			Array.Clear(this.window, 8192, this.window.Length - 8192);
			this.VerifyHashes();
			this.bufPos = 8192;
			this.bufEnd = this.bufPos;
		}

		private uint HashValue(uint hash, byte b)
		{
			return hash << 4 ^ (uint)b;
		}

		private uint InsertString(ref uint hash)
		{
			hash = this.HashValue(hash, this.window[this.bufPos + 2]);
			uint num = (uint)this.lookup[(int)(hash & 2047u)];
			this.lookup[(int)(hash & 2047u)] = (ushort)this.bufPos;
			this.prev[this.bufPos & 8191] = (ushort)num;
			return num;
		}

		private void InsertStrings(ref uint hash, int matchLen)
		{
			Debug.Assert(matchLen > 0, "Invalid match Len!");
			bool flag = this.bufEnd - this.bufPos <= matchLen;
			if (flag)
			{
				this.bufPos += matchLen - 1;
			}
			else
			{
				while (--matchLen > 0)
				{
					this.InsertString(ref hash);
					this.bufPos++;
				}
			}
		}

		internal bool GetNextSymbolOrMatch(Match match)
		{
			Debug.Assert(this.bufPos >= 8192 && this.bufPos < 16384, "Invalid Buffer Position!");
			uint hash = this.HashValue(0u, this.window[this.bufPos]);
			hash = this.HashValue(hash, this.window[this.bufPos + 1]);
			int position = 0;
			this.VerifyHashes();
			bool flag = this.bufEnd - this.bufPos <= 3;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				int num2 = (int)this.InsertString(ref hash);
				bool flag2 = num2 != 0;
				if (flag2)
				{
					num = this.FindMatch(num2, out position, 32, 32);
					bool flag3 = this.bufPos + num > this.bufEnd;
					if (flag3)
					{
						num = this.bufEnd - this.bufPos;
					}
				}
				else
				{
					num = 0;
				}
			}
			bool flag4 = num < 3;
			if (flag4)
			{
				match.State = MatchState.HasSymbol;
				match.Symbol = this.window[this.bufPos];
				this.bufPos++;
			}
			else
			{
				this.bufPos++;
				bool flag5 = num <= 6;
				if (flag5)
				{
					int position2 = 0;
					int num3 = (int)this.InsertString(ref hash);
					bool flag6 = num3 != 0;
					int num4;
					if (flag6)
					{
						num4 = this.FindMatch(num3, out position2, (num < 4) ? 32 : 8, 32);
						bool flag7 = this.bufPos + num4 > this.bufEnd;
						if (flag7)
						{
							num4 = this.bufEnd - this.bufPos;
						}
					}
					else
					{
						num4 = 0;
					}
					bool flag8 = num4 > num;
					if (flag8)
					{
						match.State = MatchState.HasSymbolAndMatch;
						match.Symbol = this.window[this.bufPos - 1];
						match.Position = position2;
						match.Length = num4;
						this.bufPos++;
						num = num4;
						this.InsertStrings(ref hash, num);
					}
					else
					{
						match.State = MatchState.HasMatch;
						match.Position = position;
						match.Length = num;
						num--;
						this.bufPos++;
						this.InsertStrings(ref hash, num);
					}
				}
				else
				{
					match.State = MatchState.HasMatch;
					match.Position = position;
					match.Length = num;
					this.InsertStrings(ref hash, num);
				}
			}
			bool flag9 = this.bufPos == 16384;
			if (flag9)
			{
				this.MoveWindows();
			}
			return true;
		}

		private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
		{
			Debug.Assert(this.bufPos >= 0 && this.bufPos < 16384, "Invalid Buffer position!");
			Debug.Assert(search < this.bufPos, "Invalid starting search point!");
			Debug.Assert(this.RecalculateHash(search) == this.RecalculateHash(this.bufPos));
			int num = 0;
			int num2 = 0;
			int num3 = this.bufPos - 8192;
			Debug.Assert(num3 >= 0, "bufPos is less than FastEncoderWindowSize!");
			byte b = this.window[this.bufPos];
			while (search > num3)
			{
				Debug.Assert(this.RecalculateHash(search) == this.RecalculateHash(this.bufPos), "Corrupted hash link!");
				bool flag = this.window[search + num] == b;
				if (flag)
				{
					int i;
					for (i = 0; i < 258; i++)
					{
						bool flag2 = this.window[this.bufPos + i] != this.window[search + i];
						if (flag2)
						{
							break;
						}
					}
					bool flag3 = i > num;
					if (flag3)
					{
						num = i;
						num2 = search;
						bool flag4 = i > 32;
						if (flag4)
						{
							break;
						}
						b = this.window[this.bufPos + i];
					}
				}
				bool flag5 = --searchDepth == 0;
				if (flag5)
				{
					break;
				}
				Debug.Assert((int)this.prev[search & 8191] < search, "we should always go backwards!");
				search = (int)this.prev[search & 8191];
			}
			matchPos = this.bufPos - num2 - 1;
			bool flag6 = num == 3 && matchPos >= 16384;
			int result;
			if (flag6)
			{
				result = 0;
			}
			else
			{
				Debug.Assert(num < 3 || matchPos < 8192, "Only find match inside FastEncoderWindowSize");
				result = num;
			}
			return result;
		}

		[Conditional("DEBUG")]
		private void VerifyHashes()
		{
			for (int i = 0; i < 2048; i++)
			{
				ushort num = this.lookup[i];
				while (num != 0 && this.bufPos - (int)num < 8192)
				{
					Debug.Assert((ulong)this.RecalculateHash((int)num) == (ulong)((long)i), "Incorrect Hashcode!");
					ushort num2 = this.prev[(int)(num & 8191)];
					bool flag = this.bufPos - (int)num2 >= 8192;
					if (flag)
					{
						break;
					}
					Debug.Assert(num2 < num, "pointer is messed up!");
					num = num2;
				}
			}
		}

		private uint RecalculateHash(int position)
		{
			return (uint)(((int)this.window[position] << 8 ^ (int)this.window[position + 1] << 4 ^ (int)this.window[position + 2]) & 2047);
		}
	}
}

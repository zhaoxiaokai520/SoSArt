using System;
using System.Text;

namespace FlatBuffers
{
	public class FlatBufferBuilder
	{
		private int _space;

		private ByteBuffer _bb;

		private int _minAlign = 1;

		private int[] _vtable = new int[16];

		private int _vtableSize = -1;

		private int _objectStart;

		private int[] _vtables = new int[16];

		private int _numVtables = 0;

		private int _vectorNumElems = 0;

		public int Offset
		{
			get
			{
				return this._bb.Length - this._space;
			}
		}

		public int SizedArrayLength
		{
			get
			{
				return this._bb.Data.Length - this._bb.Position;
			}
		}

		public ByteBuffer DataBuffer
		{
			get
			{
				return this._bb;
			}
		}

		public FlatBufferBuilder(int initialSize)
		{
			bool flag = initialSize <= 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("initialSize", initialSize, "Must be greater than zero");
			}
			this._space = initialSize;
			this._bb = new ByteBuffer(new byte[initialSize]);
		}

		public void Clear()
		{
			this._space = this._bb.Length;
			this._bb.Reset();
			this._minAlign = 1;
			while (this._vtableSize > 0)
			{
				int[] arg_3F_0 = this._vtable;
				int num = this._vtableSize - 1;
				this._vtableSize = num;
				arg_3F_0[num] = 0;
			}
			this._vtableSize = -1;
			this._objectStart = 0;
			this._numVtables = 0;
			this._vectorNumElems = 0;
		}

		public void Pad(int size)
		{
			this._bb.PutByte(this._space -= size, 0, size);
		}

		private void GrowBuffer()
		{
			byte[] data = this._bb.Data;
			int num = data.Length;
			bool flag = ((long)num & (long)(System.Convert.ToUInt64(- 1073741824))) != 0L;
			if (flag)
			{
				throw new Exception("FlatBuffers: cannot grow buffer beyond 2 gigabytes.");
			}
			int num2 = num << 1;
			byte[] array = new byte[num2];
			Buffer.BlockCopy(data, 0, array, num2 - num, num);
			this._bb = new ByteBuffer(array, num2);
		}

		public void Prep(int size, int additionalBytes)
		{
			bool flag = size > this._minAlign;
			if (flag)
			{
				this._minAlign = size;
			}
			int num = ~(this._bb.Length - this._space + additionalBytes) + 1 & size - 1;
			while (this._space < num + size + additionalBytes)
			{
				int length = this._bb.Length;
				this.GrowBuffer();
				this._space += this._bb.Length - length;
			}
			bool flag2 = num > 0;
			if (flag2)
			{
				this.Pad(num);
			}
		}

		public void PutBool(bool x)
		{
			this._bb.PutByte(--this._space, System.Convert.ToByte(x ? 1 : 0));
		}

		public void PutSbyte(sbyte x)
		{
			this._bb.PutSbyte(--this._space, x);
		}

		public void PutByte(byte x)
		{
			this._bb.PutByte(--this._space, x);
		}

		public void PutShort(short x)
		{
			this._bb.PutShort(this._space -= 2, x);
		}

		public void PutUshort(ushort x)
		{
			this._bb.PutUshort(this._space -= 2, x);
		}

		public void PutInt(int x)
		{
			this._bb.PutInt(this._space -= 4, x);
		}

		public void PutUint(uint x)
		{
			this._bb.PutUint(this._space -= 4, x);
		}

		public void PutLong(long x)
		{
			this._bb.PutLong(this._space -= 8, x);
		}

		public void PutUlong(ulong x)
		{
			this._bb.PutUlong(this._space -= 8, x);
		}

		public void PutFloat(float x)
		{
			this._bb.PutFloat(this._space -= 4, x);
		}

		public void PutDouble(double x)
		{
			this._bb.PutDouble(this._space -= 8, x);
		}

		public void AddBool(bool x)
		{
			this.Prep(1, 0);
			this.PutBool(x);
		}

		public void AddSbyte(sbyte x)
		{
			this.Prep(1, 0);
			this.PutSbyte(x);
		}

		public void AddByte(byte x)
		{
			this.Prep(1, 0);
			this.PutByte(x);
		}

		public void AddShort(short x)
		{
			this.Prep(2, 0);
			this.PutShort(x);
		}

		public void AddUshort(ushort x)
		{
			this.Prep(2, 0);
			this.PutUshort(x);
		}

		public void AddInt(int x)
		{
			this.Prep(4, 0);
			this.PutInt(x);
		}

		public void AddUint(uint x)
		{
			this.Prep(4, 0);
			this.PutUint(x);
		}

		public void AddLong(long x)
		{
			this.Prep(8, 0);
			this.PutLong(x);
		}

		public void AddUlong(ulong x)
		{
			this.Prep(8, 0);
			this.PutUlong(x);
		}

		public void AddFloat(float x)
		{
			this.Prep(4, 0);
			this.PutFloat(x);
		}

		public void AddDouble(double x)
		{
			this.Prep(8, 0);
			this.PutDouble(x);
		}

		public void AddOffset(int off)
		{
			this.Prep(4, 0);
			bool flag = off > this.Offset;
			if (flag)
			{
				throw new ArgumentException();
			}
			off = this.Offset - off + 4;
			this.PutInt(off);
		}

		public void StartVector(int elemSize, int count, int alignment)
		{
			this.NotNested();
			this._vectorNumElems = count;
			this.Prep(4, elemSize * count);
			this.Prep(alignment, elemSize * count);
		}

		public VectorOffset EndVector()
		{
			this.PutInt(this._vectorNumElems);
			return new VectorOffset(this.Offset);
		}

		public void Nested(int obj)
		{
			bool flag = obj != this.Offset;
			if (flag)
			{
				throw new Exception("FlatBuffers: struct must be serialized inline.");
			}
		}

		public void NotNested()
		{
			bool flag = this._vtableSize >= 0;
			if (flag)
			{
				throw new Exception("FlatBuffers: object serialization must not be nested.");
			}
		}

		public void StartObject(int numfields)
		{
			bool flag = numfields < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("Flatbuffers: invalid numfields");
			}
			this.NotNested();
			bool flag2 = this._vtable.Length < numfields;
			if (flag2)
			{
				this._vtable = new int[numfields];
			}
			this._vtableSize = numfields;
			this._objectStart = this.Offset;
		}

		public void Slot(int voffset)
		{
			bool flag = voffset >= this._vtableSize;
			if (flag)
			{
				throw new IndexOutOfRangeException("Flatbuffers: invalid voffset");
			}
			this._vtable[voffset] = this.Offset;
		}

		public void AddBool(int o, bool x, bool d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddBool(x);
				this.Slot(o);
			}
		}

		public void AddSbyte(int o, sbyte x, sbyte d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddSbyte(x);
				this.Slot(o);
			}
		}

		public void AddByte(int o, byte x, byte d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddByte(x);
				this.Slot(o);
			}
		}

		public void AddShort(int o, short x, int d)
		{
			bool flag = (int)x != d;
			if (flag)
			{
				this.AddShort(x);
				this.Slot(o);
			}
		}

		public void AddUshort(int o, ushort x, ushort d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddUshort(x);
				this.Slot(o);
			}
		}

		public void AddInt(int o, int x, int d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddInt(x);
				this.Slot(o);
			}
		}

		public void AddUint(int o, uint x, uint d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddUint(x);
				this.Slot(o);
			}
		}

		public void AddLong(int o, long x, long d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddLong(x);
				this.Slot(o);
			}
		}

		public void AddUlong(int o, ulong x, ulong d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddUlong(x);
				this.Slot(o);
			}
		}

		public void AddFloat(int o, float x, double d)
		{
			bool flag = (double)x != d;
			if (flag)
			{
				this.AddFloat(x);
				this.Slot(o);
			}
		}

		public void AddDouble(int o, double x, double d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddDouble(x);
				this.Slot(o);
			}
		}

		public void AddOffset(int o, int x, int d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.AddOffset(x);
				this.Slot(o);
			}
		}

		public StringOffset CreateString(string s)
		{
			this.NotNested();
			this.AddByte(0);
			int byteCount = Encoding.UTF8.GetByteCount(s);
			this.StartVector(1, byteCount, 1);
			Encoding.UTF8.GetBytes(s, 0, s.Length, this._bb.Data, this._space -= byteCount);
			return new StringOffset(this.EndVector().Value);
		}

		public void AddStruct(int voffset, int x, int d)
		{
			bool flag = x != d;
			if (flag)
			{
				this.Nested(x);
				this.Slot(voffset);
			}
		}

		public int EndObject()
		{
			bool flag = this._vtableSize < 0;
			if (flag)
			{
				throw new InvalidOperationException("Flatbuffers: calling endObject without a startObject");
			}
			this.AddInt(0);
			int offset = this.Offset;
			for (int i = this._vtableSize - 1; i >= 0; i--)
			{
				short x = (short)((this._vtable[i] != 0) ? (offset - this._vtable[i]) : 0);
				this.AddShort(x);
				this._vtable[i] = 0;
			}
			this.AddShort((short)(offset - this._objectStart));
			this.AddShort((short)((this._vtableSize + 2) * 2));
			int num = 0;
			for (int j = 0; j < this._numVtables; j++)
			{
				int num2 = this._bb.Length - this._vtables[j];
				int space = this._space;
				short @short = this._bb.GetShort(num2);
				bool flag2 = @short == this._bb.GetShort(space);
				if (flag2)
				{
					for (int k = 2; k < (int)@short; k += 2)
					{
						bool flag3 = this._bb.GetShort(num2 + k) != this._bb.GetShort(space + k);
						if (flag3)
						{
							goto IL_138;
						}
					}
					num = this._vtables[j];
					break;
				}
				IL_138:;
			}
			bool flag4 = num != 0;
			if (flag4)
			{
				this._space = this._bb.Length - offset;
				this._bb.PutInt(this._space, num - offset);
			}
			else
			{
				bool flag5 = this._numVtables == this._vtables.Length;
				if (flag5)
				{
					int[] array = new int[this._numVtables * 2];
					Array.Copy(this._vtables, array, this._vtables.Length);
					this._vtables = array;
				}
				int[] arg_1F5_0 = this._vtables;
				int numVtables = this._numVtables;
				this._numVtables = numVtables + 1;
				arg_1F5_0[numVtables] = this.Offset;
				this._bb.PutInt(this._bb.Length - offset, this.Offset - offset);
			}
			this._vtableSize = -1;
			return offset;
		}

		public void Required(int table, int field)
		{
			int num = this._bb.Length - table;
			int num2 = num - this._bb.GetInt(num);
			bool flag = this._bb.GetShort(num2 + field) != 0;
			bool flag2 = !flag;
			if (flag2)
			{
				throw new InvalidOperationException("FlatBuffers: field " + field + " must be set");
			}
		}

		public void Finish(int rootTable)
		{
			this.Prep(this._minAlign, 4);
			this.AddOffset(rootTable);
			this._bb.Position = this._space;
		}

		public byte[] SizedByteArray()
		{
			byte[] array = new byte[this._bb.Data.Length - this._bb.Position];
			Buffer.BlockCopy(this._bb.Data, this._bb.Position, array, 0, this._bb.Data.Length - this._bb.Position);
			return array;
		}

		public void SizedByteArray(ref byte[] buffer, int pos = 0)
		{
			Buffer.BlockCopy(this._bb.Data, this._bb.Position, buffer, pos, this._bb.Data.Length - this._bb.Position);
		}

		public void Finish(int rootTable, string fileIdentifier)
		{
			this.Prep(this._minAlign, 8);
			bool flag = fileIdentifier.Length != 4;
			if (flag)
			{
				throw new ArgumentException("FlatBuffers: file identifier must be length " + 4, "fileIdentifier");
			}
			for (int i = 3; i >= 0; i--)
			{
				this.AddByte((byte)fileIdentifier[i]);
			}
			this.Finish(rootTable);
		}
	}
}

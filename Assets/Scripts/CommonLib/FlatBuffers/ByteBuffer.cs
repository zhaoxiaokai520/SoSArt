using System;

namespace FlatBuffers
{
	public class ByteBuffer
	{
		protected byte[] _buffer;

		protected int _pos;

		private static ByteBuffer s_static = new ByteBuffer(null, 0);

		private static float[] floathelper = new float[1];

		private static int[] inthelper = new int[1];

		private static double[] doublehelper = new double[1];

		private static ulong[] ulonghelper = new ulong[1];

		public int Length
		{
			get
			{
				return this._buffer.Length;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._buffer;
			}
		}

		public int Position
		{
			get
			{
				return this._pos;
			}
			set
			{
				this._pos = value;
			}
		}

		public ByteBuffer(byte[] buffer) : this(buffer, 0)
		{
		}

		public ByteBuffer(byte[] buffer, int pos)
		{
			this._buffer = buffer;
			this._pos = pos;
		}

		public static ByteBuffer SBuffer(byte[] buffer, int pos = 0)
		{
			ByteBuffer.s_static.SetBuffer(buffer, pos);
			return ByteBuffer.s_static;
		}

		public void SetBuffer(byte[] buffer, int pos)
		{
			this._buffer = buffer;
			this._pos = pos;
		}

		public void Reset()
		{
			this._pos = 0;
		}

		public void Advance(int len)
		{
			this._pos += len;
		}

		public static ushort ReverseBytes(ushort input)
		{
			return (ushort)((int)(input & 255) << 8 | (int)((uint)(input & 65280) >> 8));
		}

		public static uint ReverseBytes(uint input)
		{
			return (input & 255u) << 24 | (input & 65280u) << 8 | (input & 16711680u) >> 8 | (input & 4278190080u) >> 24;
		}

		public static ulong ReverseBytes(ulong input)
		{
			return (input & 255uL) << 56 | (input & 65280uL) << 40 | (input & 16711680uL) << 24 | (input & System.Convert.ToUInt64(-16777216)) << 8 | (input & 1095216660480uL) >> 8 | (input & 280375465082880uL) >> 24 | (input & 71776119061217280uL) >> 40 | (input & 18374686479671623680uL) >> 56;
		}

		private void AssertOffsetAndLength(int offset, int length)
		{
			bool flag = offset < 0 || offset >= this._buffer.Length || offset + length > this._buffer.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException();
			}
		}

		public void PutSbyte(int offset, sbyte value)
		{
			this.AssertOffsetAndLength(offset, 1);
			this._buffer[offset] = (byte)value;
		}

		public void PutByte(int offset, byte value)
		{
			this.AssertOffsetAndLength(offset, 1);
			this._buffer[offset] = value;
		}

		public void PutByte(int offset, byte value, int count)
		{
			this.AssertOffsetAndLength(offset, count);
			for (int i = 0; i < count; i++)
			{
				this._buffer[offset + i] = value;
			}
		}

		public void Put(int offset, byte value)
		{
			this.PutByte(offset, value);
		}

		public void PutShort(int offset, short value)
		{
			this.PutUshort(offset, (ushort)value);
		}

		public void PutUshort(int offset, ushort value)
		{
            //this.AssertOffsetAndLength(offset, 2);
            //fixed (byte* buffer = this._buffer)
            //{
            //    ((short*)buffer)[offset] = (short)(BitConverter.IsLittleEndian ? value : ByteBuffer.ReverseBytes(value));
            //}
        }

		public void PutInt(int offset, int value)
		{
			this.PutUint(offset, (uint)value);
		}

		public void PutUint(int offset, uint value)
		{
			//this.AssertOffsetAndLength(offset, 4);
			//fixed (byte* buffer = this._buffer)
			//{
			//	((int*)buffer)[offset] = (int)(BitConverter.IsLittleEndian ? value : ByteBuffer.ReverseBytes(value));
			//}
		}

		public void PutLong(int offset, long value)
		{
			this.PutUlong(offset, (ulong)value);
		}

		public void PutUlong(int offset, ulong value)
		{
			//this.AssertOffsetAndLength(offset, 8);
			//fixed (byte* buffer = this._buffer)
			//{
			//	((long*)buffer)[offset] = (long)(BitConverter.IsLittleEndian ? value : ByteBuffer.ReverseBytes(value));
			//}
		}

		public void PutFloat(int offset, float value)
		{
			//this.AssertOffsetAndLength(offset, 4);
			//fixed (byte* buffer = this._buffer)
			//{
			//	bool isLittleEndian = BitConverter.IsLittleEndian;
			//	if (isLittleEndian)
			//	{
			//		((float*)buffer)[offset / 4] = value;
			//	}
			//	else
			//	{
			//		((int*)buffer)[offset / 4] = (int)ByteBuffer.ReverseBytes(*(uint*)(&value));
			//	}
			//}
		}

		public void PutDouble(int offset, double value)
		{
			//this.AssertOffsetAndLength(offset, 8);
			//fixed (byte* buffer = this._buffer)
			//{
			//	bool isLittleEndian = BitConverter.IsLittleEndian;
			//	if (isLittleEndian)
			//	{
			//		((double*)buffer)[offset / 8] = value;
			//	}
			//	else
			//	{
			//		((long*)buffer)[offset / 8] = (long)ByteBuffer.ReverseBytes((ulong)((long*)buffer)[offset / 8]);
			//	}
			//}
		}

		public sbyte GetSbyte(int index)
		{
			this.AssertOffsetAndLength(index, 1);
			return (sbyte)this._buffer[index];
		}

		public byte Get(int index)
		{
			this.AssertOffsetAndLength(index, 1);
			return this._buffer[index];
		}

		public short GetShort(int offset)
		{
			return (short)this.GetUshort(offset);
		}

		public ushort GetUshort(int offset)
		{
            //this.AssertOffsetAndLength(offset, 2);
            //fixed (byte* buffer = this._buffer)
            //{
            //	return BitConverter.IsLittleEndian ? ((ushort*)buffer)[offset / 2] : ByteBuffer.ReverseBytes(((ushort*)buffer)[offset / 2]);
            //}
            return 0;
		}

		public int GetInt(int offset)
		{
			return (int)this.GetUint(offset);
		}

		public uint GetUint(int offset)
		{
            //this.AssertOffsetAndLength(offset, 4);
            //fixed (byte* buffer = this._buffer)
            //{
            //	return BitConverter.IsLittleEndian ? ((uint*)buffer)[offset / 4] : ByteBuffer.ReverseBytes(((uint*)buffer)[offset / 4]);
            //}
            return 0;
		}

		public long GetLong(int offset)
		{
			return (long)this.GetUlong(offset);
		}

		public ulong GetUlong(int offset)
		{
            //this.AssertOffsetAndLength(offset, 8);
            //fixed (byte* buffer = this._buffer)
            //{
            //	return (ulong)(BitConverter.IsLittleEndian ? ((long*)buffer)[offset / 8] : ((long)ByteBuffer.ReverseBytes((ulong)((long*)buffer)[offset / 8])));
            //}
            return 0;
		}

		public float GetFloat(int offset)
		{
            //this.AssertOffsetAndLength(offset, 4);
            //fixed (byte* buffer = this._buffer)
            //{
            //	bool isLittleEndian = BitConverter.IsLittleEndian;
            //	float result;
            //	if (isLittleEndian)
            //	{
            //		result = ((float*)buffer)[offset / 4];
            //	}
            //	else
            //	{
            //		uint num = ByteBuffer.ReverseBytes(((uint*)buffer)[offset / 4]);
            //		result = *(float*)(&num);
            //	}
            //	return result;
            //}
            return 0.0f;
		}

		public double GetDouble(int offset)
		{
            //this.AssertOffsetAndLength(offset, 8);
            //fixed (byte* buffer = this._buffer)
            //{
            //	bool isLittleEndian = BitConverter.IsLittleEndian;
            //	double result;
            //	if (isLittleEndian)
            //	{
            //		result = ((double*)buffer)[offset / 8];
            //	}
            //	else
            //	{
            //		ulong num = ByteBuffer.ReverseBytes((ulong)((long*)buffer)[offset / 8]);
            //		result = *(double*)(&num);
            //	}
            //	return result;
            //}
            return 0.0f;
		}
	}
}

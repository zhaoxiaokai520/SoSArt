using System;
using System.Text;

namespace FlatBuffers
{
	public abstract class Table
	{
		protected int bb_pos;

		protected ByteBuffer bb;

		public ByteBuffer ByteBuffer
		{
			get
			{
				return this.bb;
			}
		}

		public int __offset(int vtableOffset)
		{
			int num = this.bb_pos - this.bb.GetInt(this.bb_pos);
			return (int)((vtableOffset < (int)this.bb.GetShort(num)) ? this.bb.GetShort(num + vtableOffset) : 0);
		}

		protected int __indirect(int offset)
		{
			return offset + this.bb.GetInt(offset);
		}

		protected string __string(int offset)
		{
			offset += this.bb.GetInt(offset);
			int @int = this.bb.GetInt(offset);
			int index = offset + 4;
			return Encoding.UTF8.GetString(this.bb.Data, index, @int);
		}

		protected int __vector_len(int offset)
		{
			offset += this.bb_pos;
			offset += this.bb.GetInt(offset);
			return this.bb.GetInt(offset);
		}

		public int __vector(int offset)
		{
			offset += this.bb_pos;
			return offset + this.bb.GetInt(offset) + 4;
		}

		protected ArraySegment<byte>? __vector_as_arraysegment(int offset)
		{
			int num = this.__offset(offset);
			bool flag = num == 0;
			ArraySegment<byte>? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int offset2 = this.__vector(num);
				int count = this.__vector_len(num);
				result = new ArraySegment<byte>?(new ArraySegment<byte>(this.bb.Data, offset2, count));
			}
			return result;
		}

		protected TTable __union<TTable>(TTable t, int offset) where TTable : Table
		{
			offset += this.bb_pos;
			t.bb_pos = offset + this.bb.GetInt(offset);
			t.bb = this.bb;
			return t;
		}

		protected static bool __has_identifier(ByteBuffer bb, string ident)
		{
			bool flag = ident.Length != 4;
			if (flag)
			{
				throw new ArgumentException("FlatBuffers: file identifier must be length " + 4, "ident");
			}
			bool result;
			for (int i = 0; i < 4; i++)
			{
				bool flag2 = ident[i] != (char)bb.Get(bb.Position + 4 + i);
				if (flag2)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
	}
}

using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class vector3i : Table
	{
		public int X
		{
			get
			{
				int num = base.__offset(4);
				if (num == 0)
				{
					return 0;
				}
				return this.bb.GetInt(num + this.bb_pos);
			}
		}

		public int Y
		{
			get
			{
				int num = base.__offset(6);
				if (num == 0)
				{
					return 0;
				}
				return this.bb.GetInt(num + this.bb_pos);
			}
		}

		public int Z
		{
			get
			{
				int num = base.__offset(8);
				if (num == 0)
				{
					return 0;
				}
				return this.bb.GetInt(num + this.bb_pos);
			}
		}

		public static vector3i GetRootAsvector3i(ByteBuffer _bb)
		{
			return vector3i.GetRootAsvector3i(_bb, new vector3i());
		}

		public static vector3i GetRootAsvector3i(ByteBuffer _bb, vector3i obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public vector3i __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public static Offset<vector3i> Createvector3i(FlatBufferBuilder builder, int x = 0, int y = 0, int z = 0)
		{
			builder.StartObject(3);
			vector3i.AddZ(builder, z);
			vector3i.AddY(builder, y);
			vector3i.AddX(builder, x);
			return vector3i.Endvector3i(builder);
		}

		public static void Startvector3i(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddX(FlatBufferBuilder builder, int x)
		{
			builder.AddInt(0, x, 0);
		}

		public static void AddY(FlatBufferBuilder builder, int y)
		{
			builder.AddInt(1, y, 0);
		}

		public static void AddZ(FlatBufferBuilder builder, int z)
		{
			builder.AddInt(2, z, 0);
		}

		public static Offset<vector3i> Endvector3i(FlatBufferBuilder builder)
		{
			return new Offset<vector3i>(builder.EndObject());
		}
	}
}

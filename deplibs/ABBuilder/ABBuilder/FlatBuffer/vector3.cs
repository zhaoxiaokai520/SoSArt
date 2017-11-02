using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class vector3 : Table
	{
		public float X
		{
			get
			{
				int num = base.__offset(4);
				if (num == 0)
				{
					return 0f;
				}
				return this.bb.GetFloat(num + this.bb_pos);
			}
		}

		public float Y
		{
			get
			{
				int num = base.__offset(6);
				if (num == 0)
				{
					return 0f;
				}
				return this.bb.GetFloat(num + this.bb_pos);
			}
		}

		public float Z
		{
			get
			{
				int num = base.__offset(8);
				if (num == 0)
				{
					return 0f;
				}
				return this.bb.GetFloat(num + this.bb_pos);
			}
		}

		public static vector3 GetRootAsvector3(ByteBuffer _bb)
		{
			return vector3.GetRootAsvector3(_bb, new vector3());
		}

		public static vector3 GetRootAsvector3(ByteBuffer _bb, vector3 obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public vector3 __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public static Offset<vector3> Createvector3(FlatBufferBuilder builder, float x = 0f, float y = 0f, float z = 0f)
		{
			builder.StartObject(3);
			vector3.AddZ(builder, z);
			vector3.AddY(builder, y);
			vector3.AddX(builder, x);
			return vector3.Endvector3(builder);
		}

		public static void Startvector3(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddX(FlatBufferBuilder builder, float x)
		{
			builder.AddFloat(0, x, 0.0);
		}

		public static void AddY(FlatBufferBuilder builder, float y)
		{
			builder.AddFloat(1, y, 0.0);
		}

		public static void AddZ(FlatBufferBuilder builder, float z)
		{
			builder.AddFloat(2, z, 0.0);
		}

		public static Offset<vector3> Endvector3(FlatBufferBuilder builder)
		{
			return new Offset<vector3>(builder.EndObject());
		}
	}
}

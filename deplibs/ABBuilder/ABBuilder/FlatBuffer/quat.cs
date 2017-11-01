using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class quat : Table
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

		public float W
		{
			get
			{
				int num = base.__offset(10);
				if (num == 0)
				{
					return 0f;
				}
				return this.bb.GetFloat(num + this.bb_pos);
			}
		}

		public static quat GetRootAsquat(ByteBuffer _bb)
		{
			return quat.GetRootAsquat(_bb, new quat());
		}

		public static quat GetRootAsquat(ByteBuffer _bb, quat obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public quat __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public static Offset<quat> Createquat(FlatBufferBuilder builder, float x = 0f, float y = 0f, float z = 0f, float w = 0f)
		{
			builder.StartObject(4);
			quat.AddW(builder, w);
			quat.AddZ(builder, z);
			quat.AddY(builder, y);
			quat.AddX(builder, x);
			return quat.Endquat(builder);
		}

		public static void Startquat(FlatBufferBuilder builder)
		{
			builder.StartObject(4);
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

		public static void AddW(FlatBufferBuilder builder, float w)
		{
			builder.AddFloat(3, w, 0.0);
		}

		public static Offset<quat> Endquat(FlatBufferBuilder builder)
		{
			return new Offset<quat>(builder.EndObject());
		}
	}
}

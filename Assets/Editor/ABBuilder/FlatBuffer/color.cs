using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class color : Table
	{
		public float R
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

		public float G
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

		public float B
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

		public float A
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

		public static color GetRootAscolor(ByteBuffer _bb)
		{
			return color.GetRootAscolor(_bb, new color());
		}

		public static color GetRootAscolor(ByteBuffer _bb, color obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public color __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public static Offset<color> Createcolor(FlatBufferBuilder builder, float r = 0f, float g = 0f, float b = 0f, float a = 0f)
		{
			builder.StartObject(4);
			color.AddA(builder, a);
			color.AddB(builder, b);
			color.AddG(builder, g);
			color.AddR(builder, r);
			return color.Endcolor(builder);
		}

		public static void Startcolor(FlatBufferBuilder builder)
		{
			builder.StartObject(4);
		}

		public static void AddR(FlatBufferBuilder builder, float r)
		{
			builder.AddFloat(0, r, 0.0);
		}

		public static void AddG(FlatBufferBuilder builder, float g)
		{
			builder.AddFloat(1, g, 0.0);
		}

		public static void AddB(FlatBufferBuilder builder, float b)
		{
			builder.AddFloat(2, b, 0.0);
		}

		public static void AddA(FlatBufferBuilder builder, float a)
		{
			builder.AddFloat(3, a, 0.0);
		}

		public static Offset<color> Endcolor(FlatBufferBuilder builder)
		{
			return new Offset<color>(builder.EndObject());
		}
	}
}

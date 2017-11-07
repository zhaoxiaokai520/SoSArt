using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class TemplateObj : Table
	{
		public string ObjectName
		{
			get
			{
				int num = base.__offset(4);
				if (num == 0)
				{
					return null;
				}
				return base.__string(num + this.bb_pos);
			}
		}

		public int Id
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

		public bool IsTemp
		{
			get
			{
				int num = base.__offset(8);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public static TemplateObj GetRootAsTemplateObj(ByteBuffer _bb)
		{
			return TemplateObj.GetRootAsTemplateObj(_bb, new TemplateObj());
		}

		public static TemplateObj GetRootAsTemplateObj(ByteBuffer _bb, TemplateObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public TemplateObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public ArraySegment<byte>? GetObjectNameBytes()
		{
			return base.__vector_as_arraysegment(4);
		}

		public static Offset<TemplateObj> CreateTemplateObj(FlatBufferBuilder builder, StringOffset objectNameOffset = default(StringOffset), int id = 0, bool isTemp = false)
		{
			builder.StartObject(3);
			TemplateObj.AddId(builder, id);
			TemplateObj.AddObjectName(builder, objectNameOffset);
			TemplateObj.AddIsTemp(builder, isTemp);
			return TemplateObj.EndTemplateObj(builder);
		}

		public static void StartTemplateObj(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddObjectName(FlatBufferBuilder builder, StringOffset objectNameOffset)
		{
			builder.AddOffset(0, objectNameOffset.Value, 0);
		}

		public static void AddId(FlatBufferBuilder builder, int id)
		{
			builder.AddInt(1, id, 0);
		}

		public static void AddIsTemp(FlatBufferBuilder builder, bool isTemp)
		{
			builder.AddBool(2, isTemp, false);
		}

		public static Offset<TemplateObj> EndTemplateObj(FlatBufferBuilder builder)
		{
			return new Offset<TemplateObj>(builder.EndObject());
		}
	}
}

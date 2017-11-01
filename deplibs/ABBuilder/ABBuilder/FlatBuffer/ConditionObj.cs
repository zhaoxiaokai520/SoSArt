using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class ConditionObj : Table
	{
		public int Id
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

		public bool Status
		{
			get
			{
				int num = base.__offset(6);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public static ConditionObj GetRootAsConditionObj(ByteBuffer _bb)
		{
			return ConditionObj.GetRootAsConditionObj(_bb, new ConditionObj());
		}

		public static ConditionObj GetRootAsConditionObj(ByteBuffer _bb, ConditionObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public ConditionObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public static Offset<ConditionObj> CreateConditionObj(FlatBufferBuilder builder, int id = 0, bool status = false)
		{
			builder.StartObject(2);
			ConditionObj.AddId(builder, id);
			ConditionObj.AddStatus(builder, status);
			return ConditionObj.EndConditionObj(builder);
		}

		public static void StartConditionObj(FlatBufferBuilder builder)
		{
			builder.StartObject(2);
		}

		public static void AddId(FlatBufferBuilder builder, int id)
		{
			builder.AddInt(0, id, 0);
		}

		public static void AddStatus(FlatBufferBuilder builder, bool status)
		{
			builder.AddBool(1, status, false);
		}

		public static Offset<ConditionObj> EndConditionObj(FlatBufferBuilder builder)
		{
			return new Offset<ConditionObj>(builder.EndObject());
		}
	}
}

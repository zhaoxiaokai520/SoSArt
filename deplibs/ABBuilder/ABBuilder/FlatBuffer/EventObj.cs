using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class EventObj : Table
	{
		public int Time
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

		public int Length
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

		public int VarListLength
		{
			get
			{
				int num = base.__offset(8);
				if (num == 0)
				{
					return 0;
				}
				return base.__vector_len(num);
			}
		}

		public static EventObj GetRootAsEventObj(ByteBuffer _bb)
		{
			return EventObj.GetRootAsEventObj(_bb, new EventObj());
		}

		public static EventObj GetRootAsEventObj(ByteBuffer _bb, EventObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public EventObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public VarContext GetVarList(int j)
		{
			return this.GetVarList(new VarContext(), j);
		}

		public VarContext GetVarList(VarContext obj, int j)
		{
			int num = base.__offset(8);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public static Offset<EventObj> CreateEventObj(FlatBufferBuilder builder, int time = 0, int length = 0, VectorOffset varListOffset = default(VectorOffset))
		{
			builder.StartObject(3);
			EventObj.AddVarList(builder, varListOffset);
			EventObj.AddLength(builder, length);
			EventObj.AddTime(builder, time);
			return EventObj.EndEventObj(builder);
		}

		public static void StartEventObj(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddTime(FlatBufferBuilder builder, int time)
		{
			builder.AddInt(0, time, 0);
		}

		public static void AddLength(FlatBufferBuilder builder, int length)
		{
			builder.AddInt(1, length, 0);
		}

		public static void AddVarList(FlatBufferBuilder builder, VectorOffset varListOffset)
		{
			builder.AddOffset(2, varListOffset.Value, 0);
		}

		public static VectorOffset CreateVarListVector(FlatBufferBuilder builder, Offset<VarContext>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartVarListVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<EventObj> EndEventObj(FlatBufferBuilder builder)
		{
			return new Offset<EventObj>(builder.EndObject());
		}
	}
}

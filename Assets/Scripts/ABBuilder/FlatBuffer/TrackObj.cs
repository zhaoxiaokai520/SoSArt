using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class TrackObj : Table
	{
		public bool Enabled
		{
			get
			{
				int num = base.__offset(4);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public string TrackName
		{
			get
			{
				int num = base.__offset(6);
				if (num == 0)
				{
					return null;
				}
				return base.__string(num + this.bb_pos);
			}
		}

		public string EventType
		{
			get
			{
				int num = base.__offset(8);
				if (num == 0)
				{
					return null;
				}
				return base.__string(num + this.bb_pos);
			}
		}

		public string RefParamName
		{
			get
			{
				int num = base.__offset(10);
				if (num == 0)
				{
					return null;
				}
				return base.__string(num + this.bb_pos);
			}
		}

		public bool UseRefParam
		{
			get
			{
				int num = base.__offset(12);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public bool ExecOnActionCompleted
		{
			get
			{
				int num = base.__offset(14);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public bool ExecOnForceStopped
		{
			get
			{
				int num = base.__offset(16);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public bool StopAfterLastEvent
		{
			get
			{
				int num = base.__offset(18);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public bool HasCondition
		{
			get
			{
				int num = base.__offset(20);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public int ConditionLength
		{
			get
			{
				int num = base.__offset(22);
				if (num == 0)
				{
					return 0;
				}
				return base.__vector_len(num);
			}
		}

		public int EvtsLength
		{
			get
			{
				int num = base.__offset(24);
				if (num == 0)
				{
					return 0;
				}
				return base.__vector_len(num);
			}
		}

		public static TrackObj GetRootAsTrackObj(ByteBuffer _bb)
		{
			return TrackObj.GetRootAsTrackObj(_bb, new TrackObj());
		}

		public static TrackObj GetRootAsTrackObj(ByteBuffer _bb, TrackObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public TrackObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public ArraySegment<byte>? GetTrackNameBytes()
		{
			return base.__vector_as_arraysegment(6);
		}

		public ArraySegment<byte>? GetEventTypeBytes()
		{
			return base.__vector_as_arraysegment(8);
		}

		public ArraySegment<byte>? GetRefParamNameBytes()
		{
			return base.__vector_as_arraysegment(10);
		}

		public ConditionObj GetCondition(int j)
		{
			return this.GetCondition(new ConditionObj(), j);
		}

		public ConditionObj GetCondition(ConditionObj obj, int j)
		{
			int num = base.__offset(22);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public EventObj GetEvts(int j)
		{
			return this.GetEvts(new EventObj(), j);
		}

		public EventObj GetEvts(EventObj obj, int j)
		{
			int num = base.__offset(24);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public static Offset<TrackObj> CreateTrackObj(FlatBufferBuilder builder, bool enabled = false, StringOffset trackNameOffset = default(StringOffset), StringOffset eventTypeOffset = default(StringOffset), StringOffset refParamNameOffset = default(StringOffset), bool useRefParam = false, bool execOnActionCompleted = false, bool execOnForceStopped = false, bool stopAfterLastEvent = false, bool hasCondition = false, VectorOffset conditionOffset = default(VectorOffset), VectorOffset evtsOffset = default(VectorOffset))
		{
			builder.StartObject(11);
			TrackObj.AddEvts(builder, evtsOffset);
			TrackObj.AddCondition(builder, conditionOffset);
			TrackObj.AddRefParamName(builder, refParamNameOffset);
			TrackObj.AddEventType(builder, eventTypeOffset);
			TrackObj.AddTrackName(builder, trackNameOffset);
			TrackObj.AddHasCondition(builder, hasCondition);
			TrackObj.AddStopAfterLastEvent(builder, stopAfterLastEvent);
			TrackObj.AddExecOnForceStopped(builder, execOnForceStopped);
			TrackObj.AddExecOnActionCompleted(builder, execOnActionCompleted);
			TrackObj.AddUseRefParam(builder, useRefParam);
			TrackObj.AddEnabled(builder, enabled);
			return TrackObj.EndTrackObj(builder);
		}

		public static void StartTrackObj(FlatBufferBuilder builder)
		{
			builder.StartObject(11);
		}

		public static void AddEnabled(FlatBufferBuilder builder, bool enabled)
		{
			builder.AddBool(0, enabled, false);
		}

		public static void AddTrackName(FlatBufferBuilder builder, StringOffset trackNameOffset)
		{
			builder.AddOffset(1, trackNameOffset.Value, 0);
		}

		public static void AddEventType(FlatBufferBuilder builder, StringOffset eventTypeOffset)
		{
			builder.AddOffset(2, eventTypeOffset.Value, 0);
		}

		public static void AddRefParamName(FlatBufferBuilder builder, StringOffset refParamNameOffset)
		{
			builder.AddOffset(3, refParamNameOffset.Value, 0);
		}

		public static void AddUseRefParam(FlatBufferBuilder builder, bool useRefParam)
		{
			builder.AddBool(4, useRefParam, false);
		}

		public static void AddExecOnActionCompleted(FlatBufferBuilder builder, bool execOnActionCompleted)
		{
			builder.AddBool(5, execOnActionCompleted, false);
		}

		public static void AddExecOnForceStopped(FlatBufferBuilder builder, bool execOnForceStopped)
		{
			builder.AddBool(6, execOnForceStopped, false);
		}

		public static void AddStopAfterLastEvent(FlatBufferBuilder builder, bool stopAfterLastEvent)
		{
			builder.AddBool(7, stopAfterLastEvent, false);
		}

		public static void AddHasCondition(FlatBufferBuilder builder, bool hasCondition)
		{
			builder.AddBool(8, hasCondition, false);
		}

		public static void AddCondition(FlatBufferBuilder builder, VectorOffset conditionOffset)
		{
			builder.AddOffset(9, conditionOffset.Value, 0);
		}

		public static VectorOffset CreateConditionVector(FlatBufferBuilder builder, Offset<ConditionObj>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartConditionVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddEvts(FlatBufferBuilder builder, VectorOffset evtsOffset)
		{
			builder.AddOffset(10, evtsOffset.Value, 0);
		}

		public static VectorOffset CreateEvtsVector(FlatBufferBuilder builder, Offset<EventObj>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartEvtsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<TrackObj> EndTrackObj(FlatBufferBuilder builder)
		{
			return new Offset<TrackObj>(builder.EndObject());
		}
	}
}

using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class ActionObj : Table
	{
		public int Length
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

		public bool Loop
		{
			get
			{
				int num = base.__offset(6);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public int TrackLength
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

		public static ActionObj GetRootAsActionObj(ByteBuffer _bb)
		{
			return ActionObj.GetRootAsActionObj(_bb, new ActionObj());
		}

		public static ActionObj GetRootAsActionObj(ByteBuffer _bb, ActionObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public ActionObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public TrackObj GetTrack(int j)
		{
			return this.GetTrack(new TrackObj(), j);
		}

		public TrackObj GetTrack(TrackObj obj, int j)
		{
			int num = base.__offset(8);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public static Offset<ActionObj> CreateActionObj(FlatBufferBuilder builder, int length = 0, bool loop = false, VectorOffset trackOffset = default(VectorOffset))
		{
			builder.StartObject(3);
			ActionObj.AddTrack(builder, trackOffset);
			ActionObj.AddLength(builder, length);
			ActionObj.AddLoop(builder, loop);
			return ActionObj.EndActionObj(builder);
		}

		public static void StartActionObj(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddLength(FlatBufferBuilder builder, int length)
		{
			builder.AddInt(0, length, 0);
		}

		public static void AddLoop(FlatBufferBuilder builder, bool loop)
		{
			builder.AddBool(1, loop, false);
		}

		public static void AddTrack(FlatBufferBuilder builder, VectorOffset trackOffset)
		{
			builder.AddOffset(2, trackOffset.Value, 0);
		}

		public static VectorOffset CreateTrackVector(FlatBufferBuilder builder, Offset<TrackObj>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartTrackVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<ActionObj> EndActionObj(FlatBufferBuilder builder)
		{
			return new Offset<ActionObj>(builder.EndObject());
		}
	}
}

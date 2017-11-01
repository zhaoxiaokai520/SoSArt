using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class AgeData : Table
	{
		public string Name
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

		public int DataLength
		{
			get
			{
				int num = base.__offset(6);
				if (num == 0)
				{
					return 0;
				}
				return base.__vector_len(num);
			}
		}

		public static AgeData GetRootAsAgeData(ByteBuffer _bb)
		{
			return AgeData.GetRootAsAgeData(_bb, new AgeData());
		}

		public static AgeData GetRootAsAgeData(ByteBuffer _bb, AgeData obj)
		{
			return obj.__init(_bb.GetInt(_bb.get_Position()) + _bb.get_Position(), _bb);
		}

		public AgeData __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public ArraySegment<byte>? GetNameBytes()
		{
			return base.__vector_as_arraysegment(4);
		}

		public byte GetData(int j)
		{
			int num = base.__offset(6);
			if (num == 0)
			{
				return 0;
			}
			return this.bb.Get(base.__vector(num) + j);
		}

		public ArraySegment<byte>? GetDataBytes()
		{
			return base.__vector_as_arraysegment(6);
		}

		public static Offset<AgeData> CreateAgeData(FlatBufferBuilder builder, StringOffset nameOffset = default(StringOffset), VectorOffset dataOffset = default(VectorOffset))
		{
			builder.StartObject(2);
			AgeData.AddData(builder, dataOffset);
			AgeData.AddName(builder, nameOffset);
			return AgeData.EndAgeData(builder);
		}

		public static void StartAgeData(FlatBufferBuilder builder)
		{
			builder.StartObject(2);
		}

		public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset)
		{
			builder.AddOffset(0, nameOffset.Value, 0);
		}

		public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset)
		{
			builder.AddOffset(1, dataOffset.Value, 0);
		}

		public static VectorOffset CreateDataVector(FlatBufferBuilder builder, byte[] data)
		{
			builder.StartVector(1, data.Length, 1);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddByte(data[i]);
			}
			return builder.EndVector();
		}

		public static void StartDataVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(1, numElems, 1);
		}

		public static Offset<AgeData> EndAgeData(FlatBufferBuilder builder)
		{
			return new Offset<AgeData>(builder.EndObject());
		}
	}
}

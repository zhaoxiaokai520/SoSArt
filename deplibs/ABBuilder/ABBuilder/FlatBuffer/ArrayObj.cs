using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class ArrayObj : Table
	{
		public int DataListLength
		{
			get
			{
				int num = base.__offset(4);
				if (num == 0)
				{
					return 0;
				}
				return base.__vector_len(num);
			}
		}

		public static ArrayObj GetRootAsArrayObj(ByteBuffer _bb)
		{
			return ArrayObj.GetRootAsArrayObj(_bb, new ArrayObj());
		}

		public static ArrayObj GetRootAsArrayObj(ByteBuffer _bb, ArrayObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public ArrayObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public VarContext GetDataList(int j)
		{
			return this.GetDataList(new VarContext(), j);
		}

		public VarContext GetDataList(VarContext obj, int j)
		{
			int num = base.__offset(4);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public static Offset<ArrayObj> CreateArrayObj(FlatBufferBuilder builder, VectorOffset dataListOffset = default(VectorOffset))
		{
			builder.StartObject(1);
			ArrayObj.AddDataList(builder, dataListOffset);
			return ArrayObj.EndArrayObj(builder);
		}

		public static void StartArrayObj(FlatBufferBuilder builder)
		{
			builder.StartObject(1);
		}

		public static void AddDataList(FlatBufferBuilder builder, VectorOffset dataListOffset)
		{
			builder.AddOffset(0, dataListOffset.Value, 0);
		}

		public static VectorOffset CreateDataListVector(FlatBufferBuilder builder, Offset<VarContext>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartDataListVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<ArrayObj> EndArrayObj(FlatBufferBuilder builder)
		{
			return new Offset<ArrayObj>(builder.EndObject());
		}
	}
}

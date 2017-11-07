using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class VarObj : Table
	{
		public byte DataType
		{
			get
			{
				int num = base.__offset(4);
				if (num == 0)
				{
					return 0;
				}
				return this.bb.Get(num + this.bb_pos);
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

		public static VarObj GetRootAsVarObj(ByteBuffer _bb)
		{
			return VarObj.GetRootAsVarObj(_bb, new VarObj());
		}

		public static VarObj GetRootAsVarObj(ByteBuffer _bb, VarObj obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public VarObj __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
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

		public static Offset<VarObj> CreateVarObj(FlatBufferBuilder builder, byte dataType = 0, VectorOffset dataOffset = default(VectorOffset))
		{
			builder.StartObject(2);
			VarObj.AddData(builder, dataOffset);
			VarObj.AddDataType(builder, dataType);
			return VarObj.EndVarObj(builder);
		}

		public static void StartVarObj(FlatBufferBuilder builder)
		{
			builder.StartObject(2);
		}

		public static void AddDataType(FlatBufferBuilder builder, byte dataType)
		{
			builder.AddByte(0, dataType, 0);
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

		public static Offset<VarObj> EndVarObj(FlatBufferBuilder builder)
		{
			return new Offset<VarObj>(builder.EndObject());
		}
	}
}

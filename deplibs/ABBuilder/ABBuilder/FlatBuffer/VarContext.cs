using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class VarContext : Table
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

		public string RefParamName
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

		public bool UseRefParam
		{
			get
			{
				int num = base.__offset(8);
				return num != 0 && this.bb.Get(num + this.bb_pos) > 0;
			}
		}

		public VarObj Context
		{
			get
			{
				return this.GetContext(new VarObj());
			}
		}

		public static VarContext GetRootAsVarContext(ByteBuffer _bb)
		{
			return VarContext.GetRootAsVarContext(_bb, new VarContext());
		}

		public static VarContext GetRootAsVarContext(ByteBuffer _bb, VarContext obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public VarContext __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public ArraySegment<byte>? GetNameBytes()
		{
			return base.__vector_as_arraysegment(4);
		}

		public ArraySegment<byte>? GetRefParamNameBytes()
		{
			return base.__vector_as_arraysegment(6);
		}

		public VarObj GetContext(VarObj obj)
		{
			int num = base.__offset(10);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(num + this.bb_pos), this.bb);
		}

		public static Offset<VarContext> CreateVarContext(FlatBufferBuilder builder, StringOffset nameOffset = default(StringOffset), StringOffset refParamNameOffset = default(StringOffset), bool useRefParam = false, Offset<VarObj> contextOffset = default(Offset<VarObj>))
		{
			builder.StartObject(4);
			VarContext.AddContext(builder, contextOffset);
			VarContext.AddRefParamName(builder, refParamNameOffset);
			VarContext.AddName(builder, nameOffset);
			VarContext.AddUseRefParam(builder, useRefParam);
			return VarContext.EndVarContext(builder);
		}

		public static void StartVarContext(FlatBufferBuilder builder)
		{
			builder.StartObject(4);
		}

		public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset)
		{
			builder.AddOffset(0, nameOffset.Value, 0);
		}

		public static void AddRefParamName(FlatBufferBuilder builder, StringOffset refParamNameOffset)
		{
			builder.AddOffset(1, refParamNameOffset.Value, 0);
		}

		public static void AddUseRefParam(FlatBufferBuilder builder, bool useRefParam)
		{
			builder.AddBool(2, useRefParam, false);
		}

		public static void AddContext(FlatBufferBuilder builder, Offset<VarObj> contextOffset)
		{
			builder.AddOffset(3, contextOffset.Value, 0);
		}

		public static Offset<VarContext> EndVarContext(FlatBufferBuilder builder)
		{
			return new Offset<VarContext>(builder.EndObject());
		}
	}
}

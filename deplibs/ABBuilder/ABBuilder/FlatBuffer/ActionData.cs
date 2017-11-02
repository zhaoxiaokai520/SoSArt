using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class ActionData : Table
	{
		public int TemplateObjsLength
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

		public int ReferenceParamsLength
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

		public ActionObj Action
		{
			get
			{
				return this.GetAction(new ActionObj());
			}
		}

		public static ActionData GetRootAsActionData(ByteBuffer _bb)
		{
			return ActionData.GetRootAsActionData(_bb, new ActionData());
		}

		public static ActionData GetRootAsActionData(ByteBuffer _bb, ActionData obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public ActionData __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public TemplateObj GetTemplateObjs(int j)
		{
			return this.GetTemplateObjs(new TemplateObj(), j);
		}

		public TemplateObj GetTemplateObjs(TemplateObj obj, int j)
		{
			int num = base.__offset(4);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public VarContext GetReferenceParams(int j)
		{
			return this.GetReferenceParams(new VarContext(), j);
		}

		public VarContext GetReferenceParams(VarContext obj, int j)
		{
			int num = base.__offset(6);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public ActionObj GetAction(ActionObj obj)
		{
			int num = base.__offset(8);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(num + this.bb_pos), this.bb);
		}

		public static Offset<ActionData> CreateActionData(FlatBufferBuilder builder, VectorOffset templateObjsOffset = default(VectorOffset), VectorOffset referenceParamsOffset = default(VectorOffset), Offset<ActionObj> actionOffset = default(Offset<ActionObj>))
		{
			builder.StartObject(3);
			ActionData.AddAction(builder, actionOffset);
			ActionData.AddReferenceParams(builder, referenceParamsOffset);
			ActionData.AddTemplateObjs(builder, templateObjsOffset);
			return ActionData.EndActionData(builder);
		}

		public static void StartActionData(FlatBufferBuilder builder)
		{
			builder.StartObject(3);
		}

		public static void AddTemplateObjs(FlatBufferBuilder builder, VectorOffset templateObjsOffset)
		{
			builder.AddOffset(0, templateObjsOffset.Value, 0);
		}

		public static VectorOffset CreateTemplateObjsVector(FlatBufferBuilder builder, Offset<TemplateObj>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartTemplateObjsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddReferenceParams(FlatBufferBuilder builder, VectorOffset referenceParamsOffset)
		{
			builder.AddOffset(1, referenceParamsOffset.Value, 0);
		}

		public static VectorOffset CreateReferenceParamsVector(FlatBufferBuilder builder, Offset<VarContext>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartReferenceParamsVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static void AddAction(FlatBufferBuilder builder, Offset<ActionObj> actionOffset)
		{
			builder.AddOffset(2, actionOffset.Value, 0);
		}

		public static Offset<ActionData> EndActionData(FlatBufferBuilder builder)
		{
			return new Offset<ActionData>(builder.EndObject());
		}
	}
}

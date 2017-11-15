using FlatBuffers;
using System;

namespace MobaGo.FlatBuffer
{
	public sealed class HeroAgeDatas : Table
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

		public int DatasLength
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

		public static HeroAgeDatas GetRootAsHeroAgeDatas(ByteBuffer _bb)
		{
			return HeroAgeDatas.GetRootAsHeroAgeDatas(_bb, new HeroAgeDatas());
		}

		public static HeroAgeDatas GetRootAsHeroAgeDatas(ByteBuffer _bb, HeroAgeDatas obj)
		{
			return obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
		}

		public HeroAgeDatas __init(int _i, ByteBuffer _bb)
		{
			this.bb_pos = _i;
			this.bb = _bb;
			return this;
		}

		public AgeData GetDatas(int j)
		{
			return this.GetDatas(new AgeData(), j);
		}

		public AgeData GetDatas(AgeData obj, int j)
		{
			int num = base.__offset(6);
			if (num == 0)
			{
				return null;
			}
			return obj.__init(base.__indirect(base.__vector(num) + j * 4), this.bb);
		}

		public static Offset<HeroAgeDatas> CreateHeroAgeDatas(FlatBufferBuilder builder, int id = 0, VectorOffset datasOffset = default(VectorOffset))
		{
			builder.StartObject(2);
			HeroAgeDatas.AddDatas(builder, datasOffset);
			HeroAgeDatas.AddId(builder, id);
			return HeroAgeDatas.EndHeroAgeDatas(builder);
		}

		public static void StartHeroAgeDatas(FlatBufferBuilder builder)
		{
			builder.StartObject(2);
		}

		public static void AddId(FlatBufferBuilder builder, int id)
		{
			builder.AddInt(0, id, 0);
		}

		public static void AddDatas(FlatBufferBuilder builder, VectorOffset datasOffset)
		{
			builder.AddOffset(1, datasOffset.Value, 0);
		}

		public static VectorOffset CreateDatasVector(FlatBufferBuilder builder, Offset<AgeData>[] data)
		{
			builder.StartVector(4, data.Length, 4);
			for (int i = data.Length - 1; i >= 0; i--)
			{
				builder.AddOffset(data[i].Value);
			}
			return builder.EndVector();
		}

		public static void StartDatasVector(FlatBufferBuilder builder, int numElems)
		{
			builder.StartVector(4, numElems, 4);
		}

		public static Offset<HeroAgeDatas> EndHeroAgeDatas(FlatBufferBuilder builder)
		{
			return new Offset<HeroAgeDatas>(builder.EndObject());
		}
	}
}

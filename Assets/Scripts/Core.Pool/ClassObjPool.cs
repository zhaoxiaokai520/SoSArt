using System;

namespace MobaGo.Common
{
	public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
	{
		private static ClassObjPool<T> instance;

		public static uint NewSeq()
		{
			if (ClassObjPool<T>.instance == null)
			{
				ClassObjPool<T>.instance = new ClassObjPool<T>();
			}
			ClassObjPool<T>.instance.reqSeq += 1u;
			return ClassObjPool<T>.instance.reqSeq;
		}

		public static T Get()
		{
			if (ClassObjPool<T>.instance == null)
			{
				ClassObjPool<T>.instance = new ClassObjPool<T>();
			}
			if (ClassObjPool<T>.instance.pool.Count > 0)
			{
				T arg_7D_0 = (T)((object)ClassObjPool<T>.instance.pool[ClassObjPool<T>.instance.pool.Count - 1]);
				ClassObjPool<T>.instance.pool.RemoveAt(ClassObjPool<T>.instance.pool.Count - 1);
				ClassObjPool<T>.instance.reqSeq += 1u;
				arg_7D_0.usingSeq = ClassObjPool<T>.instance.reqSeq;
				arg_7D_0.holder = ClassObjPool<T>.instance;
				arg_7D_0.OnUse();
				return arg_7D_0;
			}
			T arg_C5_0 = Activator.CreateInstance<T>();
			ClassObjPool<T>.instance.reqSeq += 1u;
			arg_C5_0.usingSeq = ClassObjPool<T>.instance.reqSeq;
			arg_C5_0.holder = ClassObjPool<T>.instance;
			arg_C5_0.OnUse();
			return arg_C5_0;
		}

		public override void Release(PooledClassObject obj)
		{
			T t = obj as T;
			obj.usingSeq = 0u;
			obj.holder = null;
			this.pool.Add(t);
		}
	}
}

using System;

namespace Common
{
	public class TObjPool<T> : IObjPoolBase where T : PooledClassObject, new()
	{
		private static TObjPool<T> instance;

		public static uint NewSeq()
		{
			bool flag = TObjPool<T>.instance == null;
			if (flag)
			{
				TObjPool<T>.instance = new TObjPool<T>();
			}
			TObjPool<T>.instance.reqSeq += 1u;
			return TObjPool<T>.instance.reqSeq;
		}

		public static T Get()
		{
			bool flag = TObjPool<T>.instance == null;
			if (flag)
			{
				TObjPool<T>.instance = new TObjPool<T>();
			}
			bool flag2 = TObjPool<T>.instance.pool.Count > 0;
			T result;
			if (flag2)
			{
				T t = (T)((object)TObjPool<T>.instance.pool[TObjPool<T>.instance.pool.Count - 1]);
				TObjPool<T>.instance.pool.RemoveAt(TObjPool<T>.instance.pool.Count - 1);
				TObjPool<T>.instance.reqSeq += 1u;
				t.usingSeq = TObjPool<T>.instance.reqSeq;
				t.holder = TObjPool<T>.instance;
				t.OnUse();
				result = t;
			}
			else
			{
				T t2 = Activator.CreateInstance<T>();
				TObjPool<T>.instance.reqSeq += 1u;
				t2.usingSeq = TObjPool<T>.instance.reqSeq;
				t2.holder = TObjPool<T>.instance;
				t2.OnUse();
				result = t2;
			}
			return result;
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

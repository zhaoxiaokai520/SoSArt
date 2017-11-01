using System;
using System.Collections.Generic;

namespace MobaGo.Common
{
	public abstract class ClassObjPoolBase : IObjPool
	{
		protected List<object> pool = new List<object>(128);

		protected uint reqSeq;

		public int capacity
		{
			get
			{
				return this.pool.Capacity;
			}
			set
			{
				this.pool.Capacity = value;
			}
		}

		public abstract void Release(PooledClassObject obj);
	}
}

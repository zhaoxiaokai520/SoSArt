using System;
using System.Collections.Generic;

namespace Common
{
	public abstract class IObjPoolBase : IObjPoolCtrl
	{
		protected List<object> pool = new List<object>(64);

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

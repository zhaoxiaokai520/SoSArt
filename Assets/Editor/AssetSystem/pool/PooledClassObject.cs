using System;

namespace MobaGo.Common
{
	public class PooledClassObject
	{
        public PooledClassObject next;

        public uint usingSeq;

		public IObjPool holder;

		public bool bChkReset = true;

		public virtual void OnUse()
		{
		}

		public virtual void OnRelease()
		{
		}

		public void Release()
		{
			if (holder != null)
			{
				OnRelease();
				holder.Release(this);
			}
		}
	}
}

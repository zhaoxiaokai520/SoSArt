using System;

namespace Common
{
	public class PooledClassObject
	{
		public uint usingSeq;

		public IObjPoolCtrl holder;

		public bool bChkReset = true;

		public virtual void OnUse()
		{
		}

		public virtual void OnRelease()
		{
		}

		public void Release()
		{
			bool flag = this.holder != null;
			if (flag)
			{
				this.OnRelease();
				this.holder.Release(this);
			}
		}
	}
}

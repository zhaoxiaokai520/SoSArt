using System;

namespace MobaGo.Common
{
	public class PooledClassObject
	{
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
			if (this.holder != null)
			{
				this.OnRelease();
				this.holder.Release(this);
			}
		}
	}
}

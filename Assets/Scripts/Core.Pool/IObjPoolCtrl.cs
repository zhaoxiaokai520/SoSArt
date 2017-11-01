using System;

namespace MobaGo.Common
{
	public interface IObjPoolCtrl
	{
		void Release(PooledClassObject obj);
	}
}

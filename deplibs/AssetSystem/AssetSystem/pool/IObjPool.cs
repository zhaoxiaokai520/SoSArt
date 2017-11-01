using System;

namespace MobaGo.Common
{
	public interface IObjPool
	{
		void Release(PooledClassObject obj);
	}
}

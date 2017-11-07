using System;

namespace Common
{
	public interface IObjPoolCtrl
	{
		void Release(PooledClassObject obj);
	}
}

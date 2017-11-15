using System;

public static class PoolHandleExtension
{
	public static void Release(this ISmartObj obj)
	{
		bool flag = obj != null;
		if (flag)
		{
			AbstractSmartObj abstractSmartObj = (AbstractSmartObj)obj;
			bool flag2 = abstractSmartObj != null && abstractSmartObj.holder != null;
			if (flag2)
			{
				abstractSmartObj.holder.Recycle(obj);
			}
		}
	}
}

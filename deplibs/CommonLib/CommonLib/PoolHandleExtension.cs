using System;
using UnityEngine;

public static class PoolHandleExtension
{
	public static void Release(this ISmartObj obj)
	{
		if (obj != null)
		{
			if (obj.handle == null)
			{
				Debug.LogError("PoolHandle: target object is not initialized from SmartReferencePool type:" + obj.GetType());
				return;
			}
			obj.handle.Release(obj.index);
			obj.OnRelease();
		}
	}
}

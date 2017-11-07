using System;
using System.Collections.Generic;

public class SmartReferencePool : Singleton<SmartReferencePool>
{
	private Dictionary<Type, object> _objectPool = new Dictionary<Type, object>();

	public void CreatePool<T>(int autoCreateSize = 4, int allocGran = 4) where T : ISmartObj
	{
		Type typeFromHandle = typeof(T);
		this.CreatePool(typeFromHandle, autoCreateSize, allocGran);
	}

	public void CreatePool(Type classT, int autoCreateSize = 32, int allocGran = 4)
	{
		bool flag = classT != null && autoCreateSize > 0;
		if (flag)
		{
		}
	}

	public T Fetch<T>(int autoCreateSize = 32, int allocGran = 4) where T : AbstractSmartObj
	{
		Type typeFromHandle = typeof(T);
		bool flag = typeFromHandle != null;
		T result;
		if (flag)
		{
			bool flag2 = this._objectPool.ContainsKey(typeFromHandle);
			TPoolClass<T> tPoolClass;
			if (flag2)
			{
				tPoolClass = (TPoolClass<T>)this._objectPool[typeFromHandle];
			}
			else
			{
				tPoolClass = TPoolClass<T>._inst;
				this._objectPool.Add(typeFromHandle, tPoolClass);
			}
			result = tPoolClass.Create();
		}
		else
		{
			result = default(T);
		}
		return result;
	}
}

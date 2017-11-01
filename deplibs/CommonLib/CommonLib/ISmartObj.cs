using System;

public interface ISmartObj
{
	int index
	{
		get;
		set;
	}

	SmartReferencePool.PoolHandle handle
	{
		get;
		set;
	}

	void Reset();

	void OnRelease();
}

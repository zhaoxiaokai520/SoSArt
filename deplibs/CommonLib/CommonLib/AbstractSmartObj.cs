using System;

public abstract class AbstractSmartObj : ISmartObj
{
	private int _index = -1;

	private SmartReferencePool.PoolHandle _handle;

	public int index
	{
		get
		{
			return this._index;
		}
		set
		{
			this._index = value;
		}
	}

	public SmartReferencePool.PoolHandle handle
	{
		get
		{
			return this._handle;
		}
		set
		{
			this._handle = value;
		}
	}

	public virtual void Reset()
	{
	}

	public abstract void OnRelease();
}

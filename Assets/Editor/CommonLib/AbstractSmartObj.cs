using System;

public abstract class AbstractSmartObj : ISmartObj
{
	public ISmartObjHolder holder;

	private int _index = -1;

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

	public virtual void Reset()
	{
	}

	public abstract void OnRelease();
}

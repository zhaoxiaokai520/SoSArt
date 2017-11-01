using System;
using System.Collections.Generic;

public abstract class CUtilListBase
{
	protected List<object> Context;

	public int Count
	{
		get
		{
			return Context.Count;
		}
	}

	public void Clear()
	{
		Context.Clear();
	}

	public void RemoveAt(int index)
	{
		Context.RemoveAt(index);
	}

	public void RemoveRange(int index, int count)
	{
		Context.RemoveRange(index, count);
	}

	public void Reverse()
	{
		Context.Reverse();
	}

	public void Sort()
	{
		Context.Sort();
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using xGameUtility;

public class TPoolClass<T> : ISmartObjHolder where T : AbstractSmartObj
{
	protected static TBetterList<T> _FreeList = new TBetterList<T>();

	public static TPoolClass<T> _inst = TPoolClass<T>._TCreate();

	private static int FetchCount = 0;

	private static int HitCount = 0;

	private static int MissCount = 0;

	private static TPoolClass<T> _TCreate()
	{
		TPoolClass<T> tPoolClass = new TPoolClass<T>();
		GlobalPool.AddPoolType(tPoolClass);
		return tPoolClass;
	}

	public string Dump()
	{
		return string.Format("[{0}]FetchCount={1},Miss={2},_FreeList={3},", new object[]
		{
			typeof(T).ToString(),
			TPoolClass<T>.FetchCount,
			TPoolClass<T>.MissCount,
			TPoolClass<T>._FreeList.Count
		});
	}

	public static T AutoCreate(int nCount)
	{
		return TPoolClass<T>.AutoCreate();
	}

	public static T AutoCreate()
	{
		TPoolClass<T>.FetchCount++;
		bool flag = TPoolClass<T>._FreeList.Count > 0;
		T t;
		if (flag)
		{
			t = TPoolClass<T>._FreeList.Pop();
			t.index = -1;
		}
		else
		{
			TPoolClass<T>.MissCount++;
			t = (T)((object)Activator.CreateInstance(typeof(T)));
			t.holder = TPoolClass<T>._inst;
		}
		return t;
	}

	public T Create()
	{
		return TPoolClass<T>.AutoCreate();
	}

	public void Recycle(ISmartObj pNode)
	{
		T t = (T)((object)pNode);
		bool flag = t != null;
		if (flag)
		{
			t.OnRelease();
			bool flag2 = t.index == -1;
			if (flag2)
			{
				t.index = 1;
				TPoolClass<T>._FreeList.Add(t);
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"ERROR: TPoolClass:Recycle :已经在池内 Count=",
					TPoolClass<T>._FreeList.Count,
					";type = ",
					pNode.GetType()
				}));
			}
		}
	}

	public static void CheckRecycle(T pNode, bool bTest)
	{
		bool flag = pNode != null;
		if (flag)
		{
		}
	}

	public void RecycleArray(T[] arr)
	{
		bool flag = arr != null;
		if (flag)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				bool flag2 = arr[i] != null;
				if (flag2)
				{
					this.Recycle(arr[i]);
				}
			}
			Array.Clear(arr, 0, arr.Length);
		}
	}

	public int RecycleList(List<T> ls)
	{
		int count = ls.Count;
		for (int i = 0; i < count; i++)
		{
			this.Recycle(ls[i]);
		}
		ls.Clear();
		return count;
	}

	public int RecycleList(TBetterList<T> ls)
	{
		int count = ls.Count;
		for (int i = 0; i < count; i++)
		{
			this.Recycle(ls[i]);
		}
		ls.Clear();
		return count;
	}

	public static int GetFreeCount()
	{
		return TPoolClass<T>._FreeList.Count;
	}

	public static void EnsureSize(int Len)
	{
		TPoolClass<T>._FreeList.EnsureSize(Len);
	}

	public static void Reset()
	{
		TPoolClass<T>._FreeList.Release();
	}

	public static void ResetPool()
	{
		TPoolClass<T>._FreeList.Release();
	}
}

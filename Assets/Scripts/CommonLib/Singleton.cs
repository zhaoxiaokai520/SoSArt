using System;

public class Singleton<T> where T : class, new()
{
	private static T s_instance;

	public static T instance
	{
		get
		{
			bool flag = Singleton<T>.s_instance == null;
			if (flag)
			{
				Singleton<T>.CreateInstance();
			}
			return Singleton<T>.s_instance;
		}
	}

	protected Singleton()
	{
	}

	public static void CreateInstance()
	{
		bool flag = Singleton<T>.s_instance == null;
		if (flag)
		{
			Singleton<T>.s_instance = Activator.CreateInstance<T>();
			bool flag2 = Singleton<T>.s_instance is Singleton<T>;
			if (flag2)
			{
				(Singleton<T>.s_instance as Singleton<T>).Init();
			}
		}
	}

	public static void DestroyInstance()
	{
		bool flag = Singleton<T>.s_instance != null;
		if (flag)
		{
			(Singleton<T>.s_instance as Singleton<T>).UnInit();
			Singleton<T>.s_instance = default(T);
		}
	}

	public static T GetInstance()
	{
		bool flag = Singleton<T>.s_instance == null;
		if (flag)
		{
			Singleton<T>.CreateInstance();
		}
		return Singleton<T>.s_instance;
	}

	public static bool HasInstance()
	{
		return Singleton<T>.s_instance != null;
	}

	public virtual void Init()
	{
	}

	public virtual void UnInit()
	{
	}
}

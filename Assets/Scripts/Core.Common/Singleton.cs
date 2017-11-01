using System;
using UnityEngine;
public class AutoSingletonAttribute : Attribute
{
    public bool bAutoCreate;

    public AutoSingletonAttribute(bool bCreate)
    {
        bAutoCreate = bCreate;
    }
}

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    private static bool _destroyed;

    public static T instance
    {
        get
        {
            return MonoSingleton<T>.GetInstance();
        }
    }

    public static T GetInstance()
    {
        if (MonoSingleton<T>._instance == null && !MonoSingleton<T>._destroyed)
        {
            System.Type typeFromHandle = typeof(T);
            MonoSingleton<T>._instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeFromHandle));
            if (MonoSingleton<T>._instance == null)
            {
                object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(AutoSingletonAttribute), true);
                if (customAttributes.Length > 0 && !((AutoSingletonAttribute)customAttributes[0]).bAutoCreate)
                {
                    return (T)((object)null);
                }
                GameObject gameObject = new GameObject(typeof(T).Name);
                MonoSingleton<T>._instance = gameObject.AddComponent<T>();
                GameObject gameObject2 = GameObject.Find("BootObj");
                if (gameObject2 != null)
                {
                    gameObject.transform.SetParent(gameObject2.transform);
                }
            }
        }
        return MonoSingleton<T>._instance;
    }

    public static void DestroyInstance()
    {
        if (MonoSingleton<T>._instance != null)
        {
            UnityEngine.Object.Destroy(MonoSingleton<T>._instance.gameObject);
        }
        MonoSingleton<T>._destroyed = true;
        MonoSingleton<T>._instance = (T)((object)null);
    }

    public static void ClearDestroy()
    {
        MonoSingleton<T>.DestroyInstance();
        MonoSingleton<T>._destroyed = false;
    }

    protected virtual void Awake()
    {
        if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject != base.gameObject)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(base.gameObject);
            }
        }
        else if (MonoSingleton<T>._instance == null)
        {
            MonoSingleton<T>._instance = base.GetComponent<T>();
        }
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        Init();
    }

    protected virtual void OnDestroy()
    {
        if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject == base.gameObject)
        {
            MonoSingleton<T>._instance = (T)((object)null);
        }
    }

    public virtual void DestroySelf()
    {
        MonoSingleton<T>._instance = (T)((object)null);
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public static bool HasInstance()
    {
        return MonoSingleton<T>._instance != null;
    }

    protected virtual void Init()
    {
    }
}


public class Singleton<T> where T : class, new()
{
	private static T s_instance;

	public static T instance
	{
		get
		{
			if (Singleton<T>.s_instance == null)
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
		if (Singleton<T>.s_instance == null)
		{
            // Kaede: Activator.CreateInstance<T> is using reflection to judge which class might be instantiated.
            // by the data of experiment
            // for construct a T-like object of 100 million times 
            // Activator.CreateInstance<T>: 12342 msec
            // new T(): 1119 msec
            Singleton<T>.s_instance = new T(); //Activator.CreateInstance<T>();
			(Singleton<T>.s_instance as Singleton<T>).Init();
		}
	}

	public static void DestroyInstance()
	{
		if (Singleton<T>.s_instance != null)
		{
			(Singleton<T>.s_instance as Singleton<T>).UnInit();
            Singleton<T>.s_instance = (T)null;
		}
	}

	public static T GetInstance()
	{
		if (Singleton<T>.s_instance == null)
		{
			Singleton<T>.CreateInstance();
		}
		return Singleton<T>.s_instance;
	}

	public static bool HasInstance()
	{
		return Singleton<T>.s_instance != null;
	}

    public virtual void Init() { }

	public virtual void UnInit() { }
}

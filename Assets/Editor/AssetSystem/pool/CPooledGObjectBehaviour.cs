using System;
using UnityEngine;

public class CPooledGObjectBehaviour : MonoBehaviour
{
    public bool m_isInit;

	public Vector3 m_defaultScale;

	protected IPooledMonoBehaviour[] m_cachedIPooledMonos;

#if LOCAL_BUILD
    public string m_prefabKey = null;
    public void Initialize(string prefabKey)
    
#else
    public int m_prefabKey = 0;
    public void Initialize(int prefabKey)
#endif
    {
        MonoBehaviour[] monos = base.gameObject.GetComponentsInChildren<MonoBehaviour>(true);
        if (monos != null && monos.Length > 0)
		{
			int count = 0;
            for (int i = 0; i < monos.Length; i++)
			{
                if (monos[i] is IPooledMonoBehaviour)
				{
                    count++;
				}
			}
            m_cachedIPooledMonos = new IPooledMonoBehaviour[count];
			int index = 0;
            for (int j = 0; j < monos.Length; j++)
			{
                if (monos[j] is IPooledMonoBehaviour)
				{
                    m_cachedIPooledMonos[index] = (monos[j] as IPooledMonoBehaviour);
					index++;
				}
			}
		}
		else
		{
			m_cachedIPooledMonos = new IPooledMonoBehaviour[0];
		}
		m_prefabKey = prefabKey;
		m_defaultScale = base.gameObject.transform.localScale;
		m_isInit = true;
	}

	public void AddCachedMono(MonoBehaviour mono, bool defaultEnabled)
	{
		if (mono == null)
		{
			return;
		}
		if (mono is IPooledMonoBehaviour)
		{
			IPooledMonoBehaviour[] array = new IPooledMonoBehaviour[m_cachedIPooledMonos.Length + 1];
			for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
			{
				array[i] = m_cachedIPooledMonos[i];
			}
			array[m_cachedIPooledMonos.Length] = (mono as IPooledMonoBehaviour);
			m_cachedIPooledMonos = array;
		}
	}

    virtual public void OnCreate()
	{
		if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
		{
			for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
			{
				if (m_cachedIPooledMonos[i] != null)
				{
					m_cachedIPooledMonos[i].OnCreate();
				}
			}
		}
	}

	virtual public void OnGet()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}

        if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
		{
			for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
			{
				if (m_cachedIPooledMonos[i] != null)
				{
					m_cachedIPooledMonos[i].OnGet();
				}
			}
		}
	}

    virtual public void OnRecycle()
	{
		if (m_cachedIPooledMonos != null && m_cachedIPooledMonos.Length > 0)
		{
			for (int i = 0; i < m_cachedIPooledMonos.Length; i++)
			{
				if (m_cachedIPooledMonos[i] != null)
				{
					m_cachedIPooledMonos[i].OnRecycle();
				}
			}
		}
		base.gameObject.SetActive(false);
	}

    virtual public void OnPrepare()
	{
		base.gameObject.SetActive(false);
	}
}

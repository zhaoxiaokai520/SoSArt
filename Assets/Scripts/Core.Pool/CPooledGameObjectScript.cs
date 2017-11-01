using System;
using UnityEngine;

public class CPooledGameObjectScript : MonoBehaviour
{
	public string m_prefabKey;

	public bool m_isInit;

	public Vector3 m_defaultScale;

	private IPooledMonoBehaviour[] m_cachedIPooledMonos;

	private bool m_inUse;

	public void Initialize(string prefabKey)
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
		m_inUse = false;
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

	public void OnCreate()
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

	public void OnGet()
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
		m_inUse = true;
	}

	public void OnRecycle()
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
		m_inUse = false;
	}

	public void OnPrepare()
	{
		base.gameObject.SetActive(false);
	}
}

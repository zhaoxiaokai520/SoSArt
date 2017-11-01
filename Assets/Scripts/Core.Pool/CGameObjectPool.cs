using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CUtilObjectPool : Singleton<CUtilObjectPool>
{
	private class DelayRecycle_t
	{
		public GameObject recycleObj;

		public int timeMillSecondsLeft;

		public CUtilObjectPool.OnDelayRecycleDelegate callback;
	}

	public delegate void OnDelayRecycleDelegate(GameObject recycleObj);

	private CUtilDic<string, Queue<CPooledGameObjectScript>> m_pooledGameObjectMap = new CUtilDic<string, Queue<CPooledGameObjectScript>>();

	private LinkedList<CUtilObjectPool.DelayRecycle_t> m_delayRecycle = new LinkedList<CUtilObjectPool.DelayRecycle_t>();

	private GameObject m_poolRoot;

	private bool m_clearPooledObjects;

	private int m_clearPooledObjectsExecuteFrame;

	private static int s_frameCounter;

	public override void Init()
	{
		m_poolRoot = new GameObject("CUtilObjectPool");
		GameObject gameObject = GameObject.Find("BootObj");
		if (gameObject != null)
		{
			m_poolRoot.transform.SetParent(gameObject.transform);
		}
	}

	public override void UnInit()
	{
	}

	public void Update()
	{
		CUtilObjectPool.s_frameCounter++;
		UpdateDelayRecycle();
		if (m_clearPooledObjects && m_clearPooledObjectsExecuteFrame == CUtilObjectPool.s_frameCounter)
		{
			ExecuteClearPooledObjects();
			m_clearPooledObjects = false;
		}
	}

	public void ClearPooledObjects()
	{
		m_clearPooledObjects = true;
		m_clearPooledObjectsExecuteFrame = CUtilObjectPool.s_frameCounter + 1;
	}

	public void ExecuteClearPooledObjects()
	{
		for (LinkedListNode<CUtilObjectPool.DelayRecycle_t> linkedListNode = m_delayRecycle.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (null != linkedListNode.Value.recycleObj)
			{
				RecycleGameObject(linkedListNode.Value.recycleObj);
			}
		}
		m_delayRecycle.Clear();
		CUtilDic<string, Queue<CPooledGameObjectScript>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Queue<CPooledGameObjectScript>> current = enumerator.Current;
			Queue<CPooledGameObjectScript> value = current.Value;
			while (value.Count > 0)
			{
				CPooledGameObjectScript cPooledGameObjectScript = value.Dequeue();
				if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
				{
					UnityEngine.Object.Destroy(cPooledGameObjectScript.gameObject);
				}
			}
		}
		m_pooledGameObjectMap.Clear();
	}

	public void UpdateParticleChecker(int maxNum)
	{
	}

	private void UpdateDelayRecycle()
	{
		int num = (int)(1000f * Time.deltaTime);
		LinkedListNode<CUtilObjectPool.DelayRecycle_t> linkedListNode = m_delayRecycle.First;
		while (linkedListNode != null)
		{
			LinkedListNode<CUtilObjectPool.DelayRecycle_t> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode2.Next;
			if (null == linkedListNode2.Value.recycleObj)
			{
				m_delayRecycle.Remove(linkedListNode2);
			}
			else
			{
				linkedListNode2.Value.timeMillSecondsLeft -= num;
				if (linkedListNode2.Value.timeMillSecondsLeft <= 0)
				{
					if (linkedListNode2.Value.callback != null)
					{
						linkedListNode2.Value.callback(linkedListNode2.Value.recycleObj);
					}
					RecycleGameObject(linkedListNode2.Value.recycleObj);
					m_delayRecycle.Remove(linkedListNode2);
				}
			}
		}
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, kAssetType resourceType)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, kAssetType resourceType, out bool isInit)
	{
		return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, kAssetType resourceType)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, kAssetType resourceType, out bool isInit)
	{
		return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit);
	}

	public GameObject GetGameObject(string prefabFullPath, kAssetType resourceType)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, kAssetType resourceType, out bool isInit)
	{
		return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit);
	}

	private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, kAssetType resourceType, out bool isInit)
	{
		string text = CFileManager.EraseExtension(prefabFullPath).ToLower();
		Queue<CPooledGameObjectScript> queue = null;
		if (!m_pooledGameObjectMap.TryGetValue(text, out queue))
		{
			queue = new Queue<CPooledGameObjectScript>();
			m_pooledGameObjectMap.Add(text, queue);
		}
		CPooledGameObjectScript cPooledGameObjectScript = null;
		while (queue.Count > 0)
		{
			cPooledGameObjectScript = queue.Dequeue();
			if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
			{
				cPooledGameObjectScript.gameObject.transform.SetParent(null, true);
				cPooledGameObjectScript.gameObject.transform.position=(pos);
				cPooledGameObjectScript.gameObject.transform.rotation=(rot);
				cPooledGameObjectScript.gameObject.transform.localScale=(cPooledGameObjectScript.m_defaultScale);
				break;
			}
			cPooledGameObjectScript = null;
		}
		if (cPooledGameObjectScript == null)
		{
			cPooledGameObjectScript = CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, text);
		}
		if (cPooledGameObjectScript == null)
		{
			isInit = false;
			return null;
		}
		isInit = cPooledGameObjectScript.m_isInit;
		cPooledGameObjectScript.OnGet();
		return cPooledGameObjectScript.gameObject;
	}

	public void RecycleGameObjectDelay(GameObject pooledGameObject, int delayMillSeconds, CUtilObjectPool.OnDelayRecycleDelegate callback = null)
	{
		CUtilObjectPool.DelayRecycle_t DelayRecycle_t = new CUtilObjectPool.DelayRecycle_t();
		DelayRecycle_t.recycleObj = pooledGameObject;
		DelayRecycle_t.timeMillSecondsLeft = delayMillSeconds;
		DelayRecycle_t.callback = callback;
		m_delayRecycle.AddLast(DelayRecycle_t);
	}

	public void RecycleGameObject(GameObject pooledGameObject)
	{
		_RecycleGameObject(pooledGameObject, false);
	}

	public void RecyclePreparedGameObject(GameObject pooledGameObject)
	{
		_RecycleGameObject(pooledGameObject, true);
	}

	private void _RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
	{
		if (pooledGameObject == null)
		{
			return;
		}
		CPooledGameObjectScript poolScript = pooledGameObject.GetComponent<CPooledGameObjectScript>();
        if (poolScript != null)
		{
			Queue<CPooledGameObjectScript> queue = null;
            if (m_pooledGameObjectMap.TryGetValue(poolScript.m_prefabKey, out queue))
			{
                queue.Enqueue(poolScript);
                poolScript.OnRecycle();
                poolScript.gameObject.transform.SetParent(m_poolRoot.transform, true);
                poolScript.m_isInit = setIsInit;
				return;
			}
		}
		UnityEngine.Object.Destroy(pooledGameObject);
	}

	public void PrepareGameObject(string prefabFullPath, kAssetType resourceType, int amount)
	{
		string text = CFileManager.EraseExtension(prefabFullPath).ToLower();
		Queue<CPooledGameObjectScript> queue = null;
		if (!m_pooledGameObjectMap.TryGetValue(text, out queue))
		{
			queue = new Queue<CPooledGameObjectScript>();
			m_pooledGameObjectMap.Add(text, queue);
		}
		if (queue.Count >= amount)
		{
			return;
		}
		amount -= queue.Count;
		for (int i = 0; i < amount; i++)
		{
			CPooledGameObjectScript cPooledGameObjectScript = CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, text);
			if (cPooledGameObjectScript != null)
			{
				queue.Enqueue(cPooledGameObjectScript);
				cPooledGameObjectScript.gameObject.transform.SetParent(m_poolRoot.transform, true);
				cPooledGameObjectScript.OnPrepare();
			}
		}
	}

	private CPooledGameObjectScript CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, kAssetType resourceType, string prefabKey)
	{
		bool needCached = resourceType == kAssetType.BattleScene;
		GameObject gameObject = AssetSystem.instance.GetAsset(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2;
		if (useRotation)
		{
			gameObject2 = (UnityEngine.Object.Instantiate(gameObject, pos, rot) as GameObject);
		}
		else
		{
			gameObject2 = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
			gameObject2.transform.position=(pos);
		}
		CPooledGameObjectScript cPooledGameObjectScript = gameObject2.GetComponent<CPooledGameObjectScript>();
		if (cPooledGameObjectScript == null)
		{
			cPooledGameObjectScript = gameObject2.AddComponent<CPooledGameObjectScript>();
		}
		cPooledGameObjectScript.Initialize(prefabKey);
		cPooledGameObjectScript.OnCreate();
		return cPooledGameObjectScript;
	}
}

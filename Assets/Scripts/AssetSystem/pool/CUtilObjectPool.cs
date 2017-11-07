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
#if LOCAL_BUILD
    private CUtilDic<string, Queue<CPooledGObjectBehaviour>> m_pooledGameObjectMap = new CUtilDic<string, Queue<CPooledGObjectBehaviour>>();
#else
    private CUtilDic<int, Queue<CPooledGObjectBehaviour>> m_pooledGameObjectMap = new CUtilDic<int, Queue<CPooledGObjectBehaviour>>();
#endif
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
#if DEBUG_LOGOUT
        DebugHelper.LogOutResouceLoad("******************************************END********************************************************");
        //  ActionSystem.instance.LogActionPool();
        //GameCharMgr.instance.LogGameCharPool();
         CUtilObjectPool.instance.LogGameObjectPool();
#endif
        m_delayRecycle.Clear();
#if LOCAL_BUILD
        CUtilDic<string, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;
#else
        CUtilDic<int, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;
#endif
            Queue<CPooledGObjectBehaviour> value = current.Value;
			while (value.Count > 0)
			{
				CPooledGObjectBehaviour cPooledGameObjectScript = value.Dequeue();
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
		int num = (int)(1000f * Time.smoothDeltaTime);
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

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, eAssetType resourceType, Type poolType = null)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out flag, poolType);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, eAssetType resourceType, out bool isInit, Type poolType = null)
	{
		return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit, poolType);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, eAssetType resourceType, Type poolType = null)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out flag, poolType);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, eAssetType resourceType, out bool isInit, Type poolType = null)
	{
		return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit, poolType);
	}

	public GameObject GetGameObject(string prefabFullPath, eAssetType resourceType, Type poolType = null)
	{
		bool flag = false;
		return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out flag, poolType);
	}

	public GameObject GetGameObject(string prefabFullPath, eAssetType resourceType, out bool isInit, Type poolType = null)
	{
		return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit, poolType);
	}

	private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, eAssetType resourceType, out bool isInit, Type poolType)
	{
        bool needCached = resourceType == eAssetType.BattleScene;
        GameObject gameObject = AssetSystem.instance.GetAsset(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
        if (gameObject == null)
        {
            isInit = false;
            return null;
        }
#if LOCAL_BUILD
        string text = CFileManager.ToKey(prefabFullPath);
#else
        int text = gameObject.GetInstanceID();
#endif
        Queue<CPooledGObjectBehaviour> queue = null;
		if (!m_pooledGameObjectMap.TryGetValue(text, out queue))
		{
			queue = new Queue<CPooledGObjectBehaviour>();
			m_pooledGameObjectMap.Add(text, queue);
		}
		CPooledGObjectBehaviour cPooledGameObjectScript = null;
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
			cPooledGameObjectScript = CreateGameObjectWithPool(gameObject, pos, useRotation ? rot : Quaternion.identity, text, poolType);
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

#if DEBUG_LOGOUT
    public void LogGameObjectPool()
    {
        int counter = 0;
#if LOCAL_BUILD
        CUtilDic<string, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;
#else
        CUtilDic<int, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;
#endif
            DebugHelper.LogOutResouceLoad("LogGameObjectPool : " + current.Key + " Counter : " + current.Value.Count  + "\n");
            counter += current.Value.Count;
        }
        DebugHelper.LogOutResouceLoad("****************************************GameEffectObjCounter : " + counter + "************************************");
    }
#endif

    public void LogGameObjectPoolPC()
    {
        System.IO.StreamWriter sw = System.IO.File.CreateText("D:/objpool.txt");
        int counter = 0;
#if LOCAL_BUILD
        CUtilDic<string, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;


#else
        CUtilDic<int, Queue<CPooledGObjectBehaviour>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<int, Queue<CPooledGObjectBehaviour>> current = enumerator.Current;

#endif

            sw.WriteLine(current.Key + " Counter : " + current.Value.Count);
            counter += current.Value.Count;
        }
        sw.WriteLine("Total : " + counter);
        sw.Flush();
        sw.Close();
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
		CPooledGObjectBehaviour poolScript = pooledGameObject.GetComponent<CPooledGObjectBehaviour>();
#if LOCAL_BUILD
        if (poolScript != null && poolScript.m_prefabKey != null)
#else
        if (poolScript != null && poolScript.m_prefabKey != 0)
#endif
        {
            Queue<CPooledGObjectBehaviour> queue = null;
            if (m_pooledGameObjectMap != null && m_pooledGameObjectMap.TryGetValue(poolScript.m_prefabKey, out queue))
			{
			    if (null != queue)
			    {
                    queue.Enqueue(poolScript);
			    }
                poolScript.OnRecycle();
                poolScript.m_isInit = setIsInit;
                //protect null exception
                if (null != m_poolRoot)
                {
                    poolScript.gameObject.transform.SetParent(m_poolRoot.transform, true);
                }
				return;
			}
		}
		UnityEngine.Object.Destroy(pooledGameObject);
	}

	public void PrepareGameObject(string prefabFullPath, eAssetType resourceType, int amount, Type poolType)
	{
        bool needCached = resourceType == eAssetType.BattleScene;
        GameObject gameObject = AssetSystem.instance.GetAsset(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
        if (gameObject == null)
        {
            return;
        }

        Queue<CPooledGObjectBehaviour> queue = null;

#if LOCAL_BUILD
        string text = CFileManager.ToKey(prefabFullPath);
#else
        int text = gameObject.GetInstanceID();
#endif
        if (!m_pooledGameObjectMap.TryGetValue(text, out queue))
        {
            queue = new Queue<CPooledGObjectBehaviour>();
			m_pooledGameObjectMap.Add(text, queue);
		}
		if (queue.Count >= amount)
		{
			return;
		}
		amount -= queue.Count;
		for (int i = 0; i < amount; i++)
		{
			CPooledGObjectBehaviour cPooledGameObjectScript = CreateGameObjectWithPool(gameObject, Vector3.zero, Quaternion.identity, text, poolType);
            //DebugHelper.Assert(cPooledGameObjectScript != null, "Failed Create Game object from \"{0}\"", new object[]
            //{
            //    prefabFullPath
            //});
			if (cPooledGameObjectScript != null)
			{
				queue.Enqueue(cPooledGameObjectScript);
				cPooledGameObjectScript.gameObject.transform.SetParent(m_poolRoot.transform, true);
				cPooledGameObjectScript.OnPrepare();
			}
		}
    }

#if LOCAL_BUILD
    private CPooledGObjectBehaviour CreateGameObjectWithPool(GameObject gameObject, Vector3 pos, Quaternion rot, string prefabKey, Type poolType)
#else
    private CPooledGObjectBehaviour CreateGameObjectWithPool(GameObject gameObject, Vector3 pos, Quaternion rot, int prefabKey, Type poolType)
#endif
    {
        if (gameObject == null)
        {
            Debug.LogError("Pool error:" + prefabKey);
            return null;
        }

        GameObject gameObject2 = (UnityEngine.Object.Instantiate(gameObject, pos, rot) as GameObject);

        if (poolType == null)
        {
           poolType = typeof(CPooledGObjectBehaviour);
        }

		//DebugHelper.Assert(gameObject2 != null);
		CPooledGObjectBehaviour cPooledGameObjectScript = gameObject2.GetComponent<CPooledGObjectBehaviour>();

        if(cPooledGameObjectScript != null && !cPooledGameObjectScript.GetType().Equals(poolType))
        {
            GameObject.DestroyImmediate(cPooledGameObjectScript);
            cPooledGameObjectScript = null;
            Debug.LogError("CPooledGObjectBehaviour error:" + gameObject2.name);
        }

        if (cPooledGameObjectScript == null)
		{
			cPooledGameObjectScript = gameObject2.AddComponent(poolType) as CPooledGObjectBehaviour;
		}
		cPooledGameObjectScript.Initialize(prefabKey);
		cPooledGameObjectScript.OnCreate();
		return cPooledGameObjectScript;
	}
}

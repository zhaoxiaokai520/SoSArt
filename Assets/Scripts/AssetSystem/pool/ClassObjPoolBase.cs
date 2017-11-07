using System;
using System.Collections;
using System.Collections.Generic;
using xGameUtility;

namespace MobaGo.Common
{
	public abstract class ClassObjPoolBase : IObjPool
	{

#if DEBUG_LOGOUT
        protected Hashtable monitor = new Hashtable();
#endif
        protected PooledClassObject mBegin;
        protected int poolCount = 0;
        protected uint reqSeq;
 
        public void ObjPoolSet(PooledClassObject obj)
        {
            poolCount++;
            obj.next = mBegin;
            mBegin = obj;
        }

        public object ObjPoolGet()
        {
            PooledClassObject getObject = mBegin;
            if(getObject != null)
            {
                mBegin = getObject.next;
                getObject.next = null;
                poolCount--;
            }
            return (object)getObject;
        }

        public abstract void Release(PooledClassObject obj);
#if DEBUG_LOGOUT
        public abstract void Cleanup();
#endif
    }


    //zzAdd:for 
    public class TSimplePoolClass<T>
        where T : new()
    {
        protected static TBetterList<T> _FreeList = new TBetterList<T>();

        static int FetchCount = 0;
        static int HitCount = 0;
        static int MissCount = 0;
        public string Dump()
        {
            return string.Format("[{0}]FetchCount={1},Miss={2},_FreeList={3},", typeof(T).ToString(), FetchCount, MissCount, _FreeList.Count);
        }
        //预先创建一些..
        public static T AutoCreate(int nCount)
        {
            return AutoCreate();
        }
        public static T AutoCreate()
        {
            FetchCount++;
            T pNode;
            if (_FreeList.Count > 0)
            {
                // HitCount++;
                //int index = _FreeList.Count-1;
                //pNode = _FreeList[index];
                //UnityEngine.Debug.LogWarning("txzModi: TPoolClass:AutoCreate: Count=" + _FreeList.Count + ";type = " + pNode.GetType());
                //_FreeList.RemoveAt(index);
                pNode = _FreeList.Pop();
            }
            else
            {
                MissCount++;
                pNode = new T();
            }
            return pNode;
        }

        //public T Create()
        //{
        //    return AutoCreate();
        //}

        public static void Recycle(T pNode)
        {
            if (pNode != null)
            {
                _FreeList.Add(pNode);
            }
        }
    }


}

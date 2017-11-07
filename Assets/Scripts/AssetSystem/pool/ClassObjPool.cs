using System.Collections.Generic;
using UnityEngine;

namespace MobaGo.Common
{
    public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
    {
        private static ClassObjPool<T> instance;

        public static uint NewSeq()
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }
            ClassObjPool<T>.instance.reqSeq += 1u;
            return ClassObjPool<T>.instance.reqSeq;
        }
#if DEBUG_LOGOUT
        private void AddToMonitor(T obj)
        {
            List<T> cache = null;
            if (ClassObjPool<T>.instance.monitor.ContainsKey(typeof(T)))
            {
                cache = (List<T>)ClassObjPool<T>.instance.monitor[typeof(T)];
            }
            else
            {
                cache = new List<T>();
                ClassObjPool<T>.instance.monitor[typeof(T)] = cache;
            }

            cache.Add(obj);
        }
#endif
        public int maxSize = 0;
        public static void Allocate(int Size, bool append = false)
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }

            int newSize = Size;
            if (!append)
            {
                if (ClassObjPool<T>.instance.poolCount >= Size)
                {
                    return;
                }
            }
            else
            {
                newSize = Size + ClassObjPool<T>.instance.poolCount;
            }

            int oldsize = ClassObjPool<T>.instance.poolCount;
            for (int i = oldsize; i < newSize; i++)
            {
                T tnew = new T();
                ClassObjPool<T>.instance.ObjPoolSet(tnew);
            }

            ClassObjPool<T>.instance.maxSize = ClassObjPool<T>.instance.poolCount;
        }
#if DEBUG_LOGOUT
        public static int GetCount()
        {
            if (ClassObjPool<T>.instance == null)
            {
                return 0;
            }
            return ClassObjPool<T>.instance.poolCount;

        }

        public static T TestGet(int index)
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }

            T t = (T)ClassObjPool<T>.instance.ObjPoolGet();
            if (t != null)
            {
                t.usingSeq = ClassObjPool<T>.instance.reqSeq;
                t.holder = ClassObjPool<T>.instance;
                return t;
            }
            return null;
        }
#endif

        public static T Get()
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }

            T t = (T)ClassObjPool<T>.instance.ObjPoolGet();
            if (t != null)
            {
                ClassObjPool<T>.instance.reqSeq += 1u;
                t.usingSeq = ClassObjPool<T>.instance.reqSeq;
                t.holder = ClassObjPool<T>.instance;
                t.OnUse();
#if DEBUG_LOGOUT
                ClassObjPool<T>.instance.AddToMonitor(t);
#endif
                return t;
            }

            T t2 = new T();
            ClassObjPool<T>.instance.reqSeq += 1u;
            t2.usingSeq = ClassObjPool<T>.instance.reqSeq;
            t2.holder = ClassObjPool<T>.instance;
            t2.OnUse();

            ClassObjPool<T>.instance.maxSize++;
            //if (AssetGroupInfo_t.s_inBattle_no_bundle_load)
            //    UnityEngine.Debug.LogError(typeof(T).ToString() + "poolgrowupto " + ClassObjPool<T>.instance.maxSize);
#if DEBUG_LOGOUT
            ClassObjPool<T>.instance.AddToMonitor(t2);
#endif
            return t2;
        }

        public override void Release(PooledClassObject obj)
        {
            T t = obj as T;
            obj.usingSeq = 0u;
            obj.holder = null;

            ObjPoolSet(t);

#if DEBUG_LOGOUT
            if (monitor.ContainsKey(typeof(T)))
            {
                List<T> lst = (List<T>)monitor[typeof(T)];
                lst.Remove(t);
            }
#endif

        }
#if DEBUG_LOGOUT
        public override void Cleanup()
        {
            System.Collections.IDictionaryEnumerator iter = monitor.GetEnumerator();
            while (iter != null && iter.MoveNext())
            {
                List<T> lst = (List<T>)iter.Value;
                if (lst != null)
                {
                    for (int ii = 0; ii < lst.Count; ++ii)
                    {
                        Release(lst[ii]);
                    }
                    lst.Clear();
                }
            }
            monitor.Clear();
        }

        public static void GarbageCollect()
        {
            if (ClassObjPool<T>.instance != null)
                ClassObjPool<T>.instance.Cleanup();
        }
#endif
    }
}

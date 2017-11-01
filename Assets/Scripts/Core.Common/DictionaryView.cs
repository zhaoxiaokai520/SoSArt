using System;
using System.Collections;
using System.Collections.Generic;

public class CUtilDic<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<TKey, object> Reference;

		private Dictionary<TKey, object>.Enumerator Iter;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
                KeyValuePair<TKey, object> current = Iter.Current;
                
                if (current.Value != null)
				{
				    return new KeyValuePair<TKey, TValue>(current.Key, (TValue)current.Value);
				}
				
				return new KeyValuePair<TKey, TValue>(current.Key, default(TValue));
			}
		}

		public Enumerator(Dictionary<TKey, object> InReference)
		{
			Reference = InReference;
			Iter = Reference.GetEnumerator();
		}

		public void Reset()
		{
			Iter = Reference.GetEnumerator();
		}

		public void Dispose()
		{
			Iter.Dispose();
			Reference = null;
		}

		public bool MoveNext()
		{
			return Iter.MoveNext();
		}
	}

	protected Dictionary<TKey, object> Context;

	public int Count
	{
		get
		{
			return Context.Count;
		}
	}

	public TValue this[TKey idx]
	{
		get
		{
            if ( Context.ContainsKey(idx) )
            {
                return (TValue)Context[idx];
            }

            return default(TValue);			
		}
		set
		{
            Context[idx] = value;
		}
	}

	public Dictionary<TKey, object>.KeyCollection Keys
	{
		get
		{
			return Context.Keys;
		}
	}

	public CUtilDic()
	{
		Context = new Dictionary<TKey, object>();
	}

	public CUtilDic(int capacity)
	{
		Context = new Dictionary<TKey, object>(capacity);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(TKey key, TValue value)
	{
		Context.Add(key, value);
	}

	public void Clear()
	{
		Context.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return Context.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return Context.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		object obj = null;
		
        bool result = Context.TryGetValue(key, out obj);
        value = result ? (TValue)obj : default(TValue);
		
		return result;
	}

	public CUtilDic<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new CUtilDic<TKey, TValue>.Enumerator(Context);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;

public class CUtilObjectDic<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<object, object> Reference;

		private Dictionary<object, object>.Enumerator Iter;

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
				KeyValuePair<object, object> current = Iter.Current;
                TKey tKey;
				if (current.Key != null)
				{
					tKey = (TKey)current.Key;
				}
				else
				{
                    tKey = default(TKey);
				}

				TValue tVal;
                if (current.Value != null)
				{
					tVal = (TValue)current.Value;
				}
				else
				{
                    tVal = default(TValue);
				}
				return new KeyValuePair<TKey, TValue>(tKey, tVal);
			}
		}

		public Enumerator(Dictionary<object, object> InReference)
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

	protected Dictionary<object, object> Context;

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
            if (Context.ContainsKey(idx))
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

	public CUtilObjectDic()
	{
		Context = new Dictionary<object, object>();
	}

	public CUtilObjectDic(int capacity)
	{
		Context = new Dictionary<object, object>(capacity);
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

	public CUtilObjectDic<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new CUtilObjectDic<TKey, TValue>.Enumerator(Context);
	}
}

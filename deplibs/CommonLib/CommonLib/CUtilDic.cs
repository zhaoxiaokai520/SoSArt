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
				KeyValuePair<TKey, object> current = this.Iter.Current;
				TKey arg_4C_0 = current.Key;
				KeyValuePair<TKey, object> current2 = this.Iter.Current;
				TValue value;
				if (current2.Value != null)
				{
					KeyValuePair<TKey, object> current3 = this.Iter.Current;
					value = (TValue)((object)current3.Value);
				}
				else
				{
					value = default(TValue);
				}
				return new KeyValuePair<TKey, TValue>(arg_4C_0, value);
			}
		}

		public Enumerator(Dictionary<TKey, object> InReference)
		{
			this.Reference = InReference;
			this.Iter = this.Reference.GetEnumerator();
		}

		public void Reset()
		{
			this.Iter = this.Reference.GetEnumerator();
		}

		public void Dispose()
		{
			this.Iter.Dispose();
			this.Reference = null;
		}

		public bool MoveNext()
		{
			return this.Iter.MoveNext();
		}
	}

	protected Dictionary<TKey, object> Context;

	public int Count
	{
		get
		{
			return this.Context.Count;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			object obj = this.Context[key];
			if (obj != null)
			{
				return (TValue)((object)obj);
			}
			return default(TValue);
		}
		set
		{
			this.Context[key] = value;
		}
	}

	public Dictionary<TKey, object>.KeyCollection Keys
	{
		get
		{
			return this.Context.Keys;
		}
	}

	public CUtilDic()
	{
		this.Context = new Dictionary<TKey, object>();
	}

	public CUtilDic(int capacity)
	{
		this.Context = new Dictionary<TKey, object>(capacity);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(TKey key, TValue value)
	{
		this.Context.Add(key, value);
	}

	public void Clear()
	{
		this.Context.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return this.Context.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return this.Context.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		object obj = null;
		bool arg_2A_0 = this.Context.TryGetValue(key, out obj);
		value = ((obj == null) ? default(TValue) : ((TValue)((object)obj)));
		return arg_2A_0;
	}

	public CUtilDic<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new CUtilDic<TKey, TValue>.Enumerator(this.Context);
	}
}

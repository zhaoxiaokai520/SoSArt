using System;
using System.Collections;
using System.Collections.Generic;

public class CUtilList<T> : CUtilListBase, IEnumerable, IEnumerable<T>
{
	private struct ComparerConverter : IComparer<object>
	{
		private IComparer<T> ComparerRef;

		public ComparerConverter(IComparer<T> comparer)
		{
			ComparerRef = comparer;
		}

		public int Compare(object x, object y)
		{
			return ComparerRef.Compare((T)x, (T)y);
		}
	}

	private struct ComparisonConverter : IComparer<object>
	{
		private Comparison<T> ComparerRef;

		public ComparisonConverter(Comparison<T> comparer)
		{
			ComparerRef = comparer;
		}

		public int Compare(object x, object y)
		{
			return ComparerRef.Invoke((T)x, (T)y);
		}
	}

	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		private List<object> Reference;

		private List<object>.Enumerator Iter;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public T Current
		{
			get
			{
                return (T)Iter.Current;
			}
		}

		public Enumerator(List<object> InReference)
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

	public T this[int index]
	{
		get
		{
            //if (Context.Contains(index))
            //if( index>=0 && index<Context.Count )
            //{
            //    return (T)Context[index];
            //}

            //return default(T);
            return (T)Context[index];
		}
		set
		{
			Context[index] = value;
		}
	}

	public CUtilList()
	{
		Context = new List<object>();
	}

	public CUtilList(IEnumerable<T> collection)
	{
		Context = new List<object>();
		IEnumerator<T> iter = collection.GetEnumerator();
        while (iter.MoveNext())
		{
            Context.Add(iter.Current);
		}
	}

	public CUtilList(int capacity)
	{
		Context = new List<object>(capacity);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(T item)
	{
		Context.Add(item);
	}

	public void AddRange(IEnumerable<T> collection)
	{
		if (collection != null)
		{
            IEnumerator<T> iter = collection.GetEnumerator();
            while (iter.MoveNext())
			{
                Context.Add(iter.Current);
			}
		}
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		return Context.BinarySearch(item, new CUtilList<T>.ComparerConverter(comparer));
	}

	public bool Contains(T item)
	{
		return Context.Contains(item);
	}

	public int IndexOf(T item)
	{
		return Context.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		Context.Insert(index, item);
	}

	public int LastIndexOf(T item)
	{
		return Context.LastIndexOf(item);
	}

	public bool Remove(T item)
	{
		return Context.Remove(item);
	}

	public void Sort(IComparer<T> comparer)
	{
		Context.Sort(new CUtilList<T>.ComparerConverter(comparer));
	}

	public void Sort(Comparison<T> comparison)
	{
		Context.Sort(new CUtilList<T>.ComparisonConverter(comparison));
	}

	public CUtilList<T>.Enumerator GetEnumerator()
	{
		return new CUtilList<T>.Enumerator(Context);
	}
}

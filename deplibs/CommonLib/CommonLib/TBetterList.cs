using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace xGameUtility
{
	public class TBetterList<T>
	{
		public delegate int CompareFunc(T left, T right);

		public T[] buffer;

		public int size = 0;

		public int Count
		{
			get
			{
				return this.size;
			}
		}

		[DebuggerHidden]
		public T this[int i]
		{
			get
			{
				return this.buffer[i];
			}
			set
			{
				this.buffer[i] = value;
			}
		}

		[DebuggerHidden, DebuggerStepThrough]
		public IEnumerator<T> GetEnumerator()
		{
			bool flag = this.buffer != null;
			if (flag)
			{
				int num;
				for (int i = 0; i < this.size; i = num)
				{
					yield return this.buffer[i];
					num = i + 1;
				}
			}
			yield break;
		}

		private void ReAllocate(int Len)
		{
			T[] array = new T[Len];
			bool flag = this.buffer != null && this.size > 0;
			if (flag)
			{
				this.buffer.CopyTo(array, 0);
			}
			this.buffer = array;
		}

		private void AllocateMore()
		{
			this.ReAllocate((this.buffer != null) ? Mathf.Max(this.buffer.Length << 1, 32) : 32);
		}

		private void EnsureSpace(int addLen)
		{
			bool flag = this.buffer != null && this.buffer.Length >= this.size + addLen;
			if (!flag)
			{
				this.ReAllocate((this.buffer != null) ? Mathf.Max(this.size + addLen << 1, 32) : 32);
			}
		}

		public void EnsureSize(int Len)
		{
			bool flag = this.buffer != null && this.buffer.Length >= Len;
			if (!flag)
			{
				this.ReAllocate(Len);
			}
		}

		private void Trim()
		{
			bool flag = this.size > 0;
			if (flag)
			{
				bool flag2 = this.size < this.buffer.Length;
				if (flag2)
				{
					T[] array = new T[this.size];
					for (int i = 0; i < this.size; i++)
					{
						array[i] = this.buffer[i];
					}
					this.buffer = array;
				}
			}
			else
			{
				this.buffer = null;
			}
		}

		public void Reverse()
		{
			bool flag = this.size > 1;
			if (flag)
			{
				int num = this.size - 1;
				int num2 = this.size >> 1;
				for (int i = 0; i < num2; i++)
				{
					T t = this.buffer[i];
					this.buffer[i] = this.buffer[num - i];
					this.buffer[num - i] = t;
				}
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.size; i++)
			{
				this.buffer[i] = default(T);
			}
			this.size = 0;
		}

		public void Release()
		{
			this.Clear();
			this.buffer = null;
		}

		public void Add(T item)
		{
			bool flag = this.buffer == null || this.size == this.buffer.Length;
			if (flag)
			{
				this.AllocateMore();
			}
			T[] arg_3F_0 = this.buffer;
			int num = this.size;
			this.size = num + 1;
			arg_3F_0[num] = item;
		}

		public void Add(T[] items)
		{
			this.EnsureSpace(items.Length);
			items.CopyTo(this.buffer, this.size);
			this.size += items.Length;
		}

		public void Insert(int index, T item)
		{
			bool flag = this.buffer == null || this.size == this.buffer.Length;
			if (flag)
			{
				this.AllocateMore();
			}
			bool flag2 = index < this.size;
			if (flag2)
			{
				for (int i = this.size; i > index; i--)
				{
					this.buffer[i] = this.buffer[i - 1];
				}
				this.buffer[index] = item;
				this.size++;
			}
			else
			{
				this.Add(item);
			}
		}

		public bool Contains(T item)
		{
			bool flag = this.buffer == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.size; i++)
				{
					bool flag2 = this.buffer[i].Equals(item);
					if (flag2)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public bool Remove(T item)
		{
			bool flag = this.buffer != null;
			bool result;
			if (flag)
			{
				EqualityComparer<T> @default = EqualityComparer<T>.Default;
				for (int i = 0; i < this.size; i++)
				{
					bool flag2 = @default.Equals(this.buffer[i], item);
					if (flag2)
					{
						this.size--;
						this.buffer[i] = default(T);
						for (int j = i; j < this.size; j++)
						{
							this.buffer[j] = this.buffer[j + 1];
						}
						this.buffer[this.size] = default(T);
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		public void RemoveAt(int index)
		{
			bool flag = this.buffer != null && index < this.size;
			if (flag)
			{
				this.size--;
				this.buffer[index] = default(T);
				for (int i = index; i < this.size; i++)
				{
					this.buffer[i] = this.buffer[i + 1];
				}
				this.buffer[this.size] = default(T);
			}
		}

		public T Pop()
		{
			bool flag = this.buffer != null && this.size != 0;
			T result;
			if (flag)
			{
				T[] arg_31_0 = this.buffer;
				int num = this.size - 1;
				this.size = num;
				T t = arg_31_0[num];
				this.buffer[this.size] = default(T);
				result = t;
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		public T[] ToArray()
		{
			this.Trim();
			return this.buffer;
		}

		[DebuggerHidden, DebuggerStepThrough]
		public void Sort(TBetterList<T>.CompareFunc comparer)
		{
			int num = 0;
			int num2 = this.size - 1;
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int i = num; i < num2; i++)
				{
					bool flag2 = comparer(this.buffer[i], this.buffer[i + 1]) > 0;
					if (flag2)
					{
						T t = this.buffer[i];
						this.buffer[i] = this.buffer[i + 1];
						this.buffer[i + 1] = t;
						flag = true;
					}
					else
					{
						bool flag3 = !flag;
						if (flag3)
						{
							num = ((i == 0) ? 0 : (i - 1));
						}
					}
				}
			}
		}
	}
}

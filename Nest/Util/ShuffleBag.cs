using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nest.Util
{
	[Serializable]
	public class ShuffleBag<T> : ICollection<!0>, IEnumerable<!0>, IEnumerable, IList<!0>
	{
		public T Next()
		{
			if (this.cursor >= 1)
			{
				int index = Mathf.FloorToInt(Random.value * (float)(this.cursor + 1));
				T t = this.data[index];
				this.data[index] = this.data[this.cursor];
				this.data[this.cursor] = t;
				this.cursor--;
				return t;
			}
			this.cursor = this.data.Count - 1;
			if (this.data.Count < 1)
			{
				return default(!0);
			}
			return this.data[0];
		}

		public int NextIndex()
		{
			if (this.cursor < 1)
			{
				this.cursor = this.data.Count - 1;
				return 0;
			}
			int num = Mathf.FloorToInt(Random.value * (float)(this.cursor + 1));
			T value = this.data[num];
			this.data[num] = this.data[this.cursor];
			this.data[this.cursor] = value;
			this.cursor--;
			return num;
		}

		public ShuffleBag(T[] initalValues)
		{
			for (int i = 0; i < initalValues.Length; i++)
			{
				this.Add(initalValues[i]);
			}
		}

		public ShuffleBag()
		{
		}

		public int IndexOf(T item)
		{
			return this.data.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.cursor = this.data.Count;
			this.data.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.cursor = this.data.Count - 2;
			this.data.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return this.data[index];
			}
			set
			{
				this.data[index] = value;
			}
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		public void Add(T item)
		{
			this.data.Add(item);
			this.cursor = this.data.Count - 1;
		}

		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		public void Clear()
		{
			this.data.Clear();
		}

		public bool Contains(T item)
		{
			return this.data.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (T t in this.data)
			{
				array.SetValue(t, arrayIndex);
				arrayIndex++;
			}
		}

		public bool Remove(T item)
		{
			this.cursor = this.data.Count - 2;
			return this.data.Remove(item);
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		private List<T> data = new List<!0>();

		private int cursor;

		private T last;
	}
}

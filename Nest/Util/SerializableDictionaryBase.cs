using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nest.Util
{
	[Serializable]
	public class SerializableDictionaryBase<TKey, TValue> : DrawableDictionary, IDictionary<!0, !1>, ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable, ISerializationCallbackReceiver
	{
		public int Count
		{
			get
			{
				if (this._dict == null)
				{
					return 0;
				}
				return this._dict.Count;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (this._dict == null)
			{
				this._dict = new Dictionary<!0, !1>();
			}
			this._dict.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return this._dict != null && this._dict.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (this._dict == null)
				{
					this._dict = new Dictionary<!0, !1>();
				}
				return this._dict.Keys;
			}
		}

		public bool Remove(TKey key)
		{
			return this._dict != null && this._dict.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this._dict == null)
			{
				value = default(!1);
				return false;
			}
			return this._dict.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (this._dict == null)
				{
					this._dict = new Dictionary<!0, !1>();
				}
				return this._dict.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (this._dict == null)
				{
					throw new KeyNotFoundException();
				}
				return this._dict[key];
			}
			set
			{
				if (this._dict == null)
				{
					this._dict = new Dictionary<!0, !1>();
				}
				this._dict[key] = value;
			}
		}

		public void Clear()
		{
			if (this._dict != null)
			{
				this._dict.Clear();
			}
		}

		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> item)
		{
			if (this._dict == null)
			{
				this._dict = new Dictionary<!0, !1>();
			}
			((ICollection<KeyValuePair<!0, !1>>)this._dict).Add(item);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this._dict != null && ((ICollection<KeyValuePair<!0, !1>>)this._dict).Contains(item);
		}

		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this._dict == null)
			{
				return;
			}
			((ICollection<KeyValuePair<!0, !1>>)this._dict).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return this._dict != null && ((ICollection<KeyValuePair<!0, !1>>)this._dict).Remove(item);
		}

		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			if (this._dict == null)
			{
				return default(Dictionary<!0, !1>.Enumerator);
			}
			return this._dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this._dict == null)
			{
				return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
			}
			return this._dict.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			if (this._dict == null)
			{
				return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
			}
			return this._dict.GetEnumerator();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this._keys != null && this._values != null)
			{
				if (this._dict == null)
				{
					this._dict = new Dictionary<!0, !1>(this._keys.Length);
				}
				else
				{
					this._dict.Clear();
				}
				for (int i = 0; i < this._keys.Length; i++)
				{
					if (i < this._values.Length)
					{
						this._dict[this._keys[i]] = this._values[i];
					}
					else
					{
						this._dict[this._keys[i]] = default(!1);
					}
				}
			}
			this._keys = null;
			this._values = null;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this._dict == null || this._dict.Count == 0)
			{
				this._keys = null;
				this._values = null;
				return;
			}
			int count = this._dict.Count;
			this._keys = new !0[count];
			this._values = new !1[count];
			int num = 0;
			Dictionary<TKey, TValue>.Enumerator enumerator = this._dict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				!0[] keys = this._keys;
				int num2 = num;
				KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
				keys[num2] = keyValuePair.Key;
				!1[] values = this._values;
				int num3 = num;
				keyValuePair = enumerator.Current;
				values[num3] = keyValuePair.Value;
				num++;
			}
		}

		[NonSerialized]
		private Dictionary<TKey, TValue> _dict;

		[SerializeField]
		private TKey[] _keys;

		[SerializeField]
		private TValue[] _values;
	}
}

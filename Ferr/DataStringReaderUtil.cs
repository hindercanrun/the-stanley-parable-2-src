using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr
{
	public class DataStringReaderUtil
	{
		public int NameCount
		{
			get
			{
				return this._names.Length;
			}
		}

		public bool HasNext
		{
			get
			{
				return this._curr < this._words.Length;
			}
		}

		public DataStringReaderUtil(string aData, DataStringType aType)
		{
			List<string> list = DataStringUtil.SplitSmart(aData, ',');
			if (list == null)
			{
				throw new ArgumentException("Poorly formed data string! Ensure sure quotes and brackets all match!");
			}
			this._type = aType;
			this._words = (string.IsNullOrEmpty(aData) ? new string[0] : list.ToArray());
			if (this._type == DataStringType.Named)
			{
				this._names = new string[this._words.Length];
				for (int i = 0; i < this._words.Length; i++)
				{
					int num = this._words[i].IndexOf(':');
					string text = this._words[i].Substring(0, num);
					string text2 = this._words[i].Substring(num + 1);
					this._words[i] = text2;
					this._names[i] = text;
				}
			}
		}

		public string GetName(int aIndex)
		{
			return this._names[aIndex];
		}

		public int Int()
		{
			return int.Parse(this.Read());
		}

		public int Int(string aName)
		{
			return int.Parse(this.Read(aName));
		}

		public long Long()
		{
			return long.Parse(this.Read());
		}

		public long Long(string aName)
		{
			return long.Parse(this.Read(aName));
		}

		public bool Bool()
		{
			return bool.Parse(this.Read());
		}

		public bool Bool(string aName)
		{
			return bool.Parse(this.Read(aName));
		}

		public float Float()
		{
			return float.Parse(this.Read());
		}

		public float Float(string aName)
		{
			return float.Parse(this.Read(aName));
		}

		public string String()
		{
			return this.Read();
		}

		public string String(string aName)
		{
			return this.Read(aName);
		}

		public object Data()
		{
			return this.CreateObject(this.Read());
		}

		public object Data(string aName)
		{
			return this.CreateObject(this.Read(aName));
		}

		public void Data(ref IToFromDataString aBaseObject)
		{
			string text = this.Read();
			string aData = text.Substring(text.IndexOf('=') + 1);
			aBaseObject.FromDataString(aData);
		}

		public void Data(string aName, ref IToFromDataString aBaseObject)
		{
			string text = this.Read(aName);
			string aData = text.Substring(text.IndexOf('=') + 1);
			aBaseObject.FromDataString(aData);
		}

		private string Read(string aName)
		{
			if (this._type == DataStringType.Ordered)
			{
				throw new Exception("Can't do a named read from an ordered list!");
			}
			int num = Array.IndexOf<string>(this._names, aName);
			if (num == -1)
			{
				throw new Exception("Can't find data from given name: " + aName);
			}
			return this._words[num];
		}

		private string Read()
		{
			if (this._type == DataStringType.Named)
			{
				throw new Exception("Can't do an ordered read from a named list!");
			}
			if (this._curr >= this._words.Length)
			{
				throw new Exception("Reading past the end of an ordered data string!");
			}
			string result = this._words[this._curr];
			this._curr++;
			return result;
		}

		private object CreateObject(string aDataString)
		{
			if (string.IsNullOrEmpty(aDataString) || aDataString == "null")
			{
				return null;
			}
			int num = aDataString.IndexOf('=');
			string typeName = aDataString.Substring(0, num);
			string aData = aDataString.Substring(num + 1);
			Type type = Type.GetType(typeName);
			object obj = null;
			if (typeof(IToFromDataString).IsAssignableFrom(type))
			{
				if (typeof(ScriptableObject).IsAssignableFrom(type))
				{
					obj = ScriptableObject.CreateInstance(type);
				}
				else
				{
					obj = Activator.CreateInstance(type);
				}
				((IToFromDataString)obj).FromDataString(aData);
			}
			return obj;
		}

		private DataStringType _type;

		private string[] _words;

		private string[] _names;

		private int _curr;
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Ferr
{
	public class DataStringWriterUtil
	{
		public DataStringWriterUtil(DataStringType aType)
		{
			this._type = aType;
			this._builder = new StringBuilder();
			this._builder.Append('{');
		}

		public void Int(int aData)
		{
			this.Entry(aData.ToString());
		}

		public void Int(string aName, int aData)
		{
			this.Entry(aName, aData.ToString());
		}

		public void Long(long aData)
		{
			this.Entry(aData.ToString());
		}

		public void Long(string aName, long aData)
		{
			this.Entry(aName, aData.ToString());
		}

		public void Bool(bool aData)
		{
			this.Entry(aData.ToString());
		}

		public void Bool(string aName, bool aData)
		{
			this.Entry(aName, aData.ToString());
		}

		public void Float(float aData)
		{
			this.Entry(aData.ToString());
		}

		public void Float(string aName, float aData)
		{
			this.Entry(aName, aData.ToString());
		}

		public void Data(IToFromDataString aData)
		{
			if (aData == null)
			{
				this.Entry("null");
				return;
			}
			this.Entry(aData.GetType().Name + "=" + aData.ToDataString());
		}

		public void Data(string aName, IToFromDataString aData)
		{
			if (aData == null)
			{
				this.Entry(aName, "null");
				return;
			}
			this.Entry(aName, aData.GetType().Name + "=" + aData.ToDataString());
		}

		public void String(string aData)
		{
			char quoteType = this.GetQuoteType(aData);
			if (quoteType != ' ')
			{
				this.Entry(quoteType.ToString() + aData + quoteType.ToString());
				return;
			}
			this.Entry(aData);
		}

		public void String(string aName, string aData)
		{
			char quoteType = this.GetQuoteType(aData);
			if (quoteType != ' ')
			{
				this.Entry(aName, quoteType.ToString() + aData + quoteType.ToString());
				return;
			}
			this.Entry(aName, aData);
		}

		protected char GetQuoteType(string aData)
		{
			char result = ' ';
			if (!aData.StartsWith("{") && !aData.StartsWith("\"") && !aData.StartsWith("'"))
			{
				bool flag = aData.Contains("'");
				bool flag2 = aData.Contains("\"");
				if (flag && flag2)
				{
					throw new ArgumentException("String data contains -both- single and double quotes, what am I supposed to do with this?");
				}
				result = (flag ? '"' : '\'');
			}
			return result;
		}

		protected void Entry(string aData)
		{
			if (this._type == DataStringType.Named)
			{
				throw new Exception("Need a name for a named list!");
			}
			if (this._builder.Length > 1)
			{
				this._builder.Append(',');
			}
			this._builder.Append(aData);
		}

		protected void Entry(string aName, string aData)
		{
			if (this._type == DataStringType.Ordered)
			{
				throw new Exception("Name doesn't apply for ordered lists!");
			}
			if (aName.Contains(":") || aName.Contains(","))
			{
				throw new Exception("Name includes a reserved character! (: or ,) - " + aName);
			}
			if (this._names.Contains(aName))
			{
				throw new Exception("Used the same name twice: " + aName);
			}
			this._names.Add(aName);
			if (this._builder.Length > 1)
			{
				this._builder.Append(',');
			}
			this._builder.Append(aName);
			this._builder.Append(":");
			this._builder.Append(aData);
		}

		public override string ToString()
		{
			return this._builder.ToString() + "}";
		}

		private DataStringType _type;

		private StringBuilder _builder;

		private HashSet<string> _names = new HashSet<string>();
	}
}

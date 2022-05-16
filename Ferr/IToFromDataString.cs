using System;

namespace Ferr
{
	public interface IToFromDataString
	{
		string ToDataString();

		void FromDataString(string aData);
	}
}

using System;
using System.Collections.Generic;

namespace Ferr
{
	public class LambdaComparer<T> : IComparer<!0>
	{
		public LambdaComparer(Func<T, T, int> comparerFunc)
		{
			this.func = comparerFunc;
		}

		public int Compare(T x, T y)
		{
			return this.func(x, y);
		}

		private readonly Func<T, T, int> func;
	}
}

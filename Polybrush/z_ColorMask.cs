﻿using System;

namespace Polybrush
{
	public struct z_ColorMask
	{
		public z_ColorMask(bool r, bool g, bool b, bool a)
		{
			this.r = r;
			this.b = b;
			this.g = g;
			this.a = a;
		}

		public bool r;

		public bool g;

		public bool b;

		public bool a;
	}
}

using System;

namespace I2.Loc
{
	internal class TashkeelLocation
	{
		public TashkeelLocation(char tashkeel, int position)
		{
			this.tashkeel = tashkeel;
			this.position = position;
		}

		public char tashkeel;

		public int position;
	}
}

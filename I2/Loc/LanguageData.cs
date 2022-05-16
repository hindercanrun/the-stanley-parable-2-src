using System;

namespace I2.Loc
{
	[Serializable]
	public class LanguageData
	{
		public bool IsEnabled()
		{
			return (this.Flags & 1) == 0;
		}

		public void SetEnabled(bool bEnabled)
		{
			if (bEnabled)
			{
				this.Flags = (byte)((int)this.Flags & -2);
				return;
			}
			this.Flags |= 1;
		}

		public bool IsLoaded()
		{
			return (this.Flags & 4) == 0;
		}

		public bool CanBeUnloaded()
		{
			return (this.Flags & 2) == 0;
		}

		public void SetLoaded(bool loaded)
		{
			if (loaded)
			{
				this.Flags = (byte)((int)this.Flags & -5);
				return;
			}
			this.Flags |= 4;
		}

		public void SetCanBeUnLoaded(bool allowUnloading)
		{
			if (allowUnloading)
			{
				this.Flags = (byte)((int)this.Flags & -3);
				return;
			}
			this.Flags |= 2;
		}

		public string Name;

		public string Code;

		public byte Flags;

		[NonSerialized]
		public bool Compressed;
	}
}

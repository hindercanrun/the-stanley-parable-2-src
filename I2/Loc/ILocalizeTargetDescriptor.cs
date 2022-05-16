using System;

namespace I2.Loc
{
	public abstract class ILocalizeTargetDescriptor
	{
		public abstract bool CanLocalize(Localize cmp);

		public abstract ILocalizeTarget CreateTarget(Localize cmp);

		public abstract Type GetTargetType();

		public string Name;

		public int Priority;
	}
}

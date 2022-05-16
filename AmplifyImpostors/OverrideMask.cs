using System;

namespace AmplifyImpostors
{
	[Flags]
	public enum OverrideMask
	{
		OutputToggle = 1,
		NameSuffix = 2,
		RelativeScale = 4,
		ColorSpace = 8,
		QualityCompression = 16,
		FileFormat = 32
	}
}

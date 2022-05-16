using System;

namespace Polybrush
{
	[Flags]
	public enum z_MeshChannel
	{
		Null = 0,
		Position = 0,
		Normal = 1,
		Color = 2,
		Tangent = 4,
		UV0 = 8,
		UV2 = 16,
		UV3 = 32,
		UV4 = 64,
		All = 255
	}
}

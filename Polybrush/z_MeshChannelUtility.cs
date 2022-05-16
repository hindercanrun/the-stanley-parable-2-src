using System;

namespace Polybrush
{
	public static class z_MeshChannelUtility
	{
		public static z_MeshChannel StringToEnum(string str)
		{
			string text = str.ToUpper();
			foreach (object obj in Enum.GetValues(typeof(z_MeshChannel)))
			{
				if (text.Equals(((z_MeshChannel)obj).ToString().ToUpper()))
				{
					return (z_MeshChannel)obj;
				}
			}
			return z_MeshChannel.Null;
		}

		public static int UVChannelToIndex(z_MeshChannel channel)
		{
			if (channel == z_MeshChannel.UV0)
			{
				return 0;
			}
			if (channel == z_MeshChannel.UV2)
			{
				return 1;
			}
			if (channel == z_MeshChannel.UV3)
			{
				return 2;
			}
			if (channel == z_MeshChannel.UV4)
			{
				return 3;
			}
			return -1;
		}
	}
}

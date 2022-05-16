using System;

namespace AmplifyImpostors
{
	[Serializable]
	public class VersionInfo
	{
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 0, 9, 7) + ((VersionInfo.Revision > 0) ? ("r" + VersionInfo.Revision.ToString()) : "");
		}

		public static int FullNumber
		{
			get
			{
				return 9700 + (int)VersionInfo.Revision;
			}
		}

		public static string FullLabel
		{
			get
			{
				return "Version=" + VersionInfo.FullNumber;
			}
		}

		public const byte Major = 0;

		public const byte Minor = 9;

		public const byte Release = 7;

		public static byte Revision = 1;
	}
}

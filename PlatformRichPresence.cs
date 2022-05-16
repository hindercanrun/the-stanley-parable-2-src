using System;

public static class PlatformRichPresence
{
	public static void InitPlatformRichPresence(IPlatformRichPresence presences)
	{
		PlatformRichPresence.platformRP = presences;
	}

	public static void SetPresence(PresenceID presence)
	{
		IPlatformRichPresence platformRichPresence = PlatformRichPresence.platformRP;
		if (platformRichPresence == null)
		{
			return;
		}
		platformRichPresence.SetPresence(presence);
	}

	private static IPlatformRichPresence platformRP;
}

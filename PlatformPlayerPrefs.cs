using System;

public static class PlatformPlayerPrefs
{
	public static void Init(IPlatformPlayerPrefs platformPlayerPrefs)
	{
		PlatformPlayerPrefs.playerPrefs = platformPlayerPrefs;
	}

	public static void DeleteAll()
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs != null)
		{
			platformPlayerPrefs.DeleteAll();
		}
		IPlatformPlayerPrefs platformPlayerPrefs2 = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs2 == null)
		{
			return;
		}
		platformPlayerPrefs2.Save();
	}

	public static void DeleteKey(string key)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs != null)
		{
			platformPlayerPrefs.DeleteKey(key);
		}
		IPlatformPlayerPrefs platformPlayerPrefs2 = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs2 == null)
		{
			return;
		}
		platformPlayerPrefs2.Save();
	}

	public static float GetFloat(string key)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return 0f;
		}
		return platformPlayerPrefs.GetFloat(key);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return defaultValue;
		}
		return platformPlayerPrefs.GetFloat(key, defaultValue);
	}

	public static int GetInt(string key)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return 0;
		}
		return platformPlayerPrefs.GetInt(key);
	}

	public static int GetInt(string key, int defaultValue)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return defaultValue;
		}
		return platformPlayerPrefs.GetInt(key, defaultValue);
	}

	public static string GetString(string key)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return null;
		}
		return platformPlayerPrefs.GetString(key);
	}

	public static string GetString(string key, string defaultValue)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return null;
		}
		return platformPlayerPrefs.GetString(key, defaultValue);
	}

	public static bool HasKey(string key)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		return platformPlayerPrefs != null && platformPlayerPrefs.HasKey(key);
	}

	public static void Save()
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs == null)
		{
			return;
		}
		platformPlayerPrefs.Save();
	}

	public static void SetFloat(string key, float value)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs != null)
		{
			platformPlayerPrefs.SetFloat(key, value);
		}
		IPlatformPlayerPrefs platformPlayerPrefs2 = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs2 == null)
		{
			return;
		}
		platformPlayerPrefs2.Save();
	}

	public static void SetInt(string key, int value)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs != null)
		{
			platformPlayerPrefs.SetInt(key, value);
		}
		IPlatformPlayerPrefs platformPlayerPrefs2 = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs2 == null)
		{
			return;
		}
		platformPlayerPrefs2.Save();
	}

	public static void SetString(string key, string value)
	{
		IPlatformPlayerPrefs platformPlayerPrefs = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs != null)
		{
			platformPlayerPrefs.SetString(key, value);
		}
		IPlatformPlayerPrefs platformPlayerPrefs2 = PlatformPlayerPrefs.playerPrefs;
		if (platformPlayerPrefs2 == null)
		{
			return;
		}
		platformPlayerPrefs2.Save();
	}

	private static IPlatformPlayerPrefs playerPrefs;
}

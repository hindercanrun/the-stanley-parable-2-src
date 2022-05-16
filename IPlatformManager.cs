using System;

public interface IPlatformManager
{
	IPlatformAchievements Achievements { get; }

	IPlatformGamepad Gamepad { get; }

	IPlatformRichPresence RichPresence { get; }

	IPlatformPlayerPrefs PlayerPrefs { get; }

	string SystemLanguage { get; }

	bool UseLowerFPSVideos { get; }

	void Init();

	void PlatformManagerUpdate();
}

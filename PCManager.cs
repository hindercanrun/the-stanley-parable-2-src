using System;
using UnityEngine;

public class PCManager : IPlatformManager
{
	public string SystemLanguage { get; private set; }

	public bool UseLowerFPSVideos { get; private set; }

	public IPlatformAchievements Achievements { get; private set; }

	public IPlatformGamepad Gamepad { get; private set; }

	public IPlatformPlayerPrefs PlayerPrefs { get; private set; }

	public IPlatformRichPresence RichPresence { get; private set; }

	public void Init()
	{
		if (!this.isInitialized)
		{
			this.Achievements = new PCAchievements();
			this.Gamepad = new PCGamepad();
			this.PlayerPrefs = new PCFileBasePlayerPrefs();
			this.RichPresence = new PCRichPresence();
			this.SystemLanguage = Application.systemLanguage.ToString();
			this.UseLowerFPSVideos = false;
			this.isInitialized = true;
		}
	}

	public void PlatformManagerUpdate()
	{
	}

	private bool isInitialized;
}

using System;
using Steamworks;

public class PCAchievements : IPlatformAchievements
{
	public PCAchievements()
	{
		this.Init();
	}

	public void Init()
	{
	}

	public bool IsAchievementUnlocked(AchievementID achievement)
	{
		string achievementSteamID = this.GetAchievementSteamID(achievement);
		bool result;
		try
		{
			SteamUserStats.GetAchievement(achievementSteamID, out result);
		}
		catch (InvalidOperationException)
		{
			result = false;
		}
		return result;
	}

	public void UnlockAchievement(AchievementID achievement)
	{
		if (this.IsAchievementUnlocked(achievement))
		{
			return;
		}
		string achievementSteamID = this.GetAchievementSteamID(achievement);
		try
		{
			SteamUserStats.SetAchievement(achievementSteamID);
			SteamUserStats.StoreStats();
		}
		catch (InvalidOperationException)
		{
		}
		if (PlatformAchievements.AchievementUnlocked != null)
		{
			PlatformAchievements.AchievementUnlocked(achievement);
		}
	}

	private string GetAchievementSteamID(AchievementID id)
	{
		string result = "";
		switch (id)
		{
		case AchievementID.First:
			result = "first";
			break;
		case AchievementID.BeatTheGame:
			result = "beatgame";
			break;
		case AchievementID.TestPlsIgnore:
			result = "testplsignore";
			break;
		case AchievementID.WelcomeBack:
			result = "welcomeback";
			break;
		case AchievementID.YouCantJump:
			result = "nojump";
			break;
		case AchievementID.Tuesday:
			result = "tuesday";
			break;
		case AchievementID.EightEightEightEight:
			result = "8888";
			break;
		case AchievementID.Click430FiveTimes:
			result = "430";
			break;
		case AchievementID.SpeedRun:
			result = "speedrun";
			break;
		case AchievementID.SettingsWorldChampion:
			result = "settingsworldchamp";
			break;
		case AchievementID.SuperGoOutside:
			result = "supergooutside";
			break;
		}
		return result;
	}

	private bool initialized;
}

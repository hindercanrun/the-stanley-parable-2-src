using System;
using System.Collections;
using System.Collections.Generic;

public static class PlatformAchievements
{
	public static IPlatformAchievements DebugInstance
	{
		get
		{
			return PlatformAchievements.platformAch;
		}
	}

	public static event Action<AchievementID> OnAchievementUnlockedFirstTime;

	public static void InitPlatformAchievements(IPlatformAchievements achievements)
	{
		PlatformAchievements.platformAch = achievements;
	}

	public static void UnlockAchievement(AchievementID achievement)
	{
		if (!PlatformAchievements.IsAchievementUnlocked(achievement))
		{
			Action<AchievementID> onAchievementUnlockedFirstTime = PlatformAchievements.OnAchievementUnlockedFirstTime;
			if (onAchievementUnlockedFirstTime != null)
			{
				onAchievementUnlockedFirstTime(achievement);
			}
		}
		IPlatformAchievements platformAchievements = PlatformAchievements.platformAch;
		if (platformAchievements == null)
		{
			return;
		}
		platformAchievements.UnlockAchievement(achievement);
	}

	public static bool IsAchievementUnlocked(AchievementID achievement)
	{
		IPlatformAchievements platformAchievements = PlatformAchievements.platformAch;
		return platformAchievements != null && platformAchievements.IsAchievementUnlocked(achievement);
	}

	public static IEnumerable<AchievementID> AllAchievmentIDs
	{
		get
		{
			foreach (object obj in Enum.GetValues(typeof(AchievementID)))
			{
				AchievementID achievementID = (AchievementID)obj;
				if (achievementID != AchievementID.NumAchievements)
				{
					yield return achievementID;
				}
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}
	}

	public static bool AllAchievementsUnlocked()
	{
		using (IEnumerator<AchievementID> enumerator = PlatformAchievements.AllAchievmentIDs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!PlatformAchievements.IsAchievementUnlocked(enumerator.Current))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static int AchievementsUnlockedCount
	{
		get
		{
			int num = 0;
			using (IEnumerator<AchievementID> enumerator = PlatformAchievements.AllAchievmentIDs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PlatformAchievements.IsAchievementUnlocked(enumerator.Current))
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	public static int AchievementsCount
	{
		get
		{
			return 11;
		}
	}

	private static IPlatformAchievements platformAch;

	public static Action<AchievementID> AchievementUnlocked;
}

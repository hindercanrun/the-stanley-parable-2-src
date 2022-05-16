using System;

public interface IPlatformAchievements
{
	void UnlockAchievement(AchievementID achievement);

	bool IsAchievementUnlocked(AchievementID achievement);
}

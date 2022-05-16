using System;
using UnityEngine;

public class AchievementUnlock : MonoBehaviour
{
	public void Unlock()
	{
		PlatformAchievements.UnlockAchievement(this.achievementToUnlock);
	}

	[SerializeField]
	private AchievementID achievementToUnlock;
}

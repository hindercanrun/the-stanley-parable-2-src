using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementsData", menuName = "Achievements List")]
public class AchievementsData : ScriptableObject
{
	public AchievementData FindAchievement(AchievementID achievementID)
	{
		return this.achievementsList.Find((AchievementData x) => x.id == achievementID);
	}

	[SerializeField]
	private List<AchievementData> achievementsList;
}

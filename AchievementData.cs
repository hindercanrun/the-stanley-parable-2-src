using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementsData", menuName = "Achievement Data")]
public class AchievementData : ScriptableObject
{
	public string TitleTerm(bool calledAchievements)
	{
		if (!calledAchievements && this.add_TROPHY_toTagIfTrophy)
		{
			return this.titleTag + "_TROPHY";
		}
		return this.titleTag;
	}

	public string DescriptionTerm(bool calledAchievements)
	{
		if (!calledAchievements && this.add_TROPHY_toTagIfTrophy)
		{
			return this.descriptionTag + "_TROPHY";
		}
		return this.descriptionTag;
	}

	public AchievementID id;

	public Sprite textureFound;

	public Sprite textureNotFound;

	public string steamID;

	public string steamAPIName;

	[SerializeField]
	private string titleTag;

	[SerializeField]
	private string descriptionTag;

	[SerializeField]
	private bool add_TROPHY_toTagIfTrophy;

	public StringConfigurable dateFoundConfigurable;
}

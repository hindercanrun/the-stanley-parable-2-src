using System;
using System.Collections;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class TrophyPopupElement : MonoBehaviour
{
	public AchievementID ID
	{
		set
		{
			AchievementData achievementData = this.data.FindAchievement(value);
			if (achievementData == null)
			{
				return;
			}
			this.trophyImage.sprite = achievementData.textureFound;
			if (this.title != null)
			{
				this.title.Term = achievementData.TitleTerm(true);
			}
			if (this.desc != null)
			{
				this.desc.Term = achievementData.DescriptionTerm(true);
			}
		}
	}

	private void OnDestroy()
	{
		GameMaster.OnPrepareLoadingLevel -= this.SelfDestroy;
	}

	private IEnumerator Start()
	{
		GameMaster.OnPrepareLoadingLevel += this.SelfDestroy;
		this.audioCollectionPlay.Play();
		yield return new WaitForGameSeconds(this.timeToLive);
		this.SelfDestroy();
		yield break;
	}

	private void SelfDestroy()
	{
		Object.Destroy(base.gameObject);
	}

	public Image trophyImage;

	public Localize title;

	public Localize desc;

	public float timeToLive = 5f;

	public AchievementsData data;

	public PlaySoundFromAudioCollection audioCollectionPlay;
}

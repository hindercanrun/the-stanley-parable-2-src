using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementUIElement : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	public void OnSelect(BaseEventData eventData)
	{
		this.animator.SetBool("Selected", true);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		this.animator.SetBool("Selected", false);
	}

	private void Start()
	{
		Singleton<GameMaster>.Instance.AchievementsData.FindAchievement(this.achievementID).dateFoundConfigurable.Init();
	}

	private void UpdateUI()
	{
		this.UpdateUI(false);
	}

	public void UpdateUI(bool calledAchievements)
	{
		AchievementData achievementData = Singleton<GameMaster>.Instance.AchievementsData.FindAchievement(this.achievementID);
		if (achievementData == null)
		{
			return;
		}
		this.foundImage.sprite = achievementData.textureFound;
		this.unfoundImage.sprite = achievementData.textureNotFound;
		this.titleTextLoc.SetTerm(achievementData.TitleTerm(calledAchievements));
		this.descriptionTextLoc.SetTerm(achievementData.DescriptionTerm(calledAchievements));
		if (Application.isPlaying)
		{
			bool flag = PlatformAchievements.IsAchievementUnlocked(this.achievementID);
			this.animator.SetBool("Locked", !flag);
			string text = achievementData.dateFoundConfigurable.GetStringValue();
			if (Debug.isDebugBuild && flag && text == "")
			{
				text = "8/3/2026, 3:15 PM";
			}
			this.dateFoundText.text = text;
		}
	}

	[InspectorButton("UpdateUI", null)]
	public AchievementID achievementID;

	[Header("Dynamic UI Sub-Elements")]
	public Image foundImage;

	public Image unfoundImage;

	public Localize titleTextLoc;

	public Localize descriptionTextLoc;

	public TextMeshProUGUI dateFoundText;

	public Animator animator;
}

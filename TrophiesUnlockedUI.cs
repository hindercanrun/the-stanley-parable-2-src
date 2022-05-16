using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class TrophiesUnlockedUI : MonoBehaviour
{
	public bool useFakes
	{
		get
		{
			return !Application.isPlaying;
		}
	}

	private void Start()
	{
		LocalizationManager.OnLocalizeEvent += this.UpdateUI;
		BooleanConfigurable booleanConfigurable = this.calledAchievementsConfigurable;
		booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateUI));
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= this.UpdateUI;
		BooleanConfigurable booleanConfigurable = this.calledAchievementsConfigurable;
		booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateUI));
	}

	public void UpdateUI(LiveData data)
	{
		this.UpdateUI();
	}

	public void UpdateUI()
	{
		int num = this.useFakes ? this.fakeCount : PlatformAchievements.AchievementsUnlockedCount;
		int num2 = this.useFakes ? this.fakeTotal : PlatformAchievements.AchievementsCount;
		bool flag = this.useFakes ? this.fakeCalledAchievement : this.calledAchievementsConfigurable.GetBooleanValue();
		string text = (num == num2) ? this.completeTerm : this.progressTerm;
		if (!flag)
		{
			text += this.trophyTermSuffix;
		}
		string text2 = LocalizationManager.GetTranslation(text, true, 0, true, false, null, null);
		if (text2 == null)
		{
			Debug.LogError("Could not find translation");
			return;
		}
		text2 = text2.Replace("%!NUM!%", string.Format("{0}", num));
		text2 = text2.Replace("%!TOTAL!%", string.Format("{0}", num2));
		this.text.text = text2;
		float num3 = (float)num / (float)num2;
		this.percentageText.text = string.Format("{0:0}%", Mathf.FloorToInt(num3 * 100f));
		this.fillImage.anchorMax = new Vector2(num3, 1f);
		AchievementUIElement[] componentsInChildren = this.achievementUIHolder.GetComponentsInChildren<AchievementUIElement>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].UpdateUI(flag);
		}
	}

	public TextMeshProUGUI text;

	public string progressTerm = "Achievement_Status_Progress";

	public string completeTerm = "Achievement_Status_Complete";

	public string trophyTermSuffix = "_TROPHY";

	public BooleanConfigurable calledAchievementsConfigurable;

	public TextMeshProUGUI percentageText;

	public RectTransform fillImage;

	[Header("Location of the Achievement UI elements")]
	public Transform achievementUIHolder;

	[Header("Editor fake values for testing")]
	public int fakeCount = 4;

	public int fakeTotal = 12;

	[InspectorButton("UpdateUI", null)]
	public bool fakeCalledAchievement = true;
}

using System;
using UnityEngine;

public class TrophyPopupController : MonoBehaviour
{
	public static TrophyPopupController Instance { get; private set; }

	private void Awake()
	{
		TrophyPopupController.Instance = this;
	}

	private void OnEnable()
	{
		PlatformAchievements.AchievementUnlocked = (Action<AchievementID>)Delegate.Remove(PlatformAchievements.AchievementUnlocked, new Action<AchievementID>(this.ShowTrophyPopup));
		PlatformAchievements.AchievementUnlocked = (Action<AchievementID>)Delegate.Combine(PlatformAchievements.AchievementUnlocked, new Action<AchievementID>(this.ShowTrophyPopup));
	}

	private void OnDisable()
	{
		PlatformAchievements.AchievementUnlocked = (Action<AchievementID>)Delegate.Remove(PlatformAchievements.AchievementUnlocked, new Action<AchievementID>(this.ShowTrophyPopup));
	}

	private void OnDestroy()
	{
	}

	public void FireTestTrophyUI()
	{
		PlatformAchievements.UnlockAchievement((AchievementID)Random.Range(0, 11));
	}

	private bool IsCanvasGroupVisible
	{
		get
		{
			return this.displayCanvasGroup.alpha > 0f;
		}
	}

	public void ShowTrophyPopup(AchievementID achID)
	{
		if (!this.IsCanvasGroupVisible)
		{
			return;
		}
		int num = 0;
		TrophyPopupElement[] componentsInChildren = base.GetComponentsInChildren<TrophyPopupElement>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null)
			{
				num++;
			}
		}
		TrophyPopupElement component = Object.Instantiate<GameObject>(this.trophyPrefab.gameObject).GetComponent<TrophyPopupElement>();
		component.transform.parent = base.transform;
		component.transform.localScale = Vector3.one;
		component.ID = achID;
		component.GetComponent<RectTransform>().anchoredPosition = Vector3.up * this.elementHeight * (float)num;
	}

	[InspectorButton("FireTestTrophyUI", "Fire Test Trophy UI")]
	public TrophyPopupElement trophyPrefab;

	[InspectorButton("Clear", "Clear")]
	public float elementHeight = 170f;

	[SerializeField]
	private CanvasGroup displayCanvasGroup;
}

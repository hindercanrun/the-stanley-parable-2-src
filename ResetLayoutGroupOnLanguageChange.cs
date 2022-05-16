using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ResetLayoutGroupOnLanguageChange : MonoBehaviour
{
	private void Awake()
	{
		LocalizationManager.OnLocalizeEvent += this.LocalizationManager_OnLocalizeEvent;
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= this.LocalizationManager_OnLocalizeEvent;
	}

	private void OnEnable()
	{
		if (this.queueUpdate)
		{
			this.queueUpdate = false;
			this.UpdateLayoutGroup();
		}
	}

	private void LocalizationManager_OnLocalizeEvent()
	{
		this.queueUpdate = true;
		this.UpdateLayoutGroup();
	}

	private void UpdateLayoutGroup()
	{
		Canvas.ForceUpdateCanvases();
		HorizontalOrVerticalLayoutGroup componentInChildren = base.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>();
		componentInChildren.enabled = false;
		componentInChildren.enabled = true;
		Canvas.ForceUpdateCanvases();
	}

	private bool queueUpdate;
}

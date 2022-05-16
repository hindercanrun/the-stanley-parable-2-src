using System;
using UnityEngine;
using UnityEngine.Events;

public class SettingsResetPrompt : MonoBehaviour
{
	public void ShowPrompt(bool visible)
	{
		this.promptShowing = visible;
	}

	public void CallNo()
	{
		UnityEvent unityEvent = this.noCalled;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	public void CallYes()
	{
		UnityEvent unityEvent = this.yesCalled;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	private void Update()
	{
		if (this.promptShowing && Singleton<GameMaster>.Instance.stanleyActions.MenuBack.WasPressed)
		{
			this.CallNo();
		}
	}

	public GameObject[] toDisableOnPrompt;

	public GameObject resetAllPrompt;

	private bool promptShowing;

	public UnityEvent noCalled;

	public UnityEvent yesCalled;
}

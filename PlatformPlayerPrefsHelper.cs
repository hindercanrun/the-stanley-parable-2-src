using System;
using System.Collections;
using UnityEngine;

public class PlatformPlayerPrefsHelper : MonoBehaviour
{
	public static PlatformPlayerPrefsHelper Instance
	{
		get
		{
			if (PlatformPlayerPrefsHelper.instance == null)
			{
				PlatformPlayerPrefsHelper.instance = Object.FindObjectOfType<PlatformPlayerPrefsHelper>();
				if (PlatformPlayerPrefsHelper.instance == null)
				{
					PlatformPlayerPrefsHelper.instance = new GameObject("PlatformPlayerPrefsHelper").AddComponent<PlatformPlayerPrefsHelper>();
				}
				Object.DontDestroyOnLoad(PlatformPlayerPrefsHelper.instance.gameObject);
			}
			return PlatformPlayerPrefsHelper.instance;
		}
	}

	public void InitializePlatformPlayerPrefs(IPlatformPlayerPrefs playerPrefs)
	{
		if (!PlatformPlayerPrefsHelper.isInitialized)
		{
			PlatformPlayerPrefs.Init(playerPrefs);
			PlatformPlayerPrefsHelper.isInitialized = true;
			this.safeToSave = true;
			Action action = PlatformPlayerPrefsHelper.saveSystemInitialized;
			if (action == null)
			{
				return;
			}
			action();
		}
	}

	public void StartTimer(int secondsToWait)
	{
		this.safeToSave = false;
		this.needToSave = false;
		base.StartCoroutine(this.SaveTimeDelay(secondsToWait));
	}

	public IEnumerator SaveTimeDelay(int secondsToWait)
	{
		yield return new WaitForSecondsRealtime((float)secondsToWait);
		this.safeToSave = true;
		yield break;
	}

	public bool CanSave()
	{
		if (!this.safeToSave)
		{
			this.needToSave = true;
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (this.safeToSave && this.needToSave)
		{
			PlatformPlayerPrefs.Save();
		}
	}

	private static PlatformPlayerPrefsHelper instance;

	private static bool isInitialized;

	private bool safeToSave;

	private bool needToSave;

	public static Action saveSystemInitialized;
}

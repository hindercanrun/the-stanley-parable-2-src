using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPrefManager : MonoBehaviour
{
	private void Awake()
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			this.SimulateKeyExists = false;
			this.SimulateIndexMatch = false;
		}
	}

	public void DeleteContinueKeys()
	{
		PlatformPlayerPrefs.DeleteKey("ContinuePoint");
		PlatformPlayerPrefs.Save();
	}

	public void DeleteAllSinglePlaythroughKeys()
	{
		PlatformPlayerPrefs.DeleteKey("PressEnding1");
		PlatformPlayerPrefs.DeleteKey("PressEnding2");
		PlatformPlayerPrefs.Save();
	}

	public void DeleteKey()
	{
		PlatformPlayerPrefs.DeleteKey(this.Key);
		PlatformPlayerPrefs.Save();
	}

	public void SetKey()
	{
		PlatformPlayerPrefs.SetInt(this.Key, 1);
		PlatformPlayerPrefs.Save();
	}

	public void SetKeyIntValue(int value)
	{
		PlatformPlayerPrefs.SetInt(this.Key, value);
		PlatformPlayerPrefs.Save();
	}

	public void AdvanceIntValue()
	{
		if (PlatformPlayerPrefs.HasKey(this.Key))
		{
			int @int = PlatformPlayerPrefs.GetInt(this.Key);
			PlatformPlayerPrefs.SetInt(this.Key, @int + 1);
			PlatformPlayerPrefs.Save();
			return;
		}
		PlatformPlayerPrefs.SetInt(this.Key, 1);
		PlatformPlayerPrefs.Save();
	}

	public void InvokeEvents()
	{
		if (this.SimulateKeyExists || PlatformPlayerPrefs.HasKey(this.Key))
		{
			this.OnPlayerPrefsHaveKey.Invoke();
			return;
		}
		this.OnPlayerPrefsDoNotHaveKey.Invoke();
	}

	public void InvokeIndexEvents()
	{
		if (this.SimulateIndexMatch || (PlatformPlayerPrefs.HasKey(this.Key) && PlatformPlayerPrefs.GetInt(this.Key) == this.RelevantIndex))
		{
			this.OnIndexMatch.Invoke();
			return;
		}
		this.OnIndexDoNotMatch.Invoke();
	}

	public string Key = "empty";

	public UnityEvent OnPlayerPrefsHaveKey;

	public UnityEvent OnPlayerPrefsDoNotHaveKey;

	[Space]
	public int RelevantIndex;

	public UnityEvent OnIndexMatch;

	public UnityEvent OnIndexDoNotMatch;

	public bool SimulateIndexMatch;

	public bool SimulateKeyExists;
}

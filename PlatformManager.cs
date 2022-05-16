using System;
using UnityEngine;
using UnityEngine.Events;

public class PlatformManager : MonoBehaviour
{
	public static bool UseLowerFPSVideos
	{
		get
		{
			return PlatformManager.platformManager.UseLowerFPSVideos;
		}
	}

	public static PlatformManager Instance
	{
		get
		{
			if (PlatformManager.instance == null)
			{
				PlatformManager.instance = Object.FindObjectOfType<PlatformManager>();
				if (PlatformManager.instance == null)
				{
					PlatformManager.instance = new GameObject("PlatformManager").AddComponent<PlatformManager>();
				}
			}
			return PlatformManager.instance;
		}
	}

	public void Init()
	{
		if (!PlatformManager.isInitialized)
		{
			PlatformManager.platformManager = new PCManager();
			PlatformPlayerPrefsHelper.saveSystemInitialized = (Action)Delegate.Combine(PlatformPlayerPrefsHelper.saveSystemInitialized, new Action(this.OnSaveSystemInitialized));
			PlatformManager.platformManager.Init();
			PlatformAchievements.InitPlatformAchievements(PlatformManager.platformManager.Achievements);
			PlatformGamepad.InitPlatformGamepad(PlatformManager.platformManager.Gamepad);
			PlatformRichPresence.InitPlatformRichPresence(PlatformManager.platformManager.RichPresence);
			PlatformManager.isInitialized = true;
			Object.DontDestroyOnLoad(PlatformManager.instance.gameObject);
		}
	}

	private void Awake()
	{
		if (PlatformManager.instance != null && PlatformManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		PlatformManager.instance = this;
		this.Init();
	}

	private void OnSaveSystemInitialized()
	{
		UnityEvent unityEvent = this.saveSystemInitializedEvent;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		PlatformPlayerPrefsHelper.saveSystemInitialized = (Action)Delegate.Remove(PlatformPlayerPrefsHelper.saveSystemInitialized, new Action(this.OnSaveSystemInitialized));
	}

	private void Update()
	{
		IPlatformManager platformManager = PlatformManager.platformManager;
		if (platformManager == null)
		{
			return;
		}
		platformManager.PlatformManagerUpdate();
	}

	public UnityEvent saveSystemInitializedEvent;

	public static bool UseLowEndConfiguration;

	private static IPlatformManager platformManager;

	private static PlatformManager instance;

	private static bool isInitialized;
}

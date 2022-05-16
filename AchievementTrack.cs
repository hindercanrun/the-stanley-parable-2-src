using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementTrack : MonoBehaviour
{
	private void Awake()
	{
		PlatformAchievements.AchievementUnlocked = (Action<AchievementID>)Delegate.Combine(PlatformAchievements.AchievementUnlocked, new Action<AchievementID>(this.OnUnlockAchievement));
		SimpleEvent simpleEvent = this.freedomEndingCompleteEvent;
		simpleEvent.OnCall = (Action)Delegate.Combine(simpleEvent.OnCall, new Action(this.CheckSpeedrun));
		StanleyController.OnInteract = (Action<GameObject>)Delegate.Combine(StanleyController.OnInteract, new Action<GameObject>(this.UpdateYouCantJump));
		if (PlatformPlayerPrefs.HasKey(this.playingMinutesKey))
		{
			this.playingMinutes = PlatformPlayerPrefs.GetInt(this.playingMinutesKey);
		}
		this.FillConfDictionary();
	}

	private void FillConfDictionary()
	{
		foreach (FloatConfigurable floatConfigurable in this.floatConfigurables)
		{
			this.trackedFloatConfigurables.Add(floatConfigurable.Key, new AchievementTrack.NumberConfigurableStatus(floatConfigurable));
			FloatConfigurable floatConfigurable2 = floatConfigurable;
			floatConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(floatConfigurable2.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
		foreach (BooleanConfigurable booleanConfigurable in this.booleanConfigurables)
		{
			this.trackedBooleanConfigurables.Add(booleanConfigurable.Key, new AchievementTrack.BooleanConfigurableStatus(booleanConfigurable));
			BooleanConfigurable booleanConfigurable2 = booleanConfigurable;
			booleanConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(booleanConfigurable2.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
		foreach (IntConfigurable intConfigurable in this.intConfigurables)
		{
			this.trackedIntConfigurables.Add(intConfigurable.Key, new AchievementTrack.NumberConfigurableStatus(intConfigurable));
			IntConfigurable intConfigurable2 = intConfigurable;
			intConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable2.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
	}

	private void OnConfigurableValueUpdate(LiveData data)
	{
		switch (data.configureableType)
		{
		case ConfigurableTypes.Int:
		{
			AchievementTrack.NumberConfigurableStatus numberConfigurableStatus;
			if (this.trackedIntConfigurables.TryGetValue(data.key, out numberConfigurableStatus))
			{
				if (data.IntValue == numberConfigurableStatus.Intc.MaxValue)
				{
					numberConfigurableStatus.maxValueReached = true;
				}
				if (data.IntValue == numberConfigurableStatus.Intc.MinValue)
				{
					numberConfigurableStatus.minValueReached = true;
				}
				if (numberConfigurableStatus.minValueReached && numberConfigurableStatus.maxValueReached)
				{
					IntConfigurable intc = numberConfigurableStatus.Intc;
					intc.OnValueChanged = (Action<LiveData>)Delegate.Remove(intc.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
					this.trackedIntConfigurables.Remove(data.key);
				}
			}
			break;
		}
		case ConfigurableTypes.Float:
		{
			AchievementTrack.NumberConfigurableStatus numberConfigurableStatus2;
			if (this.trackedFloatConfigurables.TryGetValue(data.key, out numberConfigurableStatus2))
			{
				if (Mathf.Approximately(data.FloatValue, numberConfigurableStatus2.FloatC.MaxValue))
				{
					numberConfigurableStatus2.maxValueReached = true;
				}
				if (Mathf.Approximately(data.FloatValue, numberConfigurableStatus2.FloatC.MinValue))
				{
					numberConfigurableStatus2.minValueReached = true;
				}
				if (numberConfigurableStatus2.minValueReached && numberConfigurableStatus2.maxValueReached)
				{
					FloatConfigurable floatC = numberConfigurableStatus2.FloatC;
					floatC.OnValueChanged = (Action<LiveData>)Delegate.Remove(floatC.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
					this.trackedFloatConfigurables.Remove(data.key);
				}
			}
			break;
		}
		case ConfigurableTypes.Boolean:
		{
			AchievementTrack.BooleanConfigurableStatus booleanConfigurableStatus = null;
			if (this.trackedBooleanConfigurables.TryGetValue(data.key, out booleanConfigurableStatus) && data.BooleanValue != booleanConfigurableStatus.defaultValue)
			{
				BooleanConfigurable cachedBoolC = booleanConfigurableStatus.cachedBoolC;
				cachedBoolC.OnValueChanged = (Action<LiveData>)Delegate.Remove(cachedBoolC.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
				this.trackedBooleanConfigurables.Remove(data.key);
			}
			break;
		}
		}
		if (this.trackedFloatConfigurables.Count == 0 && this.trackedIntConfigurables.Count == 0 && this.trackedBooleanConfigurables.Count == 0)
		{
			this.settingsWorldChampToEnableBooleanConfigurable.SetValue(true);
			PlatformAchievements.UnlockAchievement(AchievementID.SettingsWorldChampion);
		}
	}

	private void OnDestroy()
	{
		PlatformAchievements.AchievementUnlocked = (Action<AchievementID>)Delegate.Remove(PlatformAchievements.AchievementUnlocked, new Action<AchievementID>(this.OnUnlockAchievement));
		SimpleEvent simpleEvent = this.freedomEndingCompleteEvent;
		simpleEvent.OnCall = (Action)Delegate.Remove(simpleEvent.OnCall, new Action(this.CheckSpeedrun));
		StanleyController.OnInteract = (Action<GameObject>)Delegate.Remove(StanleyController.OnInteract, new Action<GameObject>(this.UpdateYouCantJump));
		foreach (FloatConfigurable floatConfigurable in this.floatConfigurables)
		{
			floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(floatConfigurable.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
		foreach (BooleanConfigurable booleanConfigurable in this.booleanConfigurables)
		{
			booleanConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(booleanConfigurable.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
		foreach (IntConfigurable intConfigurable in this.intConfigurables)
		{
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnConfigurableValueUpdate));
		}
	}

	private void Start()
	{
		SceneManager.sceneLoaded += this.CheckStartupAchievements;
	}

	private void CheckStartupAchievements(Scene scene, LoadSceneMode loadsceneMode)
	{
		if (scene.name == "init" || scene.name == "Loading_UD_MASTER" || scene.name == "Settings_UD_MASTER")
		{
			return;
		}
		if (!this.checkedStartupAchievements)
		{
			this.checkedStartupAchievements = true;
			base.StartCoroutine(this.CheckStartupAchievementsRoutine());
		}
	}

	private IEnumerator CheckStartupAchievementsRoutine()
	{
		float checkTimer = 2f;
		while (checkTimer > 0f)
		{
			if (this.IsInGame)
			{
				checkTimer -= Time.unscaledDeltaTime;
			}
			yield return null;
		}
		this.CheckWelcomeBack();
		this.CheckSuperGoOutside();
		yield break;
	}

	private void Update()
	{
		this.runTime = GameMaster.RunTime;
		this.CheckCommitment();
		this.CheckJumpKeyboardMouse();
	}

	private void OnUnlockAchievement(AchievementID id)
	{
		base.StartCoroutine(this.CheckFirstAchievement());
	}

	private IEnumerator CheckFirstAchievement()
	{
		yield return new WaitForSeconds(1f);
		if (PlatformAchievements.AchievementsUnlockedCount >= 1 && !PlatformAchievements.IsAchievementUnlocked(AchievementID.First))
		{
			PlatformAchievements.UnlockAchievement(AchievementID.First);
		}
		yield break;
	}

	private void CheckCommitment()
	{
		if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Tuesday)
		{
			float num = Time.realtimeSinceStartup - this.minuteTimeStamp;
			if (num >= 60f)
			{
				int num2 = Mathf.FloorToInt(num / 60f);
				this.minuteTimeStamp = Time.realtimeSinceStartup;
				this.playingMinutes += num2;
				if (this.playingMinutes >= 1440)
				{
					PlatformAchievements.UnlockAchievement(AchievementID.Tuesday);
				}
				PlatformPlayerPrefs.SetInt(this.playingMinutesKey, this.playingMinutes);
			}
		}
	}

	private void CheckWelcomeBack()
	{
		string key = "FirstStartup";
		if (PlatformPlayerPrefs.HasKey(key))
		{
			PlatformAchievements.UnlockAchievement(AchievementID.WelcomeBack);
			return;
		}
		PlatformPlayerPrefs.SetInt(key, 1);
	}

	private void CheckSpeedrun()
	{
		if (GameMaster.RunTime < 262f)
		{
			PlatformAchievements.UnlockAchievement(AchievementID.SpeedRun);
		}
	}

	private int lastJumpCounterMax
	{
		get
		{
			return 9;
		}
	}

	private float lastJumpTimeTimeout
	{
		get
		{
			return 5f;
		}
	}

	private bool IsInGame
	{
		get
		{
			return !GameMaster.PAUSEMENUACTIVE && !GameMaster.ONMAINMENUORSETTINGS && !Singleton<GameMaster>.Instance.IsLoading && !Singleton<GameMaster>.Instance.FullScreenMoviePlaying && !StanleyController.Instance.motionFrozen;
		}
	}

	private void CheckJumpKeyboardMouse()
	{
		if (this.IsInGame && Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse && Singleton<GameMaster>.Instance.stanleyActions.JumpAction.WasPressed)
		{
			this.UpdateYouCantJump(null);
		}
	}

	private void UpdateYouCantJump(GameObject interactObject)
	{
		if (interactObject != null || this.jumpCircleConfigurable.GetBooleanValue())
		{
			return;
		}
		float num = GameMaster.RunTime;
		if (this.lastJumpTime == -1f || num - this.lastJumpTime < this.lastJumpTimeTimeout)
		{
			this.lastJumpCounter++;
			this.lastJumpTime = num;
		}
		else
		{
			this.lastJumpCounter = -1;
			this.lastJumpTime = -1f;
		}
		if (this.lastJumpCounter >= this.lastJumpCounterMax)
		{
			PlatformAchievements.UnlockAchievement(AchievementID.YouCantJump);
		}
	}

	private void CheckSuperGoOutside()
	{
		string key = "LastStartupTime";
		if (PlatformPlayerPrefs.HasKey(key))
		{
			DateTime lastStartupTime = Convert.ToDateTime(PlatformPlayerPrefs.GetString(key));
			if (this.HasItBeenTenYears(lastStartupTime))
			{
				PlatformAchievements.UnlockAchievement(AchievementID.SuperGoOutside);
			}
		}
		PlatformPlayerPrefs.SetString(key, DateTime.UtcNow.ToString());
	}

	private bool HasItBeenTenYears(DateTime lastStartupTime)
	{
		bool result = false;
		DateTime t = lastStartupTime.AddYears(10).AddDays(-1.0);
		if (DateTime.UtcNow >= t)
		{
			result = true;
		}
		lastStartupTime - DateTime.UtcNow;
		return result;
	}

	public AchievementID id;

	[SerializeField]
	private Configurable jumpCircleConfigurable;

	[SerializeField]
	private SimpleEvent freedomEndingCompleteEvent;

	[SerializeField]
	private float runTime;

	private int playingMinutes;

	private readonly string playingMinutesKey = "playingMinutes";

	private float minuteTimeStamp;

	[Header("Settings World Champion Track")]
	[SerializeField]
	private FloatConfigurable[] floatConfigurables;

	[SerializeField]
	private BooleanConfigurable[] booleanConfigurables;

	[SerializeField]
	private IntConfigurable[] intConfigurables;

	[SerializeField]
	private BooleanConfigurable settingsWorldChampToEnableBooleanConfigurable;

	private Dictionary<string, AchievementTrack.BooleanConfigurableStatus> trackedBooleanConfigurables = new Dictionary<string, AchievementTrack.BooleanConfigurableStatus>();

	private Dictionary<string, AchievementTrack.NumberConfigurableStatus> trackedIntConfigurables = new Dictionary<string, AchievementTrack.NumberConfigurableStatus>();

	private Dictionary<string, AchievementTrack.NumberConfigurableStatus> trackedFloatConfigurables = new Dictionary<string, AchievementTrack.NumberConfigurableStatus>();

	[SerializeField]
	private BooleanConfigurable sanityBool;

	private bool checkedStartupAchievements;

	private int lastJumpCounter;

	private float lastJumpTime = -1f;

	private class NumberConfigurableStatus
	{
		public NumberConfigurableStatus(Configurable _configurable)
		{
			if (_configurable is FloatConfigurable)
			{
				this.FloatC = (_configurable as FloatConfigurable);
			}
			if (_configurable is IntConfigurable)
			{
				this.Intc = (_configurable as IntConfigurable);
			}
		}

		public FloatConfigurable FloatC;

		public IntConfigurable Intc;

		public bool minValueReached;

		public bool maxValueReached;
	}

	private class BooleanConfigurableStatus
	{
		public BooleanConfigurableStatus(BooleanConfigurable boolC)
		{
			this.cachedBoolC = boolC;
			this.defaultValue = this.cachedBoolC.GetBooleanValue();
		}

		public BooleanConfigurable cachedBoolC;

		public bool defaultValue;
	}
}

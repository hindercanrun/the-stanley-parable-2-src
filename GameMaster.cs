using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : Singleton<GameMaster>
{
	public AchievementsData AchievementsData
	{
		get
		{
			return this.achievementsData;
		}
	}

	public MainMenu MenuManager
	{
		get
		{
			return this.menuManager;
		}
	}

	public bool FullScreenMoviePlaying
	{
		get
		{
			return this.fullscreenPlaybackContext != null && this.fullscreenPlaybackContext.moviePlaying;
		}
	}

	private IEnumerable<GameMaster.MoviePlaybackContext> AllMoviePlaybackContexts
	{
		get
		{
			if (this.fullscreenPlaybackContext != null)
			{
				yield return this.fullscreenPlaybackContext;
			}
			foreach (KeyValuePair<string, GameMaster.MoviePlaybackContext> keyValuePair in this.inGamePlaybackContexts)
			{
				yield return keyValuePair.Value;
			}
			Dictionary<string, GameMaster.MoviePlaybackContext>.Enumerator enumerator = default(Dictionary<string, GameMaster.MoviePlaybackContext>.Enumerator);
			yield break;
			yield break;
		}
	}

	private bool AnyMoviePlaying
	{
		get
		{
			return new List<GameMaster.MoviePlaybackContext>(this.AllMoviePlaybackContexts).FindIndex((GameMaster.MoviePlaybackContext m) => m.moviePlaying) != -1;
		}
	}

	private IMoviePlayer GetNewPlatformMoviePlayer()
	{
		return new PCMoviePlayer();
	}

	[HideInInspector]
	public fint32 GameTime { get; private set; }

	[HideInInspector]
	public float GameDeltaTime { get; private set; }

	public float GameSmoothedDeltaTime { get; private set; }

	public static fint32 RunTime { get; private set; }

	public static event GameMaster.Pause OnPause;

	public static event GameMaster.Pause OnResume;

	public static event Action OnPrepareLoadingLevel;

	public bool MouseMoved { get; private set; }

	public float masterVolume
	{
		get
		{
			return this._masterVolume;
		}
		private set
		{
			if (value != this._masterVolume)
			{
				this._masterVolume = Mathf.Clamp01(value);
			}
		}
	}

	public float musicVolume
	{
		get
		{
			return this._musicVolume;
		}
		private set
		{
			this._musicVolume = Mathf.Clamp01(value);
		}
	}

	public float modulatedMusicVolume
	{
		get
		{
			return this.musicVolume * this.masterVolume;
		}
	}

	public bool closedCaptionsOn
	{
		get
		{
			return this._closedCaptionsOn;
		}
		private set
		{
			if (value != this._closedCaptionsOn)
			{
				this._closedCaptionsOn = value;
			}
		}
	}

	private void ResetTSPOriginalValues()
	{
		this.beginAgainMap1 = false;
		this.beginAgainMap2 = false;
		this.loungePassCounter = 1;
		this.broomPassCounter = 1;
		this.bossPassCounter = 1;
		this.bossSkipCounter = 1;
		this.countPassCounter = 1;
		this.buttonPassCounter = 1;
		this.seriousPassCounter = 1;
		this.boxesNextTime = false;
		this.barking = false;
	}

	public bool barking { get; private set; }

	public bool IsLoading
	{
		get
		{
			return this.loading;
		}
	}

	public float loadProgress
	{
		get
		{
			return this._loadProgress;
		}
	}

	private void OnLoadingScreenSetupDone()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		this.UpdateSceneAudioSource();
		if (this != Singleton<GameMaster>.Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		LoadingManager.OnLoadingScreenSetupDone = (Action)Delegate.Combine(LoadingManager.OnLoadingScreenSetupDone, new Action(this.OnLoadingScreenSetupDone));
		SceneManager.sceneLoaded += this.UpdateSceneAudioSource;
		AssetBundleControl.OnSceneReady = (Action)Delegate.Combine(AssetBundleControl.OnSceneReady, new Action(this.OnSceneReady));
		PlatformPlayerPrefsHelper.saveSystemInitialized = (Action)Delegate.Combine(PlatformPlayerPrefsHelper.saveSystemInitialized, new Action(this.ReadAllPrefs));
		StanleyController.OnOutOfBounds = (Action)Delegate.Combine(StanleyController.OnOutOfBounds, new Action(this.OnOutOfBounds));
		PlatformAchievements.OnAchievementUnlockedFirstTime += this.RecordAchievementDate;
		if (this.ReporterPrefab != null)
		{
			Object.Instantiate<GameObject>(this.ReporterPrefab);
		}
		ReportUI.OnOpenReportTool = (Action)Delegate.Combine(ReportUI.OnOpenReportTool, new Action(this.OnReportPause));
		PlatformManager.Instance.Init();
		this.languageProfile.SetNewMaxValue(this.languageProfileData.profiles.Length - 1);
		IntConfigurable intConfigurable = this.languageProfile;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnLanguageChanged));
		this.InitMainMenu();
		this.stanleyActions = StanleyActions.CreateWithDefaultBindings();
		this.InputTypeDetectionInit();
		StanleyController stanleyController = Object.FindObjectOfType<StanleyController>();
		if (this.stanleyPrefab)
		{
			bool flag = stanleyController != null;
			Vector3 position = flag ? stanleyController.transform.position : this.stanleyPrefab.transform.position;
			Quaternion rotation = flag ? stanleyController.transform.rotation : this.stanleyPrefab.transform.rotation;
			Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(this.stanleyPrefab, position, rotation));
			GameObject gameObject = Object.Instantiate<GameObject>(this.choreoPrefab);
			this.captionCanvases = gameObject.GetComponentsInChildren<Canvas>();
		}
		else
		{
			Debug.LogWarning("GameMaster has been made via means other than the menu, hopefully you're just running this in the editor?");
		}
		if (this.crosshair == null && this.crosshairPrefab != null)
		{
			this.crosshair = Object.Instantiate<GameObject>(this.crosshairPrefab);
			Object.DontDestroyOnLoad(this.crosshair);
		}
		if (this.eyelidsObj == null && this.eyelidsPrefab != null)
		{
			this.eyelidsObj = Object.Instantiate<GameObject>(this.eyelidsPrefab);
			this.eyelids = this.eyelidsObj.GetComponent<Eyelids>();
			Object.DontDestroyOnLoad(this.eyelidsObj);
			this.eyelidCanvas = this.eyelidsObj.GetComponent<Canvas>();
		}
		if (this.fadeCanvasPrefab != null)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.fadeCanvasPrefab);
			gameObject2.transform.parent = base.transform;
			this.fadeImage = gameObject2.GetComponentInChildren<RawImage>();
			this.fadeImage.enabled = false;
			this.fadeImage.color = Color.clear;
			this.fadeCanvas = gameObject2.GetComponent<Canvas>();
			this.fadeCanvas.enabled = false;
		}
		Object.Instantiate<GameObject>(this.figleyOverlayPrefab).transform.parent = base.transform;
		Object.Instantiate<GameObject>(this.platformSettingsPrefab).transform.parent = base.transform;
		Object.Instantiate<GameObject>(this.cursorControllerPrefab).transform.parent = base.transform;
		Object.Instantiate<GameObject>(this.resolutionControllerPrefab).transform.parent = base.transform;
		GameMaster.ONMAINMENUORSETTINGS = this.isOnMainMenuOrSettingScene(SceneManager.GetActiveScene().name);
		foreach (Configurable configurable in this.resetableConfigurablesList.allConfigurables)
		{
			configurable.Init();
		}
		if (StanleyController.Instance != null && !GameMaster.ONMAINMENUORSETTINGS)
		{
			StanleyController.Instance.FreezeMotionAndView();
		}
		SceneManager.sceneLoaded += this.SceneLoaded;
		this.GameTime = new fint32(0U);
		if (PlatformManager.UseLowEndConfiguration)
		{
			this.useDefaultPerformanceConfiguration.SetValue(false);
		}
	}

	private void RecordAchievementDate(AchievementID achievementID)
	{
		string value = DateTime.Now.ToShortDateString();
		this.achievementsData.FindAchievement(achievementID).dateFoundConfigurable.SetValue(value);
	}

	private void OnOutOfBounds()
	{
		string name = SceneManager.GetActiveScene().name;
		this.ChangeLevel(name, true);
	}

	private void InitMainMenu()
	{
		this.gameMenu = Object.Instantiate<GameObject>(this.menuPrefab);
		this.gameMenu.transform.parent = base.transform;
		this.menuManager = this.gameMenu.GetComponent<MainMenu>();
	}

	public static event Action OnFullDataReset;

	public void ReInit()
	{
		Object.Destroy(this.gameMenu);
		this.gameMenu = null;
		this.InitMainMenu();
		Action onFullDataReset = GameMaster.OnFullDataReset;
		if (onFullDataReset != null)
		{
			onFullDataReset();
		}
		this.ResetTSPOriginalValues();
	}

	private void OnLanguageChanged(LiveData liveData)
	{
		LocalizationManager.CurrentLanguage = this.languageProfileData.profiles[liveData.IntValue].DescriptionIni2Loc;
	}

	private void OnAchievementUnlock(int id)
	{
		this.achievementConfigurables[id].SetValue(true);
		this.achievementConfigurables[id].SaveToDiskAll();
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= this.SceneLoaded;
	}

	private void OnDestroy()
	{
		LoadingManager.OnLoadingScreenSetupDone = (Action)Delegate.Remove(LoadingManager.OnLoadingScreenSetupDone, new Action(this.OnLoadingScreenSetupDone));
		SceneManager.sceneLoaded -= this.UpdateSceneAudioSource;
		AssetBundleControl.OnSceneReady = (Action)Delegate.Remove(AssetBundleControl.OnSceneReady, new Action(this.OnSceneReady));
		PlatformPlayerPrefsHelper.saveSystemInitialized = (Action)Delegate.Remove(PlatformPlayerPrefsHelper.saveSystemInitialized, new Action(this.ReadAllPrefs));
		StanleyController.OnOutOfBounds = (Action)Delegate.Remove(StanleyController.OnOutOfBounds, new Action(this.OnOutOfBounds));
		PlatformAchievements.OnAchievementUnlockedFirstTime -= this.RecordAchievementDate;
		if (this.languageProfile != null)
		{
			IntConfigurable intConfigurable = this.languageProfile;
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnLanguageChanged));
		}
	}

	private void OnReportPause()
	{
		if (this.menuCoroutine != null)
		{
			base.StopCoroutine(this.menuCoroutine);
		}
		this.menuCoroutine = this.OpenPauseMenu();
		base.StartCoroutine(this.menuCoroutine);
	}

	private void PauseActiveAudioSources()
	{
		this.pausedAudioSources.Clear();
		foreach (GameMaster.AudioSourceValues audioSourceValues in this.regularAudioSources)
		{
			if (audioSourceValues.source != null && audioSourceValues.source.isPlaying)
			{
				audioSourceValues.source.Pause();
				this.pausedAudioSources.Add(audioSourceValues.source);
			}
		}
	}

	private void UnpausePausedAudioSources()
	{
		foreach (AudioSource audioSource in this.pausedAudioSources)
		{
			if (audioSource != null)
			{
				audioSource.UnPause();
			}
		}
		this.pausedAudioSources.Clear();
	}

	public StanleyController RespawnStanley()
	{
		if (!this.stanleyPrefab)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.stanleyPrefab);
		Object.DontDestroyOnLoad(gameObject);
		return gameObject.GetComponent<StanleyController>();
	}

	private void PitchRegularAudioSources(float multiplier, bool divide = false)
	{
		for (int i = 0; i < this.regularAudioSources.Count; i++)
		{
			AudioSource source = this.regularAudioSources[i].source;
			if (source != null)
			{
				source.pitch = this.regularAudioSources[i].originalPitch * multiplier;
			}
		}
	}

	private void Update()
	{
		if (!GameMaster.PAUSEMENUACTIVE && !this.loading)
		{
			Time.timeScale = GameMaster.timeMultiplier;
			this.GameDeltaTime = Time.deltaTime;
			this.GameSmoothedDeltaTime = Time.smoothDeltaTime;
			ChoreoMaster.GameSpeed = 1f;
			PhaseManager.GameSpeed = 1f;
		}
		else
		{
			Time.timeScale = 0f;
			this.GameDeltaTime = 0f;
			this.GameSmoothedDeltaTime = 0f;
			ChoreoMaster.GameSpeed = 0f;
			PhaseManager.GameSpeed = 0f;
		}
		this.GameTime += this.GameDeltaTime;
		GameMaster.RunTime += this.GameDeltaTime;
		foreach (GameMaster.MoviePlaybackContext moviePlaybackContext in this.AllMoviePlaybackContexts)
		{
			if (moviePlaybackContext.moviePlaying && moviePlaybackContext.canSkipmovie && !GameMaster.PAUSEMENUACTIVE && this.stanleyActions.UseAction.WasPressed)
			{
				if (moviePlaybackContext.moviePlayerSceneReference != null)
				{
					moviePlaybackContext.moviePlayerSceneReference.Skipped();
				}
				moviePlaybackContext.StopMovie();
			}
		}
		Vector2 vector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		bool flag = Input.GetAxisRaw("Mouse ScrollWheel") != 0f;
		this.MouseMoved = (vector.sqrMagnitude > 0f || flag);
		this.InputTypeChangeUpdate();
		if (this.stanleyActions.MenuOpen.WasPressed && !GameMaster.PAUSEMENUACTIVE && !this.pauseMenuBlocked)
		{
			if (this.menuCoroutine != null)
			{
				base.StopCoroutine(this.menuCoroutine);
			}
			this.menuCoroutine = this.OpenPauseMenu();
			base.StartCoroutine(this.menuCoroutine);
		}
	}

	public bool IsRumbleEnabled
	{
		get
		{
			return this.rumbleConfigurable.GetBooleanValue();
		}
	}

	public bool UsingSimplifiedControls
	{
		get
		{
			return this.simplifiedControlsConfigurable.GetBooleanValue();
		}
	}

	public GameMaster.InputDevice InputDeviceType { get; private set; }

	public event Action<GameMaster.InputDevice> OnInputDeviceTypeChanged;

	public void InputTypeDetectionInit()
	{
		if (!GameMaster.IsStandalonePlatform)
		{
			this.RegisterInputDeviceTypeChange(GameMaster.ConsoleSpecificInputDevice);
			return;
		}
		if (InputManager.ActiveDevice != null)
		{
			this.RegisterControllerForPC();
			return;
		}
		this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.KeyboardAndMouse);
	}

	private void RegisterControllerForPC()
	{
		string name = InputManager.ActiveDevice.Name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 1170147956U)
		{
			if (num <= 934826445U)
			{
				if (num != 19423422U)
				{
					if (num != 934826445U)
					{
						goto IL_117;
					}
					if (!(name == "PlayStation DualShock 2 Controller"))
					{
						goto IL_117;
					}
					goto IL_107;
				}
				else
				{
					if (!(name == "PlayStation 5 Controller"))
					{
						goto IL_117;
					}
					goto IL_10F;
				}
			}
			else if (num != 1169023264U)
			{
				if (num != 1170147956U)
				{
					goto IL_117;
				}
				if (!(name == "XBox One Controller"))
				{
					goto IL_117;
				}
			}
			else
			{
				if (!(name == "PlayStation 3 Controller"))
				{
					goto IL_117;
				}
				goto IL_107;
			}
		}
		else if (num <= 1297164263U)
		{
			if (num != 1281539097U)
			{
				if (num != 1297164263U)
				{
					goto IL_117;
				}
				if (!(name == "DualSense® wireless controller"))
				{
					goto IL_117;
				}
				goto IL_10F;
			}
			else if (!(name == "Xbox 360 Controller"))
			{
				goto IL_117;
			}
		}
		else if (num != 2418349549U)
		{
			if (num != 3684424571U)
			{
				goto IL_117;
			}
			if (!(name == "PlayStation 4 Controller"))
			{
				goto IL_117;
			}
			goto IL_107;
		}
		else
		{
			if (!(name == "DUALSHOCK®4 wireless controller"))
			{
				goto IL_117;
			}
			goto IL_107;
		}
		this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.GamepadXBOXOneOrGeneric);
		return;
		IL_107:
		this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.GamepadPS4);
		return;
		IL_10F:
		this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.GamepadPS5);
		return;
		IL_117:
		Debug.LogWarning("Unknown Controller, reverting to default xbox: " + InputManager.ActiveDevice.Name);
		this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.GamepadXBOXOneOrGeneric);
	}

	public void InputTypeChangeUpdate()
	{
		if (InputManager.AnyKeyIsPressed || Singleton<GameMaster>.Instance.MouseMoved)
		{
			new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
			this.RegisterInputDeviceTypeChange(GameMaster.InputDevice.KeyboardAndMouse);
		}
		if (InputManager.ActiveDevice.AnyButtonWasPressed || Mathf.Abs(InputManager.ActiveDevice.LeftStickX.Value) > 0.1f || Mathf.Abs(InputManager.ActiveDevice.LeftStickY.Value) > 0.1f || Mathf.Abs(InputManager.ActiveDevice.RightStickX.Value) > 0.1f || Mathf.Abs(InputManager.ActiveDevice.RightStickY.Value) > 0.1f || InputManager.ActiveDevice.LeftBumper.WasPressed || InputManager.ActiveDevice.RightBumper.WasPressed)
		{
			if (GameMaster.IsStandalonePlatform)
			{
				this.RegisterControllerForPC();
				return;
			}
			this.RegisterInputDeviceTypeChange(GameMaster.ConsoleSpecificInputDevice);
		}
	}

	private static bool IsStandalonePlatform
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (platform <= RuntimePlatform.WindowsEditor)
			{
				if (platform > RuntimePlatform.WindowsPlayer && platform != RuntimePlatform.WindowsEditor)
				{
					return false;
				}
			}
			else if (platform != RuntimePlatform.LinuxPlayer && platform != RuntimePlatform.LinuxEditor)
			{
				return false;
			}
			return true;
		}
	}

	private static GameMaster.InputDevice ConsoleSpecificInputDevice
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.PS4)
			{
				if (platform != RuntimePlatform.XboxOne)
				{
					switch (platform)
					{
					case RuntimePlatform.Switch:
						return GameMaster.InputDevice.GamepadSwitch;
					case RuntimePlatform.PS5:
						return GameMaster.InputDevice.GamepadPS5;
					}
				}
				return GameMaster.InputDevice.GamepadXBOXOneOrGeneric;
			}
			return GameMaster.InputDevice.GamepadPS4;
		}
	}

	private void RegisterInputDeviceTypeChange(GameMaster.InputDevice newDeviceType)
	{
		this.usingControllerConfigurable.Init();
		this.usingControllerConfigurable.SetValue(newDeviceType > GameMaster.InputDevice.KeyboardAndMouse);
		this.usingControllerConfigurable.SaveToDiskAll();
		if (this.InputDeviceType != newDeviceType)
		{
			this.InputDeviceType = newDeviceType;
			Action<GameMaster.InputDevice> onInputDeviceTypeChanged = this.OnInputDeviceTypeChanged;
			if (onInputDeviceTypeChanged == null)
			{
				return;
			}
			onInputDeviceTypeChanged(newDeviceType);
		}
	}

	private IEnumerator OpenPauseMenu()
	{
		if (this.loading || GameMaster.ONMAINMENUORSETTINGS)
		{
			yield break;
		}
		GameMaster.PAUSEMENUACTIVE = true;
		if (StanleyController.Instance.gameObject.activeInHierarchy)
		{
			this.activateStanleyOnResume = true;
			StanleyController.Instance.FreezeMotionAndView();
		}
		else
		{
			this.activateStanleyOnResume = false;
		}
		if (GameMaster.OnPause != null)
		{
			GameMaster.OnPause();
		}
		this.PauseActiveAudioSources();
		foreach (GameMaster.MoviePlaybackContext moviePlaybackContext in this.AllMoviePlaybackContexts)
		{
			if (moviePlaybackContext.moviePlaying)
			{
				if (moviePlaybackContext.PlatformMoviePlayer != null)
				{
					moviePlaybackContext.PlatformMoviePlayer.Pause();
				}
				moviePlaybackContext.movieToResume = true;
			}
		}
		this.menuManager.CallPauseMenu();
		Canvas[] array = this.captionCanvases;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		yield break;
	}

	public void ClosePauseMenu(bool resumeGame = true)
	{
		if (resumeGame && GameMaster.OnResume != null)
		{
			GameMaster.OnResume();
		}
		this.UnpausePausedAudioSources();
		foreach (GameMaster.MoviePlaybackContext moviePlaybackContext in this.AllMoviePlaybackContexts)
		{
			if (resumeGame && moviePlaybackContext.movieToResume)
			{
				if (moviePlaybackContext.PlatformMoviePlayer != null)
				{
					moviePlaybackContext.PlatformMoviePlayer.Unpause();
				}
				moviePlaybackContext.movieToResume = false;
			}
		}
		GameMaster.PAUSEMENUACTIVE = false;
		Canvas[] array = this.captionCanvases;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (resumeGame && this.activateStanleyOnResume)
		{
			StanleyController.Instance.UnfreezeMotionAndView();
		}
		GameMaster.CursorLockState = CursorLockMode.Locked;
		GameMaster.CursorVisible = false;
	}

	public static bool CursorVisible
	{
		get
		{
			return Cursor.visible;
		}
		set
		{
			Cursor.visible = value;
		}
	}

	public static CursorLockMode CursorLockState
	{
		get
		{
			return Cursor.lockState;
		}
		set
		{
			Cursor.lockState = value;
		}
	}

	public GameMaster.MoviePlaybackContext StartMovie(bool skip, MoviePlayer player, string cameraName, string moviePath, bool isFullscreenMovie)
	{
		GameMaster.MoviePlaybackContext moviePlaybackContext;
		if (isFullscreenMovie)
		{
			if (this.fullscreenPlaybackContext == null)
			{
				this.fullscreenPlaybackContext = new GameMaster.MoviePlaybackContext(new Func<IMoviePlayer>(this.GetNewPlatformMoviePlayer));
			}
			moviePlaybackContext = this.fullscreenPlaybackContext;
		}
		else
		{
			if (!this.inGamePlaybackContexts.TryGetValue(cameraName, out moviePlaybackContext))
			{
				this.inGamePlaybackContexts[cameraName] = new GameMaster.MoviePlaybackContext(new Func<IMoviePlayer>(this.GetNewPlatformMoviePlayer));
			}
			moviePlaybackContext = this.inGamePlaybackContexts[cameraName];
		}
		if (moviePlaybackContext == null)
		{
			return null;
		}
		moviePlaybackContext.StartMovie(skip, player, cameraName, moviePath);
		return moviePlaybackContext;
	}

	public void CancelFade()
	{
		this.fadeHold = false;
		this.fading = false;
		this.fadeImage.enabled = false;
		this.fadeCanvas.enabled = false;
	}

	public IEnumerator DelayedCancelFade()
	{
		yield return new WaitForSeconds(0.5f);
		if (this.fadeCoroutine == null)
		{
			this.CancelFade();
		}
		yield break;
	}

	public void BeginFade(Color color, float inDuration, float holdDuration, bool fadeFrom, bool stayOut)
	{
		this.fadeColor = color;
		if (this.fadeCoroutine != null)
		{
			base.StopCoroutine(this.fadeCoroutine);
		}
		this.fadeImage.enabled = true;
		this.fadeCanvas.enabled = true;
		this.fadeCoroutine = this.Fade(inDuration, holdDuration, fadeFrom, stayOut);
		base.StartCoroutine(this.fadeCoroutine);
	}

	private IEnumerator Fade(float inDuration, float holdDuration, bool fadeFrom, bool stayOut)
	{
		this.fadeHold = false;
		this.fading = true;
		TimePair timer = new TimePair(inDuration);
		float fadeStartAlpha = 0f;
		float fadeEndAlpha = this.fadeColor.a;
		if (fadeFrom)
		{
			fadeStartAlpha = fadeEndAlpha;
			fadeEndAlpha = 0f;
		}
		while (timer.keepWaiting)
		{
			float t = timer.InverseLerp();
			this.fadeColor.a = Mathf.Lerp(fadeStartAlpha, fadeEndAlpha, t);
			this.fadeImage.color = this.fadeColor;
			yield return new WaitForEndOfFrame();
		}
		this.fadeColor.a = fadeEndAlpha;
		this.fadeImage.color = this.fadeColor;
		if (holdDuration > 0f)
		{
			yield return new WaitForGameSeconds(holdDuration);
		}
		if (stayOut)
		{
			this.fadeHold = true;
		}
		else
		{
			this.fadeColor.a = 0f;
		}
		this.fading = false;
		if (!this.fadeHold)
		{
			this.fadeCanvas.enabled = false;
			this.fadeImage.enabled = false;
		}
		this.fadeCoroutine = null;
		yield break;
	}

	private bool isOnMainMenuOrSettingScene(string sceneName)
	{
		sceneName = sceneName.ToLower();
		return sceneName.Contains("settings") || sceneName.Contains("menu");
	}

	private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		string text = scene.name.ToLower();
		Resources.UnloadUnusedAssets();
		GameMaster.ONMAINMENUORSETTINGS = this.isOnMainMenuOrSettingScene(text);
		StanleyController.Instance.gameObject.SetActive(!GameMaster.ONMAINMENUORSETTINGS);
		if (!this.isOnMainMenuOrSettingScene(text))
		{
			StanleyController.Instance.NewMapReset();
			PlatformRichPresence.SetPresence((PresenceID)Random.Range(0, Enum.GetValues(typeof(PresenceID)).Length));
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			this.ClosePauseMenu(false);
		}
		if (!(text == "loadingscene_ud_master"))
		{
			if (!(text == "menu_ud_master"))
			{
				if (!(text == "map1_ud_master"))
				{
					if (!(text == "map2_ud_master"))
					{
						if (!(text == "seriousroom"))
						{
							return;
						}
						GameObject gameObject = GameObject.Find("seriouspass" + this.seriousPassCounter);
						if (gameObject)
						{
							gameObject.GetComponent<HammerEntity>().Input_Enable();
							return;
						}
						return;
					}
					else
					{
						GameObject gameObject2 = GameObject.Find("countpass" + this.countPassCounter);
						if (gameObject2)
						{
							gameObject2.GetComponent<HammerEntity>().Input_Enable();
						}
						if (!this.beginAgainMap2)
						{
							this.beginAgainMap2 = true;
							return;
						}
						GameObject gameObject3 = GameObject.Find("beginagainmap2");
						if (gameObject3)
						{
							gameObject3.GetComponent<HammerEntity>().Input_Enable();
							return;
						}
						return;
					}
				}
				else
				{
					GameMaster.RunTime = 0f;
					StanleyController.Instance.currentCam.transform.localPosition = Vector3.zero;
					StanleyController.Instance.currentCam.transform.localRotation = Quaternion.identity;
					StanleyController.Instance.currentCam.transform.localScale = Vector3.one;
					StanleyController.Instance.Bucket.SetBucket(false, false, true, true);
					StanleyController.Instance.Bucket.SetAnimationSpeedImmediate(1f);
					string key = "NumberOfTimesRestarted";
					if (PlatformPlayerPrefs.HasKey(key))
					{
						int num = PlatformPlayerPrefs.GetInt(key);
						num = Mathf.Clamp(num + 1, 0, 99);
						PlatformPlayerPrefs.SetInt(key, num);
						if (num >= 3)
						{
							string key2 = "NewContentAvailable";
							if (!PlatformPlayerPrefs.HasKey(key2))
							{
								PlatformPlayerPrefs.SetInt(key2, 1);
							}
						}
					}
					else
					{
						PlatformPlayerPrefs.SetInt(key, 1);
					}
					if (this.beginAgainMap1)
					{
						GameObject gameObject4 = GameObject.Find("beginagain");
						if (gameObject4)
						{
							gameObject4.GetComponent<HammerEntity>().Input_Enable();
						}
						GameObject gameObject5 = GameObject.Find("zaxis");
						if (gameObject5)
						{
							gameObject5.GetComponent<HammerEntity>().Input_Enable();
						}
					}
					else
					{
						this.beginAgainMap1 = true;
					}
					GameObject gameObject6 = GameObject.Find("loungeenter" + this.loungePassCounter);
					if (gameObject6)
					{
						gameObject6.GetComponent<HammerEntity>().Input_Enable();
					}
					GameObject gameObject7 = GameObject.Find("broompass" + this.broomPassCounter);
					if (gameObject7)
					{
						gameObject7.GetComponent<HammerEntity>().Input_Enable();
					}
					string arg = "";
					if (this.bossPassCounter > 1)
					{
						arg = "b";
					}
					GameObject gameObject8 = GameObject.Find("bossenter" + this.bossPassCounter + arg);
					if (gameObject8)
					{
						gameObject8.GetComponent<HammerEntity>().Input_Enable();
					}
					GameObject gameObject9 = GameObject.Find("bossskip" + this.bossSkipCounter);
					if (gameObject9)
					{
						gameObject9.GetComponent<HammerEntity>().Input_Enable();
					}
					GameObject gameObject10 = GameObject.Find("buthevL" + this.buttonPassCounter + "B");
					if (gameObject10)
					{
						gameObject10.GetComponent<FuncButton>().Input_Unlock();
					}
					GameObject gameObject11 = GameObject.Find("buthevL" + this.buttonPassCounter + "S1");
					if (gameObject11)
					{
						gameObject11.GetComponent<HammerEntity>().Input_Enable();
					}
					if (!this.boxesNextTime)
					{
						return;
					}
					GameObject gameObject12 = GameObject.Find("boxaxis");
					if (gameObject12)
					{
						gameObject12.GetComponent<HammerEntity>().Input_Enable();
						this.boxesNextTime = false;
						return;
					}
					return;
				}
			}
		}
		else
		{
			using (IEnumerator<GameMaster.MoviePlaybackContext> enumerator = this.AllMoviePlaybackContexts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameMaster.MoviePlaybackContext moviePlaybackContext = enumerator.Current;
					moviePlaybackContext.StopMovie();
				}
				return;
			}
		}
		this.beginAgainMap1 = false;
		this.CancelFade();
		this.menuManager.CallMainMenu();
	}

	private void PrepareLoadingLevel(string mapName)
	{
		this.loading = true;
		AudioListener.volume = 0f;
		this.StopSound();
		this.Crosshair(false);
		StanleyController.Instance.Deparent(true);
		StanleyController.Instance.ResetClientCommandFreezes();
		StanleyController.Instance.FreezeMotionAndView();
		Object.DontDestroyOnLoad(StanleyController.Instance.gameObject);
		this.sceneComingFrom = SceneManager.GetActiveScene();
		this.SetLoadingStyle(SceneManager.GetActiveScene().name, mapName);
		this.Blackout();
		Action onPrepareLoadingLevel = GameMaster.OnPrepareLoadingLevel;
		if (onPrepareLoadingLevel != null)
		{
			onPrepareLoadingLevel();
		}
		Singleton<ChoreoMaster>.Instance.DropAll();
	}

	public bool ChangeLevel(string mapName, bool waitMinLoadTime = true)
	{
		if (this.loading)
		{
			return false;
		}
		if (AssetBundleControl.ChangeScene(mapName, "LoadingScene_UD_Master", this))
		{
			this.PrepareLoadingLevel(mapName);
			return true;
		}
		return false;
	}

	public void OnSceneReady()
	{
		this.loading = false;
		AudioListener.volume = Singleton<GameMaster>.Instance.masterVolume;
		base.StartCoroutine(this.DelayedCancelFade());
	}

	public void LoungePass()
	{
		if (this.loungePassCounter < 8)
		{
			this.loungePassCounter++;
		}
	}

	public void BroomPass()
	{
		if (this.broomPassCounter < 3)
		{
			this.broomPassCounter++;
		}
	}

	public void BossPass()
	{
		if (this.bossPassCounter < 4)
		{
			this.bossPassCounter++;
		}
	}

	public void BossSkip()
	{
		if (this.bossSkipCounter < 3)
		{
			this.bossSkipCounter++;
		}
	}

	public void CountPass()
	{
		if (this.countPassCounter < 2)
		{
			this.countPassCounter++;
		}
	}

	public void ButtonPass()
	{
		if (this.buttonPassCounter < 5)
		{
			this.buttonPassCounter++;
		}
	}

	public void SeriousPass()
	{
		if (this.seriousPassCounter < 4)
		{
			this.seriousPassCounter++;
		}
	}

	[ContextMenu("SpawnPrefab")]
	public void SpawnPrefabOnStanley()
	{
		Object.Instantiate<GameObject>(this.spawnPrefab).transform.position = StanleyController.Instance.transform.position;
	}

	public void Boxes()
	{
		this.boxesNextTime = true;
	}

	public void BarkModeOn()
	{
		this.barking = true;
	}

	public void BarkModeOff()
	{
		this.barking = false;
	}

	public void TSP_Reload(int val)
	{
		this.TSP_Reload();
	}

	private void UpdateSceneAudioSource(Scene scene, LoadSceneMode mode)
	{
		this.UpdateSceneAudioSource();
	}

	private void UpdateSceneAudioSource()
	{
		this.sceneAudioSources = Object.FindObjectsOfType<AudioSource>();
		this.regularAudioSources.Clear();
		for (int i = 0; i < this.sceneAudioSources.Length; i++)
		{
			AudioSource audioSource = this.sceneAudioSources[i];
			if (this.IsRegularAudioSource(audioSource))
			{
				GameMaster.AudioSourceValues item = new GameMaster.AudioSourceValues(audioSource, audioSource.pitch);
				this.regularAudioSources.Add(item);
			}
		}
	}

	private bool IsRegularAudioSource(AudioSource source)
	{
		Object component = source.gameObject.GetComponent<AmbientGeneric>();
		Soundscape component2 = source.gameObject.GetComponent<Soundscape>();
		ChoreoMaster component3 = source.gameObject.GetComponent<ChoreoMaster>();
		return component == null && component2 == null && component3 == null;
	}

	public void TSP_Reload()
	{
		this.TSP_Reload(GameMaster.TSP_Reload_Behaviour.Standard);
	}

	public void TSP_Reload(GameMaster.TSP_Reload_Behaviour behaviour)
	{
		if (this.fadeCoroutine != null)
		{
			base.StopCoroutine(this.fadeCoroutine);
			this.fadeCoroutine = null;
		}
		this.Blackout();
		this.eyelids.Reset();
		this.ClosePauseMenu(false);
		StanleyController.Instance.Bucket.SetBucket(false, true, true, true);
		if (behaviour == GameMaster.TSP_Reload_Behaviour.Epilogue)
		{
			this.ChangeLevel("MemoryzonePartThree_UD_MASTER", true);
			return;
		}
		if (FigleyOverlayController.Instance.FiglysFound >= 5 && !this.memoryZoneTwoComplete.GetBooleanValue())
		{
			this.ChangeLevel("MemoryzonePartTwo_UD_MASTER", true);
			return;
		}
		if (this.tspSequelNumber.GetIntValue() == 8)
		{
			this.ChangeLevel("eight_UD_MASTER", true);
			return;
		}
		this.ChangeLevel("map1_UD_MASTER", true);
	}

	private void Blackout()
	{
		switch (GameMaster.LoadingScreenStyle)
		{
		case LoadingManager.LoadScreenStyle.Standard:
		case LoadingManager.LoadScreenStyle.Message:
		case LoadingManager.LoadScreenStyle.Black:
			this.fadeImage.color = Color.black;
			break;
		case LoadingManager.LoadScreenStyle.Blue:
			this.fadeImage.color = new Color(0f, 0.1098039f, 0.4705883f, 1f);
			break;
		case LoadingManager.LoadScreenStyle.DoneyWithTheFunny:
		case LoadingManager.LoadScreenStyle.White:
			this.fadeImage.color = Color.white;
			break;
		}
		this.fadeImage.enabled = true;
		this.fadeCanvas.enabled = true;
		if (this.fadeCoroutine != null)
		{
			base.StopCoroutine(this.fadeCoroutine);
		}
		this.fadeCoroutine = null;
	}

	public void TSP_MainMenu()
	{
		this.ChangeLevel("menu_UD_Master", true);
	}

	public void StopSound()
	{
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
			Singleton<ChoreoMaster>.Instance.DropAll();
		}
	}

	public void Crosshair(bool on)
	{
		if (this.crosshair == null)
		{
			return;
		}
		this.crosshair.GetComponentInChildren<Animator>().SetBool("Show", on);
	}

	private IEnumerator CrosshairFadeRoutine()
	{
		this.crosshair.GetComponentInChildren<Animator>().SetTrigger("FadeOut");
		yield return new WaitForGameSeconds(1f);
		this.crosshair.SetActive(false);
		this.crosshairFadeCoroutine = null;
		yield break;
	}

	public void StartCredits()
	{
	}

	public void EyelidAnimate(EyelidDir dir)
	{
		if (dir == EyelidDir.Close)
		{
			this.eyelids.StartClose();
			return;
		}
		this.eyelids.StartOpen();
	}

	public void StartApartment(string gameObjectName)
	{
		GameObject gameObject = GameObject.Find(gameObjectName);
		if (gameObject)
		{
			InstructorHint component = gameObject.GetComponent<InstructorHint>();
			base.StartCoroutine(this.ApartmentEnding(component));
		}
	}

	private IEnumerator ApartmentEnding(InstructorHint hint)
	{
		int num;
		for (int i = 0; i < hint.apartmentEndingRelays.Length; i = num + 1)
		{
			LogicRelay relay = hint.apartmentEndingRelays[i];
			while (!relay.isEnabled)
			{
				yield return null;
			}
			if (i == hint.apartmentEndingRelays.Length - 1 && hint.noInputOnLastRelay)
			{
				hint.ShowHint(i, false);
				yield return new WaitForGameSeconds(7f);
				hint.HideHint();
			}
			else
			{
				hint.ShowHint(i, true);
				while (hint.waiting)
				{
					yield return null;
				}
				yield return new WaitForGameSeconds(0.5f);
				relay.Input_Trigger();
			}
			relay = null;
			num = i;
		}
		yield break;
	}

	private void SetLoadingStyle(string from, string to)
	{
		to = to.Replace("_UD_MASTER", "").ToLower();
		from = from.Replace("_UD_MASTER", "").ToLower();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(to);
		if (num <= 1551104593U)
		{
			if (num <= 847392620U)
			{
				if (num <= 375251406U)
				{
					if (num != 31788274U)
					{
						if (num != 375251406U)
						{
							goto IL_2D6;
						}
						if (!(to == "zending"))
						{
							goto IL_2D6;
						}
					}
					else
					{
						if (!(to == "map_one"))
						{
							goto IL_2D6;
						}
						goto IL_2AC;
					}
				}
				else if (num != 654019781U)
				{
					if (num != 847392620U)
					{
						goto IL_2D6;
					}
					if (!(to == "incorrect"))
					{
						goto IL_2D6;
					}
					if (BucketController.HASBUCKET)
					{
						this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.DoneyWithTheFunny, "");
						goto IL_2E2;
					}
					this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Blue, "");
					goto IL_2E2;
				}
				else if (!(to == "redstair"))
				{
					goto IL_2D6;
				}
			}
			else if (num <= 1182879452U)
			{
				if (num != 1032425025U)
				{
					if (num != 1182879452U)
					{
						goto IL_2D6;
					}
					if (!(to == "thefirstmap"))
					{
						goto IL_2D6;
					}
					goto IL_2AC;
				}
				else
				{
					if (!(to == "apartment_ending"))
					{
						goto IL_2D6;
					}
					this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.White, "");
					goto IL_2E2;
				}
			}
			else if (num != 1427181539U)
			{
				if (num != 1551104593U)
				{
					goto IL_2D6;
				}
				if (!(to == "freedom"))
				{
					goto IL_2D6;
				}
			}
			else
			{
				if (!(to == "babygame"))
				{
					goto IL_2D6;
				}
				goto IL_2D6;
			}
		}
		else if (num <= 2581912890U)
		{
			if (num <= 2368333904U)
			{
				if (num != 1745255176U)
				{
					if (num != 2368333904U)
					{
						goto IL_2D6;
					}
					if (!(to == "theonlymap"))
					{
						goto IL_2D6;
					}
					goto IL_2AC;
				}
				else if (!(to == "settings"))
				{
					goto IL_2D6;
				}
			}
			else if (num != 2424245049U)
			{
				if (num != 2581912890U)
				{
					goto IL_2D6;
				}
				if (!(to == "menu"))
				{
					goto IL_2D6;
				}
			}
			else
			{
				if (!(to == "map2"))
				{
					goto IL_2D6;
				}
				this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Message, "LOADING");
				goto IL_2E2;
			}
		}
		else if (num <= 3365091272U)
		{
			if (num != 3274577880U)
			{
				if (num != 3365091272U)
				{
					goto IL_2D6;
				}
				if (!(to == "map_two"))
				{
					goto IL_2D6;
				}
				goto IL_2AC;
			}
			else
			{
				if (!(to == "map_death"))
				{
					goto IL_2D6;
				}
				goto IL_2D6;
			}
		}
		else if (num != 3751997361U)
		{
			if (num != 3982288071U)
			{
				goto IL_2D6;
			}
			if (!(to == "buttonworld"))
			{
				goto IL_2D6;
			}
		}
		else
		{
			if (!(to == "map"))
			{
				goto IL_2D6;
			}
			goto IL_2AC;
		}
		this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Black, "");
		goto IL_2E2;
		IL_2AC:
		this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Message, "NEVER THE END IS");
		goto IL_2E2;
		IL_2D6:
		this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Message, "NEVER THE END IS");
		IL_2E2:
		if (from == "map")
		{
			this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Message, "NEVER THE END IS");
		}
		if (from == "settings")
		{
			this.UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle.Minimal, "");
		}
	}

	private void UpdateLoadingStyleChange(LoadingManager.LoadScreenStyle newStyle, string newLoadingMessage = "")
	{
		GameMaster.LoadingScreenStyle = newStyle;
		if (newLoadingMessage != "")
		{
			GameMaster.LoadingScreenMessage = newLoadingMessage;
		}
	}

	public void ReadAllPrefs()
	{
		this.SetMasterVolume(PlatformPlayerPrefs.GetFloat("PREFSKEY_MASTER_VOLUME", 1f));
		this.SetMusicVolume(PlatformPlayerPrefs.GetFloat("PREFSKEY_MUSIC_VOLUME", 1f));
		this.SetCaptionsActive(PlatformPlayerPrefs.GetInt("PREFSKEY_CAPTIONING", 1) == 1);
	}

	public void WriteAllPrefs()
	{
		PlatformPlayerPrefs.SetFloat("PREFSKEY_MASTER_VOLUME", this.masterVolume);
		PlatformPlayerPrefs.SetFloat("PREFSKEY_MUSIC_VOLUME", this.musicVolume);
		PlatformPlayerPrefs.SetInt("PREFSKEY_CAPTIONING", this.closedCaptionsOn ? 1 : 0);
	}

	public void SetMasterVolume(float val)
	{
		this.masterVolume = val;
	}

	public void SetMusicVolume(float val)
	{
		this.musicVolume = val;
	}

	public void SetCaptionsActive(bool state)
	{
		this.closedCaptionsOn = state;
	}

	public static LoadingManager.LoadScreenStyle LoadingScreenStyle = LoadingManager.LoadScreenStyle.Minimal;

	public static Action<float> OnUpdateLoadProgress;

	public static string LoadingScreenMessage = "LOADING";

	public GameObject stanleyPrefab;

	public GameObject choreoPrefab;

	public GameObject crosshairPrefab;

	public GameObject eyelidsPrefab;

	public GameObject menuPrefab;

	public GameObject fadeCanvasPrefab;

	public GameObject ReporterPrefab;

	public GameObject figleyOverlayPrefab;

	public GameObject platformSettingsPrefab;

	public GameObject cursorControllerPrefab;

	public GameObject resolutionControllerPrefab;

	[Header("Global/Scriptable Object -Data")]
	public SparkEffectData sparkEffectData;

	[SerializeField]
	private BooleanConfigurable[] achievementConfigurables;

	[SerializeField]
	private AchievementsData achievementsData;

	private GameObject crosshair;

	private GameObject eyelidsObj;

	private Eyelids eyelids;

	private GameObject gameMenu;

	private MainMenu menuManager;

	private RawImage fadeImage;

	private Camera fadeCamera;

	private IEnumerator menuCoroutine;

	private AssetBundle levelAssetBundle;

	public static bool systemRequestPauseMenu = false;

	private GameMaster.MoviePlaybackContext fullscreenPlaybackContext;

	private Dictionary<string, GameMaster.MoviePlaybackContext> inGamePlaybackContexts = new Dictionary<string, GameMaster.MoviePlaybackContext>();

	private bool fading;

	private Color fadeColor = Color.black;

	private bool fadeHold;

	private IEnumerator fadeCoroutine;

	private Canvas fadeCanvas;

	private Canvas[] captionCanvases;

	private Canvas eyelidCanvas;

	[Header("Time")]
	public static float timeMultiplier = 1f;

	private float fastForwardMultiplier = 4f;

	private float slowDownMultiplier = 0.1f;

	[HideInInspector]
	public static bool PAUSEMENUACTIVE;

	[HideInInspector]
	public bool pauseMenuBlocked;

	private bool activateStanleyOnResume;

	[HideInInspector]
	public StanleyActions stanleyActions;

	public BooleanConfigurable DefaultConfigConfigurable;

	[Header("Prefab to Spawn via Command")]
	public GameObject spawnPrefab;

	[Header("Language Selection")]
	public IntConfigurable languageProfile;

	public LanguageProfileData languageProfileData;

	[Header("Memeory Zone Configurables")]
	public BooleanConfigurable memoryZoneOneComplete;

	public BooleanConfigurable memoryZoneTwoComplete;

	public IntConfigurable tspSequelNumber;

	[Header("Configurables Init")]
	public ResetableConfigurablesList resetableConfigurablesList;

	private const string PREFSKEY_MASTER_VOLUME = "PREFSKEY_MASTER_VOLUME";

	private const string PREFSKEY_MUSIC_VOLUME = "PREFSKEY_MUSIC_VOLUME";

	private const string PREFSKEY_CAPTIONING = "PREFSKEY_CAPTIONING";

	[SerializeField]
	private AudioSource[] sceneAudioSources = new AudioSource[0];

	[SerializeField]
	private List<AudioSource> pausedAudioSources = new List<AudioSource>();

	[SerializeField]
	private List<GameMaster.AudioSourceValues> regularAudioSources = new List<GameMaster.AudioSourceValues>();

	private float _masterVolume = 1f;

	private float _musicVolume = 1f;

	private bool _closedCaptionsOn = true;

	[Header("Default Config Configurable")]
	public BooleanConfigurable useDefaultPerformanceConfiguration;

	public static bool ONMAINMENUORSETTINGS = true;

	private bool beginAgainMap1;

	private bool beginAgainMap2;

	private int loungePassCounter = 1;

	private int broomPassCounter = 1;

	private int bossPassCounter = 1;

	private int bossSkipCounter = 1;

	private int countPassCounter = 1;

	private int buttonPassCounter = 1;

	private int seriousPassCounter = 1;

	private bool boxesNextTime;

	private Scene sceneComingFrom;

	private bool loading;

	private float _loadProgress;

	[Header("Controller Rumble")]
	public BooleanConfigurable rumbleConfigurable;

	[Header("Input and Keybinding")]
	public BooleanConfigurable simplifiedControlsConfigurable;

	public BooleanConfigurable usingControllerConfigurable;

	private const string RegistrationMark = "®";

	private Coroutine crosshairFadeCoroutine;

	public class MoviePlaybackContext
	{
		public bool MoviePlaying
		{
			get
			{
				return this.moviePlaying;
			}
		}

		public GameObject CameraGameObject { get; private set; }

		private Camera Camera
		{
			get
			{
				if (!(this.CameraGameObject != null))
				{
					return null;
				}
				return this.CameraGameObject.GetComponent<Camera>();
			}
		}

		public bool CameraEnabled
		{
			get
			{
				return this.Camera != null && this.Camera.enabled;
			}
			set
			{
				if (this.Camera != null)
				{
					this.Camera.enabled = value;
				}
			}
		}

		public MoviePlaybackContext(Func<IMoviePlayer> platformMoviePlayerGenerator)
		{
			this.PlatformMoviePlayer = platformMoviePlayerGenerator();
		}

		public void StartMovie(bool skip, MoviePlayer player, string cameraName, string moviePath)
		{
			this.moviePlaying = true;
			this.moviePlayerSceneReference = player;
			this.canSkipmovie = skip;
			if (this.PlatformMoviePlayer != null)
			{
				this.PlatformMoviePlayer.OnMovieLoopPointReached -= this.OnMovieLoopPointReached;
				this.PlatformMoviePlayer.OnMovieLoopPointReached += this.OnMovieLoopPointReached;
				this.CameraGameObject = this.PlatformMoviePlayer.Play(cameraName, moviePath);
			}
		}

		private void OnMovieLoopPointReached()
		{
			if (this.moviePlayerSceneReference != null && !this.moviePlayerSceneReference.loop)
			{
				this.moviePlayerSceneReference.Ended(this);
				this.StopMovie();
			}
		}

		public void StopMovie()
		{
			this.moviePlaying = false;
			if (this.PlatformMoviePlayer != null)
			{
				this.PlatformMoviePlayer.Stop();
			}
		}

		public bool moviePlaying;

		public MoviePlayer moviePlayerSceneReference;

		public IMoviePlayer PlatformMoviePlayer;

		public bool canSkipmovie;

		public bool movieToResume;
	}

	public delegate void Pause();

	[Serializable]
	private struct AudioSourceValues
	{
		public AudioSourceValues(AudioSource _source, float pitch)
		{
			this.source = _source;
			this.originalPitch = pitch;
		}

		public AudioSource source;

		public float originalPitch;
	}

	public enum InputDevice
	{
		KeyboardAndMouse,
		GamepadXBOXOneOrGeneric,
		GamepadPS4,
		GamepadSwitch,
		GamepadXBOXSeriesX,
		GamepadPS5
	}

	public enum TSP_Reload_Behaviour
	{
		Standard,
		Epilogue
	}
}

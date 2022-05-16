using System;
using System.Collections;
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	private void Awake()
	{
		this.rootCanvas = base.GetComponent<Canvas>();
		this.cachedCaster = base.GetComponent<GraphicRaycaster>();
		this.menuAudio.SetActive(false);
	}

	private void Start()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.Instance_OnInputDeviceTypeChanged;
		this.scrollRects = base.GetComponentsInChildren<ScrollRect>(true);
		this.softMasks = base.GetComponentsInChildren<SoftMask>(true);
		this.textMeshPros = base.GetComponentsInChildren<TextMeshProUGUI>(true);
		this.uiCameras = base.GetComponentsInChildren<Camera>(true);
		this.canvases = base.GetComponentsInChildren<Canvas>(true);
		this.audioSources = base.GetComponentsInChildren<AudioSource>(true);
		if (!GameMaster.ONMAINMENUORSETTINGS)
		{
			this.SetRoot(false);
		}
		Canvas component = base.GetComponent<Canvas>();
		component.worldCamera.gameObject.SetActive(false);
		component.renderMode = RenderMode.ScreenSpaceOverlay;
	}

	private void Instance_OnInputDeviceTypeChanged(GameMaster.InputDevice deviceType)
	{
		if (this.InAMenu)
		{
			if (deviceType == GameMaster.InputDevice.KeyboardAndMouse)
			{
				GameMaster.CursorVisible = Singleton<GameMaster>.Instance.MouseMoved;
				return;
			}
			GameMaster.CursorVisible = false;
		}
	}

	public bool InAMenu { get; private set; }

	private void Update()
	{
		if (this.InAMenu && Singleton<GameMaster>.Instance.MouseMoved && !GameMaster.CursorVisible)
		{
			GameMaster.CursorVisible = true;
		}
	}

	private void EnterMenu()
	{
		this.InAMenu = true;
		this.HideConsoleOnlyOptions();
		this.SetRoot(true);
		Cursor.lockState = CursorLockMode.None;
		GameMaster.CursorVisible = false;
		this.menuAudio.SetActive(true);
	}

	public void ExitMenu()
	{
		this.SetRoot(false);
		this.menuAudio.SetActive(false);
		this.InAMenu = false;
		Cursor.lockState = CursorLockMode.Locked;
		GameMaster.CursorVisible = false;
	}

	private void SetRoot(bool status)
	{
		this.cachedCaster.enabled = status;
	}

	private IEnumerator DelayRootStatus(bool status)
	{
		yield return new WaitForSeconds(0.1f);
		this.root.gameObject.SetActive(status);
		yield break;
	}

	public void CallPauseMenu()
	{
		this.EnterMenu();
		for (int i = 0; i < this.pauseScreenEvents.Length; i++)
		{
			this.pauseScreenEvents[i].Invoke();
		}
	}

	public void CallMainMenu()
	{
		this.EnterMenu();
		base.StartCoroutine(this.WaitOneFrameAndInvokeMainMenuEvents());
	}

	private IEnumerator WaitOneFrameAndInvokeMainMenuEvents()
	{
		yield return null;
		yield return null;
		yield return null;
		for (int i = 0; i < this.mainMenuEvents.Length; i++)
		{
			this.mainMenuEvents[i].Invoke();
		}
		yield break;
	}

	public void BeginTheGame()
	{
		this.ExitMenu();
		Singleton<GameMaster>.Instance.TSP_Reload();
	}

	public void BeginTheEpilogue()
	{
		this.ExitMenu();
		Singleton<GameMaster>.Instance.TSP_Reload(GameMaster.TSP_Reload_Behaviour.Epilogue);
	}

	public void ReturnToMainMenu()
	{
		Singleton<GameMaster>.Instance.TSP_MainMenu();
	}

	public void ResumeGame()
	{
		Singleton<GameMaster>.Instance.ClosePauseMenu(true);
		this.ExitMenu();
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void HidePcOnlyOptions()
	{
		for (int i = 0; i < this.PCOnlyElements.Length; i++)
		{
			this.PCOnlyElements[i].SetActive(false);
		}
	}

	public void HideConsoleOnlyOptions()
	{
		for (int i = 0; i < this.ConsoleOnlyElements.Length; i++)
		{
			this.ConsoleOnlyElements[i].SetActive(false);
		}
	}

	[SerializeField]
	private ConfigurableEvent[] mainMenuEvents;

	[SerializeField]
	private ConfigurableEvent[] pauseScreenEvents;

	[SerializeField]
	private GameObject menuAudio;

	[Space]
	[SerializeField]
	private GameObject[] PCOnlyElements;

	[SerializeField]
	private GameObject[] ConsoleOnlyElements;

	[SerializeField]
	private ScrollRect[] scrollRects;

	[SerializeField]
	private SoftMask[] softMasks;

	[SerializeField]
	private TextMeshProUGUI[] textMeshPros;

	[SerializeField]
	private Camera[] uiCameras;

	[SerializeField]
	private Canvas[] canvases;

	[SerializeField]
	private AudioSource[] audioSources;

	private Canvas rootCanvas;

	private GraphicRaycaster cachedCaster;

	[SerializeField]
	private RectTransform root;
}

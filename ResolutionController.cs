using System;
using UnityEngine;

public class ResolutionController : MonoBehaviour
{
	public Resolution[] availableResolutions { get; private set; }

	public int maximumValue { get; private set; }

	public int SelectedResolutionIndex
	{
		get
		{
			return this.resolutionIndexConfigurable.GetIntValue();
		}
	}

	public FullScreenMode SelectedFullScreenMode
	{
		get
		{
			return ResolutionController.FullScreenModeFromIndex(this.fullscreenModeIndexConfigurable.GetIntValue());
		}
	}

	public FullScreenMode CurrentFullScreenMode
	{
		get
		{
			return Screen.fullScreenMode;
		}
	}

	private int CurrentFullScreenModeIndex
	{
		get
		{
			return ResolutionController.IndexFromFullScreenMode(Screen.fullScreenMode);
		}
	}

	public Resolution CurrentResolution
	{
		get
		{
			return Screen.currentResolution;
		}
	}

	public int CurrentResolutionIndex
	{
		get
		{
			return ResolutionController.ResolutionIndexFromResolution(this.CurrentResolution);
		}
	}

	public static ResolutionController Instance { get; private set; }

	private void Awake()
	{
		ResolutionController.Instance = this;
	}

	private void Start()
	{
		this.LoadFullscreenModeData();
		this.LoadResolutionData();
		int currentResolutionIndex = this.CurrentResolutionIndex;
		int selectedResolutionIndex = this.SelectedResolutionIndex;
	}

	private void LoadResolutionData()
	{
		this.availableResolutions = Screen.resolutions;
		this.maximumValue = this.availableResolutions.Length - 1;
		bool flag = false;
		LiveData liveData = this.resolutionIndexConfigurable.LoadOrCreateSaveData(out flag);
		this.resolutionIndexConfigurable.SetNewMinValue(0);
		this.resolutionIndexConfigurable.SetNewMaxValue(this.maximumValue);
		int num = -1;
		if (flag)
		{
			num = liveData.IntValue;
			if (num == -1 || num >= this.availableResolutions.Length || num < 0)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			num = this.CurrentResolutionIndex;
		}
		liveData.IntValue = num;
		this.resolutionIndexConfigurable.SetNewConfiguredValue(liveData);
	}

	public void LoadFullscreenModeData()
	{
		int intValue = 0;
		bool flag = false;
		LiveData liveData = this.fullscreenModeIndexConfigurable.LoadOrCreateSaveData(out flag);
		if (flag)
		{
			intValue = liveData.IntValue;
		}
		if (!flag)
		{
			intValue = this.CurrentFullScreenModeIndex;
		}
		liveData.IntValue = intValue;
		this.fullscreenModeIndexConfigurable.SetNewConfiguredValue(liveData);
	}

	public void ApplyResolutionAtIndicies(int i, int f)
	{
		this.ApplyResolution(this.availableResolutions[i], ResolutionController.FullScreenModeFromIndex(f));
	}

	public void ApplyResolutionAtIndex(int i, FullScreenMode fullScreenMode)
	{
		this.ApplyResolution(this.availableResolutions[i], fullScreenMode);
	}

	public void ApplyResolution(Resolution r, FullScreenMode fullScreenMode)
	{
		Screen.SetResolution(r.width, r.height, fullScreenMode, r.refreshRate);
		SimpleEvent simpleEvent = this.onResolutionChange;
		if (simpleEvent == null)
		{
			return;
		}
		simpleEvent.Call();
	}

	private static int IndexFromFullScreenMode(FullScreenMode fullscreenMode)
	{
		switch (fullscreenMode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			return 0;
		case FullScreenMode.FullScreenWindow:
			return 1;
		case FullScreenMode.Windowed:
			return 2;
		}
		return -1;
	}

	private static FullScreenMode FullScreenModeFromIndex(int index)
	{
		switch (index)
		{
		case 0:
			return FullScreenMode.ExclusiveFullScreen;
		case 1:
			return FullScreenMode.FullScreenWindow;
		case 2:
			return FullScreenMode.Windowed;
		default:
			return FullScreenMode.Windowed;
		}
	}

	public static int ResolutionIndexFromResolution(Resolution resolution)
	{
		int num = Array.FindIndex<Resolution>(ResolutionController.Instance.availableResolutions, (Resolution x) => ResolutionController.CompareResolutions(resolution, x));
		if (num != -1)
		{
			return num;
		}
		return 0;
	}

	public static bool CompareResolutions(Resolution a, Resolution b)
	{
		return a.width == b.width && a.height == b.height && Mathf.Abs(a.refreshRate - b.refreshRate) <= 1;
	}

	[SerializeField]
	private IntConfigurable resolutionIndexConfigurable;

	[SerializeField]
	private IntConfigurable fullscreenModeIndexConfigurable;

	[SerializeField]
	private SimpleEvent onResolutionChange;

	private const int minumumValue = 0;
}

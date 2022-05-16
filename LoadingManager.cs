using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
	private void Awake()
	{
		GameMaster.OnUpdateLoadProgress = (Action<float>)Delegate.Combine(GameMaster.OnUpdateLoadProgress, new Action<float>(this.OnUpdateLoadProgress));
		AssetBundleControl.OnUpdateLoadProgress = (Action<float>)Delegate.Combine(AssetBundleControl.OnUpdateLoadProgress, new Action<float>(this.OnUpdateLoadProgress));
		string text = GameMaster.LoadingScreenMessage;
		this.standardVersion.enabled = false;
		this.blackVersion.enabled = false;
		this.blueVersion.enabled = false;
		this.doneyVersion.enabled = false;
		this.whiteVersion.enabled = false;
		this.minimalVersion.enabled = false;
		switch (GameMaster.LoadingScreenStyle)
		{
		case LoadingManager.LoadScreenStyle.Standard:
			this.standardVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.Message:
			this.standardVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.Black:
			this.blackVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.Blue:
			this.blueVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.DoneyWithTheFunny:
			this.doneyVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.White:
			this.whiteVersion.enabled = true;
			break;
		case LoadingManager.LoadScreenStyle.Minimal:
			this.minimalVersion.enabled = true;
			break;
		}
		for (int i = 0; i < 4; i++)
		{
			text = text + " " + text;
		}
		this.leftText.text = text;
		this.rightText.text = GameMaster.LoadingScreenMessage;
		this.loadingBar.localScale = new Vector3(0f, 1f, 1f);
		this.minimalBar.fillAmount = 0f;
		if (LoadingManager.OnLoadingScreenSetupDone != null)
		{
			LoadingManager.OnLoadingScreenSetupDone();
		}
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		GameMaster.OnUpdateLoadProgress = (Action<float>)Delegate.Remove(GameMaster.OnUpdateLoadProgress, new Action<float>(this.OnUpdateLoadProgress));
		AssetBundleControl.OnUpdateLoadProgress = (Action<float>)Delegate.Remove(AssetBundleControl.OnUpdateLoadProgress, new Action<float>(this.OnUpdateLoadProgress));
	}

	private void OnUpdateLoadProgress(float _progress)
	{
		this.loadingBar.localScale = new Vector3(_progress, 1f, 1f);
		this.minimalBar.fillAmount = _progress;
	}

	public static Action OnLoadingScreenSetupDone;

	public GameObject allElementsHolder;

	public Canvas standardVersion;

	public Canvas blackVersion;

	public Canvas blueVersion;

	public Canvas doneyVersion;

	public Canvas whiteVersion;

	public Canvas minimalVersion;

	public Text leftText;

	public Text rightText;

	public RectTransform loadingBar;

	public Image minimalBar;

	public enum LoadScreenStyle
	{
		Standard,
		Message,
		Black,
		Blue,
		DoneyWithTheFunny,
		White,
		Minimal
	}
}

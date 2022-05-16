using System;
using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;

public class InstructorHint : HammerEntity
{
	public bool waiting { get; private set; }

	private void Start()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.UpdateHintVisualsCallback;
		IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
		languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Combine(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateHintVisualsCallback));
		BooleanConfigurable simplifiedControlsConfigurable = Singleton<GameMaster>.Instance.simplifiedControlsConfigurable;
		simplifiedControlsConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(simplifiedControlsConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateHintVisualsCallback));
		Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsLoaded += this.UpdateHintVisualsCallback;
		Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsSaved += this.UpdateHintVisualsCallback;
	}

	private void OnDestroy()
	{
		if (Singleton<GameMaster>.Instance != null)
		{
			Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.UpdateHintVisualsCallback;
			IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
			languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Remove(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateHintVisualsCallback));
			BooleanConfigurable simplifiedControlsConfigurable = Singleton<GameMaster>.Instance.simplifiedControlsConfigurable;
			simplifiedControlsConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(simplifiedControlsConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateHintVisualsCallback));
			Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsLoaded -= this.UpdateHintVisualsCallback;
			Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsSaved -= this.UpdateHintVisualsCallback;
		}
	}

	private void UpdateHintVisualsCallback(GameMaster.InputDevice inptDevice)
	{
		this.UpdateHintVisualsCallback();
	}

	private void UpdateHintVisualsCallback(LiveData liveData)
	{
		this.UpdateHintVisualsCallback();
	}

	private void UpdateHintVisualsCallback()
	{
		if (this.currentMessageKeyIndex >= 0)
		{
			this.UpdateHintVisuals();
		}
	}

	private void UpdateHintVisuals()
	{
		string translation;
		if (Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse)
		{
			translation = LocalizationManager.GetTranslation("TSP_Apartment_Device_Keyboard", true, 0, true, false, null, null);
		}
		else
		{
			translation = LocalizationManager.GetTranslation("TSP_Apartment_Device_Gamepad", true, 0, true, false, null, null);
		}
		string text = this.messageKeys[this.currentMessageKeyIndex];
		if (this.autoSuffixMessageKeysOnBucketHeld && BucketController.HASBUCKET)
		{
			text += "_BUCKET";
		}
		string text2 = UITextSwitcher.GetLocalizedSpritedText(text, this.currentRandomInputIndex);
		text2 = text2.Replace("%!D!%", translation);
		text2 = text2.Replace("%! D!%", translation);
		this.instructorHintText.text = text2;
		this.canvas.enabled = true;
	}

	public void ShowHintNoInput(int index)
	{
		this.ShowHint(index, false);
	}

	public void ShowHint(int index, bool takeInput = true)
	{
		if (index < 0)
		{
			return;
		}
		if (index > this.messageKeys.Length - 1)
		{
			return;
		}
		StanleyActions stanleyActions = Singleton<GameMaster>.Instance.stanleyActions;
		this.currentMessageKeyIndex = index;
		this.currentRandomInputIndex = Random.Range(0, Singleton<GameMaster>.Instance.stanleyActions.GetExtraActionInputsLength());
		this.UpdateHintVisuals();
		if (takeInput)
		{
			this.waiting = true;
			base.StartCoroutine(this.WaitForKey());
		}
		base.StartCoroutine(this.HintInOut(this.startYPos, this.holdYPos, 0f, 1f, this.inDuration));
	}

	public void HideHint()
	{
		base.StartCoroutine(this.HintInOut(this.holdYPos, this.outYPos, 1f, 0f, this.outDuration));
		this.canvas.enabled = false;
	}

	private IEnumerator WaitForKey()
	{
		yield return new WaitForGameSeconds(this.inDuration);
		while (GameMaster.PAUSEMENUACTIVE || !Singleton<GameMaster>.Instance.stanleyActions.ExtraAction(this.currentRandomInputIndex, false).WasPressed)
		{
			yield return null;
		}
		this.waiting = false;
		this.HideHint();
		yield break;
	}

	private IEnumerator HintInOut(float pos1, float pos2, float a1, float a2, float duration)
	{
		TimePair timer = new TimePair(duration);
		this.color.a = a1;
		this.instructorHintText.color = this.color;
		this.instructorHintText.rectTransform.anchoredPosition3D = new Vector3(0f, pos1, 0f);
		while (!timer.IsFinished())
		{
			float num = timer.InverseLerp();
			float a3 = num;
			this.instructorHintText.rectTransform.anchoredPosition3D = new Vector3(0f, Mathf.Lerp(pos1, pos2, num), 0f);
			this.color.a = a3;
			this.instructorHintText.color = this.color;
			yield return new WaitForEndOfFrame();
		}
		this.instructorHintText.rectTransform.anchoredPosition3D = new Vector3(0f, pos2, 0f);
		this.color.a = a2;
		this.instructorHintText.color = this.color;
		yield break;
	}

	public Color color = Color.white;

	public float inDuration = 0.5f;

	public float outDuration = 0.1f;

	public float startYPos = 60f;

	public float holdYPos = -60f;

	public float outYPos = -75f;

	public string[] messageKeys;

	public bool autoSuffixMessageKeysOnBucketHeld = true;

	public LogicRelay[] apartmentEndingRelays;

	public bool noInputOnLastRelay = true;

	private int currentMessageKeyIndex = -1;

	private int currentRandomInputIndex = -1;

	public Canvas canvas;

	public TextMeshProUGUI instructorHintText;
}

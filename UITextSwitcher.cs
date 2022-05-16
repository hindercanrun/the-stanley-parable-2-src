using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class UITextSwitcher : MonoBehaviour
{
	private void Start()
	{
		this.UpdateUI();
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.UpdateUI;
		IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
		languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Combine(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateUI));
		Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsSaved += this.UpdateUI;
		Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsLoaded += this.UpdateUI;
		BooleanConfigurable simplifiedControlsConfigurable = Singleton<GameMaster>.Instance.simplifiedControlsConfigurable;
		simplifiedControlsConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(simplifiedControlsConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateUI));
	}

	private void OnDestroy()
	{
		if (Singleton<GameMaster>.Instance != null)
		{
			Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.UpdateUI;
			IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
			languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Remove(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateUI));
			Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsSaved -= this.UpdateUI;
			Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsLoaded -= this.UpdateUI;
			BooleanConfigurable simplifiedControlsConfigurable = Singleton<GameMaster>.Instance.simplifiedControlsConfigurable;
			simplifiedControlsConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(simplifiedControlsConfigurable.OnValueChanged, new Action<LiveData>(this.UpdateUI));
		}
	}

	private void UpdateUI(LiveData d)
	{
		this.UpdateUI();
	}

	private void UpdateUI(GameMaster.InputDevice i)
	{
		this.UpdateUI();
	}

	[ContextMenu("Update UI")]
	private void UpdateUI()
	{
		string text = (this.textKey != "") ? UITextSwitcher.GetLocalizedSpritedText(this.textKey, this.inputAction) : "";
		this.tmPro.text = text;
		if (this.tmProCopyOrShadow)
		{
			this.tmProCopyOrShadow.text = text;
		}
	}

	public void SetTerm(string newTerm)
	{
		this.textKey = newTerm;
		this.UpdateUI();
	}

	public static string GetLocalizedSpritedText(string key, int extraInputActionIndex)
	{
		string text = LocalizationManager.GetTranslation(key, true, 0, true, false, null, null);
		if (text == null)
		{
			return "";
		}
		StanleyActions.KeyControllerPairSpriteTags extraActionBindingDescription = Singleton<GameMaster>.Instance.stanleyActions.GetExtraActionBindingDescription(extraInputActionIndex);
		if (Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse)
		{
			text = text.Replace("%!K!%", extraActionBindingDescription.KeyboardSpriteTag);
			text = text.Replace("%! K!%", extraActionBindingDescription.KeyboardSpriteTag);
		}
		else
		{
			text = text.Replace("%!K!%", extraActionBindingDescription.GamepadSpriteTag);
			text = text.Replace("%! K!%", extraActionBindingDescription.GamepadSpriteTag);
		}
		return text;
	}

	public static string GetLocalizedSpritedText(string key, UITextSwitcher.InputAction inputAction)
	{
		string text = LocalizationManager.GetTranslation(key, true, 0, true, false, null, null);
		if (text == null)
		{
			return "";
		}
		StanleyActions.KeyControllerPairSpriteTags keyControllerPairSpriteTags;
		if (inputAction != UITextSwitcher.InputAction.Jump)
		{
			if (inputAction != UITextSwitcher.InputAction.Use)
			{
				keyControllerPairSpriteTags = Singleton<GameMaster>.Instance.stanleyActions.GetExtraActionBindingDescription((int)inputAction);
			}
			else
			{
				keyControllerPairSpriteTags = Singleton<GameMaster>.Instance.stanleyActions.GetUseBindingDescription();
			}
		}
		else
		{
			keyControllerPairSpriteTags = Singleton<GameMaster>.Instance.stanleyActions.GetJumpBindingDescription();
		}
		if (Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse)
		{
			text = text.Replace("%!K!%", keyControllerPairSpriteTags.KeyboardSpriteTag);
		}
		else
		{
			text = text.Replace("%!K!%", keyControllerPairSpriteTags.GamepadSpriteTag);
		}
		return text;
	}

	[SerializeField]
	private string textKey;

	[SerializeField]
	private TMP_Text tmPro;

	[SerializeField]
	private TMP_Text tmProCopyOrShadow;

	[SerializeField]
	private UITextSwitcher.InputAction inputAction;

	public enum InputAction
	{
		ExtraAction1,
		ExtraAction2,
		ExtraAction3,
		ExtraAction4,
		Teleport,
		Jump,
		Use
	}
}

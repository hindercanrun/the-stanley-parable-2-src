using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class HoleUITextSwitcher : MonoBehaviour
{
	private void Start()
	{
		this.UpdateUI();
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.UpdateUI;
		IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
		languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Combine(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateUI));
	}

	private void OnDestroy()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.UpdateUI;
		IntConfigurable languageProfile = Singleton<GameMaster>.Instance.languageProfile;
		languageProfile.OnValueChanged = (Action<LiveData>)Delegate.Remove(languageProfile.OnValueChanged, new Action<LiveData>(this.UpdateUI));
	}

	private void UpdateUI(GameMaster.InputDevice i)
	{
		this.UpdateUI();
	}

	private void UpdateUI(LiveData d)
	{
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		HoleUITextSwitcher.ChangeTexts[] array = new HoleUITextSwitcher.ChangeTexts[]
		{
			this.changeTexts1,
			this.changeTexts2,
			this.changeTexts3,
			this.changeTexts4
		};
		for (int i = 0; i < 4; i++)
		{
			string localizedSpritedText = HoleUITextSwitcher.GetLocalizedSpritedText(array[i].changeTextKey, i);
			array[i].changeText.text = localizedSpritedText;
			if (array[i].changeTextShadow)
			{
				array[i].changeTextShadow.text = localizedSpritedText;
			}
		}
		this.teleportText.text = HoleUITextSwitcher.GetLocalizedSpritedText(this.teleportKey, StanleyActions.HoleTeleportIndex);
		this.teleportTextCopy.text = HoleUITextSwitcher.GetLocalizedSpritedText(this.teleportKey, StanleyActions.HoleTeleportIndex);
	}

	public static string GetLocalizedSpritedText(string key, int inputIndex)
	{
		string text = LocalizationManager.GetTranslation(key, true, 0, true, false, null, null);
		if (Singleton<GameMaster>.Instance.InputDeviceType == GameMaster.InputDevice.KeyboardAndMouse)
		{
			text = text.Replace("%!K!%", Singleton<GameMaster>.Instance.stanleyActions.GetExtraActionBindingDescription(inputIndex).KeyboardSpriteTag);
		}
		else
		{
			text = text.Replace("%!K!%", Singleton<GameMaster>.Instance.stanleyActions.GetExtraActionBindingDescription(inputIndex).GamepadSpriteTag);
		}
		return text;
	}

	[SerializeField]
	private string teleportKey;

	[SerializeField]
	private TextMeshPro teleportText;

	[SerializeField]
	private TextMeshPro teleportTextCopy;

	[SerializeField]
	private HoleUITextSwitcher.ChangeTexts changeTexts1;

	[SerializeField]
	private HoleUITextSwitcher.ChangeTexts changeTexts2;

	[SerializeField]
	private HoleUITextSwitcher.ChangeTexts changeTexts3;

	[SerializeField]
	private HoleUITextSwitcher.ChangeTexts changeTexts4;

	[Serializable]
	private struct ChangeTexts
	{
		public string changeTextKey;

		public TextMeshProUGUI changeText;

		public TextMeshProUGUI changeTextShadow;
	}
}

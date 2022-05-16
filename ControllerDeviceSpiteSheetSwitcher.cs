using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ControllerDeviceSpiteSheetSwitcher : MonoBehaviour
{
	private void Start()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.OnInputDeviceTypeChanged;
		this.OnInputDeviceTypeChanged(Singleton<GameMaster>.Instance.InputDeviceType);
	}

	private void OnDestroy()
	{
		if (Singleton<GameMaster>.Instance != null)
		{
			Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.OnInputDeviceTypeChanged;
		}
	}

	[ContextMenu("Set to XBOX")]
	private void SetToXBOX()
	{
		this.OnInputDeviceTypeChanged(GameMaster.InputDevice.GamepadXBOXOneOrGeneric);
	}

	private void OnInputDeviceTypeChanged(GameMaster.InputDevice inputDevice)
	{
		base.GetComponent<TMP_Text>().spriteAsset = PlatformSettings.Instance.GetSpriteSheetForInputDevice(inputDevice);
		this.lastInputDevice = inputDevice;
	}

	public GameMaster.InputDevice lastInputDevice;
}

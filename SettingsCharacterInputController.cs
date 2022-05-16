using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsCharacterInputController : MonoBehaviour
{
	private void Start()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.Instance_OnInputDeviceTypeChanged;
		this.Instance_OnInputDeviceTypeChanged(Singleton<GameMaster>.Instance.InputDeviceType, true);
	}

	private void Instance_OnInputDeviceTypeChanged(GameMaster.InputDevice inputDevice)
	{
		this.Instance_OnInputDeviceTypeChanged(inputDevice, false);
	}

	private void Instance_OnInputDeviceTypeChanged(GameMaster.InputDevice inputDevice, bool ignoreMouseMove)
	{
		if (inputDevice == GameMaster.InputDevice.KeyboardAndMouse)
		{
			EventSystem.current.SetSelectedGameObject(null);
			GameMaster.CursorVisible = (ignoreMouseMove || Singleton<GameMaster>.Instance.MouseMoved);
			GameMaster.CursorLockState = CursorLockMode.None;
			return;
		}
		GameMaster.CursorVisible = false;
		GameMaster.CursorLockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		this.visible_DEBUG = GameMaster.CursorVisible;
		this.cursorLockState_DEBUG = GameMaster.CursorLockState;
	}

	public bool visible_DEBUG;

	public CursorLockMode cursorLockState_DEBUG;
}

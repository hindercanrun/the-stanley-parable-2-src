using System;
using UnityEngine;

public class ControllerGameObjectSwitcher : MonoBehaviour
{
	private void Awake()
	{
		Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged += this.Instance_OnInputDeviceTypeChanged;
		this.Instance_OnInputDeviceTypeChanged(Singleton<GameMaster>.Instance.InputDeviceType);
	}

	private void OnDestroy()
	{
		if (Singleton<GameMaster>.Instance != null)
		{
			Singleton<GameMaster>.Instance.OnInputDeviceTypeChanged -= this.Instance_OnInputDeviceTypeChanged;
		}
	}

	private void Instance_OnInputDeviceTypeChanged(GameMaster.InputDevice type)
	{
		if (base.enabled)
		{
			if (this.matchTarget)
			{
				this.matchTarget.SetActive(type == this.inputDeviceToMatch);
			}
			if (this.nonMatchTarget)
			{
				this.nonMatchTarget.SetActive(type != this.inputDeviceToMatch);
			}
		}
	}

	private void Update()
	{
	}

	[Header("Sets active/notactive on match, and opposite on not match")]
	public GameMaster.InputDevice inputDeviceToMatch;

	public GameObject matchTarget;

	public GameObject nonMatchTarget;
}

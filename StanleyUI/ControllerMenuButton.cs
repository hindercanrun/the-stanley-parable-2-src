using System;
using UnityEngine;
using UnityEngine.Events;

namespace StanleyUI
{
	public class ControllerMenuButton : MonoBehaviour
	{
		private void Awake()
		{
			this.parentUIScreen = base.GetComponentInParent<UIScreen>();
		}

		public void InvokeOnButtonPress()
		{
			UnityEvent onWasPressed = this.OnWasPressed;
			if (onWasPressed == null)
			{
				return;
			}
			onWasPressed.Invoke();
		}

		private void Update()
		{
			if (this.parentUIScreen.active)
			{
				ControllerMenuButton.ControllerButton controllerButton = this.controllerButton;
				if (controllerButton != ControllerMenuButton.ControllerButton.Back_ActionRight)
				{
					if (controllerButton != ControllerMenuButton.ControllerButton.Confirm_ActionDown)
					{
						return;
					}
					if (Singleton<GameMaster>.Instance.stanleyActions.MenuConfirm.WasPressed)
					{
						this.InvokeOnButtonPress();
					}
				}
				else if (Singleton<GameMaster>.Instance.stanleyActions.MenuBack.WasPressed)
				{
					this.InvokeOnButtonPress();
					return;
				}
			}
		}

		public ControllerMenuButton.ControllerButton controllerButton;

		public UnityEvent OnWasPressed;

		private UIScreen parentUIScreen;

		public enum ControllerButton
		{
			Back_ActionRight,
			Confirm_ActionDown
		}
	}
}

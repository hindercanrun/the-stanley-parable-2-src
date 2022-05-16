using System;
using InControl;

public class PCGamepad : IPlatformGamepad
{
	private InputDevice CurrentGamepad
	{
		get
		{
			return InputManager.ActiveDevice;
		}
	}

	public InputControlType ConfirmButton { get; } = 19;

	public InputControlType BackButton { get; } = 20;

	public InputControlType JumpButton { get; } = 19;

	public void PlayVibration(float strength)
	{
		InputDevice currentGamepad = this.CurrentGamepad;
		if (currentGamepad == null)
		{
			return;
		}
		currentGamepad.Vibrate(strength, strength);
	}

	public void StopVibration()
	{
		InputDevice currentGamepad = this.CurrentGamepad;
		if (currentGamepad == null)
		{
			return;
		}
		currentGamepad.Vibrate(0f, 0f);
	}
}

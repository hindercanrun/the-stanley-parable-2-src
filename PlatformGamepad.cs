using System;
using InControl;

public static class PlatformGamepad
{
	public static InputControlType ConfirmButton
	{
		get
		{
			return PlatformGamepad.platformGamepad.ConfirmButton;
		}
	}

	public static InputControlType BackButton
	{
		get
		{
			return PlatformGamepad.platformGamepad.BackButton;
		}
	}

	public static InputControlType JumpButton
	{
		get
		{
			return PlatformGamepad.platformGamepad.JumpButton;
		}
	}

	public static void InitPlatformGamepad(IPlatformGamepad gamepad)
	{
		PlatformGamepad.platformGamepad = gamepad;
	}

	public static void PlayVibration(float strength)
	{
		IPlatformGamepad platformGamepad = PlatformGamepad.platformGamepad;
		if (platformGamepad == null)
		{
			return;
		}
		platformGamepad.PlayVibration(strength);
	}

	public static void StopVibration()
	{
		IPlatformGamepad platformGamepad = PlatformGamepad.platformGamepad;
		if (platformGamepad == null)
		{
			return;
		}
		platformGamepad.StopVibration();
	}

	private static IPlatformGamepad platformGamepad;
}

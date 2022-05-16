using System;
using InControl;

public interface IPlatformGamepad
{
	InputControlType ConfirmButton { get; }

	InputControlType BackButton { get; }

	InputControlType JumpButton { get; }

	void PlayVibration(float strength);

	void StopVibration();
}

using System;
using UnityEngine;

public class VibrationTest : MonoBehaviour
{
	private void Update()
	{
		if (Singleton<GameMaster>.Instance.stanleyActions.Up.WasPressed)
		{
			this.strength += 0.05f;
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Down.WasPressed)
		{
			this.strength -= 0.05f;
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Right.WasPressed)
		{
			PlatformGamepad.PlayVibration(this.strength);
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Left.WasPressed)
		{
			PlatformGamepad.StopVibration();
		}
	}

	private float strength;
}

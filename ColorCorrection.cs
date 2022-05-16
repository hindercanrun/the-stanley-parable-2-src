using System;
using UnityEngine;

public class ColorCorrection : HammerEntity
{
	private void Awake()
	{
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		StanleyController.Instance.StartPostProcessFade(this.lut, 0f, 1f, this.fadeInTime);
	}

	public override void Input_Disable()
	{
		base.Input_Disable();
		StanleyController.Instance.StartPostProcessFade(this.lut, 1f, 0f, this.fadeOutTime);
	}

	public Texture2D lut;

	public float fadeInTime;

	public float fadeOutTime;
}

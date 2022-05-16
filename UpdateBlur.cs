using System;
using UnityEngine;

public class UpdateBlur : MonoBehaviour
{
	private void Update()
	{
		if (MainCamera.BlurValue == 0f)
		{
			this.blur.enabled = false;
			return;
		}
		this.blur.enabled = true;
		this.blur.BlurAmount = MainCamera.BlurValue * this.blurMultiplier;
	}

	[SerializeField]
	private MobileBlur blur;

	[SerializeField]
	private float blurMultiplier = 0.333f;
}

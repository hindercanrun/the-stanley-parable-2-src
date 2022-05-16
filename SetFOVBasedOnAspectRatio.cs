using System;
using UnityEngine;

[ExecuteInEditMode]
public class SetFOVBasedOnAspectRatio : MonoBehaviour
{
	private void LateUpdate()
	{
		this.cam.fieldOfView = this.aspectRatioToFOV.Evaluate((float)Screen.width / (float)Screen.height);
	}

	public AnimationCurve aspectRatioToFOV;

	public Camera cam;
}

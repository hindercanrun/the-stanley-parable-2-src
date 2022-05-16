using System;
using UnityEngine;

[ExecuteInEditMode]
public class SetScalingByAspectRatio : MonoBehaviour
{
	private void LateUpdate()
	{
		this.aspectRatio = (float)Screen.width / (float)Screen.height;
		this.trans.localScale = new Vector3(this.aspectRatioToXScale.Evaluate(this.aspectRatio), this.aspectRatioToYScale.Evaluate(this.aspectRatio), 1f);
	}

	public AnimationCurve aspectRatioToXScale;

	public AnimationCurve aspectRatioToYScale;

	public Transform trans;

	private float aspectRatio;
}

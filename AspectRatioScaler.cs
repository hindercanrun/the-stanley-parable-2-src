using System;
using UnityEngine;

[ExecuteInEditMode]
public class AspectRatioScaler : MonoBehaviour
{
	public void Update()
	{
		float num = this.aspectRatioWidthAtScaleOne / this.aspectRatioHeightAtScaleOne;
		float num2 = (float)Screen.width / (float)Screen.height / num;
		if (this.invertScale)
		{
			num2 = 1f / num2;
		}
		base.transform.localScale = new Vector3(num2, num2, 1f);
	}

	public float aspectRatioWidthAtScaleOne = 16f;

	public float aspectRatioHeightAtScaleOne = 9f;

	public bool invertScale;
}

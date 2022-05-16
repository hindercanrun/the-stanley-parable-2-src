using System;
using UnityEngine;

public class LODGroupStaticerBiasMultiplier : MonoBehaviour
{
	public float GetBias()
	{
		if (Application.platform != RuntimePlatform.Switch)
		{
			return this.normalBiasMultiplier;
		}
		return this.switchBiasMultiplier;
	}

	public float normalBiasMultiplier = 1f;

	public float switchBiasMultiplier = 0.67f;
}

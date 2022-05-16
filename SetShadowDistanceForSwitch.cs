using System;
using UnityEngine;

public class SetShadowDistanceForSwitch : MonoBehaviour
{
	private void Start()
	{
		if (Application.isPlaying && CullForSwitchController.IsSwitchEnvironment)
		{
			this.regularShadowDistance = QualitySettings.shadowDistance;
			QualitySettings.shadowDistance = this.shadowDistanceForSwitch;
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying && CullForSwitchController.IsSwitchEnvironment)
		{
			QualitySettings.shadowDistance = this.regularShadowDistance;
		}
	}

	public float shadowDistanceForSwitch = 500f;

	private float regularShadowDistance = 150f;
}

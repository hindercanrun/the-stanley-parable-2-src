using System;
using UnityEngine;

public class FPSLockForSwitch : MonoBehaviour
{
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Switch)
		{
			QualitySettings.vSyncCount = 2;
		}
	}

	private void Update()
	{
	}
}

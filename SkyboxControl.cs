using System;
using UnityEngine;

public class SkyboxControl : MonoBehaviour
{
	public void ToggleSkybox(bool status)
	{
		if (status)
		{
			RenderSettings.skybox = this.skyboxArray[0];
			return;
		}
		RenderSettings.skybox = this.skyboxArray[1];
	}

	[SerializeField]
	private Material[] skyboxArray;
}

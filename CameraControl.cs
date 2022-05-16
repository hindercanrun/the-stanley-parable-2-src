using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	private void Awake()
	{
		this.cachedCamera = base.GetComponent<Camera>();
	}

	public void ToggleSolidColorSkybox(bool status)
	{
		if (status)
		{
			this.cachedCamera.clearFlags = CameraClearFlags.Color;
			return;
		}
		this.cachedCamera.clearFlags = CameraClearFlags.Skybox;
	}

	private Camera cachedCamera;
}

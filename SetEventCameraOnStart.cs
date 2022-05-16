using System;
using UnityEngine;

public class SetEventCameraOnStart : MonoBehaviour
{
	private void Start()
	{
		this.targetCanvas.worldCamera = StanleyController.Instance.cam;
	}

	[SerializeField]
	private Canvas targetCanvas;
}

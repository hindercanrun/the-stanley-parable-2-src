using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasOrdering : MonoBehaviour
{
	private void Awake()
	{
		this.canvas = base.GetComponent<Canvas>();
		this.canvas.planeDistance = this.defaultPlaneDistance;
	}

	private void Update()
	{
		if (this.canvas.worldCamera != StanleyController.Instance.currentCam)
		{
			this.canvas.worldCamera = StanleyController.Instance.currentCam;
		}
	}

	private Canvas canvas;

	public float defaultPlaneDistance;
}

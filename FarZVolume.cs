using System;
using UnityEngine;

public class FarZVolume : MonoBehaviour
{
	private void Awake()
	{
		this.position = base.transform.position;
		this.fullExtents = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		this.volumeBounds = new Bounds(this.position, this.fullExtents);
	}

	private void Start()
	{
		this.followTransform = StanleyController.Instance.transform;
	}

	private void FixedUpdate()
	{
		if (this.volumeBounds.Contains(StanleyController.StanleyPosition))
		{
			if (!this.touchingLastFrame)
			{
				StanleyController.Instance.SetFarZ(this.farZ, this.CameraMode);
			}
			this.touchingLastFrame = true;
			return;
		}
		this.touchingLastFrame = false;
	}

	public void ToggleDepthOnlySkybox(bool status)
	{
		if (status)
		{
			this.CameraMode = FarZVolume.CameraModes.DepthOnly;
			return;
		}
		this.CameraMode = FarZVolume.CameraModes.RenderSkybox;
	}

	public float farZ;

	public FarZVolume.CameraModes CameraMode;

	private Vector3 fullExtents;

	private Vector3 position;

	private bool touchingLastFrame;

	private Transform followTransform;

	private Bounds volumeBounds;

	public enum CameraModes
	{
		RenderSkybox,
		DepthOnly
	}
}

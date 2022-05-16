using System;
using UnityEngine;

public class ViewControl : HammerEntity
{
	private void Awake()
	{
		this.cam.nearClipPlane = 0.05f;
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		if (this.setFOV)
		{
			this.cam.fieldOfView = StanleyController.Instance.cam.fieldOfView;
		}
		StanleyController.Instance.DisableCamera(this.cam);
		if (this.freezePlayer)
		{
			StanleyController.Instance.FreezeMotion(false);
			StanleyController.Instance.FreezeView(false);
		}
		this.cam.enabled = true;
		if (this.trackTrain)
		{
			this.trackTrain.Input_StartForward();
			if (this.transformFollower)
			{
				this.transformFollower.enabled = true;
			}
		}
	}

	public override void Input_Disable()
	{
		base.Input_Disable();
		this.cam.enabled = false;
		StanleyController.Instance.EnableCamera();
		StanleyController.Instance.UnfreezeMotion(false);
		StanleyController.Instance.UnfreezeView(false);
		if (this.trackTrain)
		{
			this.trackTrain.Input_Stop();
			if (this.transformFollower)
			{
				this.transformFollower.enabled = false;
			}
		}
	}

	public float FOV = 90f;

	public bool setFOV;

	public float holdTime = 10f;

	public bool freezePlayer = true;

	public Camera cam;

	public Tracktrain trackTrain;

	public TransformFollow transformFollower;
}

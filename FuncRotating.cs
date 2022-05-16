using System;
using UnityEngine;

public class FuncRotating : HammerEntity
{
	private void Awake()
	{
		if (this.XAxis)
		{
			this.axis = -Vector3.right;
		}
		else if (this.YAxis)
		{
			this.axis = -Vector3.forward;
		}
		if (this.reverseDir)
		{
			this.axis = -this.axis;
		}
		this.currentlyOn = this.startOn;
		this.startRotation = base.transform.rotation;
	}

	private void Update()
	{
		if (!this.currentlyOn)
		{
			return;
		}
		this.currentAngle += this.maxSpeed * Singleton<GameMaster>.Instance.GameDeltaTime;
		base.transform.rotation = Quaternion.AngleAxis(this.currentAngle, this.axis) * this.startRotation;
	}

	public void Input_StartForward()
	{
		this.currentlyOn = true;
	}

	public void Input_Start()
	{
		this.currentlyOn = true;
	}

	public void Input_Stop()
	{
		this.currentlyOn = false;
	}

	public void Input_SetSpeed(float newSpeed)
	{
		this.maxSpeed = newSpeed;
	}

	public float maxSpeed;

	public bool startOn = true;

	public bool reverseDir;

	public bool XAxis;

	public bool YAxis;

	private float currentAngle;

	private bool currentlyOn = true;

	private Vector3 axis = -Vector3.up;

	private Quaternion startRotation;
}

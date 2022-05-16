using System;
using UnityEngine;

public class Tracktrain : HammerEntity
{
	private void Awake()
	{
		if (this.firstTargetObject != null)
		{
			PathTrack component = this.firstTargetObject.GetComponent<PathTrack>();
			if (component != null)
			{
				this.currentPath = component;
				if (this.currentPath.nextPathTrack)
				{
					this.nextPath = this.currentPath.nextPathTrack;
				}
				base.transform.position = this.currentPath.transform.position;
			}
		}
		if (this.initialSpeed != 0f)
		{
			this.currentSpeed = this.initialSpeed;
			this.currentPathSpeed = this.currentSpeed;
		}
	}

	private void Update()
	{
		if (this.currentSpeed == 0f)
		{
			return;
		}
		if (this.nextPath == null)
		{
			return;
		}
		if (this.velocityType != Tracktrain.VelocityTypes.Instantaneous)
		{
			if (this.nextPath.newSpeed != 0f)
			{
				float t = TimMaths.Vector3InverseLerp(this.currentPath.transform.position, this.nextPath.transform.position, base.transform.position);
				if (this.velocityType == Tracktrain.VelocityTypes.LinearBlend)
				{
					this.currentSpeed = Mathf.Lerp(this.currentPathSpeed, this.nextPath.newSpeed, t);
				}
				else
				{
					this.currentSpeed = Mathf.SmoothStep(this.currentPathSpeed, this.nextPath.newSpeed, Mathf.SmoothStep(0f, 1f, t));
				}
			}
		}
		else
		{
			this.currentSpeed = this.currentPathSpeed;
		}
		float num = this.currentSpeed * Singleton<GameMaster>.Instance.GameDeltaTime;
		Vector3 vector = Vector3.MoveTowards(base.transform.position, this.nextPath.transform.position, num);
		if (vector == this.nextPath.transform.position)
		{
			this.nextPath.Passed();
			this.currentPath = this.nextPath;
			this.nextPath = this.nextPath.nextPathTrack;
			if (this.currentPath.newSpeed != 0f)
			{
				this.currentPathSpeed = this.currentPath.newSpeed;
			}
			if (this.nextPath)
			{
				float num2 = num - Vector3.Distance(base.transform.position, vector);
				if (num2 > 0f)
				{
					vector = Vector3.MoveTowards(base.transform.position, this.nextPath.transform.position, num2);
				}
			}
		}
		base.transform.position = vector;
	}

	public void Input_StartForward()
	{
		if (this.currentPath && this.currentPath.newSpeed != 0f)
		{
			this.currentSpeed = this.currentPath.newSpeed;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		this.currentPathSpeed = this.currentSpeed;
	}

	public void Input_Stop()
	{
		this.currentSpeed = 0f;
	}

	public void Input_SetSpeed(float s)
	{
		s /= 100f;
		s = Mathf.Clamp(s, 0f, this.maxSpeed);
		this.currentSpeed = s;
	}

	public void Input_TeleportToPathNode(string nodeName)
	{
		GameObject gameObject = GameObject.Find(nodeName);
		if (gameObject == null)
		{
			return;
		}
		PathTrack component = gameObject.GetComponent<PathTrack>();
		if (component == null)
		{
			return;
		}
		this.currentPath = component;
		if (this.currentPath.nextPathTrack)
		{
			this.nextPath = this.currentPath.nextPathTrack;
		}
		base.transform.position = this.currentPath.transform.position;
	}

	private void OnValidate()
	{
		if (this.firstTargetObject == null || this.firstTargetObject.name != this.firstTarget)
		{
			this.firstTargetObject = GameObject.Find(this.firstTarget);
		}
	}

	public bool fixedOrientation = true;

	public string firstTarget = "";

	[HideInInspector]
	public GameObject firstTargetObject;

	private PathTrack currentPath;

	private PathTrack nextPath;

	public Tracktrain.VelocityTypes velocityType;

	public float heightAboveTrack;

	public float initialSpeed;

	public float maxSpeed = 1f;

	private float currentSpeed;

	private float currentPathSpeed;

	public enum VelocityTypes
	{
		Instantaneous,
		LinearBlend,
		EaseInOut
	}
}

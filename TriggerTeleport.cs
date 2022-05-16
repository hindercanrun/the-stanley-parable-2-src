using System;
using UnityEngine;

public class TriggerTeleport : TriggerMultiple
{
	private void OnValidate()
	{
		if (this.targetObj != null && this.targetObj.name != this.target)
		{
			this.target = this.targetObj.name;
		}
		if (this.targetObj == null || this.targetObj.name != this.target)
		{
			GameObject exists = GameObject.Find(this.target);
			if (exists)
			{
				this.targetObj = exists;
			}
			else
			{
				this.targetObj = null;
			}
		}
		if (this.landmarkObj != null && this.landmarkObj.name != this.landmark)
		{
			this.landmark = this.landmarkObj.name;
		}
		if (this.landmarkObj == null || this.landmarkObj.name != this.landmark)
		{
			GameObject exists2 = GameObject.Find(this.landmark);
			if (exists2)
			{
				this.landmarkObj = exists2;
			}
			else
			{
				this.landmarkObj = null;
			}
		}
		if (this.orientationObj != null && this.orientationObj.name != this.orientation)
		{
			this.orientation = this.orientationObj.name;
		}
		if (this.orientationObj == null || this.orientationObj.name != this.orientation)
		{
			GameObject gameObject = GameObject.Find(this.orientation);
			if (gameObject)
			{
				this.orientationObj = gameObject.transform;
				return;
			}
			this.orientationObj = null;
		}
	}

	public void ForceTouch()
	{
		this.StartTouch();
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
	}

	protected override void StartTouch()
	{
		if (!this.isEnabled)
		{
			return;
		}
		base.StartTouch();
		if (this.targetObj)
		{
			Vector3 position = this.targetObj.transform.position;
			if (this.landmarkObj != null)
			{
				StanleyController.Instance.Teleport(StanleyController.TeleportType.TriggerTeleport, this.landmarkObj.transform.position, this.targetObj.transform.position, -this.targetObj.transform.up, this.useLandmarkAngles, true, true, null);
			}
			else
			{
				StanleyController.Instance.Teleport(StanleyController.TeleportType.TriggerTeleport, this.targetObj.transform.position, -this.targetObj.transform.up, this.useLandmarkAngles, true, true, null);
			}
			if (this.useOrientationAngles)
			{
				StanleyController.Instance.transform.rotation = this.orientationObj.rotation;
			}
		}
		if (this.onceOnly)
		{
			Object.Destroy(this._body);
			Object.Destroy(this._collider);
		}
	}

	public bool useLandmarkAngles;

	public bool useOrientationAngles;

	public string target = "";

	public string landmark = "";

	public string orientation = "";

	public GameObject targetObj;

	public GameObject landmarkObj;

	public Transform orientationObj;
}

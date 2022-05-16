using System;
using UnityEngine;

public class PortalDoor : HammerEntity
{
	public EasyPortal EasyPortal
	{
		get
		{
			return this.easyPortal;
		}
	}

	private void Start()
	{
		if (!this.isDrawing && this.easyPortal != null)
		{
			this.easyPortal.SetStatus(false);
		}
	}

	private void OnValidate()
	{
	}

	public void Input_Open()
	{
		this.isDrawing = true;
		if (this.easyPortal != null)
		{
			this.easyPortal.SetStatus(true);
		}
	}

	public void Input_Close()
	{
		this.isDrawing = false;
		if (this.easyPortal != null)
		{
			this.easyPortal.SetStatus(false);
		}
	}

	public void Input_SetPartner()
	{
		this.Input_Close();
	}

	public void Input_SetPartner(string newPartner)
	{
		this.destination = newPartner;
		GameObject gameObject = GameObject.Find(this.destination);
		if (!gameObject)
		{
			return;
		}
		PortalDoor component = gameObject.GetComponent<PortalDoor>();
		if (!component)
		{
			return;
		}
		this.destinationChild.position = component.transform.position;
		this.destinationChild.rotation = Quaternion.LookRotation(-component.transform.forward, component.transform.up);
		if (this.easyPortal != null)
		{
			this.easyPortal.SetNewDestination(component.transform);
		}
		component.destination != base.name;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = this.destinationChild.position;
		int num = 10;
		this.gizmoOffset += 0.1f;
		if (this.gizmoOffset >= (float)num)
		{
			this.gizmoOffset = 0f;
		}
		(position2 - position) / (float)num;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)(i + Mathf.FloorToInt(this.gizmoOffset));
			if (num2 > (float)num)
			{
				num2 -= (float)num;
			}
			Gizmos.color = Color.Lerp(Color.red, Color.green, 1f - num2 / (float)num);
			Gizmos.DrawLine(Vector3.Lerp(position, position2, (float)i / (float)num), Vector3.Lerp(position, position2, ((float)i + 1f) / (float)num));
		}
	}

	[SerializeField]
	private EasyPortal easyPortal;

	public string destination;

	public Transform destinationChild;

	public bool isDrawing;

	private float gizmoOffset;
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPush : HammerEntity
{
	private void Awake()
	{
		this._body = base.GetComponent<Rigidbody>();
		this._collider = base.GetComponent<Collider>();
	}

	private void FixedUpdate()
	{
		if (!this.isEnabled)
		{
			return;
		}
		if (this.touchingBodies.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<Collider, Rigidbody> keyValuePair in this.touchingBodies)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.AddForce(this.pushDirection * this.pushAmount * Time.fixedDeltaTime * 2000f, ForceMode.Force);
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (this.touchingBodies.ContainsKey(col))
		{
			return;
		}
		Rigidbody component = col.GetComponent<Rigidbody>();
		if (component)
		{
			this.touchingBodies.Add(col, component);
			return;
		}
		this.touchingBodies.Add(col, null);
	}

	private void OnTriggerStay(Collider col)
	{
		if (this.touchingBodies.ContainsKey(col))
		{
			return;
		}
		Rigidbody component = col.GetComponent<Rigidbody>();
		if (component)
		{
			this.touchingBodies.Add(col, component);
			return;
		}
		this.touchingBodies.Add(col, null);
	}

	private void OnTriggerExit(Collider col)
	{
		if (this.touchingBodies.ContainsKey(col))
		{
			this.touchingBodies.Remove(col);
		}
	}

	private Dictionary<Collider, Rigidbody> touchingBodies = new Dictionary<Collider, Rigidbody>();

	private Rigidbody _body;

	private Collider _collider;

	public Vector3 pushDirection = Vector3.forward;

	public float pushAmount = 1f;
}

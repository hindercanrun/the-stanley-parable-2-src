using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMultiple : HammerEntity
{
	private void Awake()
	{
		this._body = base.GetComponent<Rigidbody>();
		this._collider = base.GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.touchingColliders.Contains(col))
		{
			return;
		}
		bool isEnabled = this.isEnabled;
		this.touchingColliders.Add(col);
		this.StartTouch();
	}

	private void OnTriggerExit(Collider col)
	{
		if (this.touchingColliders.Contains(col))
		{
			this.touchingColliders.Remove(col);
			if (this.isEnabled)
			{
				base.FireOutput(Outputs.OnEndTouch);
			}
		}
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		for (int i = 0; i < this.touchingColliders.Count; i++)
		{
			if (this.touchingColliders[i].CompareTag("Player"))
			{
				this.StartTouch();
			}
		}
	}

	protected virtual void StartTouch()
	{
		if (!this.isEnabled)
		{
			return;
		}
		base.FireOutput(Outputs.OnStartTouch);
		base.FireOutput(Outputs.OnTrigger);
		if (this.onceOnly)
		{
			Object.Destroy(this._body);
			Object.Destroy(this._collider);
		}
	}

	public bool onceOnly;

	private List<Collider> touchingColliders = new List<Collider>();

	protected Rigidbody _body;

	protected Collider _collider;
}

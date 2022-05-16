using System;
using UnityEngine;

public class FuncBrush : HammerEntity
{
	private void Awake()
	{
		this._renderer = base.GetComponent<MeshRenderer>();
		this._collider = base.GetComponent<MeshCollider>();
		if (!this.isEnabled)
		{
			this._renderer.enabled = false;
			if (this._collider)
			{
				this._collider.enabled = false;
			}
		}
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		if (this._renderer != null)
		{
			this._renderer.enabled = true;
		}
		if (this._collider != null)
		{
			this._collider.enabled = true;
		}
	}

	public override void Input_Disable()
	{
		base.Input_Disable();
		if (this._renderer != null)
		{
			this._renderer.enabled = false;
		}
		if (this._collider != null)
		{
			this._collider.enabled = false;
		}
	}

	private MeshRenderer _renderer;

	private MeshCollider _collider;
}

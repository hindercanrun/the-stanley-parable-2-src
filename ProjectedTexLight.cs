using System;
using UnityEngine;

public class ProjectedTexLight : HammerEntity
{
	private void Awake()
	{
		this._light = base.GetComponent<Light>();
		if (!this.startActive)
		{
			if (this._light != null)
			{
				this._light.enabled = false;
			}
			if (this.connected)
			{
				this.connected.SetActive(false);
			}
		}
	}

	public void Input_TurnOn()
	{
		if (this._light != null)
		{
			this._light.enabled = true;
		}
		if (this.connected)
		{
			this.connected.SetActive(true);
		}
	}

	public void Input_TurnOff()
	{
		if (this._light != null)
		{
			this._light.enabled = false;
		}
		if (this.connected)
		{
			this.connected.SetActive(false);
		}
	}

	private Light _light;

	public bool startActive = true;

	public GameObject connected;
}

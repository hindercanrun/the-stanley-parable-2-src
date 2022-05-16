using System;
using UnityEngine;

public class DynamicLight : HammerEntity
{
	private void Awake()
	{
		this.light = base.GetComponent<Light>();
	}

	public void Input_TurnOn()
	{
		if (this.light)
		{
			this.light.enabled = true;
		}
	}

	public void Input_TurnOff()
	{
		if (this.light)
		{
			this.light.enabled = false;
		}
	}

	private Light light;
}

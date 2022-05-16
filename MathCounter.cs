using System;
using UnityEngine;

public class MathCounter : HammerEntity
{
	private void Awake()
	{
		this.currentValue = this.startValue;
	}

	private void CheckValue(float newValue)
	{
		float num = this.currentValue;
		this.currentValue = Mathf.Clamp(newValue, this.min, this.max);
		base.FireOutput(Outputs.OutValue, this.currentValue);
		if (num != this.currentValue)
		{
			if (this.currentValue == this.max)
			{
				base.FireOutput(Outputs.OnHitMax);
			}
			if (this.currentValue == this.min)
			{
				base.FireOutput(Outputs.OnHitMin);
			}
		}
	}

	public void Input_Add(float value)
	{
		if (!this.isEnabled)
		{
			return;
		}
		Mathf.Round(value);
		this.CheckValue(this.currentValue + value);
	}

	public void Input_Subtract(float value)
	{
		if (!this.isEnabled)
		{
			return;
		}
		Mathf.Round(value);
		this.CheckValue(this.currentValue - value);
	}

	public void Input_Divide(float value)
	{
		if (!this.isEnabled)
		{
			return;
		}
		Mathf.Round(value);
		if (value == 0f)
		{
			return;
		}
		this.CheckValue(this.currentValue / value);
	}

	public void Input_Multiply(float value)
	{
		if (!this.isEnabled)
		{
			return;
		}
		Mathf.Round(value);
		this.CheckValue(this.currentValue * value);
	}

	public void Input_GetValue()
	{
		base.FireOutput(Outputs.OnGetValue, this.currentValue);
	}

	public void Input_SetValue(float value)
	{
		Mathf.Round(value);
		this.CheckValue(value);
	}

	public void Input_SetValueNoFire(float value)
	{
		Mathf.Round(value);
		this.currentValue = Mathf.Clamp(value, this.min, this.max);
	}

	public void Input_SetHitMin(float value)
	{
		Mathf.Round(value);
		this.min = value;
		this.CheckValue(this.currentValue);
	}

	public void Input_SetHitMax(float value)
	{
		Mathf.Round(value);
		this.max = value;
		this.CheckValue(this.currentValue);
	}

	public void Input_SetMinValueNoFire(float value)
	{
		Mathf.Round(value);
		this.min = value;
		this.currentValue = Mathf.Clamp(value, this.min, this.max);
	}

	public void Input_SetMaxValueNoFire(float value)
	{
		Mathf.Round(value);
		this.max = value;
		this.currentValue = Mathf.Clamp(value, this.min, this.max);
	}

	public float min;

	public float max;

	public float startValue;

	private float currentValue;
}

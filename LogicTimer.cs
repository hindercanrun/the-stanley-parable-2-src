using System;
using UnityEngine;

public class LogicTimer : HammerEntity
{
	private void Update()
	{
		if (!this.isEnabled)
		{
			return;
		}
		if (!this.toggledOn)
		{
			return;
		}
		this.currentTime += Singleton<GameMaster>.Instance.GameDeltaTime;
		if (this.currentTime > this.nextRefireTime)
		{
			this.currentTime -= this.nextRefireTime;
			if (this.oscillator)
			{
				if (this.high)
				{
					base.FireOutput(Outputs.OnTimerHigh);
				}
				else
				{
					base.FireOutput(Outputs.OnTimerLow);
				}
			}
			else
			{
				base.FireOutput(Outputs.OnTimer);
			}
			this.ChooseNewRefireTime();
		}
	}

	private void ChooseNewRefireTime()
	{
		if (this.oscillator)
		{
			if (this.high)
			{
				this.nextRefireTime = this.lowerRandomBound;
				this.high = false;
				return;
			}
			this.nextRefireTime = this.upperRandomBound;
			this.high = true;
			return;
		}
		else
		{
			if (this.useRandomTime)
			{
				this.nextRefireTime = Random.Range(this.lowerRandomBound, this.upperRandomBound);
				return;
			}
			this.nextRefireTime = this.refireTime;
			return;
		}
	}

	public void Input_Toggle()
	{
		this.toggledOn = !this.toggledOn;
	}

	public void Input_RefireTime(float newTime)
	{
		this.refireTime = (float)((int)newTime);
		this.ChooseNewRefireTime();
	}

	public void Input_ResetTimer()
	{
		this.currentTime = 0f;
	}

	public void Input_FireTimer()
	{
		base.FireOutput(Outputs.OnTimer);
	}

	public void Input_LowerRandomBound(float newBound)
	{
		this.lowerRandomBound = newBound;
	}

	public void Input_UpperRandomBound(float newBound)
	{
		this.upperRandomBound = newBound;
	}

	public void Input_AddToTimer(float addition)
	{
		this.currentTime += addition;
	}

	public void Input_SubtractFromTimer(float subtraction)
	{
		this.currentTime -= subtraction;
	}

	public float refireTime;

	public bool useRandomTime;

	public float lowerRandomBound;

	public float upperRandomBound;

	public bool oscillator;

	private bool high;

	private float currentTime;

	private float nextRefireTime;

	private bool toggledOn = true;
}

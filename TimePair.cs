using System;
using UnityEngine;

internal struct TimePair
{
	public TimePair(float duration)
	{
		this.startTime = Singleton<GameMaster>.Instance.GameTime;
		this.endTime = this.startTime + duration;
	}

	public TimePair(float start, float duration)
	{
		this.startTime = start;
		this.endTime = this.startTime + duration;
	}

	public bool IsFinished()
	{
		return Singleton<GameMaster>.Instance.GameTime > this.endTime;
	}

	public bool keepWaiting
	{
		get
		{
			return Singleton<GameMaster>.Instance.GameTime < this.endTime;
		}
	}

	public float InverseLerp()
	{
		return Mathf.InverseLerp(this.startTime, this.endTime, Singleton<GameMaster>.Instance.GameTime);
	}

	private fint32 startTime;

	private fint32 endTime;
}

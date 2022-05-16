using System;
using UnityEngine;

public class WaitForGameSeconds : CustomYieldInstruction
{
	public WaitForGameSeconds(float duration)
	{
		this.endTime = Singleton<GameMaster>.Instance.GameTime + duration;
	}

	public override bool keepWaiting
	{
		get
		{
			return Singleton<GameMaster>.Instance.GameTime < this.endTime;
		}
	}

	private fint32 endTime;
}

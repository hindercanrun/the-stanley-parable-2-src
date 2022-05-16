using System;
using System.Collections;
using UnityEngine;

public class TriggerHurt : TriggerMultiple
{
	protected override void StartTouch()
	{
		if (!this.isEnabled)
		{
			return;
		}
		base.StartTouch();
		base.StartCoroutine(this.SeeRed());
		base.FireOutput(Outputs.OnHurtPlayer);
	}

	private IEnumerator SeeRed()
	{
		Singleton<GameMaster>.Instance.BeginFade(this.red, 0.1f, 0.2f, false, false);
		yield return new WaitForGameSeconds(0.19f);
		Singleton<GameMaster>.Instance.CancelFade();
		Singleton<GameMaster>.Instance.BeginFade(this.red, 0.5f, 0f, true, false);
		yield break;
	}

	private Color red = new Color(0.7f, 0f, 0f, 0.4f);
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameText : HammerEntity
{
	public void Input_Display()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.DisplayTextNoFade());
	}

	private IEnumerator DisplayTextNoFade()
	{
		this.canvas.enabled = true;
		yield return new WaitForGameSeconds(this.holdDuration);
		this.canvas.enabled = false;
		yield break;
	}

	private IEnumerator DisplayText()
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		fint32 endInTime = startTime + this.inDuration;
		fint32 startOutTime = endInTime + this.holdDuration;
		fint32 endOutTime = startOutTime + this.outDuration;
		float a = 0f;
		this.color.a = a;
		Color sColor = this.shadow.color;
		sColor.a = a;
		this.text.color = this.color;
		this.shadow.color = sColor;
		this.canvas.enabled = true;
		while (Singleton<GameMaster>.Instance.GameTime < endInTime)
		{
			a = Mathf.InverseLerp(startTime, endInTime, Singleton<GameMaster>.Instance.GameTime);
			this.color.a = a;
			sColor.a = a;
			this.text.color = this.color;
			this.shadow.color = sColor;
			yield return new WaitForEndOfFrame();
		}
		a = 1f;
		this.color.a = a;
		sColor.a = a;
		this.text.color = this.color;
		this.shadow.color = sColor;
		yield return new WaitForGameSeconds(this.holdDuration);
		while (Singleton<GameMaster>.Instance.GameTime < endOutTime)
		{
			a = Mathf.InverseLerp(endOutTime, startOutTime, Singleton<GameMaster>.Instance.GameTime);
			this.color.a = a;
			sColor.a = a;
			this.text.color = this.color;
			this.shadow.color = sColor;
			yield return new WaitForEndOfFrame();
		}
		a = 0f;
		this.color.a = a;
		sColor.a = a;
		this.text.color = this.color;
		this.shadow.color = sColor;
		this.canvas.enabled = false;
		yield break;
	}

	public Color color = Color.white;

	public float inDuration;

	public float outDuration;

	public float holdDuration;

	public string message = "";

	public Canvas canvas;

	public Text text;

	public Text shadow;
}

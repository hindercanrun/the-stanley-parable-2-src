using System;
using System.Collections;
using UnityEngine;

public class FogController : HammerEntity
{
	private void Awake()
	{
		if (this.fogEnable)
		{
			this.Input_TurnOn();
		}
	}

	public void Input_TurnOn()
	{
		if (RenderSettings.fog)
		{
			base.StartCoroutine(this.FadeToFrom());
			return;
		}
		base.StartCoroutine(this.FadeIn());
	}

	public void Input_TurnOff()
	{
		base.StartCoroutine(this.FadeOut());
	}

	private IEnumerator FadeIn()
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		fint32 endTime = startTime + this.fadeDuration;
		RenderSettings.fogColor = this.fogColor;
		RenderSettings.fogMode = FogMode.Linear;
		float startStart = this.fogStart + 100f;
		float endStart = this.fogEnd / this.maxDensity + 150f;
		RenderSettings.fogEndDistance = endStart;
		RenderSettings.fogStartDistance = startStart;
		RenderSettings.fog = true;
		while (Singleton<GameMaster>.Instance.GameTime <= endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			RenderSettings.fogEndDistance = Mathf.Lerp(endStart, this.fogEnd / this.maxDensity, t);
			RenderSettings.fogStartDistance = Mathf.Lerp(startStart, this.fogStart, t);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private IEnumerator FadeToFrom()
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		fint32 endTime = startTime + this.fadeDuration;
		Color startColor = RenderSettings.fogColor;
		RenderSettings.fogMode = FogMode.Linear;
		float startStart = RenderSettings.fogStartDistance;
		float endStart = RenderSettings.fogEndDistance;
		while (Singleton<GameMaster>.Instance.GameTime <= endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			RenderSettings.fogEndDistance = Mathf.Lerp(endStart, this.fogEnd / this.maxDensity, t);
			RenderSettings.fogStartDistance = Mathf.Lerp(startStart, this.fogStart, t);
			RenderSettings.fogColor = Color.Lerp(startColor, this.fogColor, t);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private IEnumerator FadeOut()
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		fint32 endTime = startTime + this.fadeDuration;
		float startEnd = this.fogStart + 100f;
		float endEnd = this.fogEnd / this.maxDensity + 150f;
		while (Singleton<GameMaster>.Instance.GameTime <= endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			RenderSettings.fogEndDistance = Mathf.Lerp(this.fogEnd / this.maxDensity, endEnd, t);
			RenderSettings.fogStartDistance = Mathf.Lerp(this.fogStart, startEnd, t);
			yield return new WaitForEndOfFrame();
		}
		RenderSettings.fog = false;
		yield break;
	}

	public Color fogColor = Color.black;

	public bool fogEnable;

	public float fogStart;

	public float fogEnd = 20f;

	public float maxDensity = 1f;

	private float fadeDuration = 2f;
}

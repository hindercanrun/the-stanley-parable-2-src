using System;
using System.Collections;
using UnityEngine;

public class EnvShake : HammerEntity
{
	private IEnumerator Shake()
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		float endTime = startTime + this.duration;
		Transform cam = StanleyController.Instance.camTransform;
		while (Singleton<GameMaster>.Instance.GameTime < endTime)
		{
			float num = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			float num2 = this.amplitude / 4f;
			if (Singleton<GameMaster>.Instance.IsRumbleEnabled && !GameMaster.PAUSEMENUACTIVE)
			{
				PlatformGamepad.PlayVibration(num2 * 0.25f);
			}
			else
			{
				PlatformGamepad.StopVibration();
			}
			num = Mathf.Clamp01(1f / Mathf.Pow(num, 0.5f) - 1f);
			if (!this.globalShake)
			{
				float value = Vector3.Distance(base.transform.position, StanleyController.Instance.stanleyTransform.position);
				num *= 1f - Mathf.InverseLerp(0f, this.radius, value);
			}
			Quaternion b = Quaternion.Euler(Random.Range(-num2, num2), Random.Range(-num2, num2), Random.Range(-num2, num2));
			cam.localRotation = Quaternion.Slerp(Quaternion.identity, b, num);
			yield return new WaitForEndOfFrame();
		}
		PlatformGamepad.StopVibration();
		yield break;
	}

	public void Input_StartShake()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Shake());
	}

	public void Input_StopShake()
	{
		base.StopAllCoroutines();
		StanleyController.Instance.camTransform.localRotation = Quaternion.identity;
		PlatformGamepad.StopVibration();
	}

	public void Input_Amplitude(float amp)
	{
		this.amplitude = amp;
	}

	public void Input_Frequency(float f)
	{
		this.frequency = f;
	}

	private void OnDestroy()
	{
		PlatformGamepad.StopVibration();
	}

	public float amplitude = 4f;

	public float duration = 1f;

	public float frequency = 2.5f;

	public float radius = 5f;

	public bool globalShake;
}

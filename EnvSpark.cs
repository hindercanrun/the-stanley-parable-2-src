using System;
using UnityEngine;

public class EnvSpark : HammerEntity
{
	private string GuessSparkMagnitude()
	{
		string str;
		if (this.sparkParticle == null)
		{
			str = "PS is null";
		}
		else if (this.sparkParticle.main.startSizeMultiplier == 0.015f)
		{
			str = "small";
		}
		else
		{
			str = "not small";
			if (this.sparkParticle.main.startLifetime.constantMin == 1.5f)
			{
				str = "medium";
			}
			else if (this.sparkParticle.main.startLifetime.constantMin == 2f)
			{
				str = "huge";
			}
		}
		return "From old PS this looks like a: " + str + " spark";
	}

	private void Awake()
	{
		this.sparkInstance = this.InstantiateSparkEffect();
	}

	[ContextMenu("InstantiateSparkEffect")]
	private SparkFX InstantiateSparkEffect()
	{
		SparkEffectData sparkEffectData = Singleton<GameMaster>.Instance.sparkEffectData;
		SparkFX sparkFX;
		switch (this.magnitude)
		{
		case EnvSpark.SparkMagnitude.Small:
			sparkFX = sparkEffectData.smallParticleEffect;
			goto IL_50;
		case EnvSpark.SparkMagnitude.Large:
			sparkFX = sparkEffectData.largeParticleEffect;
			goto IL_50;
		case EnvSpark.SparkMagnitude.Huge:
			sparkFX = sparkEffectData.hugeParticleEffect;
			goto IL_50;
		}
		sparkFX = sparkEffectData.mediumParticleEffect;
		IL_50:
		GameObject gameObject = Object.Instantiate<GameObject>(sparkFX.gameObject);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		return gameObject.GetComponent<SparkFX>();
	}

	public void Input_SparkOnce()
	{
		this.sparkInstance.particleSystemFX.Play();
		this.sparkInstance.audioSource.clip = this.sparkInstance.audioClipSet[Random.Range(0, this.sparkInstance.audioClipSet.Length)];
		this.sparkInstance.audioSource.Play();
	}

	[DynamicLabel("GuessSparkMagnitude")]
	public EnvSpark.SparkMagnitude magnitude;

	[HideInInspector]
	public ParticleSystem sparkParticle;

	private SparkFX sparkInstance;

	public enum SparkMagnitude
	{
		UndefindedDefaultsMedium,
		Small,
		Medium,
		Large,
		Huge
	}
}

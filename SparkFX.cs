using System;
using UnityEngine;

public class SparkFX : MonoBehaviour
{
	private void OnValidate()
	{
		this.particleSystemFX = base.GetComponent<ParticleSystem>();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	public ParticleSystem particleSystemFX;

	public AudioSource audioSource;

	public AudioClip[] audioClipSet;
}

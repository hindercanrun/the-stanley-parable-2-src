using System;
using UnityEngine;

public class MenuPlayRandomSound : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		this.musicSource.pitch = Mathf.Lerp(this.musicSource.pitch, 1f, this.tapeStopRate * Time.deltaTime);
	}

	public void PlayRandomHoverSfx()
	{
		this.PlayRandomSfx(this.hoverSounds);
	}

	public void PlayRandomClickSfx()
	{
		this.PlayRandomSfx(this.clickSounds);
	}

	public void PlayRandomReturnSfx()
	{
		this.PlayRandomSfx(this.returnSounds);
	}

	private void PlayRandomSfx(SoundCollection collection)
	{
		if (this.randomizePitch)
		{
			this.sfxSource.pitch = Random.Range(1f, 1f + this.pitchRange);
		}
		AudioClip randomClip = collection.GetRandomClip();
		if (randomClip != null)
		{
			this.sfxSource.clip = randomClip;
			this.sfxSource.Play();
		}
	}

	public void TapeStop()
	{
		float pitch = this.musicSource.pitch;
		float pitch2 = 1f - this.tapeStopIntensity;
		this.musicSource.pitch = pitch2;
	}

	public SoundCollection hoverSounds;

	public SoundCollection clickSounds;

	public SoundCollection returnSounds;

	public AudioSource sfxSource;

	public AudioSource musicSource;

	public float pitchRange;

	public bool randomizePitch;

	public float tapeStopIntensity = 1f;

	public float tapeStopRate = 1f;
}

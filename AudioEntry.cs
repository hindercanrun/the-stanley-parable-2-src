using System;
using UnityEngine;

[Serializable]
public class AudioEntry
{
	public AudioEntry(AudioClip clip)
	{
		this.audioClip = clip;
	}

	public bool GetClip(out AudioClip clip)
	{
		clip = this.audioClip;
		return this.audioClip != null;
	}

	public float GetPitch()
	{
		return Random.Range(this.minimumPitch, this.maximumPitch);
	}

	public float GetVolume()
	{
		return this.volume;
	}

	[SerializeField]
	private AudioClip audioClip;

	[SerializeField]
	[Range(0f, 1f)]
	private float volume = 1f;

	[SerializeField]
	private float minimumPitch = 1f;

	[SerializeField]
	private float maximumPitch = 1f;
}

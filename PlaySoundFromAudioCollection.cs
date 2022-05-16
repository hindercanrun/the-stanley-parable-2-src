using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundFromAudioCollection : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	public void Play()
	{
		if (this.waitForAverageTime && this.collection != null && Time.realtimeSinceStartup - this.timeStamp <= this.collection.AverageDuration)
		{
			return;
		}
		if (this.audioSource != null && this.collection != null && this.collection.SetVolumeAndPitchAndPlayClip(this.audioSource))
		{
			this.timeStamp = Time.realtimeSinceStartup;
		}
	}

	[SerializeField]
	private AudioSource audioSource;

	private AudioSource audioSource1;

	private AudioSource audioSource2;

	[SerializeField]
	private AudioCollection collection;

	private float timeStamp;

	[SerializeField]
	private bool waitForAverageTime;
}

﻿using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlockChildSound : MonoBehaviour
{
	public void Start()
	{
		this._flockChild = base.GetComponent<FlockChild>();
		this._audio = base.GetComponent<AudioSource>();
		base.InvokeRepeating("PlayRandomSound", Random.value + 1f, 1f);
		if (this._scareSounds.Length != 0)
		{
			base.InvokeRepeating("ScareSound", 1f, 0.01f);
		}
	}

	public void PlayRandomSound()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (!this._audio.isPlaying && this._flightSounds.Length != 0 && this._flightSoundRandomChance > Random.value && !this._flockChild._landing)
			{
				this._audio.clip = this._flightSounds[Random.Range(0, this._flightSounds.Length)];
				this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				return;
			}
			if (!this._audio.isPlaying && this._idleSounds.Length != 0 && this._idleSoundRandomChance > Random.value && this._flockChild._landing)
			{
				this._audio.clip = this._idleSounds[Random.Range(0, this._idleSounds.Length)];
				this._audio.pitch = Random.Range(this._pitchMin, this._pitchMax);
				this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
				this._audio.Play();
				this._hasLanded = true;
			}
		}
	}

	public void ScareSound()
	{
		if (base.gameObject.activeInHierarchy && this._hasLanded && !this._flockChild._landing && this._idleSoundRandomChance * 2f > Random.value)
		{
			this._audio.clip = this._scareSounds[Random.Range(0, this._scareSounds.Length)];
			this._audio.volume = Random.Range(this._volumeMin, this._volumeMax);
			this._audio.PlayDelayed(Random.value * 0.2f);
			this._hasLanded = false;
		}
	}

	public AudioClip[] _idleSounds;

	public float _idleSoundRandomChance = 0.05f;

	public AudioClip[] _flightSounds;

	public float _flightSoundRandomChance = 0.05f;

	public AudioClip[] _scareSounds;

	public float _pitchMin = 0.85f;

	public float _pitchMax = 1f;

	public float _volumeMin = 0.6f;

	public float _volumeMax = 0.8f;

	private FlockChild _flockChild;

	private AudioSource _audio;

	private bool _hasLanded;
}

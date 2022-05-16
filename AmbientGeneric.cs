using System;
using System.Collections;
using UnityEngine;

public class AmbientGeneric : HammerEntity
{
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		if (this.source == null)
		{
			Debug.LogWarning("ambient_generic " + base.name + " didn't have an audio source", base.gameObject);
			this.source = this.sourceEntity.AddComponent<AudioSource>();
		}
		if (this.sourceEntity != base.gameObject)
		{
			AudioSource audioSource = this.source;
			this.source = this.sourceEntity.AddComponent<AudioSource>();
			this.source.clip = audioSource.clip;
			this.source.loop = audioSource.loop;
			this.source.volume = audioSource.volume;
			this.source.spatialBlend = audioSource.spatialBlend;
			this.source.minDistance = audioSource.minDistance;
			this.source.maxDistance = audioSource.maxDistance;
			this.source.playOnAwake = audioSource.playOnAwake;
			this.source.pitch = audioSource.pitch;
			this.source.reverbZoneMix = audioSource.reverbZoneMix;
			this.source.bypassReverbZones = audioSource.bypassReverbZones;
			this.source.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
			Object.Destroy(audioSource);
		}
		if (this.clips.Length != 0)
		{
			this.source.pitch = this.pitchRange.Random();
			this.source.volume = this.volume;
			this.source.clip = this.clips[Random.Range(0, this.clips.Length)];
			if (this.source.playOnAwake)
			{
				this.source.Play();
			}
		}
		GameMaster.OnPause += this.Pause;
		GameMaster.OnResume += this.Resume;
	}

	private void OnDestroy()
	{
		GameMaster.OnPause -= this.Pause;
		GameMaster.OnResume -= this.Resume;
	}

	private void Pause()
	{
		if (this.source != null)
		{
			this.source.Pause();
		}
	}

	private void Resume()
	{
		if (this.source != null)
		{
			this.source.UnPause();
		}
	}

	private void OnValidate()
	{
		if (this.sourceEntity == null || this.sourceEntity.name != this.sourceEntityName || this.sourceEntity != base.gameObject)
		{
			GameObject exists = GameObject.Find(this.sourceEntityName);
			if (exists)
			{
				this.sourceEntity = exists;
				return;
			}
			this.sourceEntity = base.gameObject;
		}
	}

	public void Input_PlaySound()
	{
		if (this.source == null)
		{
			return;
		}
		this.source.clip = this.clips[Random.Range(0, this.clips.Length)];
		this.source.Play();
	}

	public void Input_StopSound()
	{
		if (this.source == null)
		{
			return;
		}
		this.source.Stop();
	}

	public void Input_FadeOut(float duration)
	{
		if (this.source == null)
		{
			return;
		}
		base.StartCoroutine(this.Fade(-1f, 0f, duration));
	}

	public void Input_FadeIn(float duration)
	{
		if (this.source == null)
		{
			return;
		}
		base.StartCoroutine(this.Fade(0f, -1f, duration));
	}

	private IEnumerator Fade(float startVol, float endVol, float duration)
	{
		fint32 startTime = Singleton<GameMaster>.Instance.GameTime;
		fint32 endTime = startTime + duration;
		if (startVol < 0f)
		{
			startVol = this.volume;
		}
		if (endVol < 0f)
		{
			endVol = this.volume;
		}
		while (Singleton<GameMaster>.Instance.GameTime < endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			if (this.source == null)
			{
				yield break;
			}
			this.source.volume = Mathf.Lerp(startVol, endVol, t);
			yield return new WaitForEndOfFrame();
		}
		if (this.source == null)
		{
			yield break;
		}
		this.source.volume = endVol;
		if (this.source.volume == 0f)
		{
			this.source.Stop();
			this.source.volume = startVol;
		}
		yield break;
	}

	private AudioSource source;

	public float volume = 1f;

	public MinMax pitchRange = new MinMax(1f, 1f);

	public AudioClip[] clips;

	public string sourceEntityName = "";

	public GameObject sourceEntity;
}

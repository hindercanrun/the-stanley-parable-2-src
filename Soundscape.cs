using System;
using UnityEngine;

public class Soundscape : HammerEntity
{
	private void Awake()
	{
		if (this.clip)
		{
			this.source = base.gameObject.AddComponent<AudioSource>();
			this.source.volume = this.volume;
			this.source.pitch = this.pitch;
			this.source.clip = this.clip;
			this.source.playOnAwake = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	private AudioSource source;

	public float volume = 1f;

	public float radius = 1f;

	public float fadetime = 1f;

	public float pitch = 1f;

	public AudioClip clip;
}

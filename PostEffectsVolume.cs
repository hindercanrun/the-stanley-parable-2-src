using System;
using UnityEngine;

public class PostEffectsVolume : VolumeBase
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInChildren<PostEffectsCamera>() != null && PostEffectsVolume.OnEnterVolume != null)
		{
			PostEffectsVolume.OnEnterVolume(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponentInChildren<PostEffectsCamera>() != null && PostEffectsVolume.OnExitVolume != null)
		{
			PostEffectsVolume.OnExitVolume(this);
		}
	}

	public override ProfileBase GetProfile()
	{
		return this.Profile;
	}

	public static Action<PostEffectsVolume> OnEnterVolume;

	public static Action<PostEffectsVolume> OnExitVolume;

	public PostEffectsProfile Profile;
}

using System;
using UnityEngine;

public class ReverbVolume : VolumeBase
{
	private void OnTriggerEnter(Collider other)
	{
		if (this.Profile != null && other.GetComponentInChildren<ReverbController>() != null && ReverbVolume.OnEnterReverbVolume != null)
		{
			ReverbVolume.OnEnterReverbVolume(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.Profile != null && other.GetComponentInChildren<ReverbController>() != null && ReverbVolume.OnExitReverbVolume != null && ReverbVolume.OnExitReverbVolume != null)
		{
			ReverbVolume.OnExitReverbVolume(this);
		}
	}

	public override ProfileBase GetProfile()
	{
		return this.Profile;
	}

	protected override Color GetVolumeBaseColor()
	{
		return Color.cyan * new Color(1f, 1f, 1f, 0.25f);
	}

	[Space]
	public ReverbProfile Profile;

	public static Action<ReverbVolume> OnEnterReverbVolume;

	public static Action<ReverbVolume> OnExitReverbVolume;
}

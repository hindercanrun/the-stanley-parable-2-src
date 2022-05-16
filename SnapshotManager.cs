using System;
using UnityEngine;
using UnityEngine.Audio;

public class SnapshotManager : MonoBehaviour
{
	private void Start()
	{
		if (this.FirstSnapshot != null)
		{
			this.TransitionToSnapshot(this.FirstSnapshot, 0f);
		}
	}

	public void TransitionToSnapshot(AudioMixerSnapshot snapshot)
	{
		this.Mixer.FindSnapshot(snapshot.name).TransitionTo(this.transitionTime);
	}

	public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float customTransitionTime)
	{
		this.Mixer.FindSnapshot(snapshot.name).TransitionTo(customTransitionTime);
	}

	public void RaiseIntensityLevel()
	{
		this.intensityIndex++;
		if (this.intensityIndex >= this.IntensitySnapshots.Length)
		{
			this.intensityIndex = this.IntensitySnapshots.Length - 1;
			return;
		}
		this.TransitionToSnapshot(this.IntensitySnapshots[this.intensityIndex]);
	}

	public void ChangeTransitionTime(float newTime)
	{
		this.transitionTime = newTime;
	}

	public void LowerIntensityLevel()
	{
		this.intensityIndex--;
		if (this.intensityIndex < 0)
		{
			this.intensityIndex = 0;
			return;
		}
		this.TransitionToSnapshot(this.IntensitySnapshots[this.intensityIndex]);
	}

	public AudioMixer Mixer;

	public AudioMixerSnapshot FirstSnapshot;

	public float transitionTime = 0.3f;

	public AudioMixerSnapshot[] IntensitySnapshots;

	private int intensityIndex;
}

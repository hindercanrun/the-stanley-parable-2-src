using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoreographedScene : HammerEntity
{
	private void Start()
	{
		for (int i = 0; i < this.choreoEvents.Count; i++)
		{
			this.choreoEvents[i].owner = this;
		}
		if (this.choreoEvents.Count >= 1)
		{
			this.choreoEvents[0].PreloadAudioClips();
		}
	}

	private void AutoToken()
	{
	}

	public void Input_Start()
	{
		if (this.killed || !this.isEnabled)
		{
			return;
		}
		Singleton<ChoreoMaster>.Instance.BeginEvents(this.choreoEvents, this.interrupt, this.spawnflag49);
		base.FireOutput(Outputs.OnStart);
	}

	public void FinishedEvent(ChoreographedEvent e)
	{
		if (e.Clip == this.choreoEvents[this.choreoEvents.Count - 1].Clip)
		{
			base.FireOutput(Outputs.OnCompletion);
			return;
		}
		if (this.cancelled)
		{
			base.FireOutput(Outputs.OnCanceled);
		}
	}

	public void Cancelled()
	{
		this.cancelled = true;
	}

	public override void Input_Kill()
	{
		Singleton<ChoreoMaster>.Instance.DropOwnedEvents(this);
		this.killed = true;
		base.Input_Kill();
	}

	public bool spawnflag49;

	public ChoreographedScene.InteruptBehaviour interrupt;

	public List<ChoreographedEvent> choreoEvents = new List<ChoreographedEvent>();

	private bool cancelled;

	private bool killed;

	[InspectorButton("AutoToken", null)]
	public bool doesNothingRightNow;

	[Header("Sets 'Clip' variable in events in bulk (or creates new events if there aren't enough)")]
	public List<VoiceClip> williamDragNewVoiceClipsHere;

	public enum InteruptBehaviour
	{
		StartImmediately,
		WaitForFinish,
		InterruptAtNextEvent,
		CancelAtNextEvent
	}
}

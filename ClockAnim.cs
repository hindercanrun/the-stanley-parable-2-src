using System;
using UnityEngine;

public class ClockAnim : MonoBehaviour
{
	private void Awake()
	{
		this.anim = base.GetComponent<Animator>();
		float normalizedTime = ((float)this.startHour + (float)this.startMinute / 60f) / 12f;
		float normalizedTime2 = ((float)this.startMinute + (float)this.startSecond / 60f) / 60f;
		float normalizedTime3 = (float)this.startSecond / 60f;
		this.anim.Play("HourHand", 0, normalizedTime);
		this.anim.Play("MinuteHand", 1, normalizedTime2);
		this.anim.Play("SecondHand", 2, normalizedTime3);
		GameMaster.OnPause += this.Pause;
		GameMaster.OnResume += this.Resume;
	}

	private void OnDisable()
	{
		GameMaster.OnPause -= this.Pause;
		GameMaster.OnResume -= this.Resume;
	}

	private void Pause()
	{
		this.anim.enabled = false;
	}

	private void Resume()
	{
		this.anim.enabled = true;
	}

	private Animator anim;

	public int startHour;

	public int startMinute;

	public int startSecond;
}

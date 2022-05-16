using System;
using UnityEngine;

public class InfoParticleSystem : HammerEntity
{
	private void Awake()
	{
		this.systems = base.GetComponentsInChildren<ParticleSystem>();
		if (this.startActive)
		{
			this.Input_Start();
		}
	}

	public void Input_Start()
	{
		for (int i = 0; i < this.systems.Length; i++)
		{
			this.systems[i].Play();
		}
	}

	public void Input_Stop()
	{
		for (int i = 0; i < this.systems.Length; i++)
		{
			this.systems[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	public bool startActive;

	private ParticleSystem[] systems = new ParticleSystem[0];
}

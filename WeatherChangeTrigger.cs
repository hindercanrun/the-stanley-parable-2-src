using System;
using UnityEngine;

public class WeatherChangeTrigger : MonoBehaviour
{
	private void Start()
	{
		this.weatherAnimator.enabled = true;
		this.weatherAnimator.SetBool("stormyTriggerEntered", false);
	}

	private void OnTriggerEnter()
	{
		this.weatherAnimator.SetBool("stormyTriggerEntered", true);
	}

	private void Update()
	{
	}

	public Animator weatherAnimator;
}

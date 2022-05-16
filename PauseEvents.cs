using System;
using UnityEngine;
using UnityEngine.Events;

public class PauseEvents : MonoBehaviour
{
	private void Awake()
	{
		GameMaster.OnPause += this.OnPause;
		GameMaster.OnResume += this.OnResume;
	}

	private void OnPause()
	{
		if (this.active)
		{
			this.OnPauseEvent.Invoke();
		}
	}

	private void OnResume()
	{
		if (this.active)
		{
			this.OnResumeEvent.Invoke();
		}
	}

	[SerializeField]
	private bool active = true;

	[SerializeField]
	private UnityEvent OnPauseEvent;

	[SerializeField]
	private UnityEvent OnResumeEvent;
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEventReceiver : MonoBehaviour
{
	private void Awake()
	{
		if (this.simpleEvent != null)
		{
			SimpleEvent simpleEvent = this.simpleEvent;
			simpleEvent.OnCall = (Action)Delegate.Combine(simpleEvent.OnCall, new Action(this.OnCall));
		}
	}

	private void OnDestroy()
	{
		if (this.simpleEvent != null)
		{
			SimpleEvent simpleEvent = this.simpleEvent;
			simpleEvent.OnCall = (Action)Delegate.Remove(simpleEvent.OnCall, new Action(this.OnCall));
		}
	}

	private void OnCall()
	{
		this.OnCallEvent.Invoke();
	}

	[SerializeField]
	private SimpleEvent simpleEvent;

	[SerializeField]
	private UnityEvent OnCallEvent;
}

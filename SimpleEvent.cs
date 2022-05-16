using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "SimpleEvent/New Event")]
public class SimpleEvent : ScriptableObject
{
	[ContextMenu("Call")]
	public void Call()
	{
		if (this.OnCall != null)
		{
			this.OnCall();
		}
	}

	public Action OnCall;
}

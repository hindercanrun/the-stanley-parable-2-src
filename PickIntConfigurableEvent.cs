using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickIntConfigurableEvent : MonoBehaviour
{
	public void InvokePickEvent()
	{
		PickIntConfigurableEvent.IntPickEvent intPickEvent = this.events.Find((PickIntConfigurableEvent.IntPickEvent x) => x.intConfigurable.GetIntValue() == this.pickInteger);
		if (intPickEvent == null)
		{
			return;
		}
		UnityEvent evt = intPickEvent.evt;
		if (evt == null)
		{
			return;
		}
		evt.Invoke();
	}

	[SerializeField]
	private int pickInteger;

	[SerializeField]
	private List<PickIntConfigurableEvent.IntPickEvent> events;

	[Serializable]
	private class IntPickEvent
	{
		public IntConfigurable intConfigurable;

		public UnityEvent evt;
	}
}

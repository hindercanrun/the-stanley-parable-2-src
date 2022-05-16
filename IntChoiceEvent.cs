using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntChoiceEvent : MonoBehaviour
{
	private void Awake()
	{
		if (this.intConfigurable != null)
		{
			IntConfigurable intConfigurable = this.intConfigurable;
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
			this.intConfigurable.Init();
		}
	}

	private void Start()
	{
		if (this.intConfigurable != null)
		{
			this.intConfigurable.Init();
		}
		if (this.selfInvokeOnValueChange)
		{
			this.InvokeChoiceEvent();
		}
	}

	private void OnDestroy()
	{
		if (this.intConfigurable != null)
		{
			IntConfigurable intConfigurable = this.intConfigurable;
			intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
		}
	}

	private void OnValueChanged(LiveData data)
	{
		if (this.selfInvokeOnValueChange)
		{
			this.InvokeChoiceEvent();
		}
	}

	public void InvokeChoiceEvent()
	{
		IntChoiceEvent.IntBasedEvent intBasedEvent = this.events.Find((IntChoiceEvent.IntBasedEvent x) => x.i == this.intConfigurable.GetIntValue());
		if (intBasedEvent == null)
		{
			return;
		}
		UnityEvent evt = intBasedEvent.evt;
		if (evt == null)
		{
			return;
		}
		evt.Invoke();
	}

	[SerializeField]
	private IntConfigurable intConfigurable;

	[SerializeField]
	private bool selfInvokeOnValueChange;

	[SerializeField]
	private List<IntChoiceEvent.IntBasedEvent> events;

	[Serializable]
	private class IntBasedEvent
	{
		public int i;

		public UnityEvent evt;
	}
}

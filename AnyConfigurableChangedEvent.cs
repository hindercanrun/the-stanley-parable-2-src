using System;
using UnityEngine;
using UnityEngine.Events;

public class AnyConfigurableChangedEvent : MonoBehaviour
{
	private void Start()
	{
		foreach (Configurable configurable in this.configurables)
		{
			configurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(configurable.OnValueChanged, new Action<LiveData>(this.ValueChangedEvent));
		}
	}

	private void OnDestroy()
	{
		foreach (Configurable configurable in this.configurables)
		{
			if (configurable != null)
			{
				Configurable configurable2 = configurable;
				configurable2.OnValueChanged = (Action<LiveData>)Delegate.Remove(configurable2.OnValueChanged, new Action<LiveData>(this.ValueChangedEvent));
			}
		}
	}

	private void ValueChangedEvent(LiveData liveData)
	{
		UnityEvent onAnyConfigurableChange = this.OnAnyConfigurableChange;
		if (onAnyConfigurableChange == null)
		{
			return;
		}
		onAnyConfigurableChange.Invoke();
	}

	public Configurable[] configurables;

	public bool invokeCheckOnStart;

	public UnityEvent OnAnyConfigurableChange;
}

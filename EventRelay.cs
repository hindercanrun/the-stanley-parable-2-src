using System;
using UnityEngine;
using UnityEngine.Events;

public class EventRelay : MonoBehaviour
{
	private void Start()
	{
		if (this.invokeOnStart)
		{
			this.Invoke();
		}
	}

	public void TestInvoke()
	{
		this.Invoke();
	}

	[ContextMenu("Invoke")]
	public void Invoke()
	{
		UnityEvent unityEvent = this.invokableEvent;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	[SerializeField]
	private UnityEvent invokableEvent;

	[SerializeField]
	[InspectorButton("TestInvoke", "Test Invoke")]
	private bool invokeOnStart;
}

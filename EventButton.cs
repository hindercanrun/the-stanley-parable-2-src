using System;
using UnityEngine;
using UnityEngine.Events;

public class EventButton : MenuButton
{
	public override void OnClick(Vector3 point = default(Vector3))
	{
		this.OnTrigger.Invoke();
	}

	[SerializeField]
	private UnityEvent OnTrigger;
}

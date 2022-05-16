using System;
using UnityEngine;
using UnityEngine.Events;

public class StanleyInputEvent : MonoBehaviour
{
	private void Awake()
	{
		this.actions = StanleyActions.CreateWithDefaultBindings();
	}

	public void SetListening(bool value)
	{
		this.listening = value;
	}

	private void Update()
	{
		if (GameMaster.PAUSEMENUACTIVE || !this.listening)
		{
			return;
		}
		switch (this.inputEvent)
		{
		case StanleyInputEvent.InputEvent.ExtraEvent1:
			if (this.actions.ExtraAction(0, true).WasPressed)
			{
				this.OnInput.Invoke();
				return;
			}
			break;
		case StanleyInputEvent.InputEvent.ExtraEvent2:
			if (this.actions.ExtraAction(1, true).WasPressed)
			{
				this.OnInput.Invoke();
				return;
			}
			break;
		case StanleyInputEvent.InputEvent.ExtraEvent3:
			if (this.actions.ExtraAction(2, true).WasPressed)
			{
				this.OnInput.Invoke();
				return;
			}
			break;
		case StanleyInputEvent.InputEvent.ExtraEvent4:
			if (this.actions.ExtraAction(3, true).WasPressed)
			{
				this.OnInput.Invoke();
			}
			break;
		default:
			return;
		}
	}

	[SerializeField]
	private bool listening = true;

	[SerializeField]
	private StanleyInputEvent.InputEvent inputEvent;

	[SerializeField]
	private UnityEvent OnInput;

	private StanleyActions actions;

	public enum InputEvent
	{
		ExtraEvent1,
		ExtraEvent2,
		ExtraEvent3,
		ExtraEvent4
	}
}

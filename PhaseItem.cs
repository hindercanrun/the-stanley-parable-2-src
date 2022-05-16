using System;
using UnityEngine.Events;

[Serializable]
public class PhaseItem
{
	public PhaseItem.PhaseMode Mode;

	public string Description;

	public float Duration;

	public UnityEvent PhaseEvent;

	public enum PhaseMode
	{
		Default,
		Disabled
	}
}

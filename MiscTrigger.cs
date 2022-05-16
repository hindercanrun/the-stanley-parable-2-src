using System;
using UnityEngine;
using UnityEngine.Events;

public class MiscTrigger : MonoBehaviour
{
	private void Awake()
	{
		StanleyController.OnActuallyJumping = (Action)Delegate.Combine(StanleyController.OnActuallyJumping, new Action(this.OnActuallyJumping));
	}

	private void OnDestroy()
	{
		StanleyController.OnActuallyJumping = (Action)Delegate.Remove(StanleyController.OnActuallyJumping, new Action(this.OnActuallyJumping));
	}

	private void OnActuallyJumping()
	{
		if (this.condition == MiscTrigger.Condition.IsJumping)
		{
			this.OnConditionMet.Invoke();
		}
	}

	public void Invoke()
	{
		MiscTrigger.Condition condition = this.condition;
	}

	[SerializeField]
	private MiscTrigger.Condition condition;

	[SerializeField]
	private UnityEvent OnConditionMet;

	public enum Condition
	{
		IsJumping
	}
}

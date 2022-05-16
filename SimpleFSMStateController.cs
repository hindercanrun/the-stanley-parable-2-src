using System;
using UnityEngine;

public class SimpleFSMStateController : MonoBehaviour
{
	public SimpleFSMState CurrentState
	{
		get
		{
			return this.StateMachine.currentState;
		}
		private set
		{
		}
	}

	protected void CreateFSM(SimpleFSMState[] stateArray)
	{
		this.StateMachine = new SimpleFSM(stateArray[0]);
		for (int i = 1; i < stateArray.Length; i++)
		{
			this.StateMachine.AddState(stateArray[i]);
		}
	}

	public void StartFSM()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.StateMachine.StartFSM());
	}

	public void ChangeState<R>() where R : SimpleFSMState
	{
		this.StateMachine.ChangeState<R>();
	}

	public void StopFSM()
	{
		base.StopAllCoroutines();
		this.StateMachine.StopFSM();
	}

	[SerializeField]
	private string currentState;

	protected SimpleFSM StateMachine;
}

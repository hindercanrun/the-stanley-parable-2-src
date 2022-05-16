using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFSM
{
	public SimpleFSM(SimpleFSMState _initialState)
	{
		this.initialState = _initialState;
		this.currentState = this.initialState;
		this.AddState(this.initialState);
	}

	public IEnumerator StartFSM()
	{
		this.alive = true;
		this.inTransitionOrNotReady = true;
		this.currentState = this.initialState;
		yield return this.currentState.cntrl.StartCoroutine(this.currentState.Begin());
		this.inTransitionOrNotReady = false;
		yield break;
	}

	public void StopFSM()
	{
		this.currentState.cntrl.StopAllCoroutines();
		this.inTransitionOrNotReady = false;
		this.alive = false;
	}

	public void AddState(SimpleFSMState stateToAdd)
	{
		this._states.Add(stateToAdd.GetType(), stateToAdd);
	}

	public void ChangeState<R>() where R : SimpleFSMState
	{
		Type typeFromHandle = typeof(!!0);
		this.inTransitionOrNotReady = true;
		this.currentState.cntrl.StartCoroutine(this.PerformChangeState(this._states[typeFromHandle]));
	}

	public void ChangeState(SimpleFSMState newState)
	{
		this.inTransitionOrNotReady = true;
		this.currentState.cntrl.StartCoroutine(this.PerformChangeState(newState));
	}

	public IEnumerator PerformChangeState(SimpleFSMState newState)
	{
		this.nextState = newState.GetType();
		this.lastState = this.currentState.GetType();
		yield return this.currentState.cntrl.StartCoroutine(this.currentState.Exit());
		this.currentState = this._states[newState.GetType()];
		yield return this.currentState.cntrl.StartCoroutine(this.currentState.Begin());
		this.inTransitionOrNotReady = false;
		yield return null;
		yield break;
	}

	public void UpdateState()
	{
		if (this.currentState != null && !this.inTransitionOrNotReady)
		{
			this.currentState.Reason();
			this.currentState.DoUpdate();
		}
	}

	public void LateUpdateState()
	{
		if (this.currentState != null && !this.inTransitionOrNotReady)
		{
			this.currentState.DoLateUpdate();
		}
	}

	public void FixedUpdateState()
	{
		if (this.currentState != null && !this.inTransitionOrNotReady)
		{
			this.currentState.DoFixedUpdate();
		}
	}

	public void OnCollisionEnterState(Collision col)
	{
		this.currentState.OnCollisionState(col);
	}

	public void OnTriggerEnterState(Collider col)
	{
		this.currentState.OnTriggerEnterState(col);
	}

	public SimpleFSMState currentState;

	public Type lastState;

	public Type nextState;

	private Dictionary<Type, SimpleFSMState> _states = new Dictionary<Type, SimpleFSMState>();

	public bool inTransitionOrNotReady = true;

	public bool alive;

	private SimpleFSMState initialState;
}

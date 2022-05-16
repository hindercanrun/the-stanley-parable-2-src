using System;
using System.Collections;
using UnityEngine;

public abstract class SimpleFSMState
{
	public SimpleFSMState(SimpleFSMStateController controller)
	{
		this.cntrl = controller;
	}

	public virtual IEnumerator Begin()
	{
		yield return null;
		yield break;
	}

	public virtual IEnumerator Exit()
	{
		yield return null;
		yield break;
	}

	public virtual void Setup()
	{
	}

	public virtual void Reason()
	{
	}

	public virtual void DoUpdate()
	{
	}

	public virtual void DoFixedUpdate()
	{
	}

	public virtual void DoLateUpdate()
	{
	}

	public virtual void UpdateAnimator()
	{
	}

	public virtual void OnCollisionState(Collision col)
	{
	}

	public virtual void OnTriggerEnterState(Collider col)
	{
	}

	public SimpleFSMStateController cntrl;
}

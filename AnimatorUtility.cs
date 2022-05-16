using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorUtility : MonoBehaviour
{
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	public void SetBool(bool value)
	{
		this.animator.SetBool(this.parameter, value);
	}

	[SerializeField]
	private string parameter;

	private Animator animator;
}

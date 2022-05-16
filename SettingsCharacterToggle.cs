using System;
using UnityEngine;

public class SettingsCharacterToggle : MonoBehaviour
{
	public void SetAnimationBooleanForToggle(bool isOn)
	{
		this.animator.SetBool(this.toggleString, isOn);
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string toggleString = "Toggled";
}

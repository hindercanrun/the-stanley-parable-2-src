using System;
using UnityEngine;
using UnityEngine.UI;

public class SequelConfirmButton : MonoBehaviour
{
	public void InvokeCheck()
	{
		if (this.prefixIndexConfigurable.GetIntValue() != -1 && this.postfixIndexConfigurable.GetIntValue() != -1)
		{
			this.confirmButton.interactable = true;
			return;
		}
		this.confirmButton.interactable = false;
	}

	public Button confirmButton;

	public IntConfigurable prefixIndexConfigurable;

	public IntConfigurable postfixIndexConfigurable;
}

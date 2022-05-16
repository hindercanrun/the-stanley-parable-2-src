using System;
using InControl;
using UnityEngine;

[RequireComponent(typeof(InControlInputModule))]
public class InControlInputModuleParameterSetter : MonoBehaviour
{
	private InControlInputModule InputModule
	{
		get
		{
			if (this.inputModule == null)
			{
				this.inputModule = base.GetComponent<InControlInputModule>();
			}
			return this.inputModule;
		}
	}

	private void Start()
	{
		this.InputModule.submitButton = (InControlInputModule.Button)PlatformGamepad.ConfirmButton;
		this.InputModule.cancelButton = (InControlInputModule.Button)PlatformGamepad.BackButton;
	}

	public void SetFocusMouseOnHover(bool focusMouseOnHover)
	{
		this.InputModule.focusOnMouseHover = focusMouseOnHover;
	}

	private InControlInputModule inputModule;
}

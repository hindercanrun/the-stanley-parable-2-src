using System;
using Nest.Components;
using UnityEngine;

namespace Nest.Addons
{
	public class ButtonInterpreter : MonoBehaviour
	{
		public void Awake()
		{
			if ((this._camera = base.GetComponent<Camera>()) == null)
			{
				this._camera = base.GetComponentInChildren<Camera>();
			}
			this._results = new RaycastHit[3];
		}

		public void Update()
		{
			this._canInteract = this.FindInteractable();
			if (this._currentInteractableEvent == null)
			{
				return;
			}
			switch (this._eventState)
			{
			case ButtonInterpreter.InteractionState.Dormant:
				if (Input.GetKey(this.InteractKey))
				{
					this.InvokeButton(true);
					this._eventState = ButtonInterpreter.InteractionState.Active;
					return;
				}
				break;
			case ButtonInterpreter.InteractionState.Active:
				if (!Input.GetKey(this.InteractKey) || !this._canInteract)
				{
					this.InvokeButton(false);
					this._eventState = ButtonInterpreter.InteractionState.Used;
					return;
				}
				break;
			case ButtonInterpreter.InteractionState.Used:
				this._eventState = ButtonInterpreter.InteractionState.Dormant;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void InvokeButton(bool state)
		{
			if (this._currentInteractableEvent == null)
			{
				return;
			}
			this._currentInteractableEvent.SetBool(state);
			this._currentInteractableEvent.Invoke();
		}

		protected virtual bool FindInteractable()
		{
			int num;
			if ((num = Physics.RaycastNonAlloc(this._camera.transform.position, this._camera.transform.forward, this._results, this.MaxInteractDistance, LayerMask.GetMask(new string[]
			{
				"Interact"
			}))) <= 0)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (!(this._results[i].transform == null))
				{
					NestInput component = this._results[i].transform.GetComponent<NestInput>();
					if (this._currentInteractableEvent != component && this._eventState == ButtonInterpreter.InteractionState.Active)
					{
						this._eventState = ButtonInterpreter.InteractionState.Used;
						this.InvokeButton(false);
					}
					else if (this._currentInteractableEvent == component)
					{
						return true;
					}
					this._currentInteractableEvent = component;
				}
			}
			return this._currentInteractableEvent != null;
		}

		[Header("Sets the Max interaction Distance")]
		public float MaxInteractDistance = 25f;

		public KeyCode InteractKey;

		private Camera _camera;

		private ButtonInterpreter.InteractionState _eventState;

		private bool _canInteract;

		private NestInput _currentInteractableEvent;

		private RaycastHit[] _results;

		public enum InteractionState
		{
			Dormant,
			Active,
			Used
		}
	}
}

using System;
using System.Collections.Generic;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StanleyUI
{
	public class StanleyMenuKeybindingSelector : MonoBehaviour, IDeselectHandler, IEventSystemHandler
	{
		private PlayerAction Action
		{
			get
			{
				return Singleton<GameMaster>.Instance.stanleyActions.GetPlayerActionByName(this.actionName);
			}
		}

		public void Awake()
		{
			Singleton<GameMaster>.Instance.stanleyActions.OnKeyBindingsLoaded += this.UpdateKeyText;
		}

		private void Start()
		{
			this.SetState(StanleyMenuKeybindingSelector.State.Normal);
			this.listeningOptions = new BindingListenOptions
			{
				MaxAllowedBindingsPerType = 1U,
				IncludeControllers = false,
				IncludeUnknownControllers = false,
				IncludeNonStandardControls = false,
				IncludeKeys = true,
				IncludeModifiersAsFirstClassKeys = true,
				IncludeMouseButtons = false,
				IncludeMouseScrollWheel = false,
				UnsetDuplicateBindingsOnSet = false,
				AllowDuplicateBindingsPerSet = true,
				RejectRedundantBindings = true,
				OnBindingFound = new Func<PlayerAction, BindingSource, bool>(this.OnBindingFound),
				OnBindingAdded = new Action<PlayerAction, BindingSource>(this.OnBindingAdded),
				OnBindingEnded = new Action<PlayerAction>(this.OnBindingEnded)
			};
			this.UpdateKeyText();
			StringConfigurable stringConfigurable = this.allKeybindingsConfigurable;
			stringConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(stringConfigurable.OnValueChanged, new Action<LiveData>(this.KeyBindingsChanged));
		}

		private void KeyBindingsChanged(LiveData live)
		{
			this.UpdateKeyText();
		}

		private void OnBindingAdded(PlayerAction action, BindingSource binding)
		{
			this.SetState(StanleyMenuKeybindingSelector.State.Normal);
			Singleton<GameMaster>.Instance.stanleyActions.SaveCustomKeyBindings(this.allKeybindingsConfigurable);
		}

		private bool OnBindingFound(PlayerAction action, BindingSource binding)
		{
			if (binding == new KeyBindingSource(new Key[]
			{
				Key.Escape
			}) || this.BindingUsedAlready(binding))
			{
				action.StopListeningForBinding();
				this.UpdateKeyText();
				this.SetState(StanleyMenuKeybindingSelector.State.Normal);
				return false;
			}
			return true;
		}

		private bool BindingUsedAlready(BindingSource binding)
		{
			using (IEnumerator<PlayerAction> enumerator = Singleton<GameMaster>.Instance.stanleyActions.UsedInGameActions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasBinding(binding))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void OnBindingEnded(PlayerAction action)
		{
			this.ReenableAllInputs();
		}

		private void ReenableAllInputs()
		{
			Singleton<GameMaster>.Instance.stanleyActions.Enabled = true;
		}

		private void DisableAllInputs()
		{
			Singleton<GameMaster>.Instance.stanleyActions.Enabled = false;
		}

		public void UpdateKeyText()
		{
			foreach (BindingSource bindingSource in this.Action.Bindings)
			{
				if (bindingSource.DeviceClass == InputDeviceClass.Keyboard)
				{
					this.uiText.text = bindingSource.Name;
					break;
				}
			}
		}

		public void Activate()
		{
			if (this.state == StanleyMenuKeybindingSelector.State.Normal)
			{
				this.SetState(StanleyMenuKeybindingSelector.State.WaitingForKey);
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (this.Action.IsListeningForBinding)
			{
				this.Action.StopListeningForBinding();
				this.SetState(StanleyMenuKeybindingSelector.State.Normal);
			}
		}

		private void SetState(StanleyMenuKeybindingSelector.State newState)
		{
			this.state = newState;
			if (this.state == StanleyMenuKeybindingSelector.State.WaitingForKey)
			{
				this.uiText.gameObject.SetActive(false);
				this.pressAnyKey.gameObject.SetActive(true);
				this.Action.ListenOptions = this.listeningOptions;
				this.Action.ListenForBindingReplacing(this.Action.Bindings[0]);
				this.DisableAllInputs();
				return;
			}
			this.UpdateKeyText();
			this.uiText.gameObject.SetActive(true);
			this.pressAnyKey.gameObject.SetActive(false);
		}

		public string actionName = "";

		public StanleyMenuKeybindingSelector.State state;

		public StringConfigurable allKeybindingsConfigurable;

		public TextMeshProUGUI uiText;

		public TextMeshProUGUI pressAnyKey;

		private BindingListenOptions listeningOptions;

		public enum State
		{
			Normal,
			WaitingForKey
		}
	}
}

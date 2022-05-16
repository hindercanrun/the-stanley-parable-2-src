using System;
using System.Collections;
using Nest.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Nest.Components
{
	[AddComponentMenu("Nest/Components/Nest Input")]
	public class NestInput : MonoBehaviour
	{
		public event CastEvent EventFired;

		public bool HasFired
		{
			get
			{
				return this._fired;
			}
		}

		public NestInput.EventType CurrentEventType
		{
			get
			{
				return this._eventType;
			}
			set
			{
				this._eventType = value;
			}
		}

		public UnityEventBase CurrentEvent
		{
			get
			{
				NestInput.EventType eventType = this._eventType;
				switch (eventType)
				{
				case NestInput.EventType.Trigger:
					return this._event;
				case NestInput.EventType.Bool:
					break;
				case NestInput.EventType.Trigger | NestInput.EventType.Bool:
					goto IL_72;
				case NestInput.EventType.Float:
					return this._eventValue;
				default:
					if (eventType != NestInput.EventType.Toggle)
					{
						if (eventType != NestInput.EventType.String)
						{
							goto IL_72;
						}
						return this._eventString;
					}
					break;
				}
				if (this.Value.CurrentValue < 0.01f)
				{
					return this._event;
				}
				if (this.Value.CurrentValue > 0.99f)
				{
					return this._event2;
				}
				IL_72:
				return this._event;
			}
		}

		public bool EventPosition
		{
			get
			{
				return Mathf.Abs(this.Value.CurrentValue) < 0.01f;
			}
			set
			{
				this.Value.CurrentValue = (float)(value ? 1 : 0);
			}
		}

		public virtual void Start()
		{
			this.Value = new FloatInterpolator(this._eventOffValue, this._eventOnValue, this._interpolation);
		}

		public void Invoke()
		{
			if ((this.FireOnce && this._fired) || !base.enabled)
			{
				return;
			}
			this._fired = true;
			if (this.Delay > 0f)
			{
				base.StartCoroutine(this.InvokeDelay());
				return;
			}
			this.InvokeEvent();
		}

		private IEnumerator InvokeDelay()
		{
			yield return new WaitForGameSeconds(this.Delay);
			this.InvokeEvent();
			yield break;
		}

		public void InvokeEvent()
		{
			NestInput.EventType eventType = this._eventType;
			switch (eventType)
			{
			case NestInput.EventType.Trigger:
				this._event.Invoke();
				break;
			case NestInput.EventType.Bool:
				if (Mathf.Abs(this.Value.CurrentValue) < 0.01f)
				{
					this._event2.Invoke();
				}
				else if (Mathf.Abs(this.Value.CurrentValue) - 1f < 0.01f)
				{
					this._event.Invoke();
				}
				break;
			case NestInput.EventType.Trigger | NestInput.EventType.Bool:
				break;
			case NestInput.EventType.Float:
				this._eventValue.Invoke(this._parameterFloat);
				break;
			default:
				if (eventType != NestInput.EventType.Toggle)
				{
					if (eventType == NestInput.EventType.String)
					{
						this._eventString.Invoke(this._parameterString);
					}
				}
				else
				{
					this.Value.CurrentValue = 1f - this.Value.CurrentValue;
					if (Mathf.Abs(this.Value.CurrentValue) < 0.01f)
					{
						this._event2.Invoke();
					}
					else
					{
						this._event.Invoke();
					}
				}
				break;
			}
			if (this.EventFired != null)
			{
				this.EventFired(this);
			}
		}

		public int[] FindGameObjectsReferences()
		{
			float currentValue = this.Value.CurrentValue;
			this.Value.CurrentValue = 0f;
			int num = this.CurrentEvent.GetPersistentEventCount();
			if (this._eventType == NestInput.EventType.Bool || this._eventType == NestInput.EventType.Toggle)
			{
				num += this._event2.GetPersistentEventCount();
			}
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				Object @object = null;
				if (i > this.CurrentEvent.GetPersistentEventCount())
				{
					@object = this._event2.GetPersistentTarget(i - this.CurrentEvent.GetPersistentEventCount());
				}
				else if (this.CurrentEvent.GetPersistentEventCount() > i)
				{
					@object = this.CurrentEvent.GetPersistentTarget(i);
				}
				else if (this._event2.GetPersistentEventCount() > i)
				{
					@object = this._event2.GetPersistentTarget(i);
				}
				if (@object is GameObject)
				{
					array[i] = @object.GetInstanceID();
				}
				else if (@object is Component)
				{
					array[i] = ((Component)@object).gameObject.GetInstanceID();
				}
			}
			this.Value.CurrentValue = currentValue;
			return array;
		}

		public void SetBool(bool value)
		{
			this.Value.TargetValue = (value ? this._eventOnValue : this._eventOffValue);
			this.Value.CurrentValue = this.Value.TargetValue;
		}

		[SerializeField]
		private NestInput.EventType _eventType = NestInput.EventType.Trigger;

		[SerializeField]
		private float _eventOffValue;

		[SerializeField]
		private float _eventOnValue = 1f;

		[SerializeField]
		private SerializableDictionaryBase<string, UnityEventBase> _events;

		[SerializeField]
		public UnityEvent _event;

		[SerializeField]
		private UnityEvent _event2;

		[SerializeField]
		public NestInput.ValueEvent _eventValue;

		[SerializeField]
		public float _parameterFloat;

		[SerializeField]
		public NestInput.StringEvent _eventString;

		[SerializeField]
		public string _parameterString;

		public bool FireOnce;

		[SerializeField]
		private FloatInterpolator.Config _interpolation;

		private bool _fired;

		public float Delay;

		protected FloatInterpolator Value;

		[Serializable]
		public class ValueEvent : UnityEvent<float>
		{
		}

		[Serializable]
		public class StringEvent : UnityEvent<string>
		{
		}

		[Flags]
		public enum EventType
		{
			Trigger = 1,
			Bool = 2,
			Float = 4,
			Toggle = 16,
			String = 32
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nest.Integrations
{
	[AddComponentMenu("Cast/Integrations/Threshold Trigger")]
	public class ThresholdTrigger : BaseIntegration
	{
		public float threshold
		{
			get
			{
				return this._threshold;
			}
			set
			{
				this._threshold = value;
			}
		}

		public float offDelay
		{
			get
			{
				return this._delayToOff;
			}
			set
			{
				this._delayToOff = value;
			}
		}

		public override float InputValue
		{
			set
			{
				this._currentValue = value;
			}
		}

		private void Update()
		{
			if (this._currentValue >= this._threshold)
			{
				if (this._currentState != ThresholdTrigger.State.Enabled)
				{
					this._onEvent.Invoke();
					this._currentState = ThresholdTrigger.State.Enabled;
				}
				this._delayTimer = 0f;
				return;
			}
			if (this._currentValue < this._threshold && this._currentState != ThresholdTrigger.State.Disabled)
			{
				this._delayTimer += Time.deltaTime;
				if (this._delayTimer >= this._delayToOff)
				{
					this._offEvent.Invoke();
					this._currentState = ThresholdTrigger.State.Disabled;
				}
			}
		}

		[SerializeField]
		private float _threshold = 0.01f;

		[SerializeField]
		private float _delayToOff;

		[SerializeField]
		private UnityEvent _onEvent;

		[SerializeField]
		private UnityEvent _offEvent;

		private ThresholdTrigger.State _currentState;

		private float _currentValue;

		private float _delayTimer;

		private enum State
		{
			Dormant,
			Enabled,
			Disabled
		}
	}
}

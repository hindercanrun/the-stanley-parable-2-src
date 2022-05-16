using System;
using UnityEngine;

namespace Nest.Util
{
	public struct FloatInterpolator
	{
		public FloatInterpolator.Config Configuration { get; set; }

		public float CurrentValue { get; set; }

		public float TargetValue { get; set; }

		public FloatInterpolator(float initialValue, float targetValue, FloatInterpolator.Config config)
		{
			this.Configuration = config;
			this.CurrentValue = initialValue;
			this.TargetValue = targetValue;
			this._velocity = 0f;
			this._timeElapsed = 0f;
		}

		public float Step(float targetValue)
		{
			this.TargetValue = targetValue;
			return this.Step();
		}

		public float Step()
		{
			switch (this.Configuration.Interpolation)
			{
			case FloatInterpolator.Config.InterpolationType.Linear:
				this.CurrentValue = Ease.Linear(this._timeElapsed, this.Configuration.InterpolationSpeed, 0.1f, 1f) * this.TargetValue;
				break;
			case FloatInterpolator.Config.InterpolationType.Sine:
				this.CurrentValue = Ease.InOutSine(this._timeElapsed, this.Configuration.InterpolationSpeed, 0.1f, 1f) * this.TargetValue;
				break;
			case FloatInterpolator.Config.InterpolationType.Quadratic:
				this.CurrentValue = Ease.InQuad(this._timeElapsed, this.Configuration.InterpolationSpeed, 0.1f, 1f) * this.TargetValue;
				break;
			case FloatInterpolator.Config.InterpolationType.Exponential:
				this.CurrentValue = Ease.InOutExpo(this._timeElapsed, this.Configuration.InterpolationSpeed, 0.1f, 1f) * this.TargetValue;
				break;
			case FloatInterpolator.Config.InterpolationType.DampedSpring:
				this.CurrentValue = Ease.DampenedSpring(this.CurrentValue, this.TargetValue, ref this._velocity, this.Configuration.InterpolationSpeed);
				break;
			case FloatInterpolator.Config.InterpolationType.AnimatedCurve:
				this.CurrentValue = this.Configuration.Curve.Evaluate(this._timeElapsed) * this.TargetValue;
				break;
			default:
				this.CurrentValue = this.TargetValue;
				break;
			}
			this._timeElapsed += Time.deltaTime;
			return this.CurrentValue;
		}

		private float _velocity;

		private float _timeElapsed;

		[Serializable]
		public class Config
		{
			public FloatInterpolator.Config.InterpolationType Interpolation
			{
				get
				{
					return this._interpolationType;
				}
				set
				{
					this._interpolationType = value;
				}
			}

			public AnimationCurve Curve
			{
				get
				{
					return this._curve;
				}
				set
				{
					this._curve = value;
				}
			}

			public bool Enabled
			{
				get
				{
					return this.Interpolation > FloatInterpolator.Config.InterpolationType.Instant;
				}
			}

			public float InterpolationSpeed
			{
				get
				{
					return this._interpolationSpeed;
				}
				set
				{
					this._interpolationSpeed = value;
				}
			}

			[SerializeField]
			private FloatInterpolator.Config.InterpolationType _interpolationType = FloatInterpolator.Config.InterpolationType.DampedSpring;

			[SerializeField]
			[Range(0.1f, 50f)]
			private float _interpolationSpeed = 5f;

			[SerializeField]
			private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

			public enum InterpolationType
			{
				Instant,
				Linear,
				Sine,
				Quadratic,
				Exponential,
				DampedSpring,
				AnimatedCurve
			}
		}
	}
}

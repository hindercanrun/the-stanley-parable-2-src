using System;
using UnityEngine;

namespace Nest.Integrations
{
	[AddComponentMenu("Cast/Integrations/Animation Value Passthrough")]
	public class AnimationValuePassthrough : BaseIntegration
	{
		public override float InputValue
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public void Start()
		{
			this._animator = base.GetComponent<Animator>();
			if (this.ParameterName == string.Empty)
			{
				return;
			}
			foreach (AnimatorControllerParameter animatorControllerParameter in this._animator.parameters)
			{
				if (animatorControllerParameter.name == this.ParameterName)
				{
					this._parameter = animatorControllerParameter.nameHash;
					this._parameterType = animatorControllerParameter.type;
					return;
				}
			}
		}

		public void Update()
		{
			if (this.ParameterName == string.Empty)
			{
				return;
			}
			AnimatorControllerParameterType parameterType = this._parameterType;
			switch (parameterType)
			{
			case AnimatorControllerParameterType.Float:
				this._animator.SetFloat(this._parameter, this.InputValue);
				return;
			case (AnimatorControllerParameterType)2:
				break;
			case AnimatorControllerParameterType.Int:
				this._animator.SetInteger(this._parameter, Mathf.FloorToInt(this.InputValue));
				return;
			case AnimatorControllerParameterType.Bool:
				this._animator.SetBool(this._parameter, this.InputValue > this.Threshold);
				return;
			default:
				if (parameterType == AnimatorControllerParameterType.Trigger)
				{
					if (this.InputValue > this.Threshold)
					{
						this._animator.SetTrigger(this._parameter);
						return;
					}
					return;
				}
				break;
			}
			throw new ArgumentOutOfRangeException();
		}

		private Animator _animator;

		[SerializeField]
		private float _value;

		private int _parameter = -1;

		private AnimatorControllerParameterType _parameterType;

		public string ParameterName;

		[Tooltip("Used For Bool Parameters")]
		public float Threshold;
	}
}

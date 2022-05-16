using System;
using UnityEngine;

namespace Nest.Integrations
{
	[AddComponentMenu("Cast/Integrations/Animation Scrubbing")]
	public class AnimationScrubbing : BaseIntegration
	{
		public override float InputValue
		{
			set
			{
				this._value = value;
			}
		}

		private void Start()
		{
			this._animator = base.GetComponent<Animator>();
		}

		private void Update()
		{
			this.PlayAnimator();
		}

		public void PlayAnimator()
		{
			if (Math.Abs(this._animator.speed) < 0.01f)
			{
				this._animator.Play(this._animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, this._value);
			}
		}

		private Animator _animator;

		private float _value;
	}
}

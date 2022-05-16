using System;
using UnityEngine;

namespace Nest.Integrations
{
	public class MirrorLocalTransform : BaseIntegration
	{
		public override float InputValue
		{
			get
			{
				return base.InputValue;
			}
			set
			{
				base.InputValue = value;
			}
		}

		public void Update()
		{
			this.MirroredTransform.localPosition = base.transform.localPosition;
			this.MirroredTransform.localRotation = base.transform.localRotation;
		}

		public Transform MirroredTransform;
	}
}

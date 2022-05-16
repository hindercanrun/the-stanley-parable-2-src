using System;
using UnityEngine;

namespace Ferr
{
	public class ClampAttribute : PropertyAttribute
	{
		public ClampAttribute(float aMin, float aMax)
		{
			this.mMin = aMin;
			this.mMax = aMax;
		}

		public float mMin;

		public float mMax;
	}
}

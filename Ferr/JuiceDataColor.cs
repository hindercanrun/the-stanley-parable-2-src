using System;
using UnityEngine;

namespace Ferr
{
	[Serializable]
	public class JuiceDataColor
	{
		public bool Update()
		{
			if (this.renderer == null)
			{
				return true;
			}
			float num = Mathf.Min((Time.time - this.startTime) / this.duration, 1f);
			Color color = Color.Lerp(this.start, this.end, this.curve.Evaluate(num));
			this.renderer.color = color;
			return num >= 1f;
		}

		public void Cancel()
		{
			this.startTime = -10000f;
			this.Update();
		}

		public Material renderer;

		public Color start;

		public Color end;

		public float duration;

		public float startTime;

		public AnimationCurve curve;

		public Action callback;
	}
}

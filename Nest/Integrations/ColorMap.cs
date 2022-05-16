using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nest.Integrations
{
	[AddComponentMenu("Cast/Integrations/Color Map")]
	public class ColorMap : BaseIntegration
	{
		public override float InputValue
		{
			set
			{
				if (this._colorMode == ColorMap.ColorMode.Gradient)
				{
					this._colorEvent.Invoke(this._gradient.Evaluate(value));
					return;
				}
				int num = this._colorArray.Length;
				int num2 = Mathf.FloorToInt(value * (float)(num - 1));
				num2 = Mathf.Clamp(num2, 0, num - 2);
				float t = value * (float)(num - 1) - (float)num2;
				Color arg = Color.Lerp(this._colorArray[num2], this._colorArray[num2 + 1], t);
				this._colorEvent.Invoke(arg);
			}
		}

		[SerializeField]
		private ColorMap.ColorMode _colorMode;

		[SerializeField]
		private Gradient _gradient = new Gradient();

		[SerializeField]
		[ColorUsage(true, true, 0f, 16f, 0.125f, 3f)]
		private Color[] _colorArray = new Color[]
		{
			Color.black,
			Color.white
		};

		[SerializeField]
		private ColorMap.ColorEvent _colorEvent;

		public enum ColorMode
		{
			Gradient,
			ColorArray
		}

		[Serializable]
		public class ColorEvent : UnityEvent<Color>
		{
		}
	}
}

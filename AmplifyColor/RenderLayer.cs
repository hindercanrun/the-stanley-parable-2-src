using System;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public struct RenderLayer
	{
		public RenderLayer(LayerMask mask, Color color)
		{
			this.mask = mask;
			this.color = color;
		}

		public LayerMask mask;

		public Color color;
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(RectTransform))]
	public class PaintedMask : UIBehaviour
	{
		protected override void Start()
		{
			base.Start();
			this._renderTexture = new RenderTexture((int)this.maskSize.x, (int)this.maskSize.y, 0, RenderTextureFormat.ARGB32);
			this._renderTexture.Create();
			this.renderCamera.targetTexture = this._renderTexture;
			this.targetMask.renderTexture = this._renderTexture;
		}

		private Vector2 maskSize
		{
			get
			{
				return ((RectTransform)this.targetMask.transform).rect.size;
			}
		}

		public Camera renderCamera;

		public SoftMask targetMask;

		private RenderTexture _renderTexture;
	}
}

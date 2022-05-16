using System;
using UnityEngine;

namespace SoftMasking.Samples
{
	public class Minimap : MonoBehaviour
	{
		public void LateUpdate()
		{
			this.map.anchoredPosition = -this.marker.anchoredPosition * this._zoom;
		}

		public void ZoomIn()
		{
			this._zoom = this.Clamp(this._zoom + this.zoomStep);
			this.map.localScale = Vector3.one * this._zoom;
		}

		public void ZoomOut()
		{
			this._zoom = this.Clamp(this._zoom - this.zoomStep);
			this.map.localScale = Vector3.one * this._zoom;
		}

		private float Clamp(float zoom)
		{
			return Mathf.Clamp(zoom, this.minZoom, this.maxZoom);
		}

		public RectTransform map;

		public RectTransform marker;

		[Space]
		public float minZoom = 0.8f;

		public float maxZoom = 1.4f;

		public float zoomStep = 0.2f;

		private float _zoom = 1f;
	}
}

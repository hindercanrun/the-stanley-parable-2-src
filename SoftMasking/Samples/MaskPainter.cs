using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(RectTransform))]
	public class MaskPainter : UIBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
	{
		protected override void Awake()
		{
			base.Awake();
			this._rectTransform = base.GetComponent<RectTransform>();
		}

		protected override void Start()
		{
			base.Start();
			this.spot.enabled = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.UpdateStrokeByEvent(eventData, false);
		}

		public void OnDrag(PointerEventData eventData)
		{
			this.UpdateStrokeByEvent(eventData, true);
		}

		private void UpdateStrokeByEvent(PointerEventData eventData, bool drawTrail = false)
		{
			this.UpdateStrokePosition(eventData.position, drawTrail);
			this.UpdateStrokeColor(eventData.button);
		}

		private void UpdateStrokePosition(Vector2 screenPosition, bool drawTrail = false)
		{
			Vector2 anchoredPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rectTransform, screenPosition, null, out anchoredPosition))
			{
				Vector2 anchoredPosition2 = this.stroke.anchoredPosition;
				this.stroke.anchoredPosition = anchoredPosition;
				if (drawTrail)
				{
					this.SetUpTrail(anchoredPosition2);
				}
				this.spot.enabled = true;
			}
		}

		private void SetUpTrail(Vector2 prevPosition)
		{
			Vector2 vector = this.stroke.anchoredPosition - prevPosition;
			this.stroke.localRotation = Quaternion.AngleAxis(Mathf.Atan2(vector.y, vector.x) * 57.29578f, Vector3.forward);
			this.spot.rectTransform.offsetMin = new Vector2(-vector.magnitude, 0f);
		}

		private void UpdateStrokeColor(PointerEventData.InputButton pressedButton)
		{
			this.spot.materialForRendering.SetInt("_BlendOp", (pressedButton == PointerEventData.InputButton.Left) ? 0 : 2);
		}

		public Graphic spot;

		public RectTransform stroke;

		private RectTransform _rectTransform;
	}
}

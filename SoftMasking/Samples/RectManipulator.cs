using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(RectTransform))]
	public class RectManipulator : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.HighlightIcon(true, false);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!this._isManipulatedNow)
			{
				this.HighlightIcon(false, false);
			}
		}

		private void HighlightIcon(bool highlight, bool instant = false)
		{
			if (this.icon)
			{
				float alpha = highlight ? this.selectedAlpha : this.normalAlpha;
				float duration = instant ? 0f : this.transitionDuration;
				this.icon.CrossFadeAlpha(alpha, duration, true);
			}
			if (this.showOnHover)
			{
				this.showOnHover.forcedVisible = highlight;
			}
		}

		protected override void Start()
		{
			base.Start();
			this.HighlightIcon(false, true);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			this._isManipulatedNow = true;
			this.RememberStartTransform();
		}

		private void RememberStartTransform()
		{
			if (this.targetTransform)
			{
				this._startAnchoredPosition = this.targetTransform.anchoredPosition;
				this._startSizeDelta = this.targetTransform.sizeDelta;
				this._startRotation = this.targetTransform.localRotation.eulerAngles.z;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (this.targetTransform == null || this.parentTransform == null || !this._isManipulatedNow)
			{
				return;
			}
			Vector2 vector = this.ToParentSpace(eventData.pressPosition, eventData.pressEventCamera);
			Vector2 vector2 = this.ToParentSpace(eventData.position, eventData.pressEventCamera);
			this.DoRotate(vector, vector2);
			Vector2 parentSpaceMovement = vector2 - vector;
			this.DoMove(parentSpaceMovement);
			this.DoResize(parentSpaceMovement);
		}

		private Vector2 ToParentSpace(Vector2 position, Camera eventCamera)
		{
			Vector2 result;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentTransform, position, eventCamera, out result);
			return result;
		}

		private RectTransform parentTransform
		{
			get
			{
				return this.targetTransform.parent as RectTransform;
			}
		}

		private void DoMove(Vector2 parentSpaceMovement)
		{
			if (this.Is(RectManipulator.ManipulationType.Move))
			{
				this.MoveTo(this._startAnchoredPosition + parentSpaceMovement);
			}
		}

		private bool Is(RectManipulator.ManipulationType expected)
		{
			return (this.manipulation & expected) == expected;
		}

		private void MoveTo(Vector2 desiredAnchoredPosition)
		{
			this.targetTransform.anchoredPosition = this.ClampPosition(desiredAnchoredPosition);
		}

		private Vector2 ClampPosition(Vector2 position)
		{
			Vector2 vector = this.parentTransform.rect.size / 2f;
			return new Vector2(Mathf.Clamp(position.x, -vector.x, vector.x), Mathf.Clamp(position.y, -vector.y, vector.y));
		}

		private void DoRotate(Vector2 startParentPoint, Vector2 targetParentPoint)
		{
			if (this.Is(RectManipulator.ManipulationType.Rotate))
			{
				Vector2 startLever = startParentPoint - this.targetTransform.localPosition;
				Vector2 endLever = targetParentPoint - this.targetTransform.localPosition;
				float num = this.DeltaRotation(startLever, endLever);
				this.targetTransform.localRotation = Quaternion.AngleAxis(this._startRotation + num, Vector3.forward);
			}
		}

		private float DeltaRotation(Vector2 startLever, Vector2 endLever)
		{
			float current = Mathf.Atan2(startLever.y, startLever.x) * 57.29578f;
			float target = Mathf.Atan2(endLever.y, endLever.x) * 57.29578f;
			return Mathf.DeltaAngle(current, target);
		}

		private void DoResize(Vector2 parentSpaceMovement)
		{
			Vector3 v = Quaternion.Inverse(this.targetTransform.rotation) * parentSpaceMovement;
			Vector2 localOffset = this.ProjectResizeOffset(v);
			if (localOffset.sqrMagnitude > 0f)
			{
				this.SetSizeDirected(localOffset, this.ResizeSign());
			}
		}

		private Vector2 ProjectResizeOffset(Vector2 localOffset)
		{
			bool flag = this.Is(RectManipulator.ManipulationType.ResizeLeft) || this.Is(RectManipulator.ManipulationType.ResizeRight);
			bool flag2 = this.Is(RectManipulator.ManipulationType.ResizeUp) || this.Is(RectManipulator.ManipulationType.ResizeDown);
			return new Vector2(flag ? localOffset.x : 0f, flag2 ? localOffset.y : 0f);
		}

		private Vector2 ResizeSign()
		{
			return new Vector2(this.Is(RectManipulator.ManipulationType.ResizeLeft) ? -1f : 1f, this.Is(RectManipulator.ManipulationType.ResizeDown) ? -1f : 1f);
		}

		private void SetSizeDirected(Vector2 localOffset, Vector2 sizeSign)
		{
			Vector2 vector = this.ClampSize(this._startSizeDelta + Vector2.Scale(localOffset, sizeSign));
			this.targetTransform.sizeDelta = vector;
			Vector2 v = Vector2.Scale((vector - this._startSizeDelta) / 2f, sizeSign);
			this.MoveTo(this._startAnchoredPosition + this.targetTransform.TransformVector(v));
		}

		private Vector2 ClampSize(Vector2 size)
		{
			return new Vector2(Mathf.Max(size.x, this.minSize.x), Mathf.Max(size.y, this.minSize.y));
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			this._isManipulatedNow = false;
			if (!eventData.hovered.Contains(base.gameObject))
			{
				this.HighlightIcon(false, false);
			}
		}

		public RectTransform targetTransform;

		public RectManipulator.ManipulationType manipulation;

		public ShowOnHover showOnHover;

		[Header("Limits")]
		public Vector2 minSize;

		[Header("Display")]
		public Graphic icon;

		public float normalAlpha = 0.2f;

		public float selectedAlpha = 1f;

		public float transitionDuration = 0.2f;

		private bool _isManipulatedNow;

		private Vector2 _startAnchoredPosition;

		private Vector2 _startSizeDelta;

		private float _startRotation;

		[Flags]
		public enum ManipulationType
		{
			None = 0,
			Move = 1,
			ResizeLeft = 2,
			ResizeUp = 4,
			ResizeRight = 8,
			ResizeDown = 16,
			ResizeUpLeft = 6,
			ResizeUpRight = 12,
			ResizeDownLeft = 18,
			ResizeDownRight = 24,
			Rotate = 32
		}
	}
}

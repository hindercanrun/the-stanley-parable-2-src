using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(RectTransform))]
	public class ShowOnHover : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public bool forcedVisible
		{
			get
			{
				return this._forcedVisible;
			}
			set
			{
				if (this._forcedVisible != value)
				{
					this._forcedVisible = value;
					this.UpdateVisibility();
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			this.UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			this.SetVisible(this.ShouldBeVisible());
		}

		private bool ShouldBeVisible()
		{
			return this._forcedVisible || this._isPointerOver;
		}

		private void SetVisible(bool visible)
		{
			if (this.targetGroup)
			{
				this.targetGroup.alpha = (visible ? 1f : 0f);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			this._isPointerOver = true;
			this.UpdateVisibility();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			this._isPointerOver = false;
			this.UpdateVisibility();
		}

		public CanvasGroup targetGroup;

		private bool _forcedVisible;

		private bool _isPointerOver;
	}
}

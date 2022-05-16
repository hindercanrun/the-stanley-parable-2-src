using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoftMasking.Samples
{
	[RequireComponent(typeof(RectTransform))]
	public class Draggable : UIBehaviour, IDragHandler, IEventSystemHandler
	{
		protected override void Awake()
		{
			base.Awake();
			this._rectTransform = base.GetComponent<RectTransform>();
		}

		public void OnDrag(PointerEventData eventData)
		{
			this._rectTransform.anchoredPosition += eventData.delta;
		}

		private RectTransform _rectTransform;
	}
}

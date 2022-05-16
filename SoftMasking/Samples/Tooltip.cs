using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoftMasking.Samples
{
	public class Tooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public void LateUpdate()
		{
			Vector2 a;
			if (this.tooltip.gameObject.activeInHierarchy && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.tooltip.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out a))
			{
				this.tooltip.anchoredPosition = a + new Vector2(10f, -20f);
			}
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.tooltip.gameObject.SetActive(true);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.tooltip.gameObject.SetActive(false);
		}

		public RectTransform tooltip;
	}
}

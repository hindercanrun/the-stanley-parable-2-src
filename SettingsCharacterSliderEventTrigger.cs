using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SettingsCharacterSliderEventTrigger : MonoBehaviour, ISelectHandler, IEventSystemHandler, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.dontInvokeOnPointerEnterWhileDragging && this.dragging)
		{
			return;
		}
		SettingsCharacterSliderEventTrigger.BaseEventDataEvent baseEventDataEvent = this.pointerEnter;
		if (baseEventDataEvent == null)
		{
			return;
		}
		baseEventDataEvent.Invoke(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		this.dragging = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.dragging = false;
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (this.dontInvokeOnSelectWhileDragging && this.dragging)
		{
			return;
		}
		SettingsCharacterSliderEventTrigger.BaseEventDataEvent baseEventDataEvent = this.onSelect;
		if (baseEventDataEvent == null)
		{
			return;
		}
		baseEventDataEvent.Invoke(eventData);
	}

	[SerializeField]
	private SettingsCharacterSliderEventTrigger.BaseEventDataEvent pointerEnter;

	[SerializeField]
	private bool dontInvokeOnPointerEnterWhileDragging = true;

	[Space(30f)]
	[SerializeField]
	private SettingsCharacterSliderEventTrigger.BaseEventDataEvent onSelect;

	[SerializeField]
	private bool dontInvokeOnSelectWhileDragging = true;

	private bool dragging;

	[Serializable]
	public class BaseEventDataEvent : UnityEvent<BaseEventData>
	{
	}
}

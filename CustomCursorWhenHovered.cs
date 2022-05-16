using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomCursorWhenHovered : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.cursor.SetCursorToThis();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.cursor.ResetCursor();
	}

	public CursorProfile cursor;
}

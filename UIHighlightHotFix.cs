using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHighlightHotFix : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IDeselectHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!EventSystem.current.alreadySelecting && Singleton<GameMaster>.Instance.MouseMoved)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
			return;
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		base.GetComponent<Selectable>().OnPointerExit(null);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!EventSystem.current.alreadySelecting && Singleton<GameMaster>.Instance.MouseMoved)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}

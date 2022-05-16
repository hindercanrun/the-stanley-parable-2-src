using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnSubmit : MonoBehaviour, ISubmitHandler, IEventSystemHandler
{
	public void OnSubmit(BaseEventData eventData)
	{
		if (this.targetSelectable != null)
		{
			this.targetSelectable.Select();
		}
	}

	[SerializeField]
	private Selectable targetSelectable;
}

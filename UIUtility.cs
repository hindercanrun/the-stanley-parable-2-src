using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUtility : MonoBehaviour
{
	public void SelectSelectable(Selectable selectable)
	{
		base.StartCoroutine(UIUtility.Select(selectable));
	}

	public void Deselect()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	public static IEnumerator Select(Selectable selectable)
	{
		yield return null;
		EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
		new PointerEventData(eventSystem);
		if (eventSystem == null)
		{
			yield break;
		}
		eventSystem.SetSelectedGameObject(null);
		eventSystem.SetSelectedGameObject(selectable.gameObject);
		yield return null;
		if (selectable is Toggle)
		{
			(selectable as Toggle).isOn = true;
		}
		selectable.OnSelect(null);
		yield break;
	}

	public void OnMoveTest(AxisEventData baseData)
	{
	}

	public void OnMove(BaseEventData baseData)
	{
		Configurator component = base.GetComponent<Configurator>();
		if (component != null)
		{
			AxisEventData axisEventData = baseData as AxisEventData;
			if (axisEventData != null)
			{
				MoveDirection moveDir = axisEventData.moveDir;
				if (moveDir == MoveDirection.Left)
				{
					component.DecreaseValue();
					return;
				}
				if (moveDir != MoveDirection.Right)
				{
					return;
				}
				component.IncreaseValue();
				return;
			}
			else
			{
				Debug.Log("Not a move input!");
			}
		}
	}

	public void OnSubmit(BaseEventData baseData)
	{
		Configurator component = base.GetComponent<Configurator>();
		if (component != null)
		{
			component.IncreaseValue();
		}
	}
}

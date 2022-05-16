using System;
using StanleyUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class StanleyMenuTools
{
	public static void StanleyMenuOnPointerEnter(Action<PointerEventData> baseFunction, PointerEventData eventData)
	{
		if (Singleton<GameMaster>.Instance.MouseMoved && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
		{
			baseFunction(eventData);
		}
	}

	public static void StanleyMenuSelectableOnSelect(Selectable selectable, BaseEventData eventData)
	{
		StanleyInputModuleAssistant.RegisterUIElementSelection(selectable);
		if (!Singleton<GameMaster>.Instance.MouseMoved && !GameMaster.CursorVisible && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
		{
			StanleyMenuTools.SnapToInScrollRect(selectable.GetComponent<RectTransform>());
		}
	}

	public static void SnapToInScrollRect(RectTransform target)
	{
		ScrollRect componentInParent = target.GetComponentInParent<ScrollRect>();
		if (componentInParent == null)
		{
			return;
		}
		RectTransform content = componentInParent.content;
		if (content == null)
		{
			return;
		}
		Canvas.ForceUpdateCanvases();
		Vector2 a = componentInParent.transform.InverseTransformPoint(content.position);
		ref Vector2 ptr = componentInParent.transform.InverseTransformVector(content.transform.TransformVector(content.sizeDelta));
		Vector2 b = componentInParent.transform.InverseTransformPoint(target.position);
		Vector2 sizeDelta = componentInParent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = 0f;
		float num = (a - b - sizeDelta / 2f).y;
		float max = ptr.y - sizeDelta.y;
		num = Mathf.Clamp(num, 0f, max);
		content.anchoredPosition = new Vector2(0f, num);
	}

	public static Selectable GetPrevActiveSiblingSelectable(Transform trans, params Type[] ignoreList)
	{
		Transform prevSibling = StanleyMenuTools.GetPrevSibling(trans);
		while (!StanleyMenuTools.IsValidSelectable(prevSibling, ignoreList))
		{
			prevSibling = StanleyMenuTools.GetPrevSibling(prevSibling);
		}
		if (!(prevSibling == null))
		{
			return prevSibling.GetComponent<Selectable>();
		}
		return null;
	}

	public static Selectable GetNextActiveSiblingSelectable(Transform trans, params Type[] ignoreList)
	{
		Transform nextSibling = StanleyMenuTools.GetNextSibling(trans);
		while (!StanleyMenuTools.IsValidSelectable(nextSibling, ignoreList))
		{
			nextSibling = StanleyMenuTools.GetNextSibling(nextSibling);
		}
		if (!(nextSibling == null))
		{
			return nextSibling.GetComponent<Selectable>();
		}
		return null;
	}

	public static bool IsValidSelectable(Transform t, params Type[] ignoreList)
	{
		if (!(t != null))
		{
			return true;
		}
		Selectable component = t.GetComponent<Selectable>();
		bool flag = component != null;
		bool flag2 = false;
		if (component != null)
		{
			Type type = component.GetType();
			flag2 = (Array.FindIndex<Type>(ignoreList, (Type x) => type == x) == -1);
		}
		return flag2 && t.gameObject.activeSelf && flag;
	}

	public static UIBehaviour GetSiblingThatIsActive(this UIBehaviour ui, int direction)
	{
		UIBehaviour uibehaviour = ui;
		do
		{
			uibehaviour = uibehaviour.GetSibling(direction);
		}
		while (!(uibehaviour == null) && !uibehaviour.gameObject.activeSelf);
		return uibehaviour;
	}

	public static UIBehaviour GetSibling(this UIBehaviour ui, int direction)
	{
		if (direction == 0)
		{
			return ui;
		}
		if (direction > 0)
		{
			Transform nextSibling = StanleyMenuTools.GetNextSibling(ui.transform);
			if (nextSibling == null)
			{
				return null;
			}
			return nextSibling.GetComponent<UIBehaviour>();
		}
		else
		{
			Transform prevSibling = StanleyMenuTools.GetPrevSibling(ui.transform);
			if (prevSibling == null)
			{
				return null;
			}
			return prevSibling.GetComponent<UIBehaviour>();
		}
	}

	public static UIBehaviour GetPrevSibling(this UIBehaviour ui)
	{
		Transform prevSibling = StanleyMenuTools.GetPrevSibling(ui.transform);
		if (prevSibling == null)
		{
			return null;
		}
		return prevSibling.GetComponent<UIBehaviour>();
	}

	public static UIBehaviour GetNextSibling(this UIBehaviour ui)
	{
		Transform nextSibling = StanleyMenuTools.GetNextSibling(ui.transform);
		if (nextSibling == null)
		{
			return null;
		}
		return nextSibling.GetComponent<UIBehaviour>();
	}

	public static Transform GetPrevSibling(Transform trans)
	{
		int siblingIndex = trans.GetSiblingIndex();
		if (siblingIndex <= 0)
		{
			return null;
		}
		return trans.parent.GetChild(siblingIndex - 1);
	}

	public static Transform GetNextSibling(Transform trans)
	{
		int siblingIndex = trans.GetSiblingIndex();
		if (siblingIndex >= trans.parent.childCount - 1)
		{
			return null;
		}
		return trans.parent.GetChild(siblingIndex + 1);
	}
}

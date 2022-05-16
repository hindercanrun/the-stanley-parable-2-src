using System;
using UnityEngine;
using UnityEngine.UI;

namespace StanleyUI
{
	public static class ISelectableHolderScreenExtensions
	{
		public static Selectable GetDefaultSelectableOrFirstActiveSibling(this ISelectableHolderScreen s)
		{
			if (!s.DefaultSelectable.gameObject.activeSelf)
			{
				for (int i = 0; i < s.DefaultSelectable.transform.parent.childCount; i++)
				{
					Selectable component = s.DefaultSelectable.transform.parent.GetChild(i).GetComponent<Selectable>();
					if (component != null && component.gameObject.activeSelf)
					{
						return component;
					}
				}
			}
			return s.DefaultSelectable;
		}

		public static GameObject DefaultGameObjectOrNull(this ISelectableHolderScreen s)
		{
			if (!(s.DefaultSelectable == null))
			{
				return s.GetDefaultSelectableOrFirstActiveSibling().gameObject;
			}
			return null;
		}

		public static GameObject LastGameObjectOrNull(this ISelectableHolderScreen s)
		{
			if (!(s.LastSelectable == null))
			{
				return s.LastSelectable.gameObject;
			}
			return null;
		}
	}
}

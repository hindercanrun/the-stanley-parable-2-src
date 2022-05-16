using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StanleyUI
{
	public class StanleyMenuCreditsButton : Button
	{
		public override void OnPointerEnter(PointerEventData eventData)
		{
			StanleyMenuTools.StanleyMenuOnPointerEnter(new Action<PointerEventData>(base.OnPointerEnter), eventData);
		}

		public override void OnSelect(BaseEventData eventData)
		{
			StanleyMenuTools.StanleyMenuSelectableOnSelect(this, eventData);
			base.OnSelect(eventData);
		}

		public void OpenWebPageOnPC(string url)
		{
			if (PlatformSettings.Instance.isStandalone.GetBooleanValue())
			{
				Application.OpenURL(url);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StanleyUI
{
	[RequireComponent(typeof(ToggleGroup))]
	public class BumperToggleGroupSwitcher : MonoBehaviour, IScreenRegisterNotificationReciever
	{
		private void Update()
		{
			if (Singleton<GameMaster>.Instance == null)
			{
				return;
			}
			if (Singleton<GameMaster>.Instance.MenuManager && (this.onlyTabSwitchWhenScreenVisible == null || this.onlyTabSwitchWhenScreenVisible.active))
			{
				if (Singleton<GameMaster>.Instance.stanleyActions.MenuTabLeft.WasPressed)
				{
					this.GotoNextTab(-1);
				}
				if (Singleton<GameMaster>.Instance.stanleyActions.MenuTabRight.WasPressed)
				{
					this.GotoNextTab(1);
				}
			}
		}

		private Toggle GetActiveToggle()
		{
			using (IEnumerator<Toggle> enumerator = base.GetComponent<ToggleGroup>().ActiveToggles().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		public void RegisteredScreenVisible()
		{
			this.GotoNextTab(0);
		}

		private void GotoNextTab(int dir)
		{
			UIBehaviour siblingThatIsActive = this.GetActiveToggle().GetSiblingThatIsActive(dir);
			Toggle toggle = (siblingThatIsActive != null) ? siblingThatIsActive.GetComponent<Toggle>() : null;
			if (toggle != null)
			{
				toggle.isOn = true;
				if (toggle.GetComponent<ISelectableHolderScreen>() != null)
				{
					StanleyInputModuleAssistant.RegisterScreenAsNewlyVisible(toggle.gameObject, false);
				}
			}
		}

		public UIScreen onlyTabSwitchWhenScreenVisible;
	}
}

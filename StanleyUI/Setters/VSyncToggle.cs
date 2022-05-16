using System;
using UnityEngine;

namespace StanleyUI.Setters
{
	public class VSyncToggle : MonoBehaviour, ISettingsBoolListener
	{
		public void SetValue(bool on)
		{
			this.SetVsyncToggle(on);
		}

		public void SetVsyncToggle(bool status)
		{
			QualitySettings.vSyncCount = (status ? 1 : 0);
		}
	}
}

using System;
using StanleyUI;
using UnityEngine;

public class QualitySelector : MonoBehaviour, ISettingsIntListener
{
	private string IndexToQualitySettingString(int index)
	{
		switch (index)
		{
		case 0:
			return "Low";
		case 1:
			return "Medium";
		case 2:
			return "High";
		default:
			return "None";
		}
	}

	public void PrintQualityLocalizationTag(int index)
	{
		StringValueChangedEvent onPrintQualityLocalizationTag = this.OnPrintQualityLocalizationTag;
		if (onPrintQualityLocalizationTag == null)
		{
			return;
		}
		onPrintQualityLocalizationTag.Invoke("Menu_Quality_" + this.IndexToQualitySettingString(index));
	}

	public void SetValue(int val)
	{
		QualitySettings.SetQualityLevel(val);
		this.antiAliasingConfigurable.ForceUpdate();
		this.vSyncConfigurable.ForceUpdate();
	}

	[SerializeField]
	private StringValueChangedEvent OnPrintQualityLocalizationTag;

	[Header("Other video options that need to be force updated")]
	[SerializeField]
	private IntConfigurable antiAliasingConfigurable;

	[SerializeField]
	private BooleanConfigurable vSyncConfigurable;
}

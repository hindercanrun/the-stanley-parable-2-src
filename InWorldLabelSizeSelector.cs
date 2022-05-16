using System;
using UnityEngine;

public class InWorldLabelSizeSelector : MonoBehaviour
{
	public void PrintLocalizationTag(int index)
	{
		StringValueChangedEvent onPrintLocalizationTag = this.OnPrintLocalizationTag;
		if (onPrintLocalizationTag == null)
		{
			return;
		}
		onPrintLocalizationTag.Invoke(InWorldLabelManager.Instance.sizeProfiles[index].i2LocalizationTerm);
	}

	public void PrintFontSize(int index)
	{
		FloatValueChangedEvent onPrintFontSizeValue = this.OnPrintFontSizeValue;
		if (onPrintFontSizeValue == null)
		{
			return;
		}
		onPrintFontSizeValue.Invoke(InWorldLabelManager.Instance.sizeProfiles[index].fontSize);
	}

	[SerializeField]
	private StringValueChangedEvent OnPrintLocalizationTag;

	[SerializeField]
	private FloatValueChangedEvent OnPrintFontSizeValue;
}

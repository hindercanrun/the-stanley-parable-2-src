using System;
using UnityEngine;

public class SubtitleSizeSelector : MonoBehaviour
{
	private SubtitleSizeProfile[] Profiles
	{
		get
		{
			return this.profileData.sizeProfiles;
		}
	}

	public void PrintLocalizationTag(int index)
	{
		StringValueChangedEvent onPrintLocalizationTag = this.OnPrintLocalizationTag;
		if (onPrintLocalizationTag == null)
		{
			return;
		}
		onPrintLocalizationTag.Invoke(this.Profiles[index].DescriptionLocalizationKey);
	}

	public void PrintUIHeightReferenceValue(int index)
	{
		FloatValueChangedEvent onPrintUIHeightReferenceValue = this.OnPrintUIHeightReferenceValue;
		if (onPrintUIHeightReferenceValue == null)
		{
			return;
		}
		onPrintUIHeightReferenceValue.Invoke(this.Profiles[index].uiReferenceHeight);
	}

	[Header("Profiles")]
	[SerializeField]
	private SubtitleSizeProfileData profileData;

	[SerializeField]
	private StringValueChangedEvent OnPrintLocalizationTag;

	[SerializeField]
	private FloatValueChangedEvent OnPrintUIHeightReferenceValue;
}

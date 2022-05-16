using System;
using UnityEngine;

public class SubtitleSelector : MonoBehaviour
{
	private SubtitleProfile[] Profiles
	{
		get
		{
			return this.profileData.profiles;
		}
	}

	public void PrintLanguageTag(int index)
	{
		StringValueChangedEvent onPrintLanguageTag = this.OnPrintLanguageTag;
		if (onPrintLanguageTag == null)
		{
			return;
		}
		onPrintLanguageTag.Invoke(this.Profiles[index].DescriptionKey);
	}

	public void PrintFontSize(int index)
	{
		FloatValueChangedEvent onPrintFontSize = this.OnPrintFontSize;
		if (onPrintFontSize == null)
		{
			return;
		}
		onPrintFontSize.Invoke(this.Profiles[index].FontSize);
	}

	[Header("Profiles")]
	[SerializeField]
	private LanguageProfileData profileData;

	[SerializeField]
	private StringValueChangedEvent OnPrintLanguageTag;

	[SerializeField]
	private FloatValueChangedEvent OnPrintFontSize;
}

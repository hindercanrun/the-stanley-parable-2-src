using System;
using UnityEngine;

public class SubtitleConfigurator : Configurator
{
	private SubtitleProfile[] Profiles
	{
		get
		{
			return this.profileData.profiles;
		}
	}

	private new void Start()
	{
		base.Start();
		int num = this.profileData.profiles.Length;
	}

	public override void ApplyData()
	{
		int intValue = this.configurable.GetIntValue();
		if (intValue < 0 || intValue >= this.Profiles.Length)
		{
			return;
		}
		this.PrintFontSize(this.Profiles[intValue]);
	}

	public override void PrintValue(Configurable _configurable)
	{
		this.OnPrintValue.Invoke(this.Profiles[_configurable.GetIntValue()].DescriptionKey);
	}

	public void PrintFontSize(SubtitleProfile profile)
	{
		this.OnPrintFontSize.Invoke(profile.FontSize);
	}

	[Header("Profiles")]
	[SerializeField]
	private LanguageProfileData profileData;

	[SerializeField]
	private FloatValueChangedEvent OnPrintFontSize;
}

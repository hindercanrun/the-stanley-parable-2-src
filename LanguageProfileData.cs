using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageProfileData.asset", menuName = "Data/Language Profile Data")]
public class LanguageProfileData : ScriptableObject
{
	[Header("Profiles")]
	public SubtitleProfile[] profiles;
}

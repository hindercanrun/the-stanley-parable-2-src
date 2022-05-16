using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New SubtitleSizeProfile", menuName = "Stanley/Subtitle Size Profile")]
public class SubtitleSizeProfile : ScriptableObject
{
	public float uiReferenceHeight = 1080f;

	[Header("The English name of the language...")]
	public string Description = "";

	[Header("The tag in i2Loc")]
	public string DescriptionLocalizationKey = "";
}

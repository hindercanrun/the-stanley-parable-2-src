using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New SubtitleProfile", menuName = "Stanley/Subtitle Profile")]
public class SubtitleProfile : ScriptableObject
{
	public float FontSize = 30f;

	public float TextboxWidth = 2000f;

	public string DescriptionKey = "";

	[Header("The English name of the language...")]
	public string DescriptionIni2Loc = "";
}

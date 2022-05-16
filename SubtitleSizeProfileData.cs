using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SubtitleSizeProfileData.asset", menuName = "Data/Subtitle Size Profile Data")]
public class SubtitleSizeProfileData : ScriptableObject
{
	[Header("Profiles")]
	public SubtitleSizeProfile[] sizeProfiles;
}

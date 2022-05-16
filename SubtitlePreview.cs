using System;
using TMPro;
using UnityEngine;

public class SubtitlePreview : MonoBehaviour
{
	private void Start()
	{
		IntConfigurable intConfigurable = this.subtitleIndex;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.SubtitleSizeChange));
		this.SubtitleSizeChange(null);
	}

	private void SubtitleSizeChange(LiveData liveData)
	{
		SubtitleSizeProfile subtitleSizeProfile = this.subtitleSizeProfiles.sizeProfiles[this.subtitleIndex.GetIntValue()];
		this.subtitelPreviewText.fontSize = this.defaultFontSize / (subtitleSizeProfile.uiReferenceHeight / 1080f);
	}

	[SerializeField]
	private TMP_Text subtitelPreviewText;

	[SerializeField]
	private float defaultFontSize = 30f;

	[SerializeField]
	private SubtitleSizeProfileData subtitleSizeProfiles;

	[SerializeField]
	private IntConfigurable subtitleIndex;
}

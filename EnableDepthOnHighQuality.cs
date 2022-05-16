using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EnableDepthOnHighQuality : MonoBehaviour
{
	private void Awake()
	{
		IntConfigurable intConfigurable = this.qualitySettingsConfigurable;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnQualityLevelChange));
		QualitySettings.SetQualityLevel(this.qualitySettingsConfigurable.GetIntValue());
		this.OnQualityLevelChange(null);
	}

	private void OnQualityLevelChange(LiveData ld)
	{
		Camera component = base.GetComponent<Camera>();
		int qualityLevel = QualitySettings.GetQualityLevel();
		if (QualitySettings.names[qualityLevel] == "PC HQ")
		{
			component.depthTextureMode = DepthTextureMode.Depth;
			return;
		}
		component.depthTextureMode = DepthTextureMode.None;
	}

	[SerializeField]
	private IntConfigurable qualitySettingsConfigurable;
}

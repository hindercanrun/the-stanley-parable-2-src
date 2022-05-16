using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Voice Clip", menuName = "Stanley/New Voice Clip")]
public class VoiceClip : ScriptableObject
{
	public string DEBUG_TAG_WindowsPlayer { get; private set; }

	public string DEBUG_TAG_PS4 { get; private set; }

	public string DEBUG_TAG_PS5 { get; private set; }

	public string DEBUG_TAG_XBOX360 { get; private set; }

	public string DEBUG_TAG_XboxOne { get; private set; }

	public string DEBUG_TAG_Switch { get; private set; }

	private void OnValidate()
	{
		this.DEBUG_TAG_WindowsPlayer = this.GetVoiceAudioClipBaseName(RuntimePlatform.WindowsPlayer, false);
		this.DEBUG_TAG_PS4 = this.GetVoiceAudioClipBaseName(PlatformSettings.GetStanleyPlatform(RuntimePlatform.PS4), false);
		this.DEBUG_TAG_PS5 = this.GetVoiceAudioClipBaseName(PlatformSettings.GetStanleyPlatform(RuntimePlatform.PS5), false);
		this.DEBUG_TAG_XBOX360 = this.GetVoiceAudioClipBaseName(PlatformSettings.GetStanleyPlatform(RuntimePlatform.XBOX360), false);
		this.DEBUG_TAG_XboxOne = this.GetVoiceAudioClipBaseName(PlatformSettings.GetStanleyPlatform(RuntimePlatform.XboxOne), false);
		this.DEBUG_TAG_Switch = this.GetVoiceAudioClipBaseName(PlatformSettings.GetStanleyPlatform(RuntimePlatform.Switch), false);
	}

	public string AudioClipFolderName
	{
		get
		{
			if (!(this.AudioClipFolder == ""))
			{
				return this.AudioClipFolder + "/";
			}
			return "";
		}
	}

	private void OnEnable()
	{
		if (this.AudioClipBasename.Contains("_EN"))
		{
			this.AudioClipBasename = this.AudioClipBasename.Replace("_EN", "");
		}
	}

	public string GetVoiceAudioClipBaseName(RuntimePlatform runtimeplatform, bool useBucketIfAvailable)
	{
		return LocalizationTagTools.GetVoiceAudioClipBaseName(this.AudioClipBasename, PlatformSettings.GetStanleyPlatform(runtimeplatform), this.PlatformVariations, useBucketIfAvailable, this.HasBucketClip);
	}

	public string GetVoiceAudioClipBaseName(StanleyPlatform platform, bool useBucketIfAvailable)
	{
		return LocalizationTagTools.GetVoiceAudioClipBaseName(this.AudioClipBasename, platform, this.PlatformVariations, useBucketIfAvailable, this.HasBucketClip);
	}

	public string GetVoiceAudioClipBaseName(bool useBucketIfAvailable)
	{
		return LocalizationTagTools.GetVoiceAudioClipBaseName(this.AudioClipBasename, PlatformSettings.GetCurrentRunningPlatform(), this.PlatformVariations, useBucketIfAvailable, this.HasBucketClip);
	}

	[InspectorButton("PingAudioClip", "Ping Audio Clip", ButtonWidth = 300f)]
	public string AudioClipBasename = "";

	[SerializeField]
	private string AudioClipFolder = "";

	public bool HasBucketClip;

	public PlatformTag[] PlatformVariations = new PlatformTag[0];
}

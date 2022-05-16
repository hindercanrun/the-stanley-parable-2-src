using System;
using UnityEngine;

public static class LocalizationTagTools
{
	public static StanleyPlatform GetNextMostGeneralPlatformVariation(StanleyPlatform specificPlatform)
	{
		if (specificPlatform <= StanleyPlatform.XBOX360)
		{
			if (specificPlatform <= StanleyPlatform.Playstation4)
			{
				if (specificPlatform == StanleyPlatform.PC)
				{
					return StanleyPlatform.NoVariation;
				}
				if (specificPlatform == StanleyPlatform.Playstation4)
				{
					return StanleyPlatform.Playstation;
				}
			}
			else
			{
				if (specificPlatform == StanleyPlatform.Playstation5)
				{
					return StanleyPlatform.Playstation;
				}
				if (specificPlatform == StanleyPlatform.Playstation)
				{
					return StanleyPlatform.Console;
				}
				if (specificPlatform == StanleyPlatform.XBOX360)
				{
					return StanleyPlatform.XBOX;
				}
			}
		}
		else if (specificPlatform <= StanleyPlatform.XBOX)
		{
			if (specificPlatform == StanleyPlatform.XBOXone)
			{
				return StanleyPlatform.XBOX;
			}
			if (specificPlatform == StanleyPlatform.XBOX)
			{
				return StanleyPlatform.Console;
			}
		}
		else
		{
			if (specificPlatform == StanleyPlatform.Switch)
			{
				return StanleyPlatform.Console;
			}
			if (specificPlatform == StanleyPlatform.Console)
			{
				return StanleyPlatform.Port;
			}
			if (specificPlatform == StanleyPlatform.Mobile)
			{
				return StanleyPlatform.Port;
			}
		}
		return StanleyPlatform.Invalid;
	}

	public static string GetVoiceAudioClipBaseName(string audioClipBasename, PlatformTag[] platformVariations, bool useBucketIfAvailable, bool hasBucketClip)
	{
		return LocalizationTagTools.GetVoiceAudioClipBaseName(audioClipBasename, PlatformSettings.GetCurrentRunningPlatform(), platformVariations, useBucketIfAvailable, hasBucketClip);
	}

	public static string GetVoiceAudioClipBaseName(string audioClipBasename, RuntimePlatform runtimePlatform, PlatformTag[] platformVariations, bool useBucketIfAvailable, bool hasBucketClip)
	{
		return LocalizationTagTools.GetVoiceAudioClipBaseName(audioClipBasename, PlatformSettings.GetStanleyPlatform(runtimePlatform), platformVariations, useBucketIfAvailable, hasBucketClip);
	}

	public static string GetVoiceAudioClipBaseName(string audioClipBasename, StanleyPlatform runtimePlatform, PlatformTag[] platformVariations, bool useBucketIfAvailable, bool hasBucketClip)
	{
		int length = audioClipBasename.Length;
		PlatformTag platformTag = LocalizationTagTools.GetPlatformTag(runtimePlatform, platformVariations);
		bool flag = platformTag != null && platformTag.tag.Contains(">");
		bool flag2 = platformTag != null && !flag;
		bool flag3 = useBucketIfAvailable && hasBucketClip;
		string text = "";
		if (flag2)
		{
			text = "_" + platformTag.tag;
		}
		if (flag3)
		{
			text += LocalizationTagTools.BUCKETSUFFIX;
		}
		string text2;
		if (length > 3 && char.IsNumber(audioClipBasename[length - 1]) && char.IsNumber(audioClipBasename[length - 2]) && audioClipBasename[length - 3] == '_')
		{
			text2 = audioClipBasename.Insert(length - 3, text);
		}
		else
		{
			text2 = audioClipBasename + text;
		}
		if (flag)
		{
			string[] array = platformTag.tag.Split(new char[]
			{
				'>'
			});
			text2 = LocalizationTagTools.ReplaceLastOnly(text2, array[0], array[1]);
		}
		return text2;
	}

	public static PlatformTag GetPlatformTag(StanleyPlatform platform, PlatformTag[] platformVariations)
	{
		if (platformVariations.Length == 0)
		{
			return null;
		}
		StanleyPlatform platformToCheck = platform;
		Predicate<PlatformTag> <>9__0;
		while (platformToCheck != StanleyPlatform.NoVariation && platformToCheck != StanleyPlatform.Invalid)
		{
			Predicate<PlatformTag> match;
			if ((match = <>9__0) == null)
			{
				match = (<>9__0 = ((PlatformTag x) => x.platform == platformToCheck));
			}
			PlatformTag platformTag = Array.Find<PlatformTag>(platformVariations, match);
			if (platformTag != null)
			{
				return platformTag;
			}
			platformToCheck = LocalizationTagTools.GetNextMostGeneralPlatformVariation(platformToCheck);
		}
		return null;
	}

	private static string ReplaceLastOnly(string orig, string target, string replacement)
	{
		int num = orig.LastIndexOf(target);
		if (num == -1)
		{
			return orig;
		}
		return orig.Substring(0, num) + replacement + orig.Substring(num + target.Length);
	}

	[Obsolete]
	public static string GetBucketVoiceClipName(string baseName)
	{
		int length = baseName.Length;
		if (length > 3 && char.IsNumber(baseName[length - 1]) && char.IsNumber(baseName[length - 2]) && baseName[length - 3] == '_')
		{
			baseName = baseName.Insert(length - 3, LocalizationTagTools.BUCKETSUFFIX);
		}
		else
		{
			baseName += LocalizationTagTools.BUCKETSUFFIX;
		}
		return baseName;
	}

	public static readonly string BUCKETSUFFIX = "_BUCKET";
}

using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Message Box Dialouge", menuName = "Stanley/MessageBoxDialouge")]
public class MessageBoxDialogue : ScriptableObject
{
	public string[] Tags
	{
		get
		{
			return this.keys;
		}
	}

	private void DisplayMessagesForPlatform(StanleyPlatform platform)
	{
		if (this.localizationType == MessageBoxDialogue.LocalizeType.UnlocalizedStings)
		{
			new List<string>(this.messages);
			return;
		}
		new List<string>(this.PlatformAdjustedKeys(platform)).ConvertAll<string>((string t) => t + "\n\t\t\"" + LocalizationManager.GetTranslation(t, true, 0, true, false, null, null) + "\"");
	}

	private void DisplayAllMessages()
	{
		this.DisplayMessagesForPlatform(PlatformSettings.GetStanleyPlatform(Application.platform));
		this.DisplayMessagesForPlatform(StanleyPlatform.PC);
		this.DisplayMessagesForPlatform(StanleyPlatform.Playstation);
		this.DisplayMessagesForPlatform(StanleyPlatform.XBOX);
		this.DisplayMessagesForPlatform(StanleyPlatform.Switch);
	}

	public IEnumerable<string> PlatformAdjustedKeys()
	{
		foreach (string audioClipBasename in this.Keys())
		{
			yield return LocalizationTagTools.GetVoiceAudioClipBaseName(audioClipBasename, this.platformVariations, false, false);
		}
		IEnumerator<string> enumerator = null;
		yield break;
		yield break;
	}

	public IEnumerable<string> PlatformAdjustedKeys(StanleyPlatform platform)
	{
		foreach (string audioClipBasename in this.Keys())
		{
			yield return LocalizationTagTools.GetVoiceAudioClipBaseName(audioClipBasename, platform, this.platformVariations, false, false);
		}
		IEnumerator<string> enumerator = null;
		yield break;
		yield break;
	}

	public IEnumerable<string> Keys()
	{
		switch (this.localizationType)
		{
		case MessageBoxDialogue.LocalizeType.UnlocalizedStings:
			yield break;
		case MessageBoxDialogue.LocalizeType.KeyArray:
		{
			foreach (string text in this.keys)
			{
				yield return text;
			}
			string[] array = null;
			yield break;
		}
		case MessageBoxDialogue.LocalizeType.KeyBaseAndCount:
		{
			int num;
			for (int i = 0; i < this.keyBaseCount; i = num + 1)
			{
				yield return string.Format("{0}_{1:00}", this.keyBaseString, i);
				num = i;
			}
			yield break;
		}
		case MessageBoxDialogue.LocalizeType.LastKeyInSequence:
		{
			string baseString = this.lastKey.Substring(0, this.lastKey.Length - 2);
			int count = int.Parse(this.lastKey.Substring(this.lastKey.Length - 2));
			int num;
			for (int i = 0; i <= count; i = num + 1)
			{
				yield return string.Format("{0}{1:00}", baseString, i);
				num = i;
			}
			yield break;
		}
		default:
		{
			string baseString = null;
			yield break;
		}
		}
	}

	public string[] GetMessages()
	{
		if (this.localizationType == MessageBoxDialogue.LocalizeType.UnlocalizedStings)
		{
			return new List<string>(this.messages).ToArray();
		}
		return new List<string>(this.PlatformAdjustedKeys()).ConvertAll<string>((string t) => LocalizationManager.GetTranslation(t, true, 0, true, false, null, null)).ToArray();
	}

	[InspectorButton("DisplayAllMessages", "Display Tags and Messages ")]
	[SerializeField]
	private MessageBoxDialogue.LocalizeType localizationType;

	[Header("UnlocalizedStings")]
	[SerializeField]
	private string[] messages;

	[Header("KeyArray")]
	[SerializeField]
	private string[] keys;

	[Header("KeyBaseAndCount")]
	[SerializeField]
	private string keyBaseString;

	[SerializeField]
	private int keyBaseCount;

	[Header("LastKeyInSequence")]
	[SerializeField]
	private string lastKey;

	[Header("Platform variations")]
	public PlatformTag[] platformVariations = new PlatformTag[0];

	public enum LocalizeType
	{
		UnlocalizedStings,
		KeyArray,
		KeyBaseAndCount,
		LastKeyInSequence
	}
}

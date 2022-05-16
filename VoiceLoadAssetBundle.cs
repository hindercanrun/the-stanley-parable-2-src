using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceLoadAssetBundle : MonoBehaviour
{
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		string[] allAssetBundlesWithVariant = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Switch")).LoadAsset<AssetBundleManifest>("assetbundlemanifest").GetAllAssetBundlesWithVariant();
		for (int i = 0; i < allAssetBundlesWithVariant.Length; i++)
		{
			string text = allAssetBundlesWithVariant[i];
			if (text.Contains("voice"))
			{
				this.voiceVariants.Add(text);
				string key = text.Split(new string[]
				{
					"."
				}, StringSplitOptions.None)[1].ToLower();
				this.languageCodeAssetBundleDictionary.Add(key, i);
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.LoadAndPlay();
		}
	}

	private void LoadAndPlay()
	{
		if (this.currentVoiceBundle != null)
		{
			this.currentVoiceBundle.Unload(true);
		}
		int index = 0;
		this.wantedLanguageCode = this.wantedLanguageCode.ToLower();
		if (this.languageCodeAssetBundleDictionary.TryGetValue(this.wantedLanguageCode, out index))
		{
			string path = this.voiceVariants[index];
			this.currentVoiceBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, path));
			string name = this.audioClipToLoad + "_" + this.wantedLanguageCode;
			AudioClip clip = this.currentVoiceBundle.LoadAsset<AudioClip>(name);
			this.audioSource.clip = clip;
			this.audioSource.Play();
		}
	}

	private AudioSource audioSource;

	[SerializeField]
	private string wantedLanguageCode = "en";

	[SerializeField]
	private string audioClipToLoad = "";

	[SerializeField]
	private List<string> voiceVariants = new List<string>();

	[SerializeField]
	private Dictionary<string, int> languageCodeAssetBundleDictionary = new Dictionary<string, int>();

	[SerializeField]
	private AssetBundle currentVoiceBundle;
}

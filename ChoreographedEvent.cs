using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ChoreographedEvent
{
	public void PreloadAudioClips()
	{
		this.usePreloadedClips = true;
		string assetBundleFilenameFromVoiceClip = ChoreoMaster.GetAssetBundleFilenameFromVoiceClip(this.Clip, PlatformSettings.GetCurrentRunningPlatform(), false);
		if (ChoreoMaster.VOICEASSETBUNDLE == null || !ChoreoMaster.VOICEASSETBUNDLE.Contains(assetBundleFilenameFromVoiceClip))
		{
			return;
		}
		this.preloadedClip = ChoreoMaster.VOICEASSETBUNDLE.LoadAsset<AudioClip>(assetBundleFilenameFromVoiceClip);
		if (this.Clip.HasBucketClip)
		{
			string assetBundleFilenameFromVoiceClip2 = ChoreoMaster.GetAssetBundleFilenameFromVoiceClip(this.Clip, PlatformSettings.GetCurrentRunningPlatform(), true);
			if (!ChoreoMaster.VOICEASSETBUNDLE.Contains(assetBundleFilenameFromVoiceClip2))
			{
				return;
			}
			this.preloadedClipBucket = ChoreoMaster.VOICEASSETBUNDLE.LoadAsset<AudioClip>(assetBundleFilenameFromVoiceClip2);
		}
	}

	public void StartPreloadOfDynamicClip(MonoBehaviour behaviour)
	{
		if (this.usePreloadedClips)
		{
			return;
		}
		if (this.loadDynamicRoutine != null)
		{
			behaviour.StopCoroutine(this.loadDynamicRoutine);
		}
		this.loadDynamicRoutine = behaviour.StartCoroutine(this.PreloadDynamicClipAsync());
	}

	private IEnumerator PreloadDynamicClipAsync()
	{
		string assetBundleFilenameFromVoiceClip = ChoreoMaster.GetAssetBundleFilenameFromVoiceClip(this.Clip, PlatformSettings.GetCurrentRunningPlatform(), BucketController.HASBUCKET);
		if (ChoreoMaster.VOICEASSETBUNDLE == null || !ChoreoMaster.VOICEASSETBUNDLE.Contains(assetBundleFilenameFromVoiceClip))
		{
			this.loadDynamicRoutine = null;
			yield break;
		}
		AssetBundleRequest clipRequest = ChoreoMaster.VOICEASSETBUNDLE.LoadAssetAsync<AudioClip>(assetBundleFilenameFromVoiceClip);
		clipRequest.allowSceneActivation = true;
		while (!clipRequest.isDone)
		{
			yield return null;
		}
		this.dynamicClip = (clipRequest.asset as AudioClip);
		this.loadDynamicRoutine = null;
		yield break;
	}

	public AudioClip GetAudioClip()
	{
		if (!this.usePreloadedClips)
		{
			return this.dynamicClip;
		}
		if (!BucketController.HASBUCKET || !(this.preloadedClipBucket != null))
		{
			return this.preloadedClip;
		}
		return this.preloadedClipBucket;
	}

	public VoiceClip Clip;

	[HideInInspector]
	public ChoreographedScene owner;

	private AudioClip preloadedClip;

	private AudioClip preloadedClipBucket;

	private AudioClip dynamicClip;

	private bool usePreloadedClips;

	private Coroutine loadDynamicRoutine;
}

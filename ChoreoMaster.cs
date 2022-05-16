using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class ChoreoMaster : Singleton<ChoreoMaster>
{
	public static SubtitleUI SubtitleUIInstance
	{
		get
		{
			return Singleton<ChoreoMaster>.Instance.subtitlesUI;
		}
	}

	public static string GetAssetBundleFilenameFromVoiceClip(VoiceClip voiceClip, StanleyPlatform platform, bool hasBucket = false)
	{
		string voiceAudioClipBaseName = voiceClip.GetVoiceAudioClipBaseName(platform, hasBucket);
		return "voice_" + voiceAudioClipBaseName;
	}

	protected override void Awake()
	{
		base.Awake();
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		if (this != Singleton<ChoreoMaster>.Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		this.CacheAvailableDubs();
		this.OnAudioDubChange("default");
		this.source = base.GetComponent<AudioSource>();
		if (!this.source)
		{
			this.source = base.gameObject.AddComponent<AudioSource>();
		}
		this.soundscapeSource1 = this.soundscapeChild.AddComponent<AudioSource>();
		this.soundscapeSource2 = this.soundscapeChild.AddComponent<AudioSource>();
		this.soundscapeSource1.bypassReverbZones = true;
		this.soundscapeSource2.bypassReverbZones = true;
		this.soundscapeSource1.outputAudioMixerGroup = this.ambienceMixer;
		this.soundscapeSource2.outputAudioMixerGroup = this.ambienceMixer;
		GameMaster.OnPause += this.Pause;
		GameMaster.OnResume += this.Resume;
		this.currentContentWarningConfigurable.Init();
		GameMaster.OnPrepareLoadingLevel += this.OnPrepareLoadingLevel;
		this.FindSoundScapes();
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.FindSoundScapes();
	}

	private void FindSoundScapes()
	{
		this.scapes.Clear();
		this.currentSoundscapeClip = null;
		foreach (Soundscape scape in Object.FindObjectsOfType<Soundscape>())
		{
			this.RegisterSoundscape(scape);
		}
	}

	private void OnPrepareLoadingLevel()
	{
		this.currentContentWarningConfigurable.SetValue("");
	}

	private void OnDisable()
	{
		GameMaster.OnPause -= this.Pause;
		GameMaster.OnResume -= this.Resume;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	private void CacheAvailableDubs()
	{
		string manifestAssetBundleName = ChoreoMaster.GetManifestAssetBundleName();
		if (!File.Exists(Path.Combine(Application.streamingAssetsPath, manifestAssetBundleName)))
		{
			return;
		}
		string[] allAssetBundles = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, manifestAssetBundleName)).LoadAsset<AssetBundleManifest>("assetbundlemanifest").GetAllAssetBundles();
		int num = 0;
		foreach (string text in allAssetBundles)
		{
			if (text.Contains("voice"))
			{
				ChoreoMaster.voiceVariants.Add(text);
				string key = text.Split(new string[]
				{
					"."
				}, StringSplitOptions.None)[1].ToLower();
				ChoreoMaster.languageCodeAssetBundleDictionary.Add(key, num);
				num++;
			}
		}
	}

	public static string GetManifestAssetBundleName()
	{
		return "StreamingAssets";
	}

	private void OnAudioDubChange(string languageCode)
	{
		if (ChoreoMaster.VOICEASSETBUNDLE != null)
		{
			ChoreoMaster.VOICEASSETBUNDLE.Unload(true);
		}
		int index = 0;
		languageCode = languageCode.ToLower();
		if (ChoreoMaster.languageCodeAssetBundleDictionary.TryGetValue(languageCode, out index))
		{
			string path = ChoreoMaster.voiceVariants[index];
			ChoreoMaster.VOICEASSETBUNDLE = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, path));
			ChoreoMaster.CURRENTLANGUAGECODEVOICE = languageCode;
			return;
		}
		foreach (KeyValuePair<string, int> keyValuePair in ChoreoMaster.languageCodeAssetBundleDictionary)
		{
		}
	}

	private void Update()
	{
		if (Singleton<GameMaster>.Instance != null && Singleton<GameMaster>.Instance.closedCaptionsOn)
		{
			this.canvas.planeDistance = 0.054f;
			this.canvas.targetDisplay = 0;
		}
		else
		{
			this.canvas.planeDistance = -1f;
			this.canvas.targetDisplay = 1;
		}
		if (this.currentEvent.Clip != null && this.waitRoutine == null)
		{
			if (this.currentEvent.owner != null)
			{
				this.currentEvent.owner.FinishedEvent(this.currentEvent);
			}
			this.CheckAvailableEvents();
		}
		if (this.waitRoutine == null)
		{
			this.subtitlesUI.HideSubtitlesWithFade();
		}
		if (this.scapes.Count > 0)
		{
			int num = -1;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < this.scapes.Count; i++)
			{
				float num3;
				if (this.scapes[i].WithhinDistance(out num3) && num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num != -1 && this.scapes[num].clip != this.currentSoundscapeClip)
			{
				this.currentSoundscapeClip = this.scapes[num].clip;
				if (this.scapeChangeCoroutine != null)
				{
					base.StopCoroutine(this.scapeChangeCoroutine);
				}
				if (this.soundscapeToUse == 1)
				{
					this.scapeChangeCoroutine = this.ChangeSoundscape(this.scapes[num], this.soundscapeSource1, this.soundscapeSource2);
					base.StartCoroutine(this.scapeChangeCoroutine);
					this.soundscapeToUse = 2;
					return;
				}
				this.scapeChangeCoroutine = this.ChangeSoundscape(this.scapes[num], this.soundscapeSource2, this.soundscapeSource1);
				base.StartCoroutine(this.scapeChangeCoroutine);
				this.soundscapeToUse = 1;
			}
		}
	}

	private void Pause()
	{
		this.source.Pause();
		this.soundscapeSource1.Pause();
		this.soundscapeSource2.Pause();
	}

	private void Resume()
	{
		this.source.UnPause();
		this.soundscapeSource1.UnPause();
		this.soundscapeSource2.UnPause();
	}

	private void CheckAvailableEvents()
	{
		if (this.upcomingEvents.Count > 0)
		{
			this.currentEvent = this.upcomingEvents[0];
			this.upcomingEvents.RemoveAt(0);
			this.PlayClip(this.currentEvent);
			return;
		}
		if (this.currentEvent.owner != null)
		{
			this.currentEvent.owner.FinishedEvent(this.currentEvent);
		}
		this.currentEvent = new ChoreographedEvent();
		if (this.waitRoutine != null)
		{
			base.StopCoroutine(this.waitRoutine);
		}
		this.subtitlesUI.HideSubtitlesWithFade();
	}

	private void PlayClip(ChoreographedEvent choreoEvent)
	{
		if (this.waitRoutine != null)
		{
			base.StopCoroutine(this.waitRoutine);
		}
		AudioClip audioClip = choreoEvent.GetAudioClip();
		this.source.clip = audioClip;
		if (audioClip != null)
		{
			this.source.Play();
		}
		this.waitRoutine = base.StartCoroutine(this.WaitForClip(choreoEvent.Clip, audioClip));
	}

	private IEnumerator WaitForClip(VoiceClip voiceClip, AudioClip audioClip)
	{
		float clipDuration = 1.5f;
		if (audioClip != null)
		{
			clipDuration = audioClip.length;
		}
		this.subtitlesUI.NewSubtitle(voiceClip.GetVoiceAudioClipBaseName(PlatformSettings.GetCurrentRunningPlatform(), BucketController.HASBUCKET), clipDuration);
		float timer = 0f;
		while (timer < clipDuration)
		{
			timer += Time.deltaTime * ChoreoMaster.GameSpeed;
			yield return null;
		}
		this.source.clip = null;
		this.waitRoutine = null;
		this.CheckAvailableEvents();
		yield break;
	}

	public void BeginEvents(List<ChoreographedEvent> events, ChoreographedScene.InteruptBehaviour behaviour, bool spawnflag49)
	{
		List<ChoreographedEvent> list = new List<ChoreographedEvent>(events);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].StartPreloadOfDynamicClip(this);
		}
		if (behaviour == ChoreographedScene.InteruptBehaviour.StartImmediately || spawnflag49)
		{
			if (list.Count > 0)
			{
				this.source.Stop();
				this.currentEvent = list[0];
				this.upcomingEvents = list;
				this.CheckAvailableEvents();
				return;
			}
			this.upcomingEvents.Clear();
			return;
		}
		else if (behaviour == ChoreographedScene.InteruptBehaviour.CancelAtNextEvent)
		{
			if (list.Count > 0)
			{
				for (int j = 0; j < this.upcomingEvents.Count; j++)
				{
					if (this.upcomingEvents[j].owner != null)
					{
						this.upcomingEvents[j].owner.Cancelled();
					}
				}
				this.upcomingEvents = list;
				this.CheckAvailableEvents();
				return;
			}
			this.upcomingEvents.Clear();
			return;
		}
		else
		{
			if (behaviour == ChoreographedScene.InteruptBehaviour.InterruptAtNextEvent)
			{
				for (int k = 0; k < this.upcomingEvents.Count; k++)
				{
					if (this.upcomingEvents[k].owner != null)
					{
						this.upcomingEvents[k].owner.Cancelled();
					}
				}
				this.upcomingEvents.InsertRange(0, list);
				this.CheckAvailableEvents();
				return;
			}
			if (behaviour == ChoreographedScene.InteruptBehaviour.WaitForFinish)
			{
				for (int l = 0; l < list.Count; l++)
				{
					this.upcomingEvents.Add(list[l]);
				}
				this.CheckAvailableEvents();
			}
			return;
		}
	}

	public void DropAll()
	{
		this.source.Stop();
		if (this.waitRoutine != null)
		{
			base.StopCoroutine(this.waitRoutine);
		}
		this.currentEvent = new ChoreographedEvent();
		this.upcomingEvents.Clear();
		this.subtitlesUI.HideSubtitlesImmediate();
		this.CheckAvailableEvents();
	}

	public void DropOwnedEvents(ChoreographedScene owner)
	{
		for (int i = this.upcomingEvents.Count - 1; i >= 0; i--)
		{
			if (this.upcomingEvents[i].owner == owner)
			{
				this.upcomingEvents.RemoveAt(i);
			}
		}
		if (this.currentEvent.owner == owner)
		{
			this.CheckAvailableEvents();
		}
	}

	public void NearbySoundscape(AudioClip clip, float distance, float pitch, float volume, float fadetime)
	{
		ChoreoMaster.Scape scape = new ChoreoMaster.Scape();
		scape.clip = clip;
		scape.volume = volume;
		scape.pitch = pitch;
		scape.fadetime = fadetime;
		this.scapes.Add(scape);
	}

	private void RegisterSoundscape(Soundscape scape)
	{
		ChoreoMaster.Scape scape2 = new ChoreoMaster.Scape();
		scape2.clip = scape.clip;
		scape2.position = scape.gameObject.transform.position;
		scape2.radius = scape.radius;
		scape2.volume = scape.volume;
		scape2.pitch = scape.pitch;
		scape2.fadetime = scape.fadetime;
		scape2.playerTransform = StanleyController.Instance.transform;
		this.scapes.Add(scape2);
	}

	private IEnumerator ChangeSoundscape(ChoreoMaster.Scape scape, AudioSource inSource, AudioSource outSource)
	{
		fint32 startTime = Time.realtimeSinceStartup;
		fint32 endTime = startTime + scape.fadetime;
		inSource.clip = scape.clip;
		inSource.pitch = scape.pitch;
		inSource.loop = true;
		inSource.volume = 0f;
		inSource.Play();
		float inTargetVol = scape.volume;
		float outStartVol = outSource.volume;
		while (Time.realtimeSinceStartup < endTime)
		{
			float num = Mathf.InverseLerp(startTime, endTime, Time.realtimeSinceStartup);
			inSource.volume = num * inTargetVol;
			outSource.volume = (1f - num) * outStartVol;
			yield return new WaitForEndOfFrame();
		}
		inSource.volume = inTargetVol;
		outSource.Stop();
		yield break;
	}

	public static AssetBundle VOICEASSETBUNDLE;

	public static string CURRENTLANGUAGECODEVOICE = "default";

	public static string CURRENTLANGUAGECODETEXT = "en";

	public static readonly List<string> voiceVariants = new List<string>();

	public static readonly Dictionary<string, int> languageCodeAssetBundleDictionary = new Dictionary<string, int>();

	public static float GameSpeed = 1f;

	[SerializeField]
	private AudioSource source;

	private ChoreographedEvent currentEvent = new ChoreographedEvent();

	private List<ChoreographedEvent> upcomingEvents = new List<ChoreographedEvent>();

	private Coroutine waitRoutine;

	private IEnumerator scapeChangeCoroutine;

	public GameObject soundscapeChild;

	private AudioSource soundscapeSource1;

	private AudioSource soundscapeSource2;

	public AudioMixerGroup ambienceMixer;

	public Canvas canvas;

	[SerializeField]
	private AudioClip currentSoundscapeClip;

	[SerializeField]
	private List<ChoreoMaster.Scape> scapes = new List<ChoreoMaster.Scape>();

	private int soundscapeToUse = 1;

	[SerializeField]
	private SubtitleUI subtitlesUI;

	[Header("Content Warning")]
	public StringConfigurable currentContentWarningConfigurable;

	[Serializable]
	private class Scape
	{
		public bool WithhinDistance(out float distance)
		{
			distance = Vector3.Distance(this.position, this.playerTransform.position);
			return distance <= this.radius;
		}

		public AudioClip clip;

		public float radius;

		public Vector3 position;

		public float volume;

		public float pitch;

		public float fadetime;

		public Transform playerTransform;
	}
}

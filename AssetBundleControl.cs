using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleControl : MonoBehaviour
{
	private void Awake()
	{
		foreach (ReleaseMap releaseMap in this.releaseMaps)
		{
			foreach (ReleaseBundle releaseBundle in releaseMap.Bundles)
			{
				AssetBundleControl.allListedAssetBundles.Add(releaseBundle.name);
			}
			foreach (ReleaseMap.ReleaseScene item in releaseMap.Scenes)
			{
				AssetBundleControl.releaseScenes.Add(item);
			}
		}
	}

	private static AssetBundle LoadAssetBundle(string bundleName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, bundleName);
		if (!File.Exists(path))
		{
			return null;
		}
		AssetBundle result;
		if (AssetBundleControl.loadIntoMemory)
		{
			result = AssetBundle.LoadFromMemory(File.ReadAllBytes(path));
		}
		else
		{
			result = AssetBundle.LoadFromFile(path);
		}
		return result;
	}

	private static IEnumerator LoadAssetBundleAsync(string bundleName, Action<AssetBundle> callbackBundle, Action<float> callbackLoadProgress = null)
	{
		string path = Path.Combine(Application.streamingAssetsPath, bundleName.ToLower());
		if (!File.Exists(path))
		{
			yield break;
		}
		if (AssetBundleControl.loadIntoMemory)
		{
			byte[] binary = File.ReadAllBytes(path);
			AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromMemoryAsync(binary);
			while (!bundleRequest.isDone)
			{
				if (callbackLoadProgress != null)
				{
					callbackLoadProgress(bundleRequest.progress);
				}
				yield return null;
			}
			if (callbackLoadProgress != null)
			{
				callbackLoadProgress(1f);
			}
			if (callbackBundle != null && bundleRequest.assetBundle != null)
			{
				callbackBundle(bundleRequest.assetBundle);
			}
			bundleRequest = null;
		}
		else
		{
			AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
			while (!bundleRequest.isDone)
			{
				if (callbackLoadProgress != null)
				{
					callbackLoadProgress(bundleRequest.progress);
				}
				yield return null;
			}
			if (callbackLoadProgress != null)
			{
				callbackLoadProgress(1f);
			}
			if (callbackBundle != null && bundleRequest.assetBundle != null)
			{
				callbackBundle(bundleRequest.assetBundle);
			}
			bundleRequest = null;
		}
		yield break;
	}

	private static void SceneInBuildIndexOrAssetBundle(string sceneName, ref bool inBuildIndex, ref bool inAssetBundles)
	{
		inBuildIndex = Application.CanStreamedLevelBeLoaded(sceneName);
		inAssetBundles = File.Exists(Path.Combine(Application.streamingAssetsPath, sceneName));
	}

	private static bool ReleaseSceneAvailable(string sceneName, out ReleaseMap.ReleaseScene releaseScene)
	{
		for (int i = 0; i < AssetBundleControl.releaseScenes.Count; i++)
		{
			ReleaseMap.ReleaseScene releaseScene2 = AssetBundleControl.releaseScenes[i];
			if (releaseScene2.SceneName.ToLower().Equals(sceneName.ToLower()))
			{
				releaseScene = releaseScene2;
				return true;
			}
		}
		releaseScene = null;
		return false;
	}

	public static bool ChangeScene(string sceneName, string loadingSceneName, MonoBehaviour behaviour)
	{
		ReleaseMap.ReleaseScene releaseScene = null;
		sceneName = sceneName.ToLower();
		return AssetBundleControl.ReleaseSceneAvailable(sceneName, out releaseScene) && AssetBundleControl.ChangeScene(releaseScene, loadingSceneName, behaviour);
	}

	public static bool ChangeScene(ReleaseMap.ReleaseScene releaseScene, string loadingSceneName, MonoBehaviour behaviour)
	{
		bool flag = false;
		bool flag2 = false;
		string text = releaseScene.SceneName.ToLower();
		loadingSceneName = loadingSceneName.ToLower();
		AssetBundleControl.SceneInBuildIndexOrAssetBundle(text, ref flag, ref flag2);
		if (!flag && !flag2)
		{
			return false;
		}
		if (flag)
		{
			if (AssetBundleControl.changeSceneRoutine == null)
			{
				AssetBundleControl.changeSceneRoutine = behaviour.StartCoroutine(AssetBundleControl.LoadSceneInBuildIndex(text, loadingSceneName, true));
				return true;
			}
			return false;
		}
		else
		{
			if (!flag2)
			{
				return false;
			}
			if (AssetBundleControl.changeSceneRoutine == null)
			{
				AssetBundleControl.changeSceneRoutine = behaviour.StartCoroutine(AssetBundleControl.LoadSceneFromAssetBundle(releaseScene, behaviour, true, loadingSceneName));
				return true;
			}
			return false;
		}
	}

	private static IEnumerator LoadSceneFromAssetBundle(ReleaseMap.ReleaseScene releaseScene, MonoBehaviour behaviour, bool useLoadingScreen = false, string loadingSceneName = "")
	{
		if (useLoadingScreen)
		{
			if (loadingSceneName != "" && AssetBundleControl.loadingSceneAssetBundle == null)
			{
				AssetBundleControl.loadingSceneAssetBundle = AssetBundleControl.LoadAssetBundle(loadingSceneName);
				if (AssetBundleControl.loadingSceneAssetBundle != null)
				{
					AsyncOperation loadingSceneAsync = AssetBundleControl.GatherAndActivateSceneFromBundle(AssetBundleControl.loadingSceneAssetBundle, ref AssetBundleControl.currentLoadingSceneName);
					while (loadingSceneAsync != null && !loadingSceneAsync.isDone)
					{
						yield return null;
					}
					loadingSceneAsync = null;
				}
			}
			else if (loadingSceneName != "" && AssetBundleControl.loadingSceneAssetBundle != null)
			{
				AsyncOperation loadingSceneAsync = AssetBundleControl.GatherAndActivateSceneFromBundle(AssetBundleControl.loadingSceneAssetBundle, ref AssetBundleControl.currentLoadingSceneName);
				while (loadingSceneAsync != null && !loadingSceneAsync.isDone)
				{
					yield return null;
				}
				loadingSceneAsync = null;
			}
		}
		if (AssetBundleControl.OnScenePreLoad != null)
		{
			AssetBundleControl.OnScenePreLoad();
		}
		string sceneBundleName = releaseScene.SceneName.ToLower();
		AssetBundleControl.currentGameSceneName != sceneBundleName;
		bool changeSetDataRequired = AssetBundleControl.currentAssetSet != releaseScene.Set;
		bool flag = true;
		AssetBundleControl.currentGameSceneName != "";
		if (flag)
		{
			if (AssetBundleControl.currentSceneDataBundle != null)
			{
				AssetBundleControl.currentSceneDataBundle.Unload(true);
			}
			if (AssetBundleControl.currentSceneBundle != null)
			{
				AssetBundleControl.currentSceneBundle.Unload(true);
			}
			for (int i = 0; i < AssetBundleControl.currentSceneExclusiveAssetBundles.Count; i++)
			{
				AssetBundleControl.currentSceneExclusiveAssetBundles[i].Unload(true);
			}
			AssetBundleControl.currentSceneExclusiveAssetBundles.Clear();
			if (changeSetDataRequired)
			{
				for (int j = 0; j < AssetBundleControl.currentSetAssetBundles.Count; j++)
				{
					AssetBundleControl.currentSetAssetBundles[j].Unload(true);
				}
				AssetBundleControl.currentSetAssetBundles.Clear();
			}
			AssetBundleControl.currentAssetSet = releaseScene.Set;
			string text = sceneBundleName + "_sceneassets";
			if (File.Exists(Path.Combine(Application.streamingAssetsPath, text)))
			{
				yield return behaviour.StartCoroutine(AssetBundleControl.LoadAssetBundleAsync(text, delegate(AssetBundle newBundle)
				{
					if (newBundle != null)
					{
						AssetBundleControl.currentSceneDataBundle = newBundle;
					}
				}, delegate(float progress)
				{
					AssetBundleControl.BroadcastLoadProgressUpdate(progress, 0f, 0.2f);
				}));
			}
			else
			{
				AssetBundleControl.currentSceneDataBundle = null;
			}
			AssetBundleControl.<>c__DisplayClass33_0 CS$<>8__locals1 = new AssetBundleControl.<>c__DisplayClass33_0();
			CS$<>8__locals1.i = 0;
			while (CS$<>8__locals1.i < AssetBundleControl.allListedAssetBundles.Count)
			{
				string str = AssetBundleControl.allListedAssetBundles[CS$<>8__locals1.i];
				string bundleName = sceneBundleName + "_exclusive_" + str + ".default";
				yield return behaviour.StartCoroutine(AssetBundleControl.LoadAssetBundleAsync(bundleName, delegate(AssetBundle newBundle)
				{
					if (newBundle != null)
					{
						AssetBundleControl.currentSceneExclusiveAssetBundles.Add(newBundle);
					}
				}, delegate(float progress)
				{
					AssetBundleControl.BroadcastLoadProgressUpdate((float)(CS$<>8__locals1.i / AssetBundleControl.allListedAssetBundles.Count), 0.2f, 0.4f);
				}));
				int i2 = CS$<>8__locals1.i;
				CS$<>8__locals1.i = i2 + 1;
			}
			CS$<>8__locals1 = null;
			if (changeSetDataRequired && AssetBundleControl.currentAssetSet != null)
			{
				AssetBundleControl.<>c__DisplayClass33_1 CS$<>8__locals2 = new AssetBundleControl.<>c__DisplayClass33_1();
				CS$<>8__locals2.i = 0;
				while (CS$<>8__locals2.i < AssetBundleControl.LoadedDefaultBundles.Count)
				{
					string name = AssetBundleControl.LoadedDefaultBundles[CS$<>8__locals2.i].name;
					int num = name.IndexOf(".");
					if (num != -1)
					{
						string bundleName2 = name.Insert(num, "_" + AssetBundleControl.currentAssetSet.name);
						yield return behaviour.StartCoroutine(AssetBundleControl.LoadAssetBundleAsync(bundleName2, delegate(AssetBundle newBundle)
						{
							if (newBundle != null)
							{
								AssetBundleControl.currentSetAssetBundles.Add(newBundle);
							}
						}, delegate(float progress)
						{
							AssetBundleControl.BroadcastLoadProgressUpdate((float)(CS$<>8__locals2.i / AssetBundleControl.allListedAssetBundles.Count), 0.4f, 0.6f);
						}));
					}
					int i2 = CS$<>8__locals2.i;
					CS$<>8__locals2.i = i2 + 1;
				}
				CS$<>8__locals2 = null;
			}
			yield return behaviour.StartCoroutine(AssetBundleControl.LoadAssetBundleAsync(sceneBundleName, delegate(AssetBundle newBundle)
			{
				if (newBundle != null)
				{
					AssetBundleControl.currentSceneBundle = newBundle;
				}
			}, delegate(float progress)
			{
				AssetBundleControl.BroadcastLoadProgressUpdate(progress, 0.6f, 0.8f);
			}));
			AssetBundleControl.BroadcastLoadProgressUpdate(0.9f, 0f, 1f);
		}
		AsyncOperation async = AssetBundleControl.GatherAndActivateSceneFromBundle(AssetBundleControl.currentSceneBundle, ref AssetBundleControl.currentGameSceneName);
		async.allowSceneActivation = false;
		while (async != null && !async.isDone)
		{
			AssetBundleControl.BroadcastLoadProgressUpdate(async.progress, 0.8f, 0.95f);
			if (async.progress >= 0.9f)
			{
				AssetBundleControl.BroadcastLoadProgressUpdate(1f, 0f, 1f);
				async.allowSceneActivation = true;
			}
			yield return null;
		}
		AssetBundleControl.BroadcastLoadProgressUpdate(1f, 0f, 1f);
		if (AssetBundleControl.OnSceneReady != null)
		{
			AssetBundleControl.OnSceneReady();
		}
		AssetBundleControl.changeSceneRoutine = null;
		yield break;
	}

	private static AsyncOperation GatherAndActivateSceneFromBundle(AssetBundle bundle, ref string referenceString)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(bundle.GetAllScenePaths()[0]);
		referenceString = bundle.name;
		return SceneManager.LoadSceneAsync(fileNameWithoutExtension);
	}

	private static IEnumerator LoadSceneInBuildIndex(string mapName, string loadingSceneName, bool waitMinTime = true)
	{
		float time = Time.time;
		AsyncOperation loadlevelOperation = null;
		AsyncOperation unloadOperation = null;
		if (loadingSceneName != "")
		{
			loadlevelOperation = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
			yield return null;
			do
			{
				yield return null;
			}
			while (!loadlevelOperation.isDone);
			yield return null;
		}
		if (AssetBundleControl.currentGameSceneName == "")
		{
			AssetBundleControl.currentGameSceneName = SceneManager.GetActiveScene().name;
		}
		if (AssetBundleControl.currentGameSceneName != "")
		{
			unloadOperation = SceneManager.UnloadSceneAsync(AssetBundleControl.currentGameSceneName);
			while (unloadOperation != null && !unloadOperation.isDone)
			{
				yield return null;
			}
			yield return null;
		}
		AssetBundleControl.BroadcastLoadProgressUpdate(0.5f, 0f, 1f);
		yield return null;
		if (AssetBundleControl.OnScenePreLoad != null)
		{
			AssetBundleControl.OnScenePreLoad();
		}
		loadlevelOperation = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);
		loadlevelOperation.allowSceneActivation = false;
		yield return null;
		yield return null;
		do
		{
			AssetBundleControl.BroadcastLoadProgressUpdate(0.5f + loadlevelOperation.progress * 0.5f, 0f, 1f);
			yield return null;
		}
		while (loadlevelOperation.progress < 0.9f);
		AssetBundleControl.BroadcastLoadProgressUpdate(1f, 0f, 1f);
		loadlevelOperation.allowSceneActivation = true;
		do
		{
			yield return null;
		}
		while (!loadlevelOperation.isDone);
		if (loadingSceneName != "")
		{
			unloadOperation = SceneManager.UnloadSceneAsync(loadingSceneName);
			while (!unloadOperation.isDone)
			{
				yield return null;
			}
		}
		AssetBundleControl.currentGameSceneName = mapName;
		if (AssetBundleControl.OnSceneReady != null)
		{
			AssetBundleControl.OnSceneReady();
		}
		AssetBundleControl.changeSceneRoutine = null;
		yield break;
	}

	private void Start()
	{
		if (this.autoStart)
		{
			this.StartLoadBundles();
		}
	}

	public void StartLoadBundles()
	{
		if (this.loadingBundles != null)
		{
			return;
		}
		this.loadingBundles = base.StartCoroutine(this.LoadBundles());
	}

	private IEnumerator LoadBundles()
	{
		Object.DontDestroyOnLoad(this);
		RuntimePlatform platform = this.forcePlatform ? this.customPlatform : Application.platform;
		AssetBundleControl.loadIntoMemory = this.platformConfiguration.LoadIntoMemory(platform);
		List<string> assetBundlesToLoad = new List<string>();
		for (int j = 0; j < this.releaseMaps.Length; j++)
		{
			ReleaseMap releaseMap = this.releaseMaps[j];
			for (int k = 0; k < releaseMap.Bundles.Count; k++)
			{
				ReleaseBundle releaseBundle = releaseMap.Bundles[k];
				string str = releaseBundle.name.ToLower();
				BundleConfiguration bundleConfiguration;
				if (this.platformConfiguration.OverrideExists(platform, releaseBundle, out bundleConfiguration))
				{
					switch (bundleConfiguration.IncludeOption)
					{
					case BundleConfiguration.IncludeOptions.UseDefault:
						assetBundlesToLoad.Add(str + ".default");
						break;
					case BundleConfiguration.IncludeOptions.UseVariant:
						assetBundlesToLoad.Add(str + "." + bundleConfiguration.GetCustomVariant());
						break;
					}
				}
				else
				{
					assetBundlesToLoad.Add(str + ".default");
				}
			}
		}
		string bundlePath = Path.Combine(Application.streamingAssetsPath, "universal_sceneassets");
		if (AssetBundleControl.loadIntoMemory)
		{
			yield return base.StartCoroutine(this.LoadAssetBundleIntoMemory(bundlePath, AssetBundleControl.LoadedDefaultBundles));
		}
		else
		{
			yield return base.StartCoroutine(this.LoadAssetBundleFromFile(bundlePath, AssetBundleControl.LoadedDefaultBundles));
		}
		int num;
		for (int i = 0; i < assetBundlesToLoad.Count; i = num + 1)
		{
			string text = Path.Combine(Application.streamingAssetsPath, assetBundlesToLoad[i]);
			if (File.Exists(text))
			{
				if (AssetBundleControl.loadIntoMemory)
				{
					yield return base.StartCoroutine(this.LoadAssetBundleIntoMemory(text, AssetBundleControl.LoadedDefaultBundles));
				}
				else
				{
					yield return base.StartCoroutine(this.LoadAssetBundleFromFile(text, AssetBundleControl.LoadedDefaultBundles));
				}
				AssetBundleControl.BroadcastLoadProgressUpdate((float)i / (float)assetBundlesToLoad.Count, 0f, 1f);
			}
			num = i;
		}
		if (this.cancelLoadScene)
		{
			yield break;
		}
		if (this.debugLoadScene)
		{
			AssetBundleControl.ChangeScene(this.debugSceneName, "LoadingScene_UD_MASTER", this);
			yield break;
		}
		if (AssetBundleControl.releaseScenes.Count > 0)
		{
			string sceneName = AssetBundleControl.releaseScenes[0].SceneName;
			this.loadingBundles = null;
			AssetBundleControl.ChangeScene(sceneName, "LoadingScene_UD_MASTER", this);
			yield break;
		}
		yield break;
	}

	private IEnumerator LoadAssetBundleFromFile(string bundlePath, List<AssetBundle> referenceList)
	{
		if (!File.Exists(bundlePath))
		{
			yield break;
		}
		AssetBundleCreateRequest resultAssetBundle = AssetBundle.LoadFromFileAsync(bundlePath);
		yield return new WaitWhile(() => !resultAssetBundle.isDone);
		referenceList.Add(resultAssetBundle.assetBundle);
		yield return null;
		yield break;
	}

	private IEnumerator LoadAssetBundleIntoMemory(string bundlePath, List<AssetBundle> referenceList)
	{
		if (!File.Exists(bundlePath))
		{
			yield break;
		}
		byte[] binary = null;
		try
		{
			binary = File.ReadAllBytes(bundlePath);
		}
		catch (OutOfMemoryException)
		{
			yield break;
		}
		AssetBundleCreateRequest resultAssetBundle = AssetBundle.LoadFromMemoryAsync(binary);
		yield return new WaitWhile(() => !resultAssetBundle.isDone);
		referenceList.Add(resultAssetBundle.assetBundle);
		yield return null;
		yield break;
	}

	private static void BroadcastLoadProgressUpdate(float progress, float from = 0f, float to = 1f)
	{
		progress = Mathf.Lerp(from, to, progress);
		if (AssetBundleControl.OnUpdateLoadProgress != null)
		{
			AssetBundleControl.OnUpdateLoadProgress(progress);
		}
	}

	private void UnloadTest()
	{
		AssetBundle.UnloadAllAssetBundles(true);
	}

	private void Reload()
	{
		SceneManager.LoadScene("init");
	}

	public static Action<float> OnUpdateLoadProgress;

	public static Action OnScenePreLoad;

	public static Action OnSceneReady;

	private static List<AssetBundle> LoadedDefaultBundles = new List<AssetBundle>();

	private static List<string> allListedAssetBundles = new List<string>();

	private static string currentGameSceneName = "";

	private static string currentLoadingSceneName = "";

	private static SceneAssetSet currentAssetSet = null;

	private static AssetBundle currentSceneBundle;

	private static AssetBundle currentSceneDataBundle;

	private static List<AssetBundle> currentSceneExclusiveAssetBundles = new List<AssetBundle>();

	private static List<AssetBundle> currentSetAssetBundles = new List<AssetBundle>();

	private static AssetBundle loadingSceneAssetBundle;

	private static bool loadIntoMemory = false;

	private static Coroutine changeSceneRoutine;

	[SerializeField]
	private bool debugLoadScene;

	[SerializeField]
	private string debugSceneName;

	[SerializeField]
	private ReleaseMap[] releaseMaps;

	[SerializeField]
	private PlatformConfigurations platformConfiguration;

	[SerializeField]
	private bool autoStart;

	private Coroutine loadingBundles;

	private List<AssetBundle> activeSceneBundles = new List<AssetBundle>();

	[Space]
	[Header("DEBUG")]
	[SerializeField]
	private bool cancelLoadScene;

	[SerializeField]
	private bool forcePlatform;

	[SerializeField]
	private RuntimePlatform customPlatform;

	private static List<ReleaseMap.ReleaseScene> releaseScenes = new List<ReleaseMap.ReleaseScene>();
}

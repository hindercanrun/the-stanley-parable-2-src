using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadAssetBundle : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "maps/" + this.mapName));
		if (assetBundle.isStreamedSceneAssetBundle)
		{
			SceneManager.LoadScene(Path.GetFileNameWithoutExtension(assetBundle.GetAllScenePaths()[0]));
		}
	}

	[SerializeField]
	private string mapName = "map1_ud_master";
}

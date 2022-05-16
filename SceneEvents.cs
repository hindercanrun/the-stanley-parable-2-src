using System;
using Nest.Addons;
using UnityEngine;

public class SceneEvents : MonoBehaviour
{
	public void Send(string message)
	{
		Nest.Addons.Singleton<SceneController>.Instance.SendSceneMessage(message);
	}

	public void EnableScene(string sceneName)
	{
		Nest.Addons.Singleton<SceneController>.Instance.LoadScene(sceneName);
	}

	public void DisableScene(string sceneName)
	{
		Nest.Addons.Singleton<SceneController>.Instance.UnloadScene(sceneName);
	}
}

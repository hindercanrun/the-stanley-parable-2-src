using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nest.Addons
{
	public class SceneController : Singleton<SceneController>
	{
		private void OnEnable()
		{
			SceneManager.sceneUnloaded += this.SceneUnloaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneUnloaded -= this.SceneUnloaded;
		}

		public void RegisterPostbox(ScenePostbox postbox)
		{
			if (postbox.gameObject.scene.name == this.PauseScene)
			{
				this._pausePostbox = postbox;
			}
			this._postboxes.Add(postbox.name, new SceneController.PostboxProperty
			{
				Postbox = postbox,
				Scene = postbox.gameObject.scene
			});
		}

		public void RemovePostbox(string postboxName)
		{
		}

		public Scene FindPostboxSceneFromName(string postboxName)
		{
			SceneController.PostboxProperty postboxProperty;
			if (!this._postboxes.TryGetValue(postboxName, out postboxProperty))
			{
				return default(Scene);
			}
			return postboxProperty.Scene;
		}

		public void LoadScene(string scene)
		{
			base.StartCoroutine(this.LoadSceneOperation(scene, LoadSceneMode.Additive));
		}

		public void LoadSceneSingle(string scene)
		{
			base.StartCoroutine(this.LoadSceneOperation(scene, LoadSceneMode.Single));
		}

		public void UnloadScene(string scene)
		{
			SceneManager.UnloadScene(scene);
		}

		public void SendSceneMessage(string message)
		{
			foreach (KeyValuePair<string, SceneController.PostboxProperty> keyValuePair in this._postboxes)
			{
				if (keyValuePair.Value.Scene.isLoaded && keyValuePair.Value.Postbox.gameObject.activeInHierarchy)
				{
					keyValuePair.Value.Postbox.RecieveEvent(message);
				}
			}
		}

		public void SendSceneMessage(string recipient, string message)
		{
			SceneController.PostboxProperty postboxProperty;
			if (this._postboxes.TryGetValue(recipient, out postboxProperty))
			{
				if (postboxProperty.Scene.isLoaded && postboxProperty.Postbox.gameObject.activeInHierarchy)
				{
					postboxProperty.Postbox.RecieveEvent(message);
					return;
				}
			}
			else
			{
				Debug.LogError(string.Format("Could not find the recipient {0}", recipient));
			}
		}

		public void SendPauseEvent(string message)
		{
			if (this._pausePostbox == null)
			{
				Debug.LogWarning("Pause postbox not initialised.");
				return;
			}
			this._pausePostbox.RecieveEvent(message);
		}

		public void DisplayPause()
		{
			if (this._pauseScene == default(Scene) && (this._pauseScene = SceneManager.GetSceneByName(this.PauseScene)) == default(Scene))
			{
				base.StartCoroutine(this.LoadPause(this.PauseScene));
				return;
			}
			if (this._pausePostbox != null)
			{
				this._pausePostbox.RecieveEvent("Pause");
			}
		}

		private IEnumerator LoadPause(string scene)
		{
			yield return this.LoadSceneOperation(scene, LoadSceneMode.Additive);
			this._pauseScene = SceneManager.GetSceneByName(scene);
			while (!this._pauseScene.isLoaded)
			{
				yield return null;
			}
			this.DisplayPause();
			yield break;
		}

		private IEnumerator LoadSceneOperation(string scene, LoadSceneMode sceneMode = LoadSceneMode.Additive)
		{
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, sceneMode);
			if (sceneMode == LoadSceneMode.Single)
			{
				asyncOperation.allowSceneActivation = true;
			}
			if (!asyncOperation.isDone)
			{
				yield return null;
			}
			yield break;
		}

		private void SceneUnloaded(Scene scene)
		{
			if (!scene.isLoaded)
			{
				Resources.UnloadUnusedAssets();
			}
		}

		public string PauseScene = "Pause";

		private ScenePostbox _pausePostbox;

		private Scene _pauseScene;

		private readonly Dictionary<string, SceneController.PostboxProperty> _postboxes = new Dictionary<string, SceneController.PostboxProperty>();

		public class PostboxProperty
		{
			public Scene Scene;

			public ScenePostbox Postbox;
		}

		[Serializable]
		public class Message
		{
			public string Recipient;

			public string Contents;
		}
	}
}

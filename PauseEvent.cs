using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseEvent : MonoBehaviour
{
	private void Awake()
	{
		GameMaster.OnPause += this.OnPause;
		GameMaster.OnResume += this.OnResume;
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (this.ResumeEventOnSceneLoad)
		{
			this.OnResume();
		}
	}

	private void OnDestroy()
	{
		GameMaster.OnPause -= this.OnPause;
		GameMaster.OnResume -= this.OnResume;
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	private void OnPause()
	{
		if (!this.lowendOnly && this.defaultConfigConfigurable.GetBooleanValue())
		{
			return;
		}
		this.OnPauseEvent.Invoke(true);
		this.OnPauseInverseEvent.Invoke(false);
	}

	private void OnResume()
	{
		if (!this.lowendOnly && this.defaultConfigConfigurable.GetBooleanValue())
		{
			return;
		}
		this.OnPauseEvent.Invoke(false);
		this.OnPauseInverseEvent.Invoke(true);
	}

	[SerializeField]
	private bool lowendOnly = true;

	[SerializeField]
	private bool ResumeEventOnSceneLoad;

	[SerializeField]
	private BooleanConfigurable defaultConfigConfigurable;

	[SerializeField]
	private BooleanValueChangedEvent OnPauseEvent;

	[SerializeField]
	private BooleanValueChangedEvent OnPauseInverseEvent;
}

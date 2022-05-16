using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedAction : MonoBehaviour
{
	public void Invoke(float delay)
	{
		base.StartCoroutine(this.WaitSecondsAndDo(delay));
	}

	public void Invoke(int frames)
	{
		base.StartCoroutine(this.WaitFramesAndDo(frames));
	}

	public void CancelAnyDelayedActions()
	{
		base.StopAllCoroutines();
	}

	private IEnumerator WaitSecondsAndDo(float delay)
	{
		yield return new WaitForGameSeconds(delay);
		UnityEvent unityEvent = this.action;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		yield break;
	}

	private IEnumerator WaitFramesAndDo(int frames)
	{
		int num;
		for (int i = 0; i < frames; i = num + 1)
		{
			if (Singleton<GameMaster>.Instance.GameDeltaTime != 0f)
			{
				yield return null;
			}
			num = i;
		}
		UnityEvent unityEvent = this.action;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		yield break;
	}

	[SerializeField]
	private UnityEvent action;
}

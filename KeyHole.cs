using System;
using UnityEngine;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
	private void Start()
	{
		if (this.m_MyEvent == null)
		{
			this.m_MyEvent = new UnityEvent();
		}
	}

	private void Update()
	{
		if (Singleton<GameMaster>.Instance.stanleyActions.HoleTeleportAction.WasPressed)
		{
			this.m_MyEvent.Invoke();
		}
	}

	public UnityEvent m_MyEvent;
}

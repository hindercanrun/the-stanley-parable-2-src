using System;
using UnityEngine;
using UnityEngine.Events;

public class WalkingBackwards : MonoBehaviour
{
	private void Start()
	{
		if (this.m_MyEvent == null)
		{
			this.m_MyEvent = new UnityEvent();
		}
		if (this.m_MyEventB == null)
		{
			this.m_MyEventB = new UnityEvent();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown("w"))
		{
			this.m_MyEvent.Invoke();
		}
		if (Input.GetKeyUp("w"))
		{
			this.m_MyEventB.Invoke();
		}
	}

	public UnityEvent m_MyEvent;

	public UnityEvent m_MyEventB;
}

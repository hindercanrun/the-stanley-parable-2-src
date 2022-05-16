using System;
using UnityEngine;

public class SimpleEventCaller : MonoBehaviour
{
	public void CallSimpleEvent(SimpleEvent simpleEvent)
	{
		simpleEvent.Call();
	}
}

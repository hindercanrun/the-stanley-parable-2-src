using System;
using UnityEngine;

public class TSPArea : MonoBehaviour
{
	public GameObject StaticObjects;

	public GameObject DynamicObjects;

	public GameObject LightsBaked;

	public GameObject LightsRealtime;

	public GameObject Triggers;

	public GameObject Audio;

	public GameObject Misc;

	public GameObject FX;

	public GameObject Unsorted;

	public GameObject StaticRoot;

	public GameObject DynamicRoot;

	[SerializeField]
	[HideInInspector]
	private bool setup;

	public enum AreaRootType
	{
		Static,
		Dynamic,
		LightBaked,
		LightRealtime,
		Trigger,
		Audio,
		Misc,
		FX,
		Unsorted
	}
}

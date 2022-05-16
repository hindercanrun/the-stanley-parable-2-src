using System;
using System.Collections.Generic;
using UnityEngine;

public class RocketLeuageDebugButtons : MonoBehaviour
{
	public static RocketLeuageDebugButtons Instance { get; private set; }

	private void Awake()
	{
		RocketLeuageDebugButtons.Instance = this;
		this.registeredObjects = new Dictionary<string, List<GameObject>>();
	}

	public void RegisterObject(string id, GameObject go)
	{
		if (!this.registeredObjects.ContainsKey(id))
		{
			this.registeredObjects[id] = new List<GameObject>();
		}
		this.registeredObjects[id].Add(go);
	}

	private void OnGUI()
	{
		if (this.registeredObjects == null)
		{
			return;
		}
		if (this.guiStyle == null)
		{
			this.guiStyle = new GUIStyle("button");
		}
		GUILayout.Space(this.leadingSpace);
		this.guiStyle.fontSize = this.fontSize * Screen.width / 1920;
		foreach (KeyValuePair<string, List<GameObject>> keyValuePair in this.registeredObjects)
		{
			if (GUILayout.Button(keyValuePair.Key, this.guiStyle, Array.Empty<GUILayoutOption>()))
			{
				foreach (GameObject gameObject in keyValuePair.Value)
				{
					gameObject.SetActive(!gameObject.activeInHierarchy);
				}
			}
		}
	}

	private Dictionary<string, List<GameObject>> registeredObjects;

	public float leadingSpace = 100f;

	public int fontSize = 20;

	public bool scaleByResolutionWidth = true;

	private GUIStyle guiStyle;
}

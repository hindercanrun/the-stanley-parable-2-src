using System;
using System.Collections.Generic;
using StanleyUI;
using TMPro;
using UnityEngine;

public class DebugSettingsButtonsGenerator : MonoBehaviour
{
	private void Start()
	{
	}

	private IEnumerable<Configurable> ConfigurableList
	{
		get
		{
			return this.configurablesResettor.allConfigurables;
		}
	}

	public static void DestroyEditorSafe(GameObject go)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(go);
			return;
		}
		Object.DestroyImmediate(go);
	}

	public static GameObject InstantiateEditorSafe(GameObject prefab)
	{
		return Object.Instantiate<GameObject>(prefab);
	}

	[ContextMenu("DestroyDebugButtonInstances")]
	public void DestroyDebugButtonInstances()
	{
		for (int i = 0; i < this.debugButtonInstances.Count; i++)
		{
			DebugSettingsButtonsGenerator.DestroyEditorSafe(this.debugButtonInstances[i]);
		}
		this.debugButtonInstances.Clear();
	}

	[ContextMenu("CreateDebugButtonInstances")]
	public void CreateDebugButtonInstances()
	{
		this.DestroyDebugButtonInstances();
		foreach (Configurable configurable in this.ConfigurableList)
		{
			GameObject gameObject;
			if (configurable is IntConfigurable)
			{
				gameObject = DebugSettingsButtonsGenerator.InstantiateEditorSafe(this.debugIntSliderPrefab);
			}
			else if (configurable is BooleanConfigurable)
			{
				gameObject = DebugSettingsButtonsGenerator.InstantiateEditorSafe(this.debugTogglePrefab);
			}
			else
			{
				if (!(configurable is StringConfigurable))
				{
					continue;
				}
				gameObject = DebugSettingsButtonsGenerator.InstantiateEditorSafe(this.debugStringPrefab);
			}
			gameObject.transform.SetParent(base.transform.parent);
			gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
			GameObject gameObject2 = gameObject;
			gameObject2.name = gameObject2.name + " (" + configurable.name + ")";
			gameObject.transform.localScale = Vector3.one;
			Array.Find<TextMeshProUGUI>(gameObject.GetComponentsInChildren<TextMeshProUGUI>(), (TextMeshProUGUI x) => x.name == "Label").text = configurable.name.Replace("CONFIGURABLE_", "");
			StanleyMenuUIEntityUtility component = gameObject.GetComponent<StanleyMenuUIEntityUtility>();
			if (component != null)
			{
				component.targetConfigurable = configurable;
			}
			if (configurable is IntConfigurable)
			{
				gameObject.GetComponent<StanleyMenuSlider>().minValue = (float)(configurable as IntConfigurable).MinValue;
				gameObject.GetComponent<StanleyMenuSlider>().maxValue = (float)(configurable as IntConfigurable).MaxValue;
			}
			this.debugButtonInstances.Add(gameObject);
		}
	}

	public ResetableConfigurablesList configurablesResettor;

	public Configurable[] testConfigs;

	public List<GameObject> debugButtonInstances;

	public GameObject debugTogglePrefab;

	public GameObject debugIntSliderPrefab;

	public GameObject debugStringPrefab;
}

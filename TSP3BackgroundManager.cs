using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TSP3BackgroundManager : MonoBehaviour
{
	private void Awake()
	{
		if (TSP3BackgroundManager.playthroughSeed == -1)
		{
			TSP3BackgroundManager.playthroughSeed = Random.Range(0, 100000);
		}
		IntConfigurable intConfigurable = this.sequelNumber;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.SetupBackgroundElementsLD));
		GameMaster.OnPrepareLoadingLevel += this.SetupBackgroundElements;
		AssetBundleControl.OnSceneReady = (Action)Delegate.Combine(AssetBundleControl.OnSceneReady, new Action(this.SetupBackgroundElements));
		foreach (Transform transform in new Transform[]
		{
			this.uiObjectHolder,
			this.objectHolder
		})
		{
			for (int j = 0; j < transform.childCount; j++)
			{
				Object.Destroy(transform.GetChild(j).gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		IntConfigurable intConfigurable = this.sequelNumber;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.SetupBackgroundElementsLD));
		GameMaster.OnPrepareLoadingLevel -= this.SetupBackgroundElements;
		AssetBundleControl.OnSceneReady = (Action)Delegate.Remove(AssetBundleControl.OnSceneReady, new Action(this.SetupBackgroundElements));
		this.RemoveAllInstantiatedObjects();
	}

	private void Start()
	{
		this.SetupBackgroundElements();
	}

	private void SetupBackgroundElementsLD(LiveData ld)
	{
		this.SetupBackgroundElements();
	}

	private void SetupBackgroundElements()
	{
		int intValue = this.sequelNumber.GetIntValue();
		int seed = intValue * intValue * TSP3BackgroundManager.playthroughSeed;
		TSP3BackgroundRandomSet tsp3BackgroundRandomSet;
		if (intValue < this.presetBackgrounds.Count)
		{
			tsp3BackgroundRandomSet = this.presetBackgrounds[intValue];
		}
		else if (intValue < 100)
		{
			tsp3BackgroundRandomSet = this.afterPresets;
		}
		else
		{
			tsp3BackgroundRandomSet = ((this.after100Presets != null) ? this.after100Presets : this.afterPresets);
		}
		if (tsp3BackgroundRandomSet != null)
		{
			this.SetupTSP3Menu(tsp3BackgroundRandomSet, seed);
			return;
		}
		Debug.LogError("Could not find TSP3+ background setup data");
	}

	private void RemoveAllInstantiatedObjects()
	{
		foreach (GameObject obj in this.instantiatedGameObjects)
		{
			Object.Destroy(obj);
		}
		this.instantiatedGameObjects.Clear();
	}

	private void SetupTSP3Menu(TSP3BackgroundRandomSet setupData, int seed)
	{
		this.amplifyColor.LutTexture = setupData.GetLUTImage(seed);
		this.backgroundImage.texture = setupData.GetBackgroundImage(seed);
		this.RemoveAllInstantiatedObjects();
		foreach (GameObject gameObject in setupData.GetAllToInstantiate(seed))
		{
			if (!(gameObject == null))
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
				GameObject gameObject3 = gameObject2;
				gameObject3.name += string.Format(" {0} {1}", Time.frameCount, seed);
				this.instantiatedGameObjects.Add(gameObject2);
				RectTransform component = gameObject2.GetComponent<RectTransform>();
				if (component != null)
				{
					gameObject2.transform.parent = this.uiObjectHolder;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localRotation = Quaternion.identity;
					component.anchorMin = Vector3.zero;
					component.anchorMax = Vector3.one;
					component.anchoredPosition = Vector3.zero;
					component.sizeDelta = Vector3.zero;
					component.pivot = Vector3.one * 0.5f;
				}
				else
				{
					gameObject2.transform.parent = this.objectHolder;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localRotation = Quaternion.identity;
				}
			}
		}
	}

	[Header("LEAVE THE FIRST 3 ELEMENTS AS NULL")]
	public List<TSP3BackgroundRandomSet> presetBackgrounds;

	public TSP3BackgroundRandomSet afterPresets;

	public TSP3BackgroundRandomSet after100Presets;

	[Header("Data")]
	public IntConfigurable sequelNumber;

	public RawImage backgroundImage;

	public AmplifyColorEffect amplifyColor;

	public Transform uiObjectHolder;

	[InspectorButton("SetupBackgroundElements", null)]
	public Transform objectHolder;

	private List<GameObject> instantiatedGameObjects = new List<GameObject>();

	private static int playthroughSeed = -1;
}

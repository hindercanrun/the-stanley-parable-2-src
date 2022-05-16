using System;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
public class TerrainDetailQualitySettings : MonoBehaviour
{
	private void Start()
	{
		if (CullForSwitchController.IsSwitchEnvironment || (Application.platform == RuntimePlatform.WindowsEditor && this.forceLowInEditor))
		{
			this.SetLow();
			return;
		}
		this.SetHigh();
	}

	[ContextMenu("SetHigh")]
	public void SetHigh()
	{
		Debug.Log("Setting high quality terrain detail settings");
		this.SetQuality(this.highQuality);
	}

	[ContextMenu("SetLow")]
	public void SetLow()
	{
		Debug.Log("Setting low quality terrain detail settings");
		this.SetQuality(this.lowQuality);
	}

	public void SetQuality(TerrainDetailQualitySettings.TerrainQualitySettings q)
	{
		Terrain component = base.GetComponent<Terrain>();
		component.detailObjectDistance = q.drawDistanceMax;
		component.detailObjectDensity = q.detailDensity;
		component.basemapDistance = q.baseMapDistance;
		Shader.SetGlobalFloat("_GrassDetailFadeOutDistance", q.drawDistanceMax);
		Shader.SetGlobalFloat("_GrassDetailFadeInDistance", q.drawDistanceFadeStart);
	}

	public TerrainDetailQualitySettings.TerrainQualitySettings highQuality;

	public TerrainDetailQualitySettings.TerrainQualitySettings lowQuality;

	public bool forceLowInEditor;

	[Serializable]
	public class TerrainQualitySettings
	{
		public float drawDistanceMax = 60f;

		public float drawDistanceFadeStart = 50f;

		[Range(0f, 1f)]
		public float detailDensity = 1f;

		public float baseMapDistance = 500f;
	}
}

using System;
using UnityEngine;

public class SkyboxCycler : MonoBehaviour
{
	private void Awake()
	{
		if (this.SetOnAwake && this.defaultConfigurationConfigurable != null)
		{
			RenderSettings.skybox = (this.defaultConfigurationConfigurable.GetBooleanValue() ? this.skyboxMaterials : this.skyboxMaterialsLowEnd)[0];
		}
	}

	public void CycleSkybox()
	{
		Material[] array;
		if (this.defaultConfigurationConfigurable != null)
		{
			array = (this.defaultConfigurationConfigurable.GetBooleanValue() ? this.skyboxMaterials : this.skyboxMaterialsLowEnd);
		}
		else
		{
			array = this.skyboxMaterials;
		}
		int num = Array.FindIndex<Material>(array, (Material s) => s == RenderSettings.skybox);
		if (num != -1)
		{
			this.currentIndex = num;
		}
		this.currentIndex = (this.currentIndex + 1) % this.skyboxMaterials.Length;
		RenderSettings.skybox = array[this.currentIndex];
	}

	public Material[] skyboxMaterials;

	public Material[] skyboxMaterialsLowEnd;

	[SerializeField]
	private Configurable defaultConfigurationConfigurable;

	private int currentIndex = -1;

	public bool SetOnAwake;
}

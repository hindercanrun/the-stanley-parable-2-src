using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralSkyboxAndCloudBlender : MonoBehaviour
{
	private void Update()
	{
		this.blend = Mathf.Clamp(this.blend, 0f, (float)(this.skyboxDefinitions.Count - 1));
		int num = Mathf.FloorToInt(this.blend);
		float num2 = this.blend % 1f;
		if (num == this.skyboxDefinitions.Count - 1)
		{
			num--;
			num2 = 1f;
		}
		ProceduralSkyboxAndCloudBlender.SkyboxDefinition skyboxDefinition = this.skyboxDefinitions[num];
		ProceduralSkyboxAndCloudBlender.SkyboxDefinition skyboxDefinition2 = this.skyboxDefinitions[num + 1];
		foreach (string name in this.ColourProperties)
		{
			this.skyboxOutput.SetColor(name, Color.Lerp(skyboxDefinition.skybox.GetColor(name), skyboxDefinition2.skybox.GetColor(name), num2));
		}
		foreach (string name2 in this.FloatProperties)
		{
			this.skyboxOutput.SetFloat(name2, Mathf.Lerp(skyboxDefinition.skybox.GetFloat(name2), skyboxDefinition2.skybox.GetFloat(name2), num2));
		}
		foreach (string name3 in this.VectorProperties)
		{
			this.skyboxOutput.SetVector(name3, Vector4.Lerp(skyboxDefinition.skybox.GetVector(name3), skyboxDefinition2.skybox.GetVector(name3), num2));
		}
		for (int j = 0; j < this.skyboxDefinitions.Count; j++)
		{
			bool flag = j == num || j == num + 1;
			if (this.skyboxDefinitions[j].skyDome.gameObject.activeSelf != flag)
			{
				this.skyboxDefinitions[j].skyDome.gameObject.SetActive(flag);
				this.skyboxDefinitions[j].skyDome.sharedMaterial.SetFloat("_Fade", 0f);
			}
			if (j == num)
			{
				this.skyboxDefinitions[j].skyDome.sharedMaterial.SetFloat("_Fade", 1f - num2);
			}
			if (j == num + 1)
			{
				this.skyboxDefinitions[j].skyDome.sharedMaterial.SetFloat("_Fade", num2);
			}
		}
	}

	public List<ProceduralSkyboxAndCloudBlender.SkyboxDefinition> skyboxDefinitions;

	public Material skyboxOutput;

	public float blend;

	private string[] ColourProperties = new string[]
	{
		"_topColor",
		"_horizonColor",
		"_bottomColor",
		"_sunColor",
		"_horizonHaloColor",
		"_sunHaloColor"
	};

	private string[] FloatProperties = new string[]
	{
		"_topSkyColorBlending",
		"_bottomSkyColorBlending",
		"_sunIntensity",
		"_sunSharpness",
		"_horizonHaloIntensity",
		"_horizonHaloSize",
		"_sunHaloIntensity",
		"_sunHaloSize"
	};

	private string[] VectorProperties = new string[]
	{
		"_sunDir"
	};

	[Serializable]
	public class SkyboxDefinition
	{
		public MeshRenderer skyDome;

		public Material skybox;
	}
}

using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderValues : MonoBehaviour
{
	private void Awake()
	{
		this.SetVariables();
	}

	private void SetVariables()
	{
		Shader.SetGlobalFloat("Stanley_LightmapSimpleContrast", this.Stanley_LightmapSimpleContrast);
		Shader.SetGlobalFloat("Stanley_LightmapPower", this.Stanley_LightmapPower);
		Shader.SetGlobalInt("Stanley_AverageRoughnessMip", this.Stanley_AverageRoughnessMip);
		Shader.SetGlobalFloat("Stanley_AverageRoughnessContrast", this.Stanley_AverageRoughnessContrast);
		Shader.SetGlobalFloat("Stanley_AverageRoughnessMultiply", this.Stanley_AverageRoughnessMultiply);
		Shader.SetGlobalFloat("Stanley_ViewAngleContrast", this.Stanley_ViewAngleContrast);
		Shader.SetGlobalFloat("Stanley_ViewAngleMultiplier", this.Stanley_ViewAngleMultiplier);
		Shader.SetGlobalFloat("Stanley_ReflectionMasterContrast", this.Stanley_ReflectionMasterContrast);
		Shader.SetGlobalFloat("Stanley_ReflectionMasterMultiplier", this.Stanley_ReflectionMasterMultiplier);
		Shader.SetGlobalFloat("Stanley_ReflectionMaskMultiplier", this.Stanley_ReflectionMaskMultiplier);
		Shader.SetGlobalFloat("Stanley_ReflectionDesaturation", this.Stanley_ReflectionDesaturation);
		Shader.SetGlobalFloat("Stanley_ReflectionLightingContrast", this.Stanley_ReflectionLightingContrast);
		Shader.SetGlobalFloat("StylizedFresnelBias", this.StylizedFresnelBias);
		Shader.SetGlobalFloat("StylizedFresnelScale", this.StylizedFresnelScale);
		Shader.SetGlobalFloat("StylizedFresnelPower", this.StylizedFresnelPower);
	}

	[Header("Lightmap/Lighting")]
	[SerializeField]
	private float Stanley_LightmapSimpleContrast = 1f;

	[SerializeField]
	private float Stanley_ReflectionLightingContrast = 2.75f;

	[SerializeField]
	private float Stanley_LightmapPower = 1f;

	[Header("Roughness")]
	[SerializeField]
	private int Stanley_AverageRoughnessMip = 3;

	[SerializeField]
	private float Stanley_AverageRoughnessContrast = 1.05f;

	[SerializeField]
	private float Stanley_AverageRoughnessMultiply = 1.2f;

	[Header("View Angle")]
	[SerializeField]
	private float Stanley_ViewAngleContrast = 1f;

	[SerializeField]
	private float Stanley_ViewAngleMultiplier = 1.2f;

	[Header("Reflection Master")]
	[SerializeField]
	private float Stanley_ReflectionMasterContrast = 2f;

	[SerializeField]
	private float Stanley_ReflectionMasterMultiplier = 20f;

	[Header("Reflection Mask")]
	[SerializeField]
	[HideInInspector]
	private float Stanley_ReflectionMaskMultiplier = 10f;

	[Header("Desaturation")]
	[SerializeField]
	private float Stanley_ReflectionDesaturation = 0.6f;

	[Header("Fresnel")]
	[SerializeField]
	private float StylizedFresnelBias = 0.2f;

	[SerializeField]
	private float StylizedFresnelScale = 1.2f;

	[SerializeField]
	private float StylizedFresnelPower = 1.2f;
}

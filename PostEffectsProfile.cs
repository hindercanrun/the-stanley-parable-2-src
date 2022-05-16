using System;
using AmplifyBloom;
using AmplifyColor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Post Effects Profile", menuName = "Post Effects Control/Create new Post Effects Profile")]
public class PostEffectsProfile : ProfileBase
{
	public PostEffectsProfile(AmplifyColorEffect colorSettings, AmplifyBloomEffect bloomSettings)
	{
		if (colorSettings != null)
		{
			this.Data.SetAmplifyColor = true;
			if (colorSettings.LutTexture != null)
			{
				this.Data.LutTexture = colorSettings.LutTexture;
			}
			this.Data.Exposure = colorSettings.Exposure;
			this.Data.Tonemapper = colorSettings.Tonemapper;
		}
		if (bloomSettings != null)
		{
			this.Data.SetAmplifyBloom = true;
			this.Data.BloomIntensity = bloomSettings.OverallIntensity;
			this.Data.BloomThreshold = bloomSettings.OverallThreshold;
		}
		this.Data.SetFog = true;
		this.Data.FogColor = RenderSettings.fogColor;
		this.Data.FogStartDistance = RenderSettings.fogStartDistance;
		this.Data.FogEndDistance = RenderSettings.fogEndDistance;
		this.Data.FogDensity = RenderSettings.fogDensity;
		this.Data.FogMode = RenderSettings.fogMode;
	}

	[SerializeField]
	public PostEffectsProfile.InterpolationData Data;

	[Serializable]
	public struct InterpolationData
	{
		public float fogEndDistance { get; internal set; }

		[Header("Amplify Color")]
		public bool SetAmplifyColor;

		public Tonemapping Tonemapper;

		public Texture LutTexture;

		public float Exposure;

		public float LinearWhitePoint;

		public bool SetAmplifyBloom;

		public float BloomIntensity;

		public float BloomThreshold;

		[Header("Fog")]
		public bool SetFog;

		public Color FogColor;

		public float FogStartDistance;

		public float FogEndDistance;

		public float FogDensity;

		public FogMode FogMode;
	}
}

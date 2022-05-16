using System;
using AmplifyBloom;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostEffectsCamera : ProfileInterpolator
{
	public float ExposureMultipler
	{
		get
		{
			return 1f;
		}
	}

	private float AmplifyColor_Exposure
	{
		get
		{
			return this.lastSavedExposure;
		}
		set
		{
			this.lastSavedExposure = value;
			this.amplifyColor.Exposure = this.lastSavedExposure * this.ExposureMultipler;
		}
	}

	private void ResetExposure(LiveData ld)
	{
		this.AmplifyColor_Exposure = this.AmplifyColor_Exposure;
	}

	public PostEffectsProfile LastIntorpolatedEffectsProfile { get; private set; }

	private void Awake()
	{
		this.mobileBloom = base.GetComponent<MobileBloom>();
		this.amplifyColor = base.GetComponent<AmplifyColorEffect>();
		this.amplifyBloom = base.GetComponent<AmplifyBloomEffect>();
		base.SetupResetOnLevelLoad(true);
		PostEffectsVolume.OnEnterVolume = (Action<PostEffectsVolume>)Delegate.Combine(PostEffectsVolume.OnEnterVolume, new Action<PostEffectsVolume>(base.OnEnterVolume));
		PostEffectsVolume.OnExitVolume = (Action<PostEffectsVolume>)Delegate.Combine(PostEffectsVolume.OnExitVolume, new Action<PostEffectsVolume>(base.OnExitVolume));
		if (this.brightnessFloat != null)
		{
			FloatConfigurable floatConfigurable = this.brightnessFloat;
			floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(floatConfigurable.OnValueChanged, new Action<LiveData>(this.ResetExposure));
		}
		if (this.defaultProfile != null)
		{
			this.SetProfileInstant(this.defaultProfile);
		}
	}

	private void OnDestroy()
	{
		base.SetupResetOnLevelLoad(false);
		PostEffectsVolume.OnEnterVolume = (Action<PostEffectsVolume>)Delegate.Remove(PostEffectsVolume.OnEnterVolume, new Action<PostEffectsVolume>(base.OnEnterVolume));
		PostEffectsVolume.OnExitVolume = (Action<PostEffectsVolume>)Delegate.Remove(PostEffectsVolume.OnExitVolume, new Action<PostEffectsVolume>(base.OnExitVolume));
		if (this.brightnessFloat != null)
		{
			FloatConfigurable floatConfigurable = this.brightnessFloat;
			floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(floatConfigurable.OnValueChanged, new Action<LiveData>(this.ResetExposure));
		}
	}

	public override void Interpolate(float lerpValue)
	{
		if (this.endData.SetAmplifyColor)
		{
			this.amplifyColor.BlendAmount = lerpValue;
			this.AmplifyColor_Exposure = Mathf.Lerp(this.startData.Exposure, this.endData.Exposure, lerpValue);
			this.amplifyColor.Tonemapper = this.endData.Tonemapper;
			this.amplifyColor.LinearWhitePoint = Mathf.Lerp(this.startData.LinearWhitePoint, this.endData.LinearWhitePoint, lerpValue);
		}
		if (this.endData.SetFog)
		{
			RenderSettings.fog = true;
			RenderSettings.fogMode = this.endData.FogMode;
			RenderSettings.fogColor = Color.Lerp(this.startData.FogColor, this.endData.FogColor, lerpValue);
			RenderSettings.fogStartDistance = Mathf.Lerp(this.startData.FogStartDistance, this.endData.FogStartDistance, lerpValue);
			RenderSettings.fogEndDistance = Mathf.Lerp(this.startData.FogEndDistance, this.endData.FogEndDistance, lerpValue);
			RenderSettings.fogDensity = Mathf.Lerp(this.startData.FogStartDistance, this.endData.FogEndDistance, lerpValue);
		}
		if (this.endData.SetAmplifyBloom)
		{
			this.amplifyBloom.OverallIntensity = Mathf.Lerp(this.startData.BloomIntensity, this.endData.BloomIntensity, lerpValue);
			this.amplifyBloom.OverallThreshold = Mathf.Lerp(this.startData.BloomThreshold, this.endData.BloomThreshold, lerpValue);
			if (this.mobileBloom != null)
			{
				this.mobileBloom.BloomAmount = Mathf.Lerp(this.startData.BloomIntensity * this.mobileBloomIntensityMultiplier, this.endData.BloomIntensity * this.mobileBloomIntensityMultiplier, lerpValue);
			}
			if (this.mobileBloom != null)
			{
				this.mobileBloom.BloomThreshold = Mathf.Lerp(this.startData.BloomThreshold, this.endData.BloomThreshold, lerpValue);
			}
		}
	}

	public override void LinearInterpolationComplete()
	{
		this.amplifyColor.LutBlendTexture = null;
	}

	public override void InterpolateToProfile(ProfileBase profile, float duration, AnimationCurve curve)
	{
		PostEffectsProfile postEffectsProfile = profile as PostEffectsProfile;
		this.LastIntorpolatedEffectsProfile = postEffectsProfile;
		if (this.interpolationRoutine != null)
		{
			base.StopCoroutine(this.interpolationRoutine);
			this.interpolationRoutine = null;
		}
		if (duration == 0f)
		{
			this.SetProfileInstant(profile);
			return;
		}
		if (postEffectsProfile.Data.SetAmplifyColor)
		{
			if (this.amplifyColor.LutBlendTexture != null)
			{
				this.amplifyColor.LutTexture = this.amplifyColor.LutBlendTexture;
			}
			if (postEffectsProfile.Data.LutTexture != null)
			{
				this.amplifyColor.LutBlendTexture = postEffectsProfile.Data.LutTexture;
			}
			this.amplifyColor.BlendAmount = 0f;
		}
		this.startData.Exposure = this.AmplifyColor_Exposure;
		this.startData.LinearWhitePoint = this.amplifyColor.LinearWhitePoint;
		this.startData.BloomIntensity = this.amplifyBloom.OverallIntensity;
		this.startData.BloomThreshold = this.amplifyBloom.OverallThreshold;
		this.startData.FogColor = RenderSettings.fogColor;
		this.startData.FogStartDistance = RenderSettings.fogStartDistance;
		this.startData.FogEndDistance = RenderSettings.fogEndDistance;
		this.startData.FogDensity = RenderSettings.fogDensity;
		this.startData.FogMode = RenderSettings.fogMode;
		this.endData = postEffectsProfile.Data;
		this.interpolationRoutine = base.StartCoroutine(base.LinearInterpolation(duration, curve));
	}

	public override void SetProfileInstant(ProfileBase profile)
	{
		PostEffectsProfile postEffectsProfile = profile as PostEffectsProfile;
		if (postEffectsProfile.Data.SetAmplifyColor)
		{
			this.amplifyColor.BlendAmount = 0f;
			this.amplifyColor.LutTexture = postEffectsProfile.Data.LutTexture;
			this.AmplifyColor_Exposure = postEffectsProfile.Data.Exposure;
			this.amplifyColor.LinearWhitePoint = postEffectsProfile.Data.LinearWhitePoint;
		}
		if (postEffectsProfile.Data.SetFog)
		{
			RenderSettings.fogColor = postEffectsProfile.Data.FogColor;
			RenderSettings.fogStartDistance = postEffectsProfile.Data.FogStartDistance;
			RenderSettings.fogEndDistance = postEffectsProfile.Data.FogEndDistance;
			RenderSettings.fogDensity = postEffectsProfile.Data.FogDensity;
			RenderSettings.fogMode = postEffectsProfile.Data.FogMode;
		}
		if (postEffectsProfile.Data.SetAmplifyBloom)
		{
			this.amplifyBloom.OverallIntensity = postEffectsProfile.Data.BloomIntensity;
			this.amplifyBloom.OverallThreshold = postEffectsProfile.Data.BloomThreshold;
			if (this.mobileBloom != null)
			{
				this.mobileBloom.BloomAmount = postEffectsProfile.Data.BloomIntensity * this.mobileBloomIntensityMultiplier;
			}
			if (this.mobileBloom != null)
			{
				this.mobileBloom.BloomThreshold = postEffectsProfile.Data.BloomThreshold;
			}
		}
		if (this.amplifyColor != null)
		{
			this.amplifyColor.LutBlendTexture = null;
		}
		if (this.currentProfile != null)
		{
			this.previousProfile = this.currentProfile;
		}
		this.currentProfile = postEffectsProfile;
	}

	public override void FeatherToProfile(ProfileBase profile, VolumeBase volume, AnimationCurve curve)
	{
		PostEffectsProfile postEffectsProfile = profile as PostEffectsProfile;
		if (this.interpolationRoutine != null)
		{
			base.StopCoroutine(this.interpolationRoutine);
			this.interpolationRoutine = null;
		}
		if (postEffectsProfile.Data.SetAmplifyColor)
		{
			if (this.amplifyColor.LutBlendTexture != null)
			{
				this.amplifyColor.LutTexture = this.amplifyColor.LutBlendTexture;
			}
			if (postEffectsProfile.Data.LutTexture != null)
			{
				this.amplifyColor.LutBlendTexture = postEffectsProfile.Data.LutTexture;
			}
			this.amplifyColor.BlendAmount = 0f;
		}
		this.startData.Exposure = this.AmplifyColor_Exposure;
		this.startData.BloomIntensity = this.amplifyBloom.OverallIntensity;
		this.startData.BloomThreshold = this.amplifyBloom.OverallThreshold;
		this.startData.FogColor = RenderSettings.fogColor;
		this.startData.FogStartDistance = RenderSettings.fogStartDistance;
		this.startData.FogEndDistance = RenderSettings.fogEndDistance;
		this.startData.FogDensity = RenderSettings.fogDensity;
		this.startData.FogMode = RenderSettings.fogMode;
		this.endData = postEffectsProfile.Data;
		this.interpolationRoutine = base.StartCoroutine(base.DistanceBasedInterpolation(volume, curve));
	}

	private AmplifyColorEffect amplifyColor;

	private AmplifyBloomEffect amplifyBloom;

	private MobileBloom mobileBloom;

	[SerializeField]
	private float mobileBloomIntensityMultiplier = 2f;

	public FloatConfigurable brightnessFloat;

	private float lastSavedExposure;

	private PostEffectsProfile.InterpolationData startData;

	private PostEffectsProfile.InterpolationData endData;
}

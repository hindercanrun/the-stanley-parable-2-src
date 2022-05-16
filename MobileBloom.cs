using System;
using UnityEngine;
using UnityEngine.XR;

[ExecuteInEditMode]
public class MobileBloom : MonoBehaviour
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.BloomDiffusion == 0f && this.BloomAmount == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (XRSettings.enabled)
		{
			this.half = XRSettings.eyeTextureDesc;
			this.half.height = this.half.height / 2;
			this.half.width = this.half.width / 2;
			this.quarter = XRSettings.eyeTextureDesc;
			this.quarter.height = this.quarter.height / 4;
			this.quarter.width = this.quarter.width / 4;
			this.eighths = XRSettings.eyeTextureDesc;
			this.eighths.height = this.eighths.height / 8;
			this.eighths.width = this.eighths.width / 8;
			this.sixths = XRSettings.eyeTextureDesc;
			this.sixths.height = this.sixths.height / ((XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass) ? 8 : 16);
			this.sixths.width = this.sixths.width / ((XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass) ? 8 : 16);
		}
		else
		{
			this.half = new RenderTextureDescriptor(Screen.width / 1, Screen.height / 1);
			this.quarter = new RenderTextureDescriptor(Screen.width / 2, Screen.height / 2);
			this.eighths = new RenderTextureDescriptor(Screen.width / 4, Screen.height / 4);
			this.sixths = new RenderTextureDescriptor(Screen.width / 8, Screen.height / 8);
		}
		this.material.SetFloat(MobileBloom.blurAmountString, this.BloomDiffusion);
		this.material.SetColor(MobileBloom.bloomColorString, this.BloomAmount * this.BloomColor);
		this.knee = this.BloomThreshold * this.BloomSoftness;
		this.material.SetVector(MobileBloom.blDataString, new Vector4(this.BloomThreshold, this.BloomThreshold - this.knee, 2f * this.knee, 1f / (4f * this.knee + 1E-05f)));
		this.numberOfPasses = Mathf.Clamp(Mathf.CeilToInt(this.BloomDiffusion * 4f), 1, 4);
		this.material.SetFloat(MobileBloom.blurAmountString, (this.numberOfPasses > 1) ? ((this.BloomDiffusion > 1f) ? this.BloomDiffusion : ((this.BloomDiffusion * 4f - (float)Mathf.FloorToInt(this.BloomDiffusion * 4f - 0.001f)) * 0.5f + 0.5f)) : (this.BloomDiffusion * 4f));
		RenderTexture renderTexture = null;
		if (this.numberOfPasses == 1 || this.BloomDiffusion == 0f)
		{
			renderTexture = RenderTexture.GetTemporary(this.half);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.material, 0);
		}
		else if (this.numberOfPasses == 2)
		{
			renderTexture = RenderTexture.GetTemporary(this.half);
			RenderTexture temporary = RenderTexture.GetTemporary(this.quarter);
			renderTexture.filterMode = FilterMode.Bilinear;
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, temporary, this.material, 0);
			Graphics.Blit(temporary, renderTexture, this.material, 1);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (this.numberOfPasses == 3)
		{
			renderTexture = RenderTexture.GetTemporary(this.quarter);
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.eighths);
			renderTexture.filterMode = FilterMode.Bilinear;
			temporary2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.material, 0);
			Graphics.Blit(renderTexture, temporary2, this.material, 1);
			Graphics.Blit(temporary2, renderTexture, this.material, 1);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else if (this.numberOfPasses == 4)
		{
			renderTexture = RenderTexture.GetTemporary(this.quarter);
			RenderTexture temporary3 = RenderTexture.GetTemporary(this.eighths);
			RenderTexture temporary4 = RenderTexture.GetTemporary(this.sixths);
			renderTexture.filterMode = FilterMode.Bilinear;
			temporary3.filterMode = FilterMode.Bilinear;
			temporary4.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.material, 0);
			Graphics.Blit(renderTexture, temporary3, this.material, 1);
			Graphics.Blit(temporary3, temporary4, this.material, 1);
			Graphics.Blit(temporary4, temporary3, this.material, 1);
			Graphics.Blit(temporary3, renderTexture, this.material, 1);
			RenderTexture.ReleaseTemporary(temporary3);
			RenderTexture.ReleaseTemporary(temporary4);
		}
		this.material.SetTexture(MobileBloom.bloomTexString, renderTexture);
		RenderTexture.ReleaseTemporary(renderTexture);
		Graphics.Blit(source, destination, this.material, 2);
	}

	[Range(0f, 2f)]
	public float BloomDiffusion = 2f;

	public Color BloomColor = Color.white;

	[Range(0f, 5f)]
	public float BloomAmount = 1f;

	[Range(0f, 1f)]
	public float BloomThreshold;

	[Range(0f, 1f)]
	public float BloomSoftness;

	private static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");

	private static readonly int bloomColorString = Shader.PropertyToID("_BloomColor");

	private static readonly int blDataString = Shader.PropertyToID("_BloomData");

	private static readonly int bloomTexString = Shader.PropertyToID("_BloomTex");

	public Material material;

	private int numberOfPasses;

	private float knee;

	private RenderTextureDescriptor half;

	private RenderTextureDescriptor quarter;

	private RenderTextureDescriptor eighths;

	private RenderTextureDescriptor sixths;
}

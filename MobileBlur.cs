using System;
using UnityEngine;
using UnityEngine.XR;

[ExecuteInEditMode]
public class MobileBlur : MonoBehaviour
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.BlurAmount == 0f)
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
			this.half = new RenderTextureDescriptor(Screen.width / 2, Screen.height / 2);
			this.quarter = new RenderTextureDescriptor(Screen.width / 4, Screen.height / 4);
			this.eighths = new RenderTextureDescriptor(Screen.width / 8, Screen.height / 8);
			this.sixths = new RenderTextureDescriptor(Screen.width / 16, Screen.height / 16);
		}
		if (this.KernelSize == 2)
		{
			this.material.DisableKeyword(MobileBlur.kernelKeyword);
		}
		else
		{
			this.material.EnableKeyword(MobileBlur.kernelKeyword);
		}
		this.pass = ((this.algorithm == MobileBlur.Algorithm.Box) ? 0 : 1);
		if (this.maskTexture != null || this.previous != this.maskTexture)
		{
			this.previous = this.maskTexture;
			this.material.SetTexture(MobileBlur.maskTexString, this.maskTexture);
		}
		RenderTexture renderTexture = null;
		this.numberOfPasses = Mathf.Clamp(Mathf.CeilToInt(this.BlurAmount * 4f), 1, 4);
		this.material.SetFloat(MobileBlur.blurAmountString, (this.numberOfPasses > 1) ? ((this.BlurAmount > 1f) ? this.BlurAmount : ((this.BlurAmount * 4f - (float)Mathf.FloorToInt(this.BlurAmount * 4f - 0.001f)) * 0.5f + 0.5f)) : (this.BlurAmount * 4f));
		if (this.numberOfPasses == 1)
		{
			renderTexture = RenderTexture.GetTemporary(this.half);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.material, this.pass);
		}
		else if (this.numberOfPasses == 2)
		{
			renderTexture = RenderTexture.GetTemporary(this.half);
			RenderTexture temporary = RenderTexture.GetTemporary(this.quarter);
			renderTexture.filterMode = FilterMode.Bilinear;
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, temporary, this.material, this.pass);
			Graphics.Blit(temporary, renderTexture, this.material, this.pass);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (this.numberOfPasses == 3)
		{
			renderTexture = RenderTexture.GetTemporary(this.quarter);
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.eighths);
			renderTexture.filterMode = FilterMode.Bilinear;
			temporary2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.material, this.pass);
			Graphics.Blit(renderTexture, temporary2, this.material, this.pass);
			Graphics.Blit(temporary2, renderTexture, this.material, this.pass);
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
			Graphics.Blit(source, renderTexture, this.material, this.pass);
			Graphics.Blit(renderTexture, temporary3, this.material, this.pass);
			Graphics.Blit(temporary3, temporary4, this.material, this.pass);
			Graphics.Blit(temporary4, temporary3, this.material, this.pass);
			Graphics.Blit(temporary3, renderTexture, this.material, this.pass);
			RenderTexture.ReleaseTemporary(temporary3);
			RenderTexture.ReleaseTemporary(temporary4);
		}
		this.material.SetTexture(MobileBlur.blurTexString, renderTexture);
		RenderTexture.ReleaseTemporary(renderTexture);
		Graphics.Blit(source, destination, this.material, 2);
	}

	public MobileBlur.Algorithm algorithm = MobileBlur.Algorithm.Box;

	[Range(0f, 2f)]
	public float BlurAmount = 1f;

	[Range(2f, 3f)]
	public int KernelSize = 2;

	public Texture2D maskTexture;

	private Texture2D previous;

	public Material material;

	private static readonly string kernelKeyword = "KERNEL";

	private static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");

	private static readonly int blurTexString = Shader.PropertyToID("_BlurTex");

	private static readonly int maskTexString = Shader.PropertyToID("_MaskTex");

	private int numberOfPasses;

	private int pass;

	private RenderTextureDescriptor half;

	private RenderTextureDescriptor quarter;

	private RenderTextureDescriptor eighths;

	private RenderTextureDescriptor sixths;

	public enum Algorithm
	{
		Box = 1,
		Gaussian
	}
}

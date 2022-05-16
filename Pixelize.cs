using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class Pixelize : MonoBehaviour
{
	private Shader ScreenAndMaskShader
	{
		get
		{
			if (this._screenAndMaskShader == null)
			{
				this._screenAndMaskShader = Shader.Find("Hidden/PostProcess/Pixelize/ScreenAndMask");
			}
			return this._screenAndMaskShader;
		}
	}

	private Material ScreenAndMaskMaterial
	{
		get
		{
			if (this._screenAndMaskMaterial == null)
			{
				this._screenAndMaskMaterial = new Material(this.ScreenAndMaskShader);
			}
			return this._screenAndMaskMaterial;
		}
	}

	private RenderTexture TemporaryRenderTarget
	{
		get
		{
			if (this._temporaryRenderTexture == null)
			{
				this.CreateTemporaryRenderTarget();
			}
			return this._temporaryRenderTexture;
		}
	}

	private Shader CombineLayersShader
	{
		get
		{
			if (this._combineLayersShader == null)
			{
				this._combineLayersShader = Shader.Find("Hidden/PostProcess/Pixelize/CombineLayers");
			}
			return this._combineLayersShader;
		}
	}

	private Material CombineLayersMaterial
	{
		get
		{
			if (this._combineLayersMaterial == null)
			{
				this._combineLayersMaterial = new Material(this.CombineLayersShader);
			}
			return this._combineLayersMaterial;
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		this.CheckTemporaryRenderTarget();
		Graphics.Blit(src, this.TemporaryRenderTarget, this.ScreenAndMaskMaterial);
		Graphics.Blit(this.TemporaryRenderTarget, dest, this.CombineLayersMaterial);
	}

	private void CreateTemporaryRenderTarget()
	{
		this._temporaryRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		this._temporaryRenderTexture.useMipMap = true;
		this._temporaryRenderTexture.autoGenerateMips = true;
		this._temporaryRenderTexture.wrapMode = TextureWrapMode.Clamp;
		this._temporaryRenderTexture.filterMode = FilterMode.Point;
		this._temporaryRenderTexture.Create();
	}

	private void CheckTemporaryRenderTarget()
	{
		if (this.TemporaryRenderTarget.width != Screen.width || this.TemporaryRenderTarget.width != Screen.height)
		{
			this.ReleaseTemporaryRenderTarget();
		}
	}

	private void ReleaseTemporaryRenderTarget()
	{
		this._temporaryRenderTexture.Release();
		this._temporaryRenderTexture = null;
	}

	private Shader _screenAndMaskShader;

	private Material _screenAndMaskMaterial;

	private RenderTexture _temporaryRenderTexture;

	private Shader _combineLayersShader;

	private Material _combineLayersMaterial;
}

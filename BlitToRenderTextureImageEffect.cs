using System;
using UnityEngine;

public class BlitToRenderTextureImageEffect : MonoBehaviour
{
	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (this.renderTexture == null || this.renderTexture.height != Screen.height || this.renderTexture.width != Screen.width)
		{
			if (this.renderTexture == null)
			{
				Object.Destroy(this.renderTexture);
			}
			this.renderTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			this.targetMaterial.SetTexture("_MainTex", this.renderTexture);
		}
		Graphics.Blit(src, this.renderTexture);
		Graphics.Blit(src, dst);
	}

	public Material targetMaterial;

	private RenderTexture renderTexture;
}

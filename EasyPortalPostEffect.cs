using System;
using UnityEngine;

public class EasyPortalPostEffect : MonoBehaviour
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, this.postprocessMaterial);
	}

	public Material postprocessMaterial;
}

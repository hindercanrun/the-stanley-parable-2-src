using System;
using System.Collections;
using UnityEngine;

public class ReadPixelsToTexture : MonoBehaviour
{
	private void Update()
	{
		if (!this.rendering)
		{
			base.StartCoroutine(this.RenderScreen());
		}
	}

	private IEnumerator RenderScreen()
	{
		this.mat.SetTexture("_MainTex", this.tex);
		while ((float)this.frameCounter < this.skipFrames)
		{
			this.frameCounter++;
			yield return null;
		}
		this.frameCounter = 0;
		this.rendering = false;
		yield break;
	}

	[SerializeField]
	private Material mat;

	[SerializeField]
	private float skipFrames;

	private int frameCounter;

	private bool rendering;

	private RenderTexture tex;
}

using System;
using UnityEngine;

public class LightmapSwapper : MonoBehaviour
{
	private void Start()
	{
		this.lightmapNative = LightmapSettings.lightmaps;
		this.lightmapResource = new LightmapData[this.lightmapNative.Length];
		for (int i = 0; i < this.lightmapNative.Length; i++)
		{
			this.lightmapResource[i] = new LightmapData();
			this.lightmapResource[i].lightmapColor = (Resources.Load("ropetest/" + this.lightmapNative[i].lightmapColor.name) as Texture2D);
			this.lightmapResource[i].lightmapDir = (Resources.Load("ropetest/" + this.lightmapNative[i].lightmapDir.name) as Texture2D);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			this.Swap();
		}
	}

	private void Swap()
	{
		if (this.toggle)
		{
			LightmapSettings.lightmaps = this.lightmapNative;
		}
		else
		{
			LightmapSettings.lightmaps = this.lightmapResource;
		}
		this.toggle = !this.toggle;
	}

	private LightmapData[] lightmapNative;

	private LightmapData[] lightmapResource;

	private bool toggle;
}

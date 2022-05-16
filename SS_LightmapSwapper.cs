using System;
using System.Collections;
using UnityEngine;

public class SS_LightmapSwapper : MonoBehaviour
{
	private void OnValidate()
	{
	}

	public void GetLightmapIndex()
	{
		if (!this.ren)
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].lightmapIndex > -1)
				{
					this.ren = componentsInChildren[i];
					break;
				}
			}
		}
		if (this.ren)
		{
			this.lightmapIndex = this.ren.lightmapIndex;
			return;
		}
		this.lightmapIndex = -1;
	}

	public void SwapLightmap(Texture2D tex, int index)
	{
		this.lightmaps = LightmapSettings.lightmaps;
		int num = this.lightmaps.Length;
		if (num <= index || num == 0)
		{
			return;
		}
		this.lightmaps[index].lightmapColor = tex;
		LightmapSettings.lightmaps = this.lightmaps;
	}

	public void SwapLightmapFromArray(int index)
	{
		this.GetLightmapIndex();
		if (this.lightmapIndex > -1)
		{
			this.SwapLightmap(this.lightmapArray[index], this.lightmapIndex);
		}
	}

	public void RestoreLightmap(Texture2D tex, int index)
	{
		this.lightmaps = LightmapSettings.lightmaps;
		this.lightmaps[index].lightmapColor = tex;
		LightmapSettings.lightmaps = this.lightmaps;
	}

	[ContextMenu("Swap Lightmap")]
	public void SwapLightmap()
	{
		this.GetLightmapIndex();
		if (this.lightmapIndex > -1)
		{
			this.SwapLightmap(this.newLightmap, this.lightmapIndex);
			if (this.lpStorage)
			{
				this.lpStorage.ApplyHarmonics();
			}
		}
	}

	[ContextMenu("Restore Lightmap")]
	public void RestoreLightmap()
	{
		if (this.savedLightmap)
		{
			this.GetLightmapIndex();
			if (this.lightmapIndex > -1)
			{
				this.RestoreLightmap(this.savedLightmap, this.lightmapIndex);
			}
		}
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
		if (this.applyNewLightmapOnEnable && this.newLightmap != null)
		{
			this.SwapLightmap();
		}
		yield break;
	}

	public bool applyNewLightmapOnEnable = true;

	[Space]
	private int lightmapIndex = -1;

	public Texture2D newLightmap;

	public Texture2D[] lightmapArray;

	[NonSerialized]
	public Texture2D savedLightmap;

	private Renderer ren;

	private LightmapData[] lightmaps;

	public SS_LightProbeStorage lpStorage;
}

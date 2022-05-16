using System;
using UnityEngine;

public class InfoOverlay : HammerEntity
{
	private void Awake()
	{
		if (this.render == null)
		{
			this.render = base.GetComponent<MeshRenderer>();
		}
	}

	public void IncrementTextureIndex()
	{
		this.render.material.SetInt("_Index", this.render.material.GetInt("_Index") + 1);
	}

	public void SetTextureIndex(int index)
	{
		this.render.material.SetInt("_Index", index);
	}

	public Renderer render;
}

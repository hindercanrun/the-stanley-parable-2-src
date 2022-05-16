using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureToggle : HammerEntity
{
	private void OnValidate()
	{
		InfoOverlay[] array = Object.FindObjectsOfType<InfoOverlay>();
		List<InfoOverlay> list = new List<InfoOverlay>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name == this.target)
			{
				list.Add(array[i]);
			}
		}
		if (list.Count > 0)
		{
			this.targetObjects = list.ToArray();
		}
	}

	public void Input_IncrementTextureIndex()
	{
		for (int i = 0; i < this.targetObjects.Length; i++)
		{
			if (this.targetObjects[i] != null)
			{
				this.targetObjects[i].IncrementTextureIndex();
			}
		}
	}

	public void Input_SetTextureIndex(float floatdex)
	{
		int textureIndex = Mathf.RoundToInt(floatdex);
		for (int i = 0; i < this.targetObjects.Length; i++)
		{
			if (this.targetObjects[i] != null)
			{
				this.targetObjects[i].SetTextureIndex(textureIndex);
			}
		}
	}

	public string target = "";

	public InfoOverlay[] targetObjects;
}

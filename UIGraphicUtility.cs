using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGraphicUtility : MonoBehaviour
{
	public void SetColor(int colorIndex)
	{
		if (this.colorArray.Length == 0 || this.colorArray.Length <= colorIndex)
		{
			return;
		}
		if (this.graphic != null)
		{
			this.graphic.color = this.colorArray[colorIndex];
		}
	}

	[SerializeField]
	private MaskableGraphic graphic;

	[SerializeField]
	private Color[] colorArray;
}

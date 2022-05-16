using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextStyleControl : MonoBehaviour
{
	private void Awake()
	{
		this.label = base.GetComponent<TextMeshProUGUI>();
	}

	public void SetChangedColor()
	{
		if (this.label != null)
		{
			this.label.color = this.changedColor;
		}
	}

	public void SetDefaultColor()
	{
		if (this.label != null)
		{
			this.label.color = this.defaultColor;
		}
	}

	private TextMeshProUGUI label;

	[SerializeField]
	private Color changedColor = Color.red;

	[SerializeField]
	private Color defaultColor = Color.white;
}

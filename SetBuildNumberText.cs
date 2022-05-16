using System;
using TMPro;
using UnityEngine;

public class SetBuildNumberText : MonoBehaviour
{
	private void Start()
	{
		this.UpdateText();
	}

	private void UpdateText()
	{
		if (this.text != null)
		{
			this.text.text = (Application.version ?? "");
		}
	}

	private void OnValidate()
	{
		this.UpdateText();
	}

	[InspectorButton("UpdateText", null)]
	[SerializeField]
	private TMP_Text text;
}

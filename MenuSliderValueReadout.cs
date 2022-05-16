using System;
using TMPro;
using UnityEngine;

public class MenuSliderValueReadout : MonoBehaviour
{
	private void Start()
	{
		this.valueText = base.GetComponent<TextMeshPro>();
	}

	public void textUpdate(int value)
	{
		this.valueText.text = value.ToString();
	}

	private TextMeshPro valueText;
}

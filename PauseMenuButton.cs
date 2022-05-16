using System;
using TMPro;
using UnityEngine;

public class PauseMenuButton : MonoBehaviour
{
	private void Start()
	{
		this.initialText.text = this.buttonTextObject.text;
	}

	public void BoldText()
	{
		this.buttonTextObject.fontStyle = FontStyles.Bold;
	}

	public void UnboldText()
	{
		this.buttonTextObject.fontStyle = FontStyles.Normal;
	}

	public TextMeshProUGUI buttonTextObject;

	private TextMeshProUGUI initialText;
}

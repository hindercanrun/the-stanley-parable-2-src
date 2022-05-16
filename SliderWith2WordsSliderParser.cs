using System;
using TMPro;
using UnityEngine;

public class SliderWith2WordsSliderParser : MonoBehaviour, IMessageBoxDialogueParser
{
	public string[] ParseDialogue(MessageBoxDialogue dialogue)
	{
		string[] messages = dialogue.GetMessages();
		this.sliderTextRight.text = ((messages.Length > 2) ? messages[2] : "");
		this.sliderTextLeft.text = ((messages.Length > 1) ? messages[1] : "");
		return new string[]
		{
			(messages.Length != 0) ? messages[0] : ""
		};
	}

	[SerializeField]
	private TextMeshProUGUI sliderTextLeft;

	[SerializeField]
	private TextMeshProUGUI sliderTextRight;
}

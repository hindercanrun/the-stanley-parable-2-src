using System;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSliderEvent : MonoBehaviour
{
	private void Awake()
	{
		Color color = Color.Lerp(this.remappedColorMinimumValue, this.remappedColorMaximumValue, 0.5f);
		this.imageToChange.color = color;
	}

	public void ValueChanged(float value)
	{
		Color color = Color.Lerp(this.remappedColorMinimumValue, this.remappedColorMaximumValue, value);
		this.imageToChange.color = color;
	}

	[SerializeField]
	private Image imageToChange;

	[SerializeField]
	private Color remappedColorMinimumValue;

	[SerializeField]
	private Color remappedColorMaximumValue;

	public enum SliderEventTypes
	{
		ChangeColor,
		ChangeSprite
	}
}

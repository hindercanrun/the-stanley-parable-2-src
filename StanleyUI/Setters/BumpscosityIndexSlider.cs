using System;
using TMPro;
using UnityEngine;

namespace StanleyUI.Setters
{
	public class BumpscosityIndexSlider : MonoBehaviour, ISettingsIntListener
	{
		public void SetValueFloatHACK(float f)
		{
			this.SetValue(Mathf.RoundToInt(f));
		}

		public void SetValue(int val)
		{
			this.tmpro.text = this.replacementNumbers[Mathf.Clamp(val, 0, this.replacementNumbers.Length - 1)];
		}

		public TextMeshProUGUI tmpro;

		public string[] replacementNumbers = new string[]
		{
			"0",
			"1",
			"12",
			"50",
			"100",
			"1000"
		};
	}
}

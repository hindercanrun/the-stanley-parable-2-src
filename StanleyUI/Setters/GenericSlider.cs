using System;
using TMPro;
using UnityEngine;

namespace StanleyUI.Setters
{
	public class GenericSlider : MonoBehaviour, ISettingsFloatListener
	{
		public virtual void SetValue(float val)
		{
			this.textLabel.text = string.Concat(Mathf.RoundToInt(val));
		}

		public TextMeshProUGUI textLabel;
	}
}

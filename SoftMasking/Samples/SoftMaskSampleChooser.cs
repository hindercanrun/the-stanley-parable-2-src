using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SoftMasking.Samples
{
	public class SoftMaskSampleChooser : MonoBehaviour
	{
		public void Start()
		{
			string activeSceneName = SceneManager.GetActiveScene().name;
			int num = this.dropdown.options.FindIndex((Dropdown.OptionData x) => x.text == activeSceneName);
			if (num >= 0)
			{
				this.dropdown.value = num;
				this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.Choose));
				return;
			}
			this.Fallback(activeSceneName);
		}

		private void Fallback(string activeSceneName)
		{
			this.dropdown.gameObject.SetActive(false);
			this.fallbackLabel.gameObject.SetActive(true);
			this.fallbackLabel.text = activeSceneName;
		}

		public void Choose(int sampleIndex)
		{
			SceneManager.LoadScene(this.dropdown.options[sampleIndex].text);
		}

		public Dropdown dropdown;

		public Text fallbackLabel;
	}
}

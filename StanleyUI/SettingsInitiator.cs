using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StanleyUI
{
	public class SettingsInitiator : MonoBehaviour
	{
		private IEnumerator Start()
		{
			Dictionary<GameObject, bool> origStates = new Dictionary<GameObject, bool>();
			foreach (GameObject gameObject in this.settingsPages)
			{
				origStates[gameObject] = gameObject.activeSelf;
				gameObject.SetActive(true);
			}
			yield return null;
			foreach (GameObject gameObject2 in this.settingsPages)
			{
				gameObject2.SetActive(origStates[gameObject2]);
			}
			yield break;
		}

		public GameObject[] settingsPages;
	}
}

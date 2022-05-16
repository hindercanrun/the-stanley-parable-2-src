using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Util
{
	public class DropdownFix : MonoBehaviour
	{
		private IEnumerator Start()
		{
			Object.Destroy(base.GetComponent<GraphicRaycaster>());
			yield return null;
			Object.Destroy(base.GetComponent<Canvas>());
			yield break;
		}
	}
}

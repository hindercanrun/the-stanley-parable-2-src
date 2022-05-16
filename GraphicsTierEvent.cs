using System;
using UnityEngine;

public class GraphicsTierEvent : MonoBehaviour
{
	private void Start()
	{
		string[] names = QualitySettings.names;
		QualitySettings.GetQualityLevel();
	}

	private void Update()
	{
	}
}

using System;
using UnityEngine;

public class MenuFakePauseHotkey : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			MonoBehaviour.print("fake pause!");
			this.enableThis.SetActive(true);
			this.disableThis.SetActive(false);
		}
	}

	public GameObject enableThis;

	public GameObject disableThis;
}

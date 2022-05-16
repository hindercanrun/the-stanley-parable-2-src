using System;
using UnityEngine;

public class DisableOnLoad : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}

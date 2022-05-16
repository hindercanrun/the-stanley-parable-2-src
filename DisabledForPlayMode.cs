using System;
using UnityEngine;

public class DisabledForPlayMode : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}

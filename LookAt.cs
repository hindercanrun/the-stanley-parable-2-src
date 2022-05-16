using System;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	private void Update()
	{
		base.transform.LookAt(this.target);
	}

	public Transform target;
}

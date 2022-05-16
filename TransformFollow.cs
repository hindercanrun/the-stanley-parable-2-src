using System;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
	private void Update()
	{
		if (base.transform.position != this.follow.position)
		{
			base.transform.position = this.follow.position;
		}
	}

	public Transform follow;
}

using System;
using UnityEngine;

public class ResetPositionEvent : MonoBehaviour
{
	public void UpdateLocalPosition()
	{
		this.target.localPosition = this.localPositionToSetOnEvent;
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Vector3 localPositionToSetOnEvent;
}

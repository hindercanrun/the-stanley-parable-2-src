using System;
using UnityEngine;

public class MoveToTargetEventHolder : MonoBehaviour
{
	public void MoveToTarget(Transform target)
	{
		base.transform.position = target.position;
	}
}

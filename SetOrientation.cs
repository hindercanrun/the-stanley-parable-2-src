using System;
using UnityEngine;

public class SetOrientation : MonoBehaviour
{
	public void MoveAndRotateToHere(Transform t)
	{
		t.position = base.transform.position;
		t.rotation = base.transform.rotation;
	}
}

using System;
using UnityEngine;

public class Physbox : HammerEntity
{
	private void OnValidate()
	{
		MeshCollider[] componentsInChildren = base.GetComponentsInChildren<MeshCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].convex = true;
		}
		base.GetComponent<Rigidbody>().isKinematic = false;
	}
}

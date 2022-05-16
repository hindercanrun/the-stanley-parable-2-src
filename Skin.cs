using System;
using UnityEngine;

public class Skin : MonoBehaviour
{
	public void ValidateSkin()
	{
		if (!this.primary)
		{
			return;
		}
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		SkinnedMeshRenderer componentInChildren2 = base.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.materials = this.materials;
		}
		if (componentInChildren2)
		{
			componentInChildren2.materials = this.materials;
		}
	}

	public int index;

	public bool primary;

	public Material[] materials;
}

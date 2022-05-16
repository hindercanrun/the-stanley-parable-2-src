using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
	public GameObject graphicsClone { get; set; }

	public Vector3 previousOffsetFromPortal { get; set; }

	public Material[] originalMaterials { get; set; }

	public Material[] cloneMaterials { get; set; }

	public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.transform.position = pos;
		base.transform.rotation = rot;
	}

	public virtual void EnterPortalThreshold()
	{
		if (!this.InstantiateClone)
		{
			return;
		}
		if (this.graphicsClone == null)
		{
			this.graphicsClone = Object.Instantiate<GameObject>(this.graphicsObject);
			this.graphicsClone.transform.parent = this.graphicsObject.transform.parent;
			this.graphicsClone.transform.localScale = this.graphicsObject.transform.localScale;
			this.originalMaterials = this.GetMaterials(this.graphicsObject);
			this.cloneMaterials = this.GetMaterials(this.graphicsClone);
			return;
		}
		this.graphicsClone.SetActive(true);
	}

	public virtual void ExitPortalThreshold()
	{
		if (!this.InstantiateClone)
		{
			return;
		}
		this.graphicsClone.SetActive(false);
		for (int i = 0; i < this.originalMaterials.Length; i++)
		{
			this.originalMaterials[i].SetVector("sliceNormal", Vector3.zero);
		}
	}

	public void SetSliceOffsetDst(float dst, bool clone)
	{
		if (!this.InstantiateClone)
		{
			return;
		}
		for (int i = 0; i < this.originalMaterials.Length; i++)
		{
			if (clone)
			{
				this.cloneMaterials[i].SetFloat("sliceOffsetDst", dst);
			}
			else
			{
				this.originalMaterials[i].SetFloat("sliceOffsetDst", dst);
			}
		}
	}

	private Material[] GetMaterials(GameObject g)
	{
		MeshRenderer[] componentsInChildren = g.GetComponentsInChildren<MeshRenderer>();
		List<Material> list = new List<Material>();
		MeshRenderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Material item in array[i].materials)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public GameObject graphicsObject;

	public bool InstantiateClone;
}

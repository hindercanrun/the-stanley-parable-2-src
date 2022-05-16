using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedCull : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		yield return null;
		if (CullForSwitchController.IsSwitchEnvironment)
		{
			this.SetupDistanceBasedCull();
			this.c = null;
		}
		yield break;
	}

	[ContextMenu("SetupDistanceBasedCull")]
	private void SetupDistanceBasedCull()
	{
		this.c = Camera.main;
		float[] layerCullDistances = this.c.layerCullDistances;
		layerCullDistances[this.distanceCullLayer] = this.cullDistance;
		this.c.layerCullDistances = layerCullDistances;
		this.c.layerCullSpherical = true;
	}

	[ContextMenu("DisplayDistanceBasedCull")]
	private void DisplayDistanceBasedCull()
	{
		this.c = Camera.main;
		for (int i = 0; i < 32; i++)
		{
			Debug.Log(string.Concat(new object[]
			{
				i,
				" ",
				LayerMask.LayerToName(i),
				"\t",
				this.c.layerCullDistances[i]
			}));
		}
	}

	[ContextMenu("FindAllCullitems")]
	private void FindAllCullItems()
	{
		this.c = Camera.main;
		this.cullItems = new List<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
		{
			if (meshRenderer.gameObject.layer == this.distanceCullLayer)
			{
				this.cullItems.Add(meshRenderer);
			}
		}
	}

	private void Update()
	{
		if (this.c == null)
		{
			return;
		}
		foreach (MeshRenderer meshRenderer in this.cullItems)
		{
			bool flag = (this.c.transform.position - meshRenderer.transform.position).sqrMagnitude > this.cullDistance * this.cullDistance;
			if (flag && meshRenderer.enabled)
			{
				meshRenderer.enabled = false;
			}
			if (!flag && !meshRenderer.enabled)
			{
				meshRenderer.enabled = true;
			}
		}
	}

	[SerializeField]
	public float cullDistance = 20f;

	[SerializeField]
	public int distanceCullLayer = 30;

	public Camera c;

	public List<MeshRenderer> cullItems;
}

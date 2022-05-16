using System;
using UnityEngine;
using UnityEngine.Rendering;

public class RendererControl : MonoBehaviour
{
	private void Start()
	{
		if (this.disableShadowsOnStart)
		{
			this.DisableShadowCastingInChildren();
			this.DisableShadowReceivingInChildren();
		}
	}

	public void DisableShadowCastingInChildren()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].shadowCastingMode = ShadowCastingMode.Off;
		}
	}

	public void DisableShadowReceivingInChildren()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].receiveShadows = false;
		}
	}

	public void SetDefaultLayer(bool status)
	{
		foreach (Transform transform in base.GetComponentsInChildren<Transform>(true))
		{
			if (status)
			{
				transform.gameObject.layer = this.defaultLayer;
			}
			else
			{
				transform.gameObject.layer = this.lowEndLayer;
			}
		}
	}

	[SerializeField]
	private bool disableShadowsOnStart;

	[SerializeField]
	private int defaultLayer;

	[SerializeField]
	private int lowEndLayer;
}

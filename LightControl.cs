using System;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightControl : MonoBehaviour
{
	private void Awake()
	{
		this.cachedLight = base.GetComponent<Light>();
		this.cachedShadows = this.cachedLight.shadows;
	}

	public void SetRange(float range)
	{
		this.cachedLight.range = range;
	}

	public void UpdateRange(bool status)
	{
		this.cachedLight.range = (status ? this.defaultRange : this.lowEndRange);
	}

	public void UpdateMask(bool status)
	{
		this.cachedLight.cullingMask = (status ? this.defaultLayerMask : this.lowEndMask);
	}

	public void SetShadows(bool status)
	{
		if (status)
		{
			this.cachedLight.shadows = this.cachedShadows;
			return;
		}
		this.cachedLight.shadows = LightShadows.None;
	}

	private Light cachedLight;

	[SerializeField]
	private float defaultRange = 5f;

	[SerializeField]
	private float lowEndRange = 3f;

	[SerializeField]
	private LayerMask defaultLayerMask;

	[SerializeField]
	private LayerMask lowEndMask;

	private LightShadows cachedShadows;
}

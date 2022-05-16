using System;
using AmazingAssets.CurvedWorld;
using UnityEngine;

public class CurveAnimation : MonoBehaviour
{
	public float RealtimeSinceStartup
	{
		get
		{
			return this.startTime + Time.realtimeSinceStartup;
		}
	}

	private void Awake()
	{
		this.controller = base.GetComponent<CurvedWorldController>();
	}

	private void Update()
	{
		this.controller.SetBendCurvatureSize(this.GetMappedNoise(this.RealtimeSinceStartup * this.speed + this.curveXOffset, Time.realtimeSinceStartup * this.speed + this.horizontalXOffset) * this.range);
		this.controller.SetBendHorizontalSize(this.GetMappedNoise(this.RealtimeSinceStartup * this.speed + this.horizontalXOffset, Time.realtimeSinceStartup * this.speed + this.horizontalYOffset) * this.range);
		this.controller.SetBendVerticalSize(this.GetMappedNoise(this.RealtimeSinceStartup * this.speed + this.verticalXOffset, Time.realtimeSinceStartup * this.speed + this.verticalYOffset) * this.range);
	}

	private float GetMappedNoise(float x, float y)
	{
		return Mathf.Lerp(-1f, 1f, Mathf.PerlinNoise(x, y));
	}

	private CurvedWorldController controller;

	[SerializeField]
	private float speed = 10f;

	[SerializeField]
	private float range = 10f;

	[SerializeField]
	private float curveXOffset;

	[SerializeField]
	private float curveYOffset;

	[SerializeField]
	private float horizontalXOffset;

	[SerializeField]
	private float horizontalYOffset;

	[SerializeField]
	private float verticalXOffset;

	[SerializeField]
	private float verticalYOffset;

	[SerializeField]
	private float startTime = 12f;
}

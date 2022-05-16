using System;
using UnityEngine;

[Serializable]
public class Ferr2DT_SegmentDescription
{
	public Ferr2DT_SegmentDescription()
	{
		this.body = new Rect[]
		{
			new Rect(0f, 0f, 50f, 50f)
		};
		this.applyTo = Ferr2DT_TerrainDirection.Top;
	}

	public Ferr2DT_TerrainDirection applyTo;

	public float zOffset;

	public float yOffset;

	public Rect leftCap;

	public Rect innerLeftCap;

	public Rect rightCap;

	public Rect innerRightCap;

	public Rect[] body;

	public float capOffset;
}

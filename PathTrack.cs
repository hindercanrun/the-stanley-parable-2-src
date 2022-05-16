using System;
using UnityEngine;

public class PathTrack : HammerEntity
{
	private void OnDrawGizmos()
	{
		if (this.nextPathTrack != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, this.nextPathTrack.transform.position);
		}
	}

	private void OnValidate()
	{
		if (this.nextPathTrack == null || this.nextPathTrack.name != this.nextPath)
		{
			GameObject gameObject = GameObject.Find(this.nextPath);
			if (gameObject)
			{
				this.nextPathTrack = gameObject.GetComponent<PathTrack>();
				return;
			}
			this.nextPathTrack = null;
		}
	}

	public void Passed()
	{
		base.FireOutput(Outputs.OnPass);
	}

	public string nextPath = "";

	public PathTrack nextPathTrack;

	public float newSpeed;

	public PathTrack.OrientationTypes orientationType;

	private bool checkedRecently;

	public enum OrientationTypes
	{
		NoChange,
		DirectionOfMotion,
		ThisTracksAngles
	}
}

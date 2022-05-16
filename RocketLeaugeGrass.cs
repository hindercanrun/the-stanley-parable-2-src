using System;
using UnityEngine;

public class RocketLeaugeGrass : MonoBehaviour
{
	private void Start()
	{
	}

	[ContextMenu("Do Update Step")]
	private void Update()
	{
		if (this.followTarget != null)
		{
			Vector3 position = this.followTarget.transform.position;
			position.x -= Mathf.Repeat(position.x, this.step);
			position.z -= Mathf.Repeat(position.z, this.step);
			position.y = base.transform.position.y;
			base.transform.position = position;
		}
	}

	public string followTargetTag;

	private GameObject followTarget;

	public float step = 0.5f;
}

using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.position = StanleyController.StanleyPosition + this.offset;
	}

	public Vector3 offset = new Vector3(0f, 7.5f, 0f);
}

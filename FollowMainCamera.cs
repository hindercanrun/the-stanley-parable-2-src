using System;
using UnityEngine;

public class FollowMainCamera : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.position = StanleyController.Instance.cam.transform.position;
		base.transform.rotation = StanleyController.Instance.cam.transform.rotation;
	}
}

using System;
using UnityEngine;

public class CubemapRotation : MonoBehaviour
{
	private void Update()
	{
		this.yRot = base.transform.eulerAngles.y;
		Quaternion q = Quaternion.Euler(0f, this.yRot, 0f);
		Matrix4x4 value = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
		base.GetComponent<Renderer>().material.SetMatrix("_Rotation", value);
	}

	[SerializeField]
	private float yRot = 60f;
}

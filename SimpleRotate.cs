using System;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
	private void Start()
	{
		this.startRotation = base.transform.localEulerAngles.y;
	}

	public void ResetToStartRotation()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.y = this.startRotation;
		base.transform.localEulerAngles = localEulerAngles;
	}

	private void Update()
	{
		base.transform.localEulerAngles += Vector3.up * this.speed * Time.deltaTime;
	}

	public float speed = 45f;

	private float startRotation;
}

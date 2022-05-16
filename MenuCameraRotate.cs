using System;
using UnityEngine;

public class MenuCameraRotate : MonoBehaviour
{
	private void Awake()
	{
		this.seed = Random.value;
	}

	private void Update()
	{
		base.transform.localRotation = Quaternion.Euler(new Vector3(this.maximumAngularShake.x * (Mathf.PerlinNoise(this.seed + 3f, Time.time * this.frequency) * 2f - 1f), this.maximumAngularShake.y * (Mathf.PerlinNoise(this.seed + 4f, Time.time * this.frequency) * 2f - 1f), this.maximumAngularShake.z * (Mathf.PerlinNoise(this.seed + 5f, Time.time * this.frequency) * 2f - 1f)) * this.intensity);
	}

	public float frequency = 0.1f;

	public float intensity = 0.5f;

	private float seed;

	private Vector3 maximumAngularShake = Vector3.one * 2f;
}

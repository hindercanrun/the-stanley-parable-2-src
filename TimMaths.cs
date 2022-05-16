using System;
using UnityEngine;

public class TimMaths
{
	public static float Vector3InverseLerp(Vector3 start, Vector3 end, Vector3 value)
	{
		Vector3 vector = end - start;
		return Mathf.Clamp01(Vector3.Dot(value - start, vector) / Vector3.Dot(vector, vector));
	}

	public static float SphereRandom()
	{
		return Mathf.Acos(Random.Range(-1f, 1f)) * 57.29578f - 90f;
	}
}

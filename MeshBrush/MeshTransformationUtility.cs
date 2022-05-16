using System;
using UnityEngine;

namespace MeshBrush
{
	public static class MeshTransformationUtility
	{
		public static void ApplyRandomScale(Transform targetTransform, Vector2 range)
		{
			float num = Mathf.Abs(Random.Range(range.x, range.y));
			targetTransform.localScale = new Vector3(num, num, num);
		}

		public static void ApplyRandomScale(Transform targetTransform, Vector4 scaleRanges)
		{
			float f = Random.Range(scaleRanges.x, scaleRanges.y);
			float f2 = Random.Range(scaleRanges.z, scaleRanges.w);
			targetTransform.localScale = new Vector3
			{
				x = Mathf.Abs(f),
				y = Mathf.Abs(f2),
				z = Mathf.Abs(f)
			};
		}

		public static void ApplyRandomScale(Transform targetTransform, Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
		{
			targetTransform.localScale = new Vector3
			{
				x = Mathf.Abs(Random.Range(rangeX.x, rangeX.y)),
				y = Mathf.Abs(Random.Range(rangeY.x, rangeY.y)),
				z = Mathf.Abs(Random.Range(rangeZ.x, rangeZ.y))
			};
		}

		public static void AddConstantScale(Transform targetTransform, Vector2 range)
		{
			float num = Random.Range(range.x, range.y);
			Vector3 vector = targetTransform.localScale + new Vector3(num, num, num);
			vector.x = Mathf.Abs(vector.x);
			vector.y = Mathf.Abs(vector.y);
			vector.z = Mathf.Abs(vector.z);
			targetTransform.localScale = vector;
		}

		public static void AddConstantScale(Transform targetTransform, float x, float y, float z)
		{
			Vector3 localScale = targetTransform.localScale + new Vector3
			{
				x = Mathf.Abs(x),
				y = Mathf.Abs(y),
				z = Mathf.Abs(z)
			};
			targetTransform.localScale = localScale;
		}

		public static void ApplyRandomRotation(Transform targetTransform, float randomRotationIntensityPercentageX, float randomRotationIntensityPercentageY, float randomRotationIntensityPercentageZ)
		{
			float x = Random.Range(0f, 3.6f * randomRotationIntensityPercentageX);
			float y = Random.Range(0f, 3.6f * randomRotationIntensityPercentageY);
			float z = Random.Range(0f, 3.6f * randomRotationIntensityPercentageZ);
			targetTransform.Rotate(new Vector3(x, y, z));
		}

		public static void ApplyMeshOffset(Transform targetTransform, float offset, Vector3 direction)
		{
			targetTransform.Translate(direction.normalized * offset * 0.01f, Space.World);
		}
	}
}

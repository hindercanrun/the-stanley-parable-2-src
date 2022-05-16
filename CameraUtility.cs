using System;
using UnityEngine;

public static class CameraUtility
{
	public static bool VisibleFromCamera(Renderer renderer, Camera camera)
	{
		return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
	}

	public static bool BoundsOverlap(MeshFilter nearObject, MeshFilter farObject, Camera camera)
	{
		CameraUtility.MinMax3D screenRectFromBounds = CameraUtility.GetScreenRectFromBounds(nearObject, camera);
		CameraUtility.MinMax3D screenRectFromBounds2 = CameraUtility.GetScreenRectFromBounds(farObject, camera);
		return screenRectFromBounds2.zMax > screenRectFromBounds.zMin && screenRectFromBounds2.xMax >= screenRectFromBounds.xMin && screenRectFromBounds2.xMin <= screenRectFromBounds.xMax && screenRectFromBounds2.yMax >= screenRectFromBounds.yMin && screenRectFromBounds2.yMin <= screenRectFromBounds.yMax;
	}

	public static CameraUtility.MinMax3D GetScreenRectFromBounds(MeshFilter renderer, Camera mainCamera)
	{
		CameraUtility.MinMax3D result = new CameraUtility.MinMax3D(float.MaxValue, float.MinValue);
		new Vector3[8];
		Bounds bounds = renderer.sharedMesh.bounds;
		bool flag = false;
		for (int i = 0; i < 8; i++)
		{
			Vector3 position = bounds.center + Vector3.Scale(bounds.extents, CameraUtility.cubeCornerOffsets[i]);
			Vector3 position2 = renderer.transform.TransformPoint(position);
			Vector3 vector = mainCamera.WorldToViewportPoint(position2);
			if (vector.z > 0f)
			{
				flag = true;
			}
			else
			{
				vector.x = (float)((vector.x <= 0.5f) ? 1 : 0);
				vector.y = (float)((vector.y <= 0.5f) ? 1 : 0);
			}
			result.AddPoint(vector);
		}
		if (!flag)
		{
			return default(CameraUtility.MinMax3D);
		}
		return result;
	}

	private static readonly Vector3[] cubeCornerOffsets = new Vector3[]
	{
		new Vector3(1f, 1f, 1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(-1f, -1f, -1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(1f, -1f, 1f)
	};

	public struct MinMax3D
	{
		public MinMax3D(float min, float max)
		{
			this.xMin = min;
			this.xMax = max;
			this.yMin = min;
			this.yMax = max;
			this.zMin = min;
			this.zMax = max;
		}

		public void AddPoint(Vector3 point)
		{
			this.xMin = Mathf.Min(this.xMin, point.x);
			this.xMax = Mathf.Max(this.xMax, point.x);
			this.yMin = Mathf.Min(this.yMin, point.y);
			this.yMax = Mathf.Max(this.yMax, point.y);
			this.zMin = Mathf.Min(this.zMin, point.z);
			this.zMax = Mathf.Max(this.zMax, point.z);
		}

		public float xMin;

		public float xMax;

		public float yMin;

		public float yMax;

		public float zMin;

		public float zMax;
	}
}

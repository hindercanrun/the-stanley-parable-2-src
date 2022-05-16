using System;
using System.Collections.Generic;
using UnityEngine;

public class RL_GrassMeshStripper : MonoBehaviour
{
	[ContextMenu("Generate Mesh With Refresh")]
	public void GenerateMeshWithRefresh()
	{
		this.GenerateMesh(true);
	}

	[ContextMenu("Generate Mesh Without Refresh")]
	public void GenerateMeshWithoutRefresh()
	{
		this.GenerateMesh(false);
	}

	public void GenerateMesh(bool forceRefreshMesh = false)
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh mesh = component.sharedMesh;
		if (mesh == null || forceRefreshMesh)
		{
			mesh = Object.Instantiate<Mesh>(this.referenceMeshFilter.sharedMesh);
		}
		List<int> list = new List<int>();
		Color32[] colors = mesh.colors32;
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		foreach (int num in triangles)
		{
			bool flag = false;
			if ((float)colors[num].r > 0.5f && Mathf.FloorToInt(vertices[num].x / this.meshSeparateion) % this.meshSeparateionAmount != 0)
			{
				flag = true;
			}
			if ((float)colors[num].g > 0.5f && Mathf.FloorToInt(vertices[num].y / this.meshSeparateion) % this.meshSeparateionAmount != 0)
			{
				flag = true;
			}
			bool flag2 = false;
			if ((float)colors[num].r > 0.5f)
			{
				bool flag3 = Mathf.FloorToInt(vertices[num].x / this.sectionSize) == this.sectionIndex.x;
				colors[num].b = (flag3 ? byte.MaxValue : 0);
				if (flag3)
				{
					flag2 = (flag2 || flag3);
				}
			}
			if ((float)colors[num].g > 0.5f)
			{
				bool flag4 = Mathf.FloorToInt(vertices[num].y / this.sectionSize) == this.sectionIndex.y;
				colors[num].b = (flag4 ? byte.MaxValue : 0);
				if (flag4)
				{
					flag2 = (flag2 || flag4);
				}
			}
			if (!flag && flag2)
			{
				list.Add(num);
			}
		}
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		for (int j = 0; j < vertices.Length; j++)
		{
			if ((float)colors[j].r > 0.5f)
			{
				if (Mathf.FloorToInt(vertices[j].y / this.sectionSize) < this.sectionIndex.y)
				{
					float num2 = this.sectionSize * (float)this.sectionIndex.y;
					dictionary[j] = num2 - vertices[j].y;
					vertices[j].y = num2;
				}
				if (Mathf.FloorToInt(vertices[j].y / this.sectionSize) >= this.sectionIndex.y + 1)
				{
					float num3 = this.sectionSize * (float)(this.sectionIndex.y + 1);
					dictionary[j] = num3 - vertices[j].y;
					vertices[j].y = num3;
				}
			}
			if ((float)colors[j].g > 0.5f)
			{
				if (Mathf.FloorToInt(vertices[j].x / this.sectionSize) < this.sectionIndex.x)
				{
					float num4 = this.sectionSize * (float)this.sectionIndex.x;
					dictionary[j] = num4 - vertices[j].x;
					vertices[j].x = num4;
				}
				if (Mathf.FloorToInt(vertices[j].x / this.sectionSize) >= this.sectionIndex.x + 1)
				{
					float num5 = this.sectionSize * (float)(this.sectionIndex.x + 1);
					dictionary[j] = num5 - vertices[j].x;
					vertices[j].x = num5;
				}
			}
		}
		Vector2[] uv = mesh.uv;
		List<int> list2 = new List<int>();
		for (int k = 0; k < list.Count; k += 3)
		{
			Vector3 a = vertices[list[k]];
			Vector3 vector = vertices[list[k + 1]];
			Vector3 b = vertices[list[k + 2]];
			Vector3 lhs = a - vector;
			Vector3 rhs = a - b;
			Vector3 vector2 = vector - b;
			float magnitude = lhs.magnitude;
			float magnitude2 = rhs.magnitude;
			float magnitude3 = vector2.magnitude;
			float num6 = magnitude * magnitude2;
			float num7 = 0f;
			if (num6 > Mathf.Epsilon)
			{
				float f = Mathf.Acos(Vector3.Dot(lhs, rhs) / num6);
				num7 = 0.5f * num6 * Mathf.Sin(f);
			}
			if (num7 > Mathf.Epsilon)
			{
				list2.Add(list[k]);
				list2.Add(list[k + 1]);
				list2.Add(list[k + 2]);
			}
			float num8 = Mathf.Max(Mathf.Max(magnitude, magnitude2), magnitude3) / this.sectionSize;
		}
		for (int l = 0; l < vertices.Length; l++)
		{
			Vector2 vector3 = uv[l];
			if ((float)colors[l].r > 0.5f)
			{
				vector3.x = vertices[l].y + (100f + vertices[l].x * vertices[l].x);
			}
			if ((float)colors[l].g > 0.5f)
			{
				vector3.x = vertices[l].x + (100f + vertices[l].y * vertices[l].y);
			}
			uv[l] = vector3;
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.colors32 = colors;
		mesh.SetTriangles(list2, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.UploadMeshData(false);
		component.sharedMesh = mesh;
	}

	[InspectorButton("GenerateMeshWithRefresh", null)]
	public MeshFilter referenceMeshFilter;

	public float meshSeparateion = 0.5f;

	public float sectionSize = 20f;

	public Vector2Int sectionIndex = new Vector2Int(0, 0);

	public float uv_U_Multiplier = 1f;

	public int meshSeparateionAmount = 2;
}

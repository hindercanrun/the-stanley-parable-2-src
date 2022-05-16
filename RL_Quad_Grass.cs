using System;
using System.Collections.Generic;
using UnityEngine;

public class RL_Quad_Grass : MonoBehaviour
{
	[ContextMenu("Create Mesh")]
	public void CreateMesh()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		Mesh mesh = component.sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		float num = (float)this.layers * this.separation;
		List<Vector3> list = new List<Vector3>();
		List<Color32> list2 = new List<Color32>();
		List<Vector2> list3 = new List<Vector2>();
		List<int> list4 = new List<int>();
		for (int i = 0; i < this.layers; i++)
		{
			float a = -num / 2f;
			float num2 = num / 2f;
			float z = -num / 2f + (float)i * this.separation;
			float num3 = (float)i / (float)this.layers;
			int num4 = 1;
			for (int j = 0; j < num4; j++)
			{
				int count = list.Count;
				float num5 = (float)j / (float)num4;
				float x = Mathf.Lerp(a, num2, num5);
				float y = 1f;
				list.Add(new Vector3(x, 0f, z));
				list.Add(new Vector3(x, y, z));
				list2.Add(new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
				list2.Add(new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
				list3.Add(new Vector2(0f, num5));
				list3.Add(new Vector2(1f, num5));
				list4.Add(count);
				list4.Add(count + 1);
				list4.Add(count + 2);
				list4.Add(count + 1);
				list4.Add(count + 3);
				list4.Add(count + 2);
			}
			list.Add(new Vector3(num2, 0f, z));
			list.Add(new Vector3(num2, 1f, z));
			list2.Add(new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
			list2.Add(new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
			list3.Add(new Vector2(0f, 1f));
			list3.Add(new Vector2(1f, 1f));
		}
		int count2 = list.Count;
		for (int k = 0; k < count2; k++)
		{
			Vector3 item = new Vector3(list[k].z, list[k].y, list[k].x);
			list.Add(item);
			list3.Add(list3[k]);
			list2.Add(new Color32(0, byte.MaxValue, 0, byte.MaxValue));
		}
		int count3 = list4.Count;
		for (int l = 0; l < count3; l++)
		{
			list4.Add(count2 + list4[l]);
		}
		mesh.SetVertices(list);
		mesh.SetUVs(0, list3);
		mesh.SetColors(list2);
		mesh.SetTriangles(list4, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.UploadMeshData(false);
		component.sharedMesh = mesh;
	}

	[InspectorButton("CreateMesh", "Create Mesh")]
	public float maxDistance = 20f;

	public int layers = 160;

	public float separation = 0.5f;
}

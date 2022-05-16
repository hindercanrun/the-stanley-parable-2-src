using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Ferr
{
	public static class Export
	{
		public static void SaveOBJ(Mesh aMesh, string aFileName)
		{
			Vector3[] vertices = aMesh.vertices;
			Vector2[] uv = aMesh.uv;
			int[] triangles = aMesh.triangles;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("o {0}\n", aMesh.name);
			for (int i = 0; i < vertices.Length; i++)
			{
				stringBuilder.AppendFormat("v {0} {1} {2}\n", vertices[i].x, vertices[i].y, vertices[i].z);
				stringBuilder.AppendFormat("vt {0} {1}\n", uv[i].x, uv[i].y);
			}
			for (int j = 0; j < triangles.Length; j += 3)
			{
				stringBuilder.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}\n", triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1);
			}
			StreamWriter streamWriter = new StreamWriter(aFileName);
			streamWriter.Write(stringBuilder.ToString());
			streamWriter.Close();
			Debug.Log(aFileName);
		}

		public static void SavePLY(Mesh aMesh, string aFileName)
		{
			Vector3[] vertices = aMesh.vertices;
			Vector2[] uv = aMesh.uv;
			Color[] colors = aMesh.colors;
			int[] triangles = aMesh.triangles;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("ply\r\nformat ascii 1.0\r\nelement vertex {0}\r\nproperty float x\r\nproperty float y\r\nproperty float z\r\nproperty float s\r\nproperty float t\r\nproperty float red\r\nproperty float green\r\nproperty float blue\r\nproperty float alpha\r\nelement face {1}\r\nproperty list uchar int vertex_index\r\nend_header\r\n", vertices.Length, triangles.Length / 3);
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = vertices[i];
				Color color = colors[i];
				Vector2 vector2 = uv[i];
				stringBuilder.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8}\n", new object[]
				{
					vector.x,
					vector.z,
					vector.y,
					vector2.x,
					vector2.y,
					color.r,
					color.g,
					color.b,
					color.a
				});
			}
			for (int j = 0; j < triangles.Length; j += 3)
			{
				stringBuilder.AppendFormat("3 {2} {1} {0}\n", triangles[j], triangles[j + 1], triangles[j + 2]);
			}
			StreamWriter streamWriter = new StreamWriter(aFileName);
			streamWriter.Write(stringBuilder.ToString());
			streamWriter.Close();
			Debug.Log(aFileName);
		}
	}
}

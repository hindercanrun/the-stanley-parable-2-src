using System;
using UnityEngine;

namespace MeshBrush
{
	public static class CombineUtility
	{
		public static Mesh Combine(CombineUtility.MeshInstance[] combines, bool generateStrips)
		{
			CombineUtility.vertexCount = 0;
			CombineUtility.triangleCount = 0;
			CombineUtility.stripCount = 0;
			foreach (CombineUtility.MeshInstance meshInstance in combines)
			{
				if (meshInstance.mesh != null)
				{
					CombineUtility.vertexCount += meshInstance.mesh.vertexCount;
					if (generateStrips)
					{
						CombineUtility.curStripCount = meshInstance.mesh.GetTriangles(meshInstance.subMeshIndex).Length;
						if (CombineUtility.curStripCount != 0)
						{
							if (CombineUtility.stripCount != 0)
							{
								if ((CombineUtility.stripCount & 1) == 1)
								{
									CombineUtility.stripCount += 3;
								}
								else
								{
									CombineUtility.stripCount += 2;
								}
							}
							CombineUtility.stripCount += CombineUtility.curStripCount;
						}
						else
						{
							generateStrips = false;
						}
					}
				}
			}
			if (!generateStrips)
			{
				foreach (CombineUtility.MeshInstance meshInstance2 in combines)
				{
					if (meshInstance2.mesh != null)
					{
						CombineUtility.triangleCount += meshInstance2.mesh.GetTriangles(meshInstance2.subMeshIndex).Length;
					}
				}
			}
			CombineUtility.vertices = new Vector3[CombineUtility.vertexCount];
			CombineUtility.normals = new Vector3[CombineUtility.vertexCount];
			CombineUtility.tangents = new Vector4[CombineUtility.vertexCount];
			CombineUtility.uv = new Vector2[CombineUtility.vertexCount];
			CombineUtility.uv1 = new Vector2[CombineUtility.vertexCount];
			CombineUtility.colors = new Color[CombineUtility.vertexCount];
			CombineUtility.triangles = new int[CombineUtility.triangleCount];
			CombineUtility.strip = new int[CombineUtility.stripCount];
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance3 in combines)
			{
				if (meshInstance3.mesh != null)
				{
					CombineUtility.Copy(meshInstance3.mesh.vertexCount, meshInstance3.mesh.vertices, CombineUtility.vertices, ref CombineUtility.offset, meshInstance3.transform);
				}
			}
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance4 in combines)
			{
				if (meshInstance4.mesh != null)
				{
					CombineUtility.invTranspose = meshInstance4.transform;
					CombineUtility.invTranspose = CombineUtility.invTranspose.inverse.transpose;
					CombineUtility.CopyNormal(meshInstance4.mesh.vertexCount, meshInstance4.mesh.normals, CombineUtility.normals, ref CombineUtility.offset, CombineUtility.invTranspose);
				}
			}
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance5 in combines)
			{
				if (meshInstance5.mesh != null)
				{
					CombineUtility.invTranspose = meshInstance5.transform;
					CombineUtility.invTranspose = CombineUtility.invTranspose.inverse.transpose;
					CombineUtility.CopyTangents(meshInstance5.mesh.vertexCount, meshInstance5.mesh.tangents, CombineUtility.tangents, ref CombineUtility.offset, CombineUtility.invTranspose);
				}
			}
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance6 in combines)
			{
				if (meshInstance6.mesh != null)
				{
					CombineUtility.Copy(meshInstance6.mesh.vertexCount, meshInstance6.mesh.uv, CombineUtility.uv, ref CombineUtility.offset);
				}
			}
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance7 in combines)
			{
				if (meshInstance7.mesh != null)
				{
					CombineUtility.Copy(meshInstance7.mesh.vertexCount, meshInstance7.mesh.uv2, CombineUtility.uv1, ref CombineUtility.offset);
				}
			}
			CombineUtility.offset = 0;
			foreach (CombineUtility.MeshInstance meshInstance8 in combines)
			{
				if (meshInstance8.mesh != null)
				{
					CombineUtility.CopyColors(meshInstance8.mesh.vertexCount, meshInstance8.mesh.colors, CombineUtility.colors, ref CombineUtility.offset);
				}
			}
			CombineUtility.triangleOffset = 0;
			CombineUtility.stripOffset = 0;
			CombineUtility.vertexOffset = 0;
			foreach (CombineUtility.MeshInstance meshInstance9 in combines)
			{
				if (meshInstance9.mesh != null)
				{
					if (generateStrips)
					{
						int[] array = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
						if (CombineUtility.stripOffset != 0)
						{
							if ((CombineUtility.stripOffset & 1) == 1)
							{
								CombineUtility.strip[CombineUtility.stripOffset] = CombineUtility.strip[CombineUtility.stripOffset - 1];
								CombineUtility.strip[CombineUtility.stripOffset + 1] = array[0] + CombineUtility.vertexOffset;
								CombineUtility.strip[CombineUtility.stripOffset + 2] = array[0] + CombineUtility.vertexOffset;
								CombineUtility.stripOffset += 3;
							}
							else
							{
								CombineUtility.strip[CombineUtility.stripOffset] = CombineUtility.strip[CombineUtility.stripOffset - 1];
								CombineUtility.strip[CombineUtility.stripOffset + 1] = array[0] + CombineUtility.vertexOffset;
								CombineUtility.stripOffset += 2;
							}
						}
						for (int j = 0; j < array.Length; j++)
						{
							CombineUtility.strip[j + CombineUtility.stripOffset] = array[j] + CombineUtility.vertexOffset;
						}
						CombineUtility.stripOffset += array.Length;
					}
					else
					{
						int[] array2 = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
						for (int k = 0; k < array2.Length; k++)
						{
							CombineUtility.triangles[k + CombineUtility.triangleOffset] = array2[k] + CombineUtility.vertexOffset;
						}
						CombineUtility.triangleOffset += array2.Length;
					}
					CombineUtility.vertexOffset += meshInstance9.mesh.vertexCount;
				}
			}
			Mesh mesh = new Mesh();
			mesh.name = "Combined Mesh";
			mesh.vertices = CombineUtility.vertices;
			mesh.normals = CombineUtility.normals;
			mesh.colors = CombineUtility.colors;
			mesh.uv = CombineUtility.uv;
			mesh.uv2 = CombineUtility.uv1;
			mesh.tangents = CombineUtility.tangents;
			if (generateStrips)
			{
				mesh.SetTriangles(CombineUtility.strip, 0);
			}
			else
			{
				mesh.triangles = CombineUtility.triangles;
			}
			return mesh;
		}

		private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
		{
			for (int i = 0; i < src.Length; i++)
			{
				dst[i + offset] = transform.MultiplyPoint(src[i]);
			}
			offset += vertexcount;
		}

		private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
		{
			for (int i = 0; i < src.Length; i++)
			{
				dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
			}
			offset += vertexcount;
		}

		private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
		{
			for (int i = 0; i < src.Length; i++)
			{
				dst[i + offset] = src[i];
			}
			offset += vertexcount;
		}

		private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
		{
			for (int i = 0; i < src.Length; i++)
			{
				dst[i + offset] = src[i];
			}
			offset += vertexcount;
		}

		private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
		{
			for (int i = 0; i < src.Length; i++)
			{
				CombineUtility.p4 = src[i];
				CombineUtility.p = new Vector3(CombineUtility.p4.x, CombineUtility.p4.y, CombineUtility.p4.z);
				CombineUtility.p = transform.MultiplyVector(CombineUtility.p).normalized;
				dst[i + offset] = new Vector4(CombineUtility.p.x, CombineUtility.p.y, CombineUtility.p.z, CombineUtility.p4.w);
			}
			offset += vertexcount;
		}

		private static int vertexCount;

		private static int triangleCount;

		private static int stripCount;

		private static int curStripCount;

		private static Vector3[] vertices;

		private static Vector3[] normals;

		private static Vector4[] tangents;

		private static Vector2[] uv;

		private static Vector2[] uv1;

		private static Color[] colors;

		private static int[] triangles;

		private static int[] strip;

		private static int offset;

		private static int triangleOffset;

		private static int stripOffset;

		private static int vertexOffset;

		private static Matrix4x4 invTranspose;

		public const string combinedMeshName = "Combined Mesh";

		private static Vector4 p4;

		private static Vector3 p;

		public struct MeshInstance
		{
			public Mesh mesh;

			public int subMeshIndex;

			public Matrix4x4 transform;
		}
	}
}

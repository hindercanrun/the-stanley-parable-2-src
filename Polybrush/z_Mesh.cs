using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polybrush
{
	public class z_Mesh
	{
		public MeshTopology GetTopology(int index)
		{
			return this.meshTopology[index];
		}

		public int subMeshCount
		{
			get
			{
				return this._subMeshCount;
			}
			set
			{
				int[][] destinationArray = new int[value][];
				MeshTopology[] destinationArray2 = new MeshTopology[value];
				if (this.indices != null)
				{
					Array.Copy(this.indices, 0, destinationArray, 0, this._subMeshCount);
				}
				Array.Copy(this.meshTopology, 0, destinationArray2, 0, this._subMeshCount);
				this.indices = destinationArray;
				this.meshTopology = destinationArray2;
				this._subMeshCount = value;
			}
		}

		public int vertexCount
		{
			get
			{
				if (this.vertices == null)
				{
					return 0;
				}
				return this.vertices.Length;
			}
		}

		public List<Vector4> GetUVs(int index)
		{
			if (index == 0)
			{
				return this.uv0;
			}
			if (index == 1)
			{
				return this.uv1;
			}
			if (index == 2)
			{
				return this.uv2;
			}
			if (index == 3)
			{
				return this.uv3;
			}
			return null;
		}

		public void SetUVs(int index, List<Vector4> uvs)
		{
			if (index == 0)
			{
				this.uv0 = uvs;
				return;
			}
			if (index == 1)
			{
				this.uv1 = uvs;
				return;
			}
			if (index == 2)
			{
				this.uv2 = uvs;
				return;
			}
			if (index == 3)
			{
				this.uv3 = uvs;
			}
		}

		public void Clear()
		{
			this.subMeshCount = 0;
			this.vertices = null;
			this.normals = null;
			this.colors = null;
			this.tangents = null;
			this.uv0 = null;
			this.uv1 = null;
			this.uv2 = null;
			this.uv3 = null;
		}

		public int[] GetTriangles()
		{
			if (this.triangles == null)
			{
				this.triangles = this.indices.SelectMany((int[] x) => x).ToArray<int>();
			}
			return this.triangles;
		}

		public int[] GetIndices(int index)
		{
			return this.indices[index];
		}

		public void SetTriangles(int[] triangles, int index)
		{
			this.indices[index] = triangles;
		}

		public void RecalculateNormals()
		{
			Vector3[] array = new Vector3[this.vertexCount];
			int[] array2 = new int[this.vertexCount];
			int[] array3 = this.triangles;
			for (int i = 0; i < array3.Length; i += 3)
			{
				int num = array3[i];
				int num2 = array3[i + 1];
				int num3 = array3[i + 2];
				Vector3 vector = z_Math.Normal(this.vertices[num], this.vertices[num2], this.vertices[num3]);
				Vector3[] array4 = array;
				int num4 = num;
				array4[num4].x = array4[num4].x + vector.x;
				Vector3[] array5 = array;
				int num5 = num2;
				array5[num5].x = array5[num5].x + vector.x;
				Vector3[] array6 = array;
				int num6 = num3;
				array6[num6].x = array6[num6].x + vector.x;
				Vector3[] array7 = array;
				int num7 = num;
				array7[num7].y = array7[num7].y + vector.y;
				Vector3[] array8 = array;
				int num8 = num2;
				array8[num8].y = array8[num8].y + vector.y;
				Vector3[] array9 = array;
				int num9 = num3;
				array9[num9].y = array9[num9].y + vector.y;
				Vector3[] array10 = array;
				int num10 = num;
				array10[num10].z = array10[num10].z + vector.z;
				Vector3[] array11 = array;
				int num11 = num2;
				array11[num11].z = array11[num11].z + vector.z;
				Vector3[] array12 = array;
				int num12 = num3;
				array12[num12].z = array12[num12].z + vector.z;
				array2[num]++;
				array2[num2]++;
				array2[num3]++;
			}
			for (int j = 0; j < this.vertexCount; j++)
			{
				this.normals[j].x = array[j].x * (float)array2[j];
				this.normals[j].y = array[j].y * (float)array2[j];
				this.normals[j].z = array[j].z * (float)array2[j];
			}
		}

		public void ApplyAttributesToUnityMesh(Mesh m, z_MeshChannel attrib = z_MeshChannel.All)
		{
			if (attrib == z_MeshChannel.All)
			{
				m.vertices = this.vertices;
				m.normals = this.normals;
				m.colors32 = this.colors;
				m.tangents = this.tangents;
				m.SetUVs(0, this.uv0);
				m.SetUVs(1, this.uv1);
				m.SetUVs(2, this.uv2);
				m.SetUVs(3, this.uv3);
				return;
			}
			if ((attrib & z_MeshChannel.Null) > z_MeshChannel.Null)
			{
				m.vertices = this.vertices;
			}
			if ((attrib & z_MeshChannel.Normal) > z_MeshChannel.Null)
			{
				m.normals = this.normals;
			}
			if ((attrib & z_MeshChannel.Color) > z_MeshChannel.Null)
			{
				m.colors32 = this.colors;
			}
			if ((attrib & z_MeshChannel.Tangent) > z_MeshChannel.Null)
			{
				m.tangents = this.tangents;
			}
			if ((attrib & z_MeshChannel.UV0) > z_MeshChannel.Null)
			{
				m.SetUVs(0, this.uv0);
			}
			if ((attrib & z_MeshChannel.UV2) > z_MeshChannel.Null)
			{
				m.SetUVs(1, this.uv1);
			}
			if ((attrib & z_MeshChannel.UV3) > z_MeshChannel.Null)
			{
				m.SetUVs(2, this.uv2);
			}
			if ((attrib & z_MeshChannel.UV4) > z_MeshChannel.Null)
			{
				m.SetUVs(3, this.uv3);
			}
		}

		public string name = "";

		public Vector3[] vertices;

		public Vector3[] normals;

		public Color32[] colors;

		public Vector4[] tangents;

		public List<Vector4> uv0;

		public List<Vector4> uv1;

		public List<Vector4> uv2;

		public List<Vector4> uv3;

		private int _subMeshCount;

		private int[] triangles;

		private int[][] indices;

		private MeshTopology[] meshTopology = new MeshTopology[1];
	}
}

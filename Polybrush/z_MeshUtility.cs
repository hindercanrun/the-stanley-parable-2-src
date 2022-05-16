using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Polybrush
{
	public static class z_MeshUtility
	{
		public static Mesh DeepCopy(Mesh src)
		{
			Mesh mesh = new Mesh();
			z_MeshUtility.Copy(mesh, src);
			return mesh;
		}

		public static void Copy(Mesh dest, Mesh src)
		{
			dest.Clear();
			dest.vertices = src.vertices;
			List<Vector4> uvs = new List<Vector4>();
			src.GetUVs(0, uvs);
			dest.SetUVs(0, uvs);
			src.GetUVs(1, uvs);
			dest.SetUVs(1, uvs);
			src.GetUVs(2, uvs);
			dest.SetUVs(2, uvs);
			src.GetUVs(3, uvs);
			dest.SetUVs(3, uvs);
			dest.normals = src.normals;
			dest.tangents = src.tangents;
			dest.boneWeights = src.boneWeights;
			dest.colors = src.colors;
			dest.colors32 = src.colors32;
			dest.bindposes = src.bindposes;
			dest.subMeshCount = src.subMeshCount;
			for (int i = 0; i < src.subMeshCount; i++)
			{
				dest.SetIndices(src.GetIndices(i), src.GetTopology(i), i);
			}
			dest.name = z_Util.IncrementPrefix("z", src.name);
		}

		public static Mesh CreateOverlayMesh(z_Mesh src)
		{
			Mesh mesh = new Mesh();
			mesh.name = "Overlay Mesh: " + src.name;
			mesh.vertices = src.vertices;
			mesh.normals = src.normals;
			mesh.colors = z_Util.Fill<Color>(new Color(0f, 0f, 0f, 0f), mesh.vertexCount);
			mesh.subMeshCount = src.subMeshCount;
			for (int i = 0; i < src.subMeshCount; i++)
			{
				if (src.GetTopology(i) == MeshTopology.Triangles)
				{
					int[] indices = src.GetIndices(i);
					int[] array = new int[indices.Length * 2];
					int num = 0;
					for (int j = 0; j < indices.Length; j += 3)
					{
						array[num++] = indices[j];
						array[num++] = indices[j + 1];
						array[num++] = indices[j + 1];
						array[num++] = indices[j + 2];
						array[num++] = indices[j + 2];
						array[num++] = indices[j];
					}
					mesh.SetIndices(array, MeshTopology.Lines, i);
				}
				else
				{
					mesh.SetIndices(src.GetIndices(i), src.GetTopology(i), i);
				}
			}
			return mesh;
		}

		public static Mesh CreateVertexBillboardMesh(z_Mesh src, List<List<int>> common)
		{
			int num = Math.Min(16383, common.Count<List<int>>());
			Vector3[] array = new Vector3[num * 4];
			Vector2[] array2 = new Vector2[num * 4];
			Vector2[] array3 = new Vector2[num * 4];
			Color[] array4 = new Color[num * 4];
			int[] array5 = new int[num * 6];
			int num2 = 0;
			int num3 = 0;
			Vector3 up = Vector3.up;
			Vector3 right = Vector3.right;
			Vector3[] vertices = src.vertices;
			for (int i = 0; i < num; i++)
			{
				int num4 = common[i][0];
				array[num3] = vertices[num4];
				array[num3 + 1] = vertices[num4];
				array[num3 + 2] = vertices[num4];
				array[num3 + 3] = vertices[num4];
				array2[num3] = Vector3.zero;
				array2[num3 + 1] = Vector3.right;
				array2[num3 + 2] = Vector3.up;
				array2[num3 + 3] = Vector3.one;
				array3[num3] = -up - right;
				array3[num3 + 1] = -up + right;
				array3[num3 + 2] = up - right;
				array3[num3 + 3] = up + right;
				array5[num2] = num3;
				array5[num2 + 1] = num3 + 1;
				array5[num2 + 2] = num3 + 2;
				array5[num2 + 3] = num3 + 1;
				array5[num2 + 4] = num3 + 3;
				array5[num2 + 5] = num3 + 2;
				array4[num3] = z_MeshUtility.clear;
				array4[num3 + 1] = z_MeshUtility.clear;
				array4[num3 + 2] = z_MeshUtility.clear;
				array4[num3 + 3] = z_MeshUtility.clear;
				num3 += 4;
				num2 += 6;
			}
			return new Mesh
			{
				vertices = array,
				uv = array2,
				uv2 = array3,
				colors = array4,
				triangles = array5
			};
		}

		public static Dictionary<int, Vector3> GetSmoothNormalLookup(z_Mesh mesh)
		{
			Vector3[] normals = mesh.normals;
			Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
			if (normals == null || normals.Length != mesh.vertexCount)
			{
				return dictionary;
			}
			List<List<int>> commonVertices = z_MeshUtility.GetCommonVertices(mesh);
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			foreach (List<int> list in commonVertices)
			{
				vector.x = 0f;
				vector.y = 0f;
				vector.z = 0f;
				foreach (int num in list)
				{
					vector2 = normals[num];
					vector.x += vector2.x;
					vector.y += vector2.y;
					vector.z += vector2.z;
				}
				vector /= (float)list.Count<int>();
				foreach (int key in list)
				{
					dictionary.Add(key, vector);
				}
			}
			return dictionary;
		}

		public static List<List<int>> GetCommonVertices(z_Mesh mesh)
		{
			List<List<int>> list;
			if (z_MeshUtility.commonVerticesCache.TryGetValue(mesh, out list))
			{
				return list;
			}
			Vector3[] v = mesh.vertices;
			list = (from y in z_Util.Fill<int>((int x) => x, v.Length).ToLookup((int x) => v[x])
			select y.ToList<int>()).ToList<List<int>>();
			if (!z_MeshUtility.commonVerticesCache.ContainsKey(mesh))
			{
				z_MeshUtility.commonVerticesCache.Add(mesh, list);
			}
			else
			{
				z_MeshUtility.commonVerticesCache[mesh] = list;
			}
			return list;
		}

		public static List<z_CommonEdge> GetEdges(z_Mesh m)
		{
			Dictionary<int, int> commonLookup = z_MeshUtility.GetCommonVertices(m).GetCommonLookup<int>();
			return z_MeshUtility.GetEdges(m, commonLookup);
		}

		public static List<z_CommonEdge> GetEdges(z_Mesh m, Dictionary<int, int> lookup)
		{
			int[] triangles = m.GetTriangles();
			int num = triangles.Length;
			List<z_CommonEdge> list = new List<z_CommonEdge>(num);
			for (int i = 0; i < num; i += 3)
			{
				list.Add(new z_CommonEdge(triangles[i], triangles[i + 1], lookup[triangles[i]], lookup[triangles[i + 1]]));
				list.Add(new z_CommonEdge(triangles[i + 1], triangles[i + 2], lookup[triangles[i + 1]], lookup[triangles[i + 2]]));
				list.Add(new z_CommonEdge(triangles[i + 2], triangles[i], lookup[triangles[i + 2]], lookup[triangles[i]]));
			}
			return list;
		}

		public static HashSet<z_CommonEdge> GetEdgesDistinct(z_Mesh mesh, out List<z_CommonEdge> duplicates)
		{
			Dictionary<int, int> commonLookup = z_MeshUtility.GetCommonVertices(mesh).GetCommonLookup<int>();
			return z_MeshUtility.GetEdgesDistinct(mesh, commonLookup, out duplicates);
		}

		private static HashSet<z_CommonEdge> GetEdgesDistinct(z_Mesh m, Dictionary<int, int> lookup, out List<z_CommonEdge> duplicates)
		{
			int[] triangles = m.GetTriangles();
			int num = triangles.Length;
			HashSet<z_CommonEdge> hashSet = new HashSet<z_CommonEdge>();
			duplicates = new List<z_CommonEdge>();
			for (int i = 0; i < num; i += 3)
			{
				z_CommonEdge item = new z_CommonEdge(triangles[i], triangles[i + 1], lookup[triangles[i]], lookup[triangles[i + 1]]);
				z_CommonEdge item2 = new z_CommonEdge(triangles[i + 1], triangles[i + 2], lookup[triangles[i + 1]], lookup[triangles[i + 2]]);
				z_CommonEdge item3 = new z_CommonEdge(triangles[i + 2], triangles[i], lookup[triangles[i + 2]], lookup[triangles[i]]);
				if (!hashSet.Add(item))
				{
					duplicates.Add(item);
				}
				if (!hashSet.Add(item2))
				{
					duplicates.Add(item2);
				}
				if (!hashSet.Add(item3))
				{
					duplicates.Add(item3);
				}
			}
			return hashSet;
		}

		public static HashSet<int> GetNonManifoldIndices(z_Mesh mesh)
		{
			List<z_CommonEdge> other;
			HashSet<z_CommonEdge> edgesDistinct = z_MeshUtility.GetEdgesDistinct(mesh, out other);
			edgesDistinct.ExceptWith(other);
			return z_CommonEdge.ToHashSet(edgesDistinct);
		}

		public static Dictionary<int, List<int>> GetAdjacentVertices(z_Mesh mesh)
		{
			List<List<int>> commonVertices = z_MeshUtility.GetCommonVertices(mesh);
			Dictionary<int, int> commonLookup = commonVertices.GetCommonLookup<int>();
			List<z_CommonEdge> list = z_MeshUtility.GetEdges(mesh, commonLookup).ToList<z_CommonEdge>();
			List<List<int>> list2 = new List<List<int>>();
			for (int i = 0; i < commonVertices.Count<List<int>>(); i++)
			{
				list2.Add(new List<int>());
			}
			for (int j = 0; j < list.Count; j++)
			{
				list2[list[j].cx].Add(list[j].y);
				list2[list[j].cy].Add(list[j].x);
			}
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			foreach (int key in mesh.GetTriangles().Distinct<int>())
			{
				dictionary.Add(key, list2[commonLookup[key]]);
			}
			return dictionary;
		}

		public static Dictionary<z_Edge, List<int>> GetAdjacentTriangles(z_Mesh m)
		{
			int num = m.GetTriangles().Length;
			if (num % 3 != 0 || num / 3 == m.vertexCount)
			{
				return new Dictionary<z_Edge, List<int>>();
			}
			Dictionary<z_Edge, List<int>> dictionary = null;
			if (z_MeshUtility.adjacentTrianglesCache.TryGetValue(m, out dictionary))
			{
				return dictionary;
			}
			int subMeshCount = m.subMeshCount;
			dictionary = new Dictionary<z_Edge, List<int>>();
			for (int i = 0; i < subMeshCount; i++)
			{
				int[] indices = m.GetIndices(i);
				for (int j = 0; j < indices.Length; j += 3)
				{
					int item = j / 3;
					z_Edge key = new z_Edge(indices[j], indices[j + 1]);
					z_Edge key2 = new z_Edge(indices[j + 1], indices[j + 2]);
					z_Edge key3 = new z_Edge(indices[j + 2], indices[j]);
					List<int> list;
					if (dictionary.TryGetValue(key, out list))
					{
						list.Add(item);
					}
					else
					{
						dictionary.Add(key, new List<int>
						{
							item
						});
					}
					if (dictionary.TryGetValue(key2, out list))
					{
						list.Add(item);
					}
					else
					{
						dictionary.Add(key2, new List<int>
						{
							item
						});
					}
					if (dictionary.TryGetValue(key3, out list))
					{
						list.Add(item);
					}
					else
					{
						dictionary.Add(key3, new List<int>
						{
							item
						});
					}
				}
			}
			z_MeshUtility.adjacentTrianglesCache.Add(m, dictionary);
			return dictionary;
		}

		public static List<List<int>> GetSmoothSeamLookup(z_Mesh m)
		{
			Vector3[] normals = m.normals;
			if (normals == null)
			{
				return null;
			}
			List<List<int>> result = null;
			if (z_MeshUtility.commonNormalsCache.TryGetValue(m, out result))
			{
				return result;
			}
			Func<int, z_RndVec3> <>9__3;
			List<List<int>> list = (from n in z_MeshUtility.GetCommonVertices(m).SelectMany(delegate(List<int> x)
			{
				Func<int, z_RndVec3> keySelector;
				if ((keySelector = <>9__3) == null)
				{
					keySelector = (<>9__3 = ((int i) => normals[i]));
				}
				return x.GroupBy(keySelector);
			})
			where n.Count<int>() > 1
			select n into t
			select t.ToList<int>()).ToList<List<int>>();
			z_MeshUtility.commonNormalsCache.Add(m, list);
			return list;
		}

		public static void RecalculateNormals(z_Mesh m)
		{
			List<List<int>> smoothSeamLookup = z_MeshUtility.GetSmoothSeamLookup(m);
			m.RecalculateNormals();
			if (smoothSeamLookup != null)
			{
				Vector3[] normals = m.normals;
				foreach (List<int> list in smoothSeamLookup)
				{
					Vector3 vector = z_Math.Average(normals, list);
					foreach (int num in list)
					{
						normals[num] = vector;
					}
				}
				m.normals = normals;
			}
		}

		public static string Print(Mesh m, int maxAttributesToList = 8)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0,-28}{1,-28}{2,-28}{3,-28}{4,-28}{5,-28}{6,-28}", new object[]
			{
				"Positions",
				"Colors",
				"Tangents",
				"UV0",
				"UV2",
				"UV3",
				"UV4"
			}));
			stringBuilder.AppendLine(string.Format("vertices: {0}   triangles: {1}", m.vertexCount, m.triangles.Length));
			Vector3[] array = m.vertices;
			Color[] array2 = m.colors;
			Vector4[] array3 = m.tangents;
			List<Vector4> list = new List<Vector4>();
			Vector2[] array4 = m.uv2;
			List<Vector4> list2 = new List<Vector4>();
			List<Vector4> list3 = new List<Vector4>();
			m.GetUVs(0, list);
			m.GetUVs(2, list2);
			m.GetUVs(3, list3);
			if (array != null && array.Count<Vector3>() != m.vertexCount)
			{
				array = null;
			}
			if (array2 != null && array2.Count<Color>() != m.vertexCount)
			{
				array2 = null;
			}
			if (array3 != null && array3.Count<Vector4>() != m.vertexCount)
			{
				array3 = null;
			}
			if (list != null && list.Count<Vector4>() != m.vertexCount)
			{
				list = null;
			}
			if (array4 != null && array4.Count<Vector2>() != m.vertexCount)
			{
				array4 = null;
			}
			if (list2 != null && list2.Count<Vector4>() != m.vertexCount)
			{
				list2 = null;
			}
			if (list3 != null && list3.Count<Vector4>() != m.vertexCount)
			{
				list3 = null;
			}
			int num = m.vertexCount;
			if (maxAttributesToList > -1 && maxAttributesToList < num)
			{
				num = maxAttributesToList;
			}
			for (int i = 0; i < num; i++)
			{
				stringBuilder.AppendLine(string.Format("{0,-28}{1,-28}{2,-28}{3,-28}{4,-28}{5,-28}{6,-28}", new object[]
				{
					(array == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}", array[i].x, array[i].y, array[i].z),
					(array2 == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
					{
						array2[i].r,
						array2[i].g,
						array2[i].b,
						array2[i].a
					}),
					(array3 == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
					{
						array3[i].x,
						array3[i].y,
						array3[i].z,
						array3[i].w
					}),
					(list == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
					{
						list[i].x,
						list[i].y,
						list[i].z,
						list[i].w
					}),
					(array4 == null) ? "null" : string.Format("{0:F2}, {1:F2}", array4[i].x, array4[i].y),
					(list2 == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
					{
						list2[i].x,
						list2[i].y,
						list2[i].z,
						list2[i].w
					}),
					(list3 == null) ? "null" : string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
					{
						list3[i].x,
						list3[i].y,
						list3[i].z,
						list3[i].w
					})
				}));
			}
			int num2 = m.triangles.Length;
			if (maxAttributesToList > -1 && maxAttributesToList * 3 < num2)
			{
				num2 = maxAttributesToList * 3;
			}
			for (int j = 0; j < num2; j += 3)
			{
				stringBuilder.AppendLine(string.Format("{0}, {1}, {2}", m.triangles[j], m.triangles[j + 1], m.triangles[j + 2]));
			}
			return stringBuilder.ToString();
		}

		private static readonly Color clear = new Color(0f, 0f, 0f, 0f);

		public static Dictionary<z_Mesh, List<List<int>>> commonVerticesCache = new Dictionary<z_Mesh, List<List<int>>>();

		private static Dictionary<z_Mesh, Dictionary<z_Edge, List<int>>> adjacentTrianglesCache = new Dictionary<z_Mesh, Dictionary<z_Edge, List<int>>>();

		private static Dictionary<z_Mesh, List<List<int>>> commonNormalsCache = new Dictionary<z_Mesh, List<List<int>>>();
	}
}

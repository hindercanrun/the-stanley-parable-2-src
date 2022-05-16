using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polybrush
{
	public static class z_Math
	{
		public static bool RayIntersectsTriangle2(Vector3 origin, Vector3 dir, Vector3 vert0, Vector3 vert1, Vector3 vert2, ref float distance, ref Vector3 normal)
		{
			z_Vector.Subtract(vert0, vert1, ref z_Math.tv1);
			z_Vector.Subtract(vert0, vert2, ref z_Math.tv2);
			z_Vector.Cross(dir, z_Math.tv2, ref z_Math.tv4);
			float num = Vector3.Dot(z_Math.tv1, z_Math.tv4);
			if (num < Mathf.Epsilon)
			{
				return false;
			}
			z_Vector.Subtract(vert0, origin, ref z_Math.tv3);
			float num2 = Vector3.Dot(z_Math.tv3, z_Math.tv4);
			if (num2 < 0f || num2 > num)
			{
				return false;
			}
			z_Vector.Cross(z_Math.tv3, z_Math.tv1, ref z_Math.tv4);
			float num3 = Vector3.Dot(dir, z_Math.tv4);
			if (num3 < 0f || num2 + num3 > num)
			{
				return false;
			}
			distance = Vector3.Dot(z_Math.tv2, z_Math.tv4) * (1f / num);
			z_Vector.Cross(z_Math.tv1, z_Math.tv2, ref normal);
			return true;
		}

		public static bool RayIntersectsTriangle(Vector3 origin, Vector3 direction, Vector3 InTriangleA, Vector3 InTriangleB, Vector3 InTriangleC, out float OutDistance, out Vector3 OutPoint)
		{
			OutDistance = 0f;
			OutPoint = new Vector3(0f, 0f, 0f);
			z_Math.tv1.x = InTriangleB.x - InTriangleA.x;
			z_Math.tv1.y = InTriangleB.y - InTriangleA.y;
			z_Math.tv1.z = InTriangleB.z - InTriangleA.z;
			z_Math.tv2.x = InTriangleC.x - InTriangleA.x;
			z_Math.tv2.y = InTriangleC.y - InTriangleA.y;
			z_Math.tv2.z = InTriangleC.z - InTriangleA.z;
			z_Vector.Cross(direction, z_Math.tv2, ref z_Math.tv3.x, ref z_Math.tv3.y, ref z_Math.tv3.z);
			float num = Vector3.Dot(z_Math.tv1, z_Math.tv3);
			if (num > -Mathf.Epsilon && num < Mathf.Epsilon)
			{
				return false;
			}
			float num2 = 1f / num;
			Vector3 vector;
			vector.x = origin.x - InTriangleA.x;
			vector.y = origin.y - InTriangleA.y;
			vector.z = origin.z - InTriangleA.z;
			float num3 = Vector3.Dot(vector, z_Math.tv3) * num2;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			z_Vector.Cross(vector, z_Math.tv1, ref z_Math.tv4.x, ref z_Math.tv4.y, ref z_Math.tv4.z);
			float num4 = Vector3.Dot(direction, z_Math.tv4) * num2;
			if (num4 < 0f || num3 + num4 > 1f)
			{
				return false;
			}
			float num5 = Vector3.Dot(z_Math.tv2, z_Math.tv4) * num2;
			if (num5 > Mathf.Epsilon)
			{
				OutDistance = num5;
				OutPoint.x = num3 * InTriangleB.x + num4 * InTriangleC.x + (1f - (num3 + num4)) * InTriangleA.x;
				OutPoint.y = num3 * InTriangleB.y + num4 * InTriangleC.y + (1f - (num3 + num4)) * InTriangleA.y;
				OutPoint.z = num3 * InTriangleB.z + num4 * InTriangleC.z + (1f - (num3 + num4)) * InTriangleA.z;
				return true;
			}
			return false;
		}

		public static Vector3 Normal(Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float ax = p1.x - p0.x;
			float ay = p1.y - p0.y;
			float az = p1.z - p0.z;
			float bx = p2.x - p0.x;
			float by = p2.y - p0.y;
			float bz = p2.z - p0.z;
			Vector3 zero = Vector3.zero;
			z_Vector.Cross(ax, ay, az, bx, by, bz, ref zero.x, ref zero.y, ref zero.z);
			zero.Normalize();
			if (zero.magnitude < Mathf.Epsilon)
			{
				return new Vector3(0f, 0f, 0f);
			}
			return zero;
		}

		public static Vector3 Normal(Vector3[] p)
		{
			if (p.Length < 3)
			{
				return Vector3.zero;
			}
			if (p.Length % 3 == 0)
			{
				Vector3 a = Vector3.zero;
				for (int i = 0; i < p.Length; i += 3)
				{
					a += z_Math.Normal(p[i], p[i + 1], p[i + 2]);
				}
				return a / ((float)p.Length / 3f);
			}
			Vector3 vector = Vector3.Cross(p[1] - p[0], p[2] - p[0]);
			if (vector.magnitude < Mathf.Epsilon)
			{
				return new Vector3(0f, 0f, 0f);
			}
			return vector.normalized;
		}

		public static void NormalTangentBitangent(Vector3[] vertices, Vector2[] uv, int[] tri, out Vector3 normal, out Vector3 tangent, out Vector3 bitangent)
		{
			normal = z_Math.Normal(vertices[tri[0]], vertices[tri[1]], vertices[tri[2]]);
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Vector4 vector3 = new Vector4(0f, 0f, 0f, 1f);
			long num = (long)tri[0];
			long num2 = (long)tri[1];
			long num3 = (long)tri[2];
			Vector3 vector4;
			Vector3 vector5;
			Vector3 vector6;
			Vector2 vector7;
			Vector2 vector8;
			Vector2 vector9;
			checked
			{
				vector4 = vertices[(int)((IntPtr)num)];
				vector5 = vertices[(int)((IntPtr)num2)];
				vector6 = vertices[(int)((IntPtr)num3)];
				vector7 = uv[(int)((IntPtr)num)];
				vector8 = uv[(int)((IntPtr)num2)];
				vector9 = uv[(int)((IntPtr)num3)];
			}
			float num4 = vector5.x - vector4.x;
			float num5 = vector6.x - vector4.x;
			float num6 = vector5.y - vector4.y;
			float num7 = vector6.y - vector4.y;
			float num8 = vector5.z - vector4.z;
			float num9 = vector6.z - vector4.z;
			float num10 = vector8.x - vector7.x;
			float num11 = vector9.x - vector7.x;
			float num12 = vector8.y - vector7.y;
			float num13 = vector9.y - vector7.y;
			float num14 = 1f / (num10 * num13 - num11 * num12);
			Vector3 b = new Vector3((num13 * num4 - num12 * num5) * num14, (num13 * num6 - num12 * num7) * num14, (num13 * num8 - num12 * num9) * num14);
			Vector3 b2 = new Vector3((num10 * num5 - num11 * num4) * num14, (num10 * num7 - num11 * num6) * num14, (num10 * num9 - num11 * num8) * num14);
			vector += b;
			vector2 += b2;
			Vector3 lhs = normal;
			Vector3.OrthoNormalize(ref lhs, ref vector);
			vector3.x = vector.x;
			vector3.y = vector.y;
			vector3.z = vector.z;
			vector3.w = ((Vector3.Dot(Vector3.Cross(lhs, vector), vector2) < 0f) ? -1f : 1f);
			tangent = vector3 * vector3.w;
			bitangent = Vector3.Cross(normal, tangent);
		}

		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				return min;
			}
			if (value <= max)
			{
				return value;
			}
			return max;
		}

		public static Vector3 Average(Vector3[] array, IEnumerable<int> indices)
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			foreach (int num2 in indices)
			{
				zero.x += array[num2].x;
				zero.y += array[num2].y;
				zero.z += array[num2].z;
				num++;
			}
			return zero / (float)num;
		}

		public static Vector3 WeightedAverage(Vector3[] array, IList<int> indices, float[] weightLookup)
		{
			float num = 0f;
			Vector3 a = Vector3.zero;
			for (int i = 0; i < indices.Count; i++)
			{
				float num2 = weightLookup[indices[i]];
				a.x += array[indices[i]].x * num2;
				a.y += array[indices[i]].y * num2;
				a.z += array[indices[i]].z * num2;
				num += num2;
			}
			if (num <= Mathf.Epsilon)
			{
				return Vector3.zero;
			}
			return a /= num;
		}

		public static bool VectorIsUniform(Vector3 vec)
		{
			return Mathf.Abs(vec.x - vec.y) < Mathf.Epsilon && Mathf.Abs(vec.x - vec.z) < Mathf.Epsilon;
		}

		private static Vector3 tv1;

		private static Vector3 tv2;

		private static Vector3 tv3;

		private static Vector3 tv4;
	}
}

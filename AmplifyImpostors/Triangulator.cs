using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyImpostors
{
	public class Triangulator
	{
		public List<Vector2> Points
		{
			get
			{
				return this.m_points;
			}
		}

		public Triangulator(Vector2[] points)
		{
			this.m_points = new List<Vector2>(points);
		}

		public Triangulator(Vector2[] points, bool invertY = true)
		{
			if (invertY)
			{
				this.m_points = new List<Vector2>();
				for (int i = 0; i < points.Length; i++)
				{
					this.m_points.Add(new Vector2(points[i].x, 1f - points[i].y));
				}
				return;
			}
			this.m_points = new List<Vector2>(points);
		}

		public int[] Triangulate()
		{
			List<int> list = new List<int>();
			int count = this.m_points.Count;
			if (count < 3)
			{
				return list.ToArray();
			}
			int[] array = new int[count];
			if (this.Area() > 0f)
			{
				for (int i = 0; i < count; i++)
				{
					array[i] = i;
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					array[j] = count - 1 - j;
				}
			}
			int k = count;
			int num = 2 * k;
			int num2 = 0;
			int num3 = k - 1;
			while (k > 2)
			{
				if (num-- <= 0)
				{
					return list.ToArray();
				}
				int num4 = num3;
				if (k <= num4)
				{
					num4 = 0;
				}
				num3 = num4 + 1;
				if (k <= num3)
				{
					num3 = 0;
				}
				int num5 = num3 + 1;
				if (k <= num5)
				{
					num5 = 0;
				}
				if (this.Snip(num4, num3, num5, k, array))
				{
					int item = array[num4];
					int item2 = array[num3];
					int item3 = array[num5];
					list.Add(item);
					list.Add(item2);
					list.Add(item3);
					num2++;
					int num6 = num3;
					for (int l = num3 + 1; l < k; l++)
					{
						array[num6] = array[l];
						num6++;
					}
					k--;
					num = 2 * k;
				}
			}
			list.Reverse();
			return list.ToArray();
		}

		private float Area()
		{
			int count = this.m_points.Count;
			float num = 0f;
			int index = count - 1;
			int i = 0;
			while (i < count)
			{
				Vector2 vector = this.m_points[index];
				Vector2 vector2 = this.m_points[i];
				num += vector.x * vector2.y - vector2.x * vector.y;
				index = i++;
			}
			return num * 0.5f;
		}

		private bool Snip(int u, int v, int w, int n, int[] V)
		{
			Vector2 vector = this.m_points[V[u]];
			Vector2 vector2 = this.m_points[V[v]];
			Vector2 vector3 = this.m_points[V[w]];
			if (Mathf.Epsilon > (vector2.x - vector.x) * (vector3.y - vector.y) - (vector2.y - vector.y) * (vector3.x - vector.x))
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (i != u && i != v && i != w)
				{
					Vector2 pt = this.m_points[V[i]];
					if (this.InsideTriangle(pt, vector, vector2, vector3))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool InsideTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
		{
			bool flag = pt.Cross(v1, v2) < 0f;
			bool flag2 = pt.Cross(v2, v3) < 0f;
			bool flag3 = pt.Cross(v3, v1) < 0f;
			return flag == flag2 && flag2 == flag3;
		}

		private List<Vector2> m_points = new List<Vector2>();
	}
}

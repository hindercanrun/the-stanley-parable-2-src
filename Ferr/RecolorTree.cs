using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr
{
	public class RecolorTree
	{
		public RecolorTree(Mesh aMesh, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true)
		{
			if (aMesh == null)
			{
				this.Create(new Vector3[]
				{
					Vector3.zero
				}, new Color[]
				{
					Color.white
				}, aTransform, aX, aY, aZ);
				return;
			}
			Vector3[] vertices = aMesh.vertices;
			Color[] array = aMesh.colors;
			if (array == null || array.Length == 0)
			{
				array = new Color[vertices.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Color.white;
				}
			}
			this.Create(vertices, array, aTransform, aX, aY, aZ);
		}

		public RecolorTree(Vector3[] aPoints, Color[] aColors, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true)
		{
			this.Create(aPoints, aColors, aTransform, aX, aY, aZ);
		}

		public RecolorTree(List<Vector3> aPoints, List<Color> aColors, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true)
		{
			this.Create(aPoints, aColors, aTransform, aX, aY, aZ);
		}

		public RecolorTree.TreePoint Get(Vector3 aAt)
		{
			RecolorTree.TreePoint result = null;
			float num = 0f;
			this.root.GetNearest(this.settings, 0, aAt, ref result, ref num);
			return result;
		}

		public void Recolor(ref Mesh aMesh, Matrix4x4? aTransform = null)
		{
			if (aMesh == null)
			{
				return;
			}
			Vector3[] vertices = aMesh.vertices;
			aMesh.colors = this.Recolor(vertices, aTransform);
		}

		public void Recolor(Vector3[] aPoints, ref Color[] aColors, Matrix4x4? aTransform = null)
		{
			if (aPoints.Length != aColors.Length)
			{
				Debug.LogError("Arguments must be the same length!");
			}
			if (aTransform != null)
			{
				for (int i = 0; i < aPoints.Length; i++)
				{
					aColors[i] = this.Get(aTransform.Value.MultiplyPoint(aPoints[i])).data;
				}
				return;
			}
			for (int j = 0; j < aPoints.Length; j++)
			{
				aColors[j] = this.Get(aPoints[j]).data;
			}
		}

		public Color[] Recolor(Vector3[] aAt, Matrix4x4? aTransform = null)
		{
			Color[] array = new Color[aAt.Length];
			if (aTransform != null)
			{
				for (int i = 0; i < aAt.Length; i++)
				{
					array[i] = this.Get(aTransform.Value.MultiplyPoint(aAt[i])).data;
				}
			}
			else
			{
				for (int j = 0; j < aAt.Length; j++)
				{
					array[j] = this.Get(aAt[j]).data;
				}
			}
			return array;
		}

		public List<Color> Recolor(List<Vector3> aAt, Matrix4x4? aTransform = null)
		{
			List<Color> list = new List<Color>(aAt.Count);
			if (aTransform != null)
			{
				for (int i = 0; i < aAt.Count; i++)
				{
					list[i] = this.Get(aTransform.Value.MultiplyPoint(aAt[i])).data;
				}
			}
			else
			{
				for (int j = 0; j < aAt.Count; j++)
				{
					list[j] = this.Get(aAt[j]).data;
				}
			}
			return list;
		}

		public void Recolor(List<Vector3> aPoints, ref List<Color> aColors, Matrix4x4? aTransform = null)
		{
			if (aTransform != null)
			{
				for (int i = 0; i < aPoints.Count; i++)
				{
					aColors.Add(this.Get(aTransform.Value.MultiplyPoint(aPoints[i])).data);
				}
				return;
			}
			for (int j = 0; j < aPoints.Count; j++)
			{
				aColors.Add(this.Get(aPoints[j]).data);
			}
		}

		public void DrawTree()
		{
			RecolorTree.TreeSettings aSettings = new RecolorTree.TreeSettings(true, true, true);
			this.root.Draw(aSettings, 0, Vector3.zero);
		}

		private void Create(Vector3[] aPoints, Color[] aColors, Matrix4x4? aTransform, bool aX, bool aY, bool aZ)
		{
			if (aPoints.Length != aColors.Length)
			{
				Debug.LogError("Arguments must be the same length!");
			}
			List<RecolorTree.TreePoint> list = new List<RecolorTree.TreePoint>(aPoints.Length);
			for (int i = 0; i < aPoints.Length; i++)
			{
				Vector3 vector = aPoints[i];
				if (aTransform != null)
				{
					vector = aTransform.Value.MultiplyPoint(vector);
				}
				list.Add(new RecolorTree.TreePoint(vector, aColors[i]));
			}
			this.settings = new RecolorTree.TreeSettings(aX, aY, aZ);
			this.root = new RecolorTree.TreeNode(this.settings, list, 0);
		}

		private void Create(List<Vector3> aPoints, List<Color> aColors, Matrix4x4? aTransform, bool aX, bool aY, bool aZ)
		{
			if (aPoints.Count != aColors.Count)
			{
				Debug.LogError("Arguments must be the same length!");
			}
			List<RecolorTree.TreePoint> list = new List<RecolorTree.TreePoint>(aPoints.Count);
			for (int i = 0; i < aPoints.Count; i++)
			{
				Vector3 vector = aPoints[i];
				if (aTransform != null)
				{
					vector = aTransform.Value.MultiplyPoint(vector);
				}
				list.Add(new RecolorTree.TreePoint(vector, aColors[i]));
			}
			this.settings = new RecolorTree.TreeSettings(aX, aY, aZ);
			this.root = new RecolorTree.TreeNode(this.settings, list, 0);
		}

		private static RecolorTree.SortX sortX = new RecolorTree.SortX();

		private static RecolorTree.SortY sortY = new RecolorTree.SortY();

		private static RecolorTree.SortZ sortZ = new RecolorTree.SortZ();

		private RecolorTree.TreeNode root;

		private RecolorTree.TreeSettings settings;

		private class SortX : IComparer<RecolorTree.TreePoint>
		{
			public int Compare(RecolorTree.TreePoint a, RecolorTree.TreePoint b)
			{
				return a.point.x.CompareTo(b.point.x);
			}
		}

		private class SortY : IComparer<RecolorTree.TreePoint>
		{
			public int Compare(RecolorTree.TreePoint a, RecolorTree.TreePoint b)
			{
				return a.point.y.CompareTo(b.point.y);
			}
		}

		private class SortZ : IComparer<RecolorTree.TreePoint>
		{
			public int Compare(RecolorTree.TreePoint a, RecolorTree.TreePoint b)
			{
				return a.point.z.CompareTo(b.point.z);
			}
		}

		public class TreeSettings
		{
			public TreeSettings(bool aUseX, bool aUseY, bool aUseZ)
			{
				List<int> list = new List<int>(3);
				if (aUseX)
				{
					list.Add(0);
				}
				if (aUseY)
				{
					list.Add(1);
				}
				if (aUseZ)
				{
					list.Add(2);
				}
				this.axes = list.ToArray();
			}

			public int GetAxis(int aDepth)
			{
				int num = aDepth % this.axes.Length;
				return this.axes[num];
			}

			public float AxisDist(int aAxis, Vector3 a, Vector3 b)
			{
				if (aAxis == 0)
				{
					return Mathf.Abs(a.x - b.x);
				}
				if (aAxis == 1)
				{
					return Mathf.Abs(a.y - b.y);
				}
				if (aAxis == 2)
				{
					return Mathf.Abs(a.z - b.z);
				}
				return 0f;
			}

			private int[] axes;
		}

		public class TreePoint
		{
			public TreePoint(Vector3 aPoint, Color aData)
			{
				this.point = aPoint;
				this.data = aData;
			}

			public Vector3 point;

			public Color data;
		}

		private class TreeNode
		{
			public bool IsLeaf
			{
				get
				{
					return this.left == null && this.right == null;
				}
			}

			public TreeNode(RecolorTree.TreeSettings aSettings, List<RecolorTree.TreePoint> aPoints, int aDepth)
			{
				int axis = aSettings.GetAxis(aDepth);
				if (axis == 0)
				{
					aPoints.Sort(RecolorTree.sortX);
				}
				else if (axis == 1)
				{
					aPoints.Sort(RecolorTree.sortY);
				}
				else if (axis == 2)
				{
					aPoints.Sort(RecolorTree.sortZ);
				}
				int num = aPoints.Count / 2;
				this.point = aPoints[num];
				List<RecolorTree.TreePoint> range = aPoints.GetRange(0, num);
				List<RecolorTree.TreePoint> range2 = aPoints.GetRange(num + 1, aPoints.Count - (num + 1));
				if (range.Count > 0)
				{
					this.left = new RecolorTree.TreeNode(aSettings, range, aDepth + 1);
				}
				if (range2.Count > 0)
				{
					this.right = new RecolorTree.TreeNode(aSettings, range2, aDepth + 1);
				}
			}

			public void GetNearest(RecolorTree.TreeSettings aSettings, int aDepth, Vector3 aPt, ref RecolorTree.TreePoint aClosest, ref float aClosestDist)
			{
				if (this.IsLeaf)
				{
					float sqrMagnitude = (this.point.point - aPt).sqrMagnitude;
					if (aClosest == null || sqrMagnitude < aClosestDist)
					{
						aClosest = this.point;
						aClosestDist = sqrMagnitude;
					}
					return;
				}
				int axis = aSettings.GetAxis(aDepth);
				bool flag = false;
				if (axis == 0)
				{
					flag = (aPt.x <= this.point.point.x);
				}
				else if (axis == 1)
				{
					flag = (aPt.y <= this.point.point.y);
				}
				else if (axis == 2)
				{
					flag = (aPt.z <= this.point.point.z);
				}
				RecolorTree.TreeNode treeNode = flag ? this.left : this.right;
				RecolorTree.TreeNode treeNode2 = flag ? this.right : this.left;
				if (treeNode == null)
				{
					treeNode = treeNode2;
					treeNode2 = null;
				}
				treeNode.GetNearest(aSettings, aDepth + 1, aPt, ref aClosest, ref aClosestDist);
				float sqrMagnitude2 = (this.point.point - aPt).sqrMagnitude;
				if (sqrMagnitude2 < aClosestDist)
				{
					aClosest = this.point;
					aClosestDist = sqrMagnitude2;
				}
				if (treeNode2 != null)
				{
					float num = aSettings.AxisDist(axis, this.point.point, aPt);
					if (num * num <= aClosestDist)
					{
						treeNode2.GetNearest(aSettings, aDepth + 1, aPt, ref aClosest, ref aClosestDist);
					}
				}
			}

			public void Draw(RecolorTree.TreeSettings aSettings, int aDepth, Vector3 aPt)
			{
				int axis = aSettings.GetAxis(aDepth);
				if (axis == 0)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(this.point.point + Vector3.left, this.point.point + Vector3.right);
				}
				else if (axis == 1)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(this.point.point + Vector3.up, this.point.point + Vector3.down);
				}
				else if (axis == 2)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(this.point.point + Vector3.forward, this.point.point + Vector3.back);
				}
				if (this.left != null)
				{
					this.left.Draw(aSettings, aDepth + 1, this.point.point);
					Gizmos.color = this.point.data;
					Gizmos.DrawLine(this.point.point, this.left.point.point);
				}
				if (this.right != null)
				{
					this.right.Draw(aSettings, aDepth + 1, this.point.point);
					Gizmos.color = this.point.data;
					Gizmos.DrawLine(this.point.point, this.right.point.point);
				}
			}

			private RecolorTree.TreePoint point;

			private RecolorTree.TreeNode left;

			private RecolorTree.TreeNode right;
		}
	}
}

using System;
using System.Collections.Generic;

namespace Polybrush
{
	public struct z_Edge : IEquatable<z_Edge>
	{
		public z_Edge(int _x, int _y)
		{
			this.x = _x;
			this.y = _y;
		}

		public bool Equals(z_Edge p)
		{
			return (p.x == this.x && p.y == this.y) || (p.x == this.y && p.y == this.x);
		}

		public override bool Equals(object b)
		{
			return b is z_Edge && this.Equals((z_Edge)b);
		}

		public static bool operator ==(z_Edge a, z_Edge b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(z_Edge a, z_Edge b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			int num = 17;
			int hashCode = ((this.x < this.y) ? this.x : this.y).GetHashCode();
			int hashCode2 = ((this.x < this.y) ? this.y : this.x).GetHashCode();
			return (num * 29 + hashCode.GetHashCode()) * 29 + hashCode2.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{{{{0},{1}}}}}", this.x, this.y);
		}

		public static List<int> ToList(IEnumerable<z_Edge> edges)
		{
			List<int> list = new List<int>();
			foreach (z_Edge z_Edge in edges)
			{
				list.Add(z_Edge.x);
				list.Add(z_Edge.y);
			}
			return list;
		}

		public static HashSet<int> ToHashSet(IEnumerable<z_Edge> edges)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (z_Edge z_Edge in edges)
			{
				hashSet.Add(z_Edge.x);
				hashSet.Add(z_Edge.y);
			}
			return hashSet;
		}

		public int x;

		public int y;
	}
}

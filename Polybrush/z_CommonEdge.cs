using System;
using System.Collections.Generic;

namespace Polybrush
{
	public struct z_CommonEdge : IEquatable<z_CommonEdge>
	{
		public int x
		{
			get
			{
				return this.edge.x;
			}
		}

		public int y
		{
			get
			{
				return this.edge.y;
			}
		}

		public int cx
		{
			get
			{
				return this.common.x;
			}
		}

		public int cy
		{
			get
			{
				return this.common.y;
			}
		}

		public z_CommonEdge(int _x, int _y, int _cx, int _cy)
		{
			this.edge = new z_Edge(_x, _y);
			this.common = new z_Edge(_cx, _cy);
		}

		public bool Equals(z_CommonEdge b)
		{
			return this.common.Equals(b.common);
		}

		public override bool Equals(object b)
		{
			return b is z_CommonEdge && this.common.Equals(((z_CommonEdge)b).common);
		}

		public static bool operator ==(z_CommonEdge a, z_CommonEdge b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(z_CommonEdge a, z_CommonEdge b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this.common.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{ {{{0}:{1}}}, {{{2}:{3}}} }}", new object[]
			{
				this.edge.x,
				this.common.x,
				this.edge.y,
				this.common.y
			});
		}

		public static List<int> ToList(IEnumerable<z_CommonEdge> edges)
		{
			List<int> list = new List<int>();
			foreach (z_CommonEdge z_CommonEdge in edges)
			{
				list.Add(z_CommonEdge.edge.x);
				list.Add(z_CommonEdge.edge.y);
			}
			return list;
		}

		public static HashSet<int> ToHashSet(IEnumerable<z_CommonEdge> edges)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (z_CommonEdge z_CommonEdge in edges)
			{
				hashSet.Add(z_CommonEdge.edge.x);
				hashSet.Add(z_CommonEdge.edge.y);
			}
			return hashSet;
		}

		public z_Edge edge;

		public z_Edge common;
	}
}

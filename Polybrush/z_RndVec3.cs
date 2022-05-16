using System;
using UnityEngine;

namespace Polybrush
{
	public struct z_RndVec3 : IEquatable<z_RndVec3>
	{
		public z_RndVec3(Vector3 vector)
		{
			this.x = vector.x;
			this.y = vector.y;
			this.z = vector.z;
		}

		public bool Equals(z_RndVec3 p)
		{
			return Mathf.Abs(this.x - p.x) < 0.0001f && Mathf.Abs(this.y - p.y) < 0.0001f && Mathf.Abs(this.z - p.z) < 0.0001f;
		}

		public bool Equals(Vector3 p)
		{
			return Mathf.Abs(this.x - p.x) < 0.0001f && Mathf.Abs(this.y - p.y) < 0.0001f && Mathf.Abs(this.z - p.z) < 0.0001f;
		}

		public override bool Equals(object b)
		{
			return (b is z_RndVec3 && this.Equals((z_RndVec3)b)) || (b is Vector3 && this.Equals((Vector3)b));
		}

		public override int GetHashCode()
		{
			return ((27 * 29 + this.round(this.x)) * 29 + this.round(this.y)) * 29 + this.round(this.z);
		}

		public override string ToString()
		{
			return string.Format("{{{0:F2}, {1:F2}, {2:F2}}}", this.x, this.y, this.z);
		}

		private int round(float v)
		{
			return (int)(v / 0.0001f);
		}

		public static implicit operator Vector3(z_RndVec3 p)
		{
			return new Vector3(p.x, p.y, p.z);
		}

		public static implicit operator z_RndVec3(Vector3 p)
		{
			return new z_RndVec3(p);
		}

		public float x;

		public float y;

		public float z;

		private const float resolution = 0.0001f;
	}
}

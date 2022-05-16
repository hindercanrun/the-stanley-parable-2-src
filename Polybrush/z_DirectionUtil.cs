using System;
using UnityEngine;

namespace Polybrush
{
	public static class z_DirectionUtil
	{
		public static Vector3 ToVector3(this z_Direction dir)
		{
			switch (dir)
			{
			case z_Direction.Up:
				return Vector3.up;
			case z_Direction.Right:
				return Vector3.right;
			case z_Direction.Forward:
				return Vector3.forward;
			default:
				return Vector3.zero;
			}
		}
	}
}

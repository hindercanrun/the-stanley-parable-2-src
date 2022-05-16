using System;
using UnityEngine;

namespace Polybrush
{
	public static class z_BrushMirrorUtility
	{
		public static Vector3 ToVector3(this z_BrushMirror mirror)
		{
			Vector3 one = Vector3.one;
			if ((mirror & z_BrushMirror.X) > z_BrushMirror.None)
			{
				one.x = -1f;
			}
			if ((mirror & z_BrushMirror.Y) > z_BrushMirror.None)
			{
				one.y = -1f;
			}
			if ((mirror & z_BrushMirror.Z) > z_BrushMirror.None)
			{
				one.z = -1f;
			}
			return one;
		}
	}
}

using System;
using UnityEngine;

namespace Polybrush
{
	public static class z_ComponentIndexUtility
	{
		public static float GetValueAtIndex(this Vector3 value, z_ComponentIndex index)
		{
			switch (index)
			{
			case z_ComponentIndex.R:
				return value.x;
			case z_ComponentIndex.G:
				return value.y;
			case z_ComponentIndex.B:
				return value.z;
			default:
				return 0f;
			}
		}

		public static float GetValueAtIndex(this Vector4 value, z_ComponentIndex index)
		{
			switch (index)
			{
			case z_ComponentIndex.R:
				return value.x;
			case z_ComponentIndex.G:
				return value.y;
			case z_ComponentIndex.B:
				return value.z;
			case z_ComponentIndex.A:
				return value.w;
			default:
				return 0f;
			}
		}

		public static float GetValueAtIndex(this Color value, z_ComponentIndex index)
		{
			switch (index)
			{
			case z_ComponentIndex.R:
				return value.r;
			case z_ComponentIndex.G:
				return value.g;
			case z_ComponentIndex.B:
				return value.b;
			case z_ComponentIndex.A:
				return value.a;
			default:
				return 0f;
			}
		}

		public static uint ToFlag(this z_ComponentIndex e)
		{
			int num = (int)(e + 1);
			if (num < 3)
			{
				return (uint)num;
			}
			if (num != 3)
			{
				return 8U;
			}
			return 4U;
		}

		public static string GetString(this z_ComponentIndex component, z_ComponentIndexType type = z_ComponentIndexType.Vector)
		{
			int num = (int)component;
			if (type == z_ComponentIndexType.Vector)
			{
				if (num == 0)
				{
					return "X";
				}
				if (num == 1)
				{
					return "Y";
				}
				if (num != 2)
				{
					return "W";
				}
				return "Z";
			}
			else
			{
				if (type != z_ComponentIndexType.Color)
				{
					return num.ToString();
				}
				if (num == 0)
				{
					return "R";
				}
				if (num == 1)
				{
					return "G";
				}
				if (num != 2)
				{
					return "A";
				}
				return "B";
			}
		}

		public static readonly GUIContent[] ComponentIndexPopupDescriptions = new GUIContent[]
		{
			new GUIContent("X (R)"),
			new GUIContent("Y (G)"),
			new GUIContent("Z (B)"),
			new GUIContent("W (A)")
		};

		public static readonly int[] ComponentIndexPopupValues = new int[]
		{
			0,
			1,
			2,
			3
		};
	}
}

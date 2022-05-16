using System;
using System.Reflection;
using UnityEngine;

namespace AmplifyImpostors
{
	public static class SpriteUtilityEx
	{
		public static Type Type
		{
			get
			{
				if (!(SpriteUtilityEx.type == null))
				{
					return SpriteUtilityEx.type;
				}
				return SpriteUtilityEx.type = Type.GetType("UnityEditor.Sprites.SpriteUtility, UnityEditor");
			}
		}

		public static void GenerateOutline(Texture2D texture, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
		{
			Vector2[][] array = new Vector2[0][];
			object[] array2 = new object[]
			{
				texture,
				rect,
				detail,
				alphaTolerance,
				holeDetection,
				array
			};
			SpriteUtilityEx.Type.GetMethod("GenerateOutline", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, array2);
			paths = (Vector2[][])array2[5];
		}

		private static Type type;
	}
}

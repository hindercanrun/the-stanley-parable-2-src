using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Polybrush
{
	public static class z_GradientSerializer
	{
		public static string Serialize(this Gradient gradient)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (GradientColorKey gradientColorKey in gradient.colorKeys)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				Color color = gradientColorKey.color;
				stringBuilder2.Append(color.ToString("F3"));
				stringBuilder.Append("&");
				StringBuilder stringBuilder3 = stringBuilder;
				float num = gradientColorKey.time;
				stringBuilder3.Append(num.ToString("F3"));
				stringBuilder.Append("|");
			}
			stringBuilder.Append("\n");
			foreach (GradientAlphaKey gradientAlphaKey in gradient.alphaKeys)
			{
				StringBuilder stringBuilder4 = stringBuilder;
				float num = gradientAlphaKey.alpha;
				stringBuilder4.Append(num.ToString("F4"));
				stringBuilder.Append("&");
				StringBuilder stringBuilder5 = stringBuilder;
				num = gradientAlphaKey.time;
				stringBuilder5.Append(num.ToString("F3"));
				stringBuilder.Append("|");
			}
			return stringBuilder.ToString();
		}

		public static bool Deserialize(string str, out Gradient gradient)
		{
			gradient = null;
			string[] array = str.Split(new char[]
			{
				'\n'
			});
			if (array.Length < 2)
			{
				return false;
			}
			string[] array2 = array[0].Split(new char[]
			{
				'|'
			});
			string[] array3 = array[1].Split(new char[]
			{
				'|'
			});
			if (array2.Length < 2 || array3.Length < 2)
			{
				return false;
			}
			List<GradientColorKey> list = new List<GradientColorKey>();
			List<GradientAlphaKey> list2 = new List<GradientAlphaKey>();
			string[] array4 = array2;
			for (int i = 0; i < array4.Length; i++)
			{
				string[] array5 = array4[i].Split(new char[]
				{
					'&'
				});
				Color col;
				float time;
				if (array5.Length >= 2 && z_GradientSerializer.TryParseColor(array5[0], out col) && float.TryParse(array5[1], out time))
				{
					list.Add(new GradientColorKey(col, time));
				}
			}
			array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				string[] array6 = array4[i].Split(new char[]
				{
					'&'
				});
				float alpha;
				float time2;
				if (array6.Length >= 2 && float.TryParse(array6[0], out alpha) && float.TryParse(array6[1], out time2))
				{
					list2.Add(new GradientAlphaKey(alpha, time2));
				}
			}
			gradient = new Gradient();
			gradient.SetKeys(list.ToArray(), list2.ToArray());
			return true;
		}

		private static bool TryParseColor(string str, out Color value)
		{
			string[] array = str.Replace("RGBA(", "").Replace(")", "").Split(new char[]
			{
				','
			});
			value = Color.white;
			if (array.Length != 4)
			{
				return false;
			}
			float num = 1f;
			if (!float.TryParse(array[0], out value.r))
			{
				return false;
			}
			if (!float.TryParse(array[1], out value.g))
			{
				return false;
			}
			if (!float.TryParse(array[2], out value.b))
			{
				return false;
			}
			if (!float.TryParse(array[3], out num))
			{
				return false;
			}
			value.a = num / 255f;
			return true;
		}
	}
}

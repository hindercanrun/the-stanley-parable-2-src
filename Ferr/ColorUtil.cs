using System;
using UnityEngine;

namespace Ferr
{
	public static class ColorUtil
	{
		public static Color HSL(float aHue, float aSaturation, float aLuminance)
		{
			float num = aHue % 1f * 360f / 60f;
			float num2 = (1f - Mathf.Abs(2f * aLuminance - 1f)) * aSaturation;
			float num3 = num2 * (1f - Mathf.Abs(num % 2f - 1f));
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			if (num < 1f)
			{
				num4 = num2;
				num5 = num3;
			}
			else if (num < 2f)
			{
				num4 = num3;
				num5 = num2;
			}
			else if (num < 3f)
			{
				num5 = num2;
				num6 = num3;
			}
			else if (num < 4f)
			{
				num5 = num3;
				num6 = num2;
			}
			else if (num < 5f)
			{
				num4 = num3;
				num6 = num2;
			}
			else
			{
				num4 = num2;
				num6 = num3;
			}
			float num7 = aLuminance - 0.5f * num2;
			return new Color(num4 + num7, num5 + num7, num6 + num7, 1f);
		}

		public static Vector3 ToHSV(float aR, float aG, float aB)
		{
			float num = Mathf.Max(aR, Mathf.Max(aG, aB));
			float num2 = Mathf.Min(aR, Mathf.Min(aG, aB));
			float x = Mathf.Atan2(2f * aR - aG - aB, ColorUtil.sqrt3 * (aG - aB));
			float y = (num == 0f) ? 0f : (1f - 1f * num2 / num);
			float z = num;
			return new Vector3(x, y, z);
		}

		public static Color HSV(float aHue, float aSaturation, float aValue)
		{
			float num = aHue % 1f * 360f / 60f;
			float num2 = aValue * aSaturation;
			float num3 = num2 * (1f - Mathf.Abs(num % 2f - 1f));
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			if (num < 1f)
			{
				num4 = num2;
				num5 = num3;
			}
			else if (num < 2f)
			{
				num4 = num3;
				num5 = num2;
			}
			else if (num < 3f)
			{
				num5 = num2;
				num6 = num3;
			}
			else if (num < 4f)
			{
				num5 = num3;
				num6 = num2;
			}
			else if (num < 5f)
			{
				num4 = num3;
				num6 = num2;
			}
			else
			{
				num4 = num2;
				num6 = num3;
			}
			float num7 = aValue - num2;
			return new Color(num4 + num7, num5 + num7, num6 + num7);
		}

		public static Color HCL(float aHue, float aChroma, float aLuma)
		{
			float num = aHue % 1f * 360f / 60f;
			float num2 = aChroma * (1f - Mathf.Abs(num % 2f - 1f));
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			if (num < 1f)
			{
				num3 = aChroma;
				num4 = num2;
			}
			else if (num < 2f)
			{
				num3 = num2;
				num4 = aChroma;
			}
			else if (num < 3f)
			{
				num4 = aChroma;
				num5 = num2;
			}
			else if (num < 4f)
			{
				num4 = num2;
				num5 = aChroma;
			}
			else if (num < 5f)
			{
				num3 = num2;
				num5 = aChroma;
			}
			else
			{
				num3 = aChroma;
				num5 = num2;
			}
			float num6 = aLuma - (0.3f * num3 + 0.59f * num4 + 0.11f * num5);
			return new Color(num3 + num6, num4 + num6, num5 + num6);
		}

		public static Color GetColorBand(Color[] aColorBand, float aValue)
		{
			aValue %= 1f;
			Color result = Color.white;
			if (aColorBand != null)
			{
				int num = (int)(aValue * (float)aColorBand.Length);
				int num2 = (int)Mathf.Min(aValue * (float)aColorBand.Length + 1f, (float)(aColorBand.Length - 1));
				num = Mathf.Max(0, num);
				num = Mathf.Min(aColorBand.Length - 1, num);
				Color a = aColorBand[num];
				Color b = aColorBand[num2];
				float num3 = aValue - (float)num * (1f / (float)aColorBand.Length);
				result = Color.Lerp(a, b, num3 / (1f / (float)aColorBand.Length));
			}
			return result;
		}

		public static Color FromHex(string aHex)
		{
			if (aHex.Length != 8)
			{
				return Color.red;
			}
			return new Color((float)Convert.ToInt32(aHex[0].ToString() + aHex[1].ToString()) / 255f, (float)Convert.ToInt32(aHex[2].ToString() + aHex[3].ToString()) / 255f, (float)Convert.ToInt32(aHex[4].ToString() + aHex[5].ToString()) / 255f, (float)Convert.ToInt32(aHex[6].ToString() + aHex[7].ToString()) / 255f);
		}

		public static string ToHex(Color aColor)
		{
			return string.Format("{0:X}{1:X}{2:X}{3:X}", new object[]
			{
				(int)(aColor.r * 255f),
				(int)(aColor.g * 255f),
				(int)(aColor.b * 255f),
				(int)(aColor.a * 255f)
			});
		}

		private static float sqrt3 = (float)Math.Sqrt(3.0);
	}
}

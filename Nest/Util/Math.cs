using System;
using UnityEngine;

namespace Nest.Util
{
	internal static class Math
	{
		public static float Lerp(float a, float b, float mix)
		{
			return a * (1f - mix) + b * mix;
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, float mix)
		{
			return a * (1f - mix) + b * mix;
		}
	}
}

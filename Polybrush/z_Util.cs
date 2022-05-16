using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Polybrush
{
	public static class z_Util
	{
		public static T[] Fill<T>(T value, int count)
		{
			T[] array = new !!0[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = value;
			}
			return array;
		}

		public static T[] Fill<T>(Func<int, T> constructor, int count)
		{
			T[] array = new !!0[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = constructor(i);
			}
			return array;
		}

		public static T[] Duplicate<T>(T[] array)
		{
			if (array == null)
			{
				return null;
			}
			T[] array2 = new !!0[array.Length];
			Array.Copy(array, 0, array2, 0, array.Length);
			return array2;
		}

		public static Dictionary<K, V> InitDictionary<K, V>(Func<int, K> keyFunc, Func<int, V> valueFunc, int count)
		{
			Dictionary<K, V> dictionary = new Dictionary<!!0, !!1>(count);
			for (int i = 0; i < count; i++)
			{
				dictionary.Add(keyFunc(i), valueFunc(i));
			}
			return dictionary;
		}

		public static string ToString<T>(this IEnumerable<T> enumerable, string delim)
		{
			if (enumerable == null)
			{
				return "";
			}
			return string.Join(delim ?? "", enumerable.Select(delegate(T x)
			{
				if (x == null)
				{
					return "";
				}
				return x.ToString();
			}).ToArray<string>());
		}

		public static string ToString<K, V>(this Dictionary<K, V> dictionary, string delim)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<K, V> keyValuePair in dictionary)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				K key = keyValuePair.Key;
				string str = key.ToString();
				string str2 = ": ";
				V value = keyValuePair.Value;
				stringBuilder2.AppendLine(str + str2 + value.ToString());
			}
			return stringBuilder.ToString();
		}

		public static void Resize<T>(ref T[] array, int newSize)
		{
			T[] array2 = new !!0[newSize];
			Array.Copy(array, array2, Math.Min(array.Length, newSize));
			array = array2;
		}

		public static Dictionary<K, T> SetValuesAsKey<T, K>(this Dictionary<T, IEnumerable<K>> dic)
		{
			Dictionary<K, T> dictionary = new Dictionary<!!1, !!0>();
			foreach (KeyValuePair<T, IEnumerable<K>> keyValuePair in dic)
			{
				foreach (K key in keyValuePair.Value)
				{
					dictionary.Add(key, keyValuePair.Key);
				}
			}
			return dictionary;
		}

		public static Dictionary<T, int> GetCommonLookup<T>(this List<List<T>> lists)
		{
			Dictionary<T, int> dictionary = new Dictionary<!!0, int>();
			int num = 0;
			foreach (List<!!0> list in lists)
			{
				foreach (T key in list)
				{
					dictionary.Add(key, num);
				}
				num++;
			}
			return dictionary;
		}

		public static Color32 Lerp(Color32 lhs, Color32 rhs, z_ColorMask mask, float alpha)
		{
			return new Color32(mask.r ? ((byte)((float)lhs.r * (1f - alpha) + (float)rhs.r * alpha)) : lhs.r, mask.g ? ((byte)((float)lhs.g * (1f - alpha) + (float)rhs.g * alpha)) : lhs.g, mask.b ? ((byte)((float)lhs.b * (1f - alpha) + (float)rhs.b * alpha)) : lhs.b, mask.a ? ((byte)((float)lhs.a * (1f - alpha) + (float)rhs.a * alpha)) : lhs.a);
		}

		public static Color32 Lerp(Color32 lhs, Color32 rhs, float alpha)
		{
			return new Color32((byte)((float)lhs.r * (1f - alpha) + (float)rhs.r * alpha), (byte)((float)lhs.g * (1f - alpha) + (float)rhs.g * alpha), (byte)((float)lhs.b * (1f - alpha) + (float)rhs.b * alpha), (byte)((float)lhs.a * (1f - alpha) + (float)rhs.a * alpha));
		}

		public static AnimationCurve ClampAnimationKeys(AnimationCurve curve, float firstKeyTime, float firstKeyValue, float secondKeyTime, float secondKeyValue)
		{
			Keyframe[] keys = curve.keys;
			int num = curve.length - 1;
			keys[0].time = firstKeyTime;
			keys[0].value = firstKeyValue;
			keys[num].time = secondKeyTime;
			keys[num].value = secondKeyValue;
			curve.keys = keys;
			return new AnimationCurve(keys);
		}

		public static Enum Next(this Enum value)
		{
			int num = Enum.GetNames(value.GetType()).Length;
			return (Enum)Enum.ToObject(value.GetType(), (Convert.ToInt32(value) + 1) % num);
		}

		public static bool IsValid<T>(this T target) where T : z_IValid
		{
			return target != null && target.IsValid;
		}

		internal static string IncrementPrefix(string prefix, string name)
		{
			Match match = new Regex("^(" + prefix + "[0-9]*_)").Match(name);
			string result;
			if (match.Success)
			{
				string s = match.Value.Replace(prefix, "").Replace("_", "");
				int num = 0;
				if (int.TryParse(s, out num))
				{
					result = name.Replace(match.Value, prefix + (num + 1) + "_");
				}
				else
				{
					result = prefix + "0_" + name;
				}
			}
			else
			{
				result = prefix + "0_" + name;
			}
			return result;
		}

		public static Mesh GetMesh(this GameObject gameObject)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh != null)
			{
				return component.sharedMesh;
			}
			SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null && component2.sharedMesh != null)
			{
				return component2.sharedMesh;
			}
			return null;
		}

		public static List<Material> GetMaterials(this GameObject gameObject)
		{
			List<Material> list = new List<Material>();
			foreach (Renderer renderer in gameObject.GetComponents<Renderer>())
			{
				list.AddRange(renderer.sharedMaterials);
			}
			return list;
		}

		public static Dictionary<T, List<K>> ToDictionary<T, K>(this IEnumerable<IGrouping<T, K>> groups)
		{
			Dictionary<T, List<K>> dictionary = new Dictionary<!!0, List<!!1>>();
			foreach (IGrouping<T, K> grouping in groups)
			{
				dictionary.Add(grouping.Key, grouping.ToList<K>());
			}
			return dictionary;
		}
	}
}

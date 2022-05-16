using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Polybrush
{
	public class z_SplatSet
	{
		public int attributeCount
		{
			get
			{
				return this.attributeLayout.Length;
			}
		}

		public z_SplatSet(int vertexCount, z_AttributeLayout[] attributes, bool preAlloc = true)
		{
			this.channelMap = z_SplatWeight.GetChannelMap(attributes);
			int count = this.channelMap.Count;
			this.attributeLayout = attributes;
			this.weights = new Vector4[count][];
			this.weightCount = vertexCount;
			if (preAlloc)
			{
				for (int i = 0; i < count; i++)
				{
					this.weights[i] = new Vector4[vertexCount];
				}
			}
		}

		public z_SplatSet(z_SplatSet other)
		{
			int attributeCount = other.attributeCount;
			this.attributeLayout = new z_AttributeLayout[attributeCount];
			Array.Copy(other.attributeLayout, 0, this.attributeLayout, 0, attributeCount);
			this.channelMap = new Dictionary<z_MeshChannel, int>();
			foreach (KeyValuePair<z_MeshChannel, int> keyValuePair in other.channelMap)
			{
				this.channelMap.Add(keyValuePair.Key, keyValuePair.Value);
			}
			int count = other.channelMap.Count;
			this.weightCount = other.weightCount;
			this.weights = new Vector4[count][];
			for (int i = 0; i < count; i++)
			{
				this.weights[i] = new Vector4[this.weightCount];
				Array.Copy(other.weights[i], this.weights[i], this.weightCount);
			}
		}

		private static Vector4 Color32ToVec4(Color32 color)
		{
			return new Vector4((float)color.r / 255f, (float)color.g / 255f, (float)color.b / 255f, (float)color.a / 255f);
		}

		private static Color32 Vec4ToColor32(Vector4 vec)
		{
			return new Color32((byte)(255f * vec.x), (byte)(255f * vec.y), (byte)(255f * vec.z), (byte)(255f * vec.w));
		}

		public z_SplatSet(z_Mesh mesh, z_AttributeLayout[] attributes) : this(mesh.vertexCount, attributes, false)
		{
			foreach (KeyValuePair<z_MeshChannel, int> keyValuePair in this.channelMap)
			{
				z_MeshChannel key = keyValuePair.Key;
				if (key <= z_MeshChannel.UV0)
				{
					if (key == z_MeshChannel.Color)
					{
						Color32[] colors = mesh.colors;
						Vector4[][] array = this.weights;
						int value = keyValuePair.Value;
						Vector4[] array2;
						if (colors == null || colors.Length != this.weightCount)
						{
							array2 = new Vector4[this.weightCount];
						}
						else
						{
							array2 = Array.ConvertAll<Color32, Vector4>(colors, (Color32 x) => z_SplatSet.Color32ToVec4(x));
						}
						array[value] = array2;
						continue;
					}
					if (key == z_MeshChannel.Tangent)
					{
						Vector4[] tangents = mesh.tangents;
						this.weights[keyValuePair.Value] = ((tangents != null && tangents.Length == this.weightCount) ? tangents : new Vector4[this.weightCount]);
						continue;
					}
					if (key != z_MeshChannel.UV0)
					{
						continue;
					}
				}
				else if (key != z_MeshChannel.UV2 && key != z_MeshChannel.UV3 && key != z_MeshChannel.UV4)
				{
					continue;
				}
				List<Vector4> uvs = mesh.GetUVs(z_MeshChannelUtility.UVChannelToIndex(keyValuePair.Key));
				this.weights[keyValuePair.Value] = ((uvs.Count == this.weightCount) ? uvs.ToArray() : new Vector4[this.weightCount]);
			}
		}

		public z_SplatWeight GetMinWeights()
		{
			z_SplatWeight z_SplatWeight = new z_SplatWeight(this.channelMap);
			foreach (z_AttributeLayout z_AttributeLayout in this.attributeLayout)
			{
				Vector4 value = z_SplatWeight[z_AttributeLayout.channel];
				value[(int)z_AttributeLayout.index] = z_AttributeLayout.min;
				z_SplatWeight[z_AttributeLayout.channel] = value;
			}
			return z_SplatWeight;
		}

		public z_SplatWeight GetMaxWeights()
		{
			z_SplatWeight z_SplatWeight = new z_SplatWeight(this.channelMap);
			foreach (z_AttributeLayout z_AttributeLayout in this.attributeLayout)
			{
				Vector4 value = z_SplatWeight[z_AttributeLayout.channel];
				value[(int)z_AttributeLayout.index] = z_AttributeLayout.max;
				z_SplatWeight[z_AttributeLayout.channel] = value;
			}
			return z_SplatWeight;
		}

		public void LerpWeights(z_SplatSet lhs, z_SplatSet rhs, int mask, float[] alpha)
		{
			Dictionary<int, uint> dictionary = new Dictionary<int, uint>();
			foreach (z_AttributeLayout z_AttributeLayout in this.attributeLayout)
			{
				int num = this.channelMap[z_AttributeLayout.channel];
				if (z_AttributeLayout.mask == mask)
				{
					if (!dictionary.ContainsKey(num))
					{
						dictionary.Add(num, z_AttributeLayout.index.ToFlag());
					}
					else
					{
						Dictionary<int, uint> dictionary2 = dictionary;
						int key = num;
						dictionary2[key] |= z_AttributeLayout.index.ToFlag();
					}
				}
			}
			foreach (KeyValuePair<int, uint> keyValuePair in dictionary)
			{
				Vector4[] array2 = lhs.weights[keyValuePair.Key];
				Vector4[] array3 = rhs.weights[keyValuePair.Key];
				Vector4[] array4 = this.weights[keyValuePair.Key];
				for (int j = 0; j < this.weightCount; j++)
				{
					if ((keyValuePair.Value & 1U) != 0U)
					{
						array4[j].x = Mathf.Lerp(array2[j].x, array3[j].x, alpha[j]);
					}
					if ((keyValuePair.Value & 2U) != 0U)
					{
						array4[j].y = Mathf.Lerp(array2[j].y, array3[j].y, alpha[j]);
					}
					if ((keyValuePair.Value & 4U) != 0U)
					{
						array4[j].z = Mathf.Lerp(array2[j].z, array3[j].z, alpha[j]);
					}
					if ((keyValuePair.Value & 8U) != 0U)
					{
						array4[j].w = Mathf.Lerp(array2[j].w, array3[j].w, alpha[j]);
					}
				}
			}
		}

		public void LerpWeights(z_SplatSet lhs, z_SplatWeight rhs, float alpha)
		{
			for (int i = 0; i < this.weightCount; i++)
			{
				foreach (KeyValuePair<z_MeshChannel, int> keyValuePair in this.channelMap)
				{
					this.weights[keyValuePair.Value][i] = Vector4.LerpUnclamped(lhs.weights[keyValuePair.Value][i], rhs[keyValuePair.Key], alpha);
				}
			}
		}

		public void CopyTo(z_SplatSet other)
		{
			if (other.weightCount != this.weightCount)
			{
				Debug.LogError("Copying splat set to mis-matched container length");
				return;
			}
			for (int i = 0; i < this.channelMap.Count; i++)
			{
				Array.Copy(this.weights[i], other.weights[i], this.weightCount);
			}
		}

		public void Apply(z_Mesh mesh)
		{
			z_AttributeLayout[] array = this.attributeLayout;
			int i = 0;
			while (i < array.Length)
			{
				z_AttributeLayout z_AttributeLayout = array[i];
				z_MeshChannel channel = z_AttributeLayout.channel;
				if (channel <= z_MeshChannel.UV0)
				{
					if (channel != z_MeshChannel.Color)
					{
						if (channel != z_MeshChannel.Tangent)
						{
							if (channel == z_MeshChannel.UV0)
							{
								goto IL_43;
							}
						}
						else
						{
							mesh.tangents = this.weights[this.channelMap[z_MeshChannel.Tangent]];
						}
					}
					else
					{
						mesh.colors = Array.ConvertAll<Vector4, Color32>(this.weights[this.channelMap[z_AttributeLayout.channel]], (Vector4 x) => z_SplatSet.Vec4ToColor32(x));
					}
				}
				else if (channel == z_MeshChannel.UV2 || channel == z_MeshChannel.UV3 || channel == z_MeshChannel.UV4)
				{
					goto IL_43;
				}
				IL_D4:
				i++;
				continue;
				IL_43:
				List<Vector4> uvs = new List<Vector4>(this.weights[this.channelMap[z_AttributeLayout.channel]]);
				mesh.SetUVs(z_MeshChannelUtility.UVChannelToIndex(z_AttributeLayout.channel), uvs);
				goto IL_D4;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (z_AttributeLayout z_AttributeLayout in this.attributeLayout)
			{
				stringBuilder.AppendLine(z_AttributeLayout.ToString());
			}
			stringBuilder.AppendLine("--");
			for (int j = 0; j < this.weightCount; j++)
			{
				stringBuilder.AppendLine(this.weights[j].ToString());
			}
			return stringBuilder.ToString();
		}

		private const float WEIGHT_EPSILON = 0.0001f;

		private int weightCount;

		private Dictionary<z_MeshChannel, int> channelMap;

		private Vector4[][] weights;

		public z_AttributeLayout[] attributeLayout;
	}
}

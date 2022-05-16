using System;
using UnityEngine;

namespace Polybrush
{
	[Serializable]
	public class z_AttributeLayout : IEquatable<z_AttributeLayout>
	{
		public float min
		{
			get
			{
				return this.range.x;
			}
			set
			{
				this.range.x = value;
			}
		}

		public float max
		{
			get
			{
				return this.range.y;
			}
			set
			{
				this.range.y = value;
			}
		}

		public z_AttributeLayout(z_MeshChannel channel, z_ComponentIndex index) : this(channel, index, Vector2.up, 0)
		{
		}

		public z_AttributeLayout(z_MeshChannel channel, z_ComponentIndex index, Vector2 range, int mask)
		{
			this.channel = channel;
			this.index = index;
			this.range = range;
			this.mask = mask;
		}

		public z_AttributeLayout(z_MeshChannel channel, z_ComponentIndex index, Vector2 range, int mask, string targetProperty, Texture2D texture = null) : this(channel, index, range, mask)
		{
			this.propertyTarget = targetProperty;
			this.previewTexture = texture;
		}

		public bool Equals(z_AttributeLayout other)
		{
			return this.channel == other.channel && this.propertyTarget.Equals(other.propertyTarget) && this.index == other.index && this.range == other.range && this.mask == other.mask;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}.{2} ({3:f2}, {4:f2})  {5}", new object[]
			{
				this.propertyTarget,
				this.channel.ToString(),
				this.index.GetString(z_ComponentIndexType.Vector),
				this.min,
				this.max,
				this.mask
			});
		}

		public const int NoMask = -1;

		public const int DefaultMask = 0;

		public static readonly int[] DefaultMaskValues = new int[]
		{
			-1,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31
		};

		public static readonly GUIContent[] DefaultMaskDescriptions = new GUIContent[]
		{
			new GUIContent("No Mask"),
			new GUIContent("0"),
			new GUIContent("1"),
			new GUIContent("2"),
			new GUIContent("3"),
			new GUIContent("4"),
			new GUIContent("5"),
			new GUIContent("6"),
			new GUIContent("7"),
			new GUIContent("8"),
			new GUIContent("9"),
			new GUIContent("10"),
			new GUIContent("11"),
			new GUIContent("12"),
			new GUIContent("13"),
			new GUIContent("14"),
			new GUIContent("15"),
			new GUIContent("16"),
			new GUIContent("17"),
			new GUIContent("18"),
			new GUIContent("19"),
			new GUIContent("20"),
			new GUIContent("21"),
			new GUIContent("22"),
			new GUIContent("23"),
			new GUIContent("24"),
			new GUIContent("25"),
			new GUIContent("26"),
			new GUIContent("27"),
			new GUIContent("28"),
			new GUIContent("29"),
			new GUIContent("30"),
			new GUIContent("31")
		};

		public static readonly Vector2 NormalizedRange = new Vector2(0f, 1f);

		public z_MeshChannel channel;

		public z_ComponentIndex index;

		public Vector2 range = new Vector2(0f, 1f);

		public string propertyTarget;

		public int mask;

		[NonSerialized]
		public Texture2D previewTexture;
	}
}

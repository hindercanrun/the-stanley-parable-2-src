using System;
using UnityEngine;

namespace Polybrush
{
	[Serializable]
	public class z_AttributeLayoutContainer : ScriptableObject, IEquatable<z_AttributeLayoutContainer>
	{
		public static z_AttributeLayoutContainer Create(Shader shader, z_AttributeLayout[] attributes)
		{
			z_AttributeLayoutContainer z_AttributeLayoutContainer = ScriptableObject.CreateInstance<z_AttributeLayoutContainer>();
			z_AttributeLayoutContainer.shader = shader;
			z_AttributeLayoutContainer.attributes = attributes;
			return z_AttributeLayoutContainer;
		}

		public bool Equals(z_AttributeLayoutContainer other)
		{
			if (this.shader != other.shader)
			{
				return false;
			}
			int num = (this.attributes == null) ? 0 : this.attributes.Length;
			int num2 = (other.attributes == null) ? 0 : other.attributes.Length;
			if (num != num2)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (!this.attributes[i].Equals(other.attributes[num2]))
				{
					return false;
				}
			}
			return true;
		}

		public Shader shader;

		public z_AttributeLayout[] attributes;
	}
}

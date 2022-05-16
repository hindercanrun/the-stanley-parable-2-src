﻿using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class TextureBlenderStandardMetallic : TextureBlender
	{
		public bool DoesShaderNameMatch(string shaderName)
		{
			return shaderName.Equals("Standard");
		}

		public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
		{
			if (shaderTexturePropertyName.Equals("_MainTex"))
			{
				this.propertyToDo = TextureBlenderStandardMetallic.Prop.doColor;
				if (sourceMat.HasProperty(shaderTexturePropertyName))
				{
					this.m_tintColor = sourceMat.GetColor("_Color");
					return;
				}
				this.m_tintColor = this.m_defaultColor;
				return;
			}
			else
			{
				if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
				{
					this.propertyToDo = TextureBlenderStandardMetallic.Prop.doMetallic;
					return;
				}
				if (!shaderTexturePropertyName.Equals("_EmissionMap"))
				{
					this.propertyToDo = TextureBlenderStandardMetallic.Prop.doNone;
					return;
				}
				this.propertyToDo = TextureBlenderStandardMetallic.Prop.doEmission;
				if (sourceMat.HasProperty(shaderTexturePropertyName))
				{
					this.m_emission = sourceMat.GetColor("_EmissionColor");
					return;
				}
				this.m_emission = this.m_defaultEmission;
				return;
			}
		}

		public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
		{
			if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doColor)
			{
				return new Color(pixelColor.r * this.m_tintColor.r, pixelColor.g * this.m_tintColor.g, pixelColor.b * this.m_tintColor.b, pixelColor.a * this.m_tintColor.a);
			}
			if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doMetallic)
			{
				return pixelColor;
			}
			if (this.propertyToDo == TextureBlenderStandardMetallic.Prop.doEmission)
			{
				return new Color(pixelColor.r * this.m_emission.r, pixelColor.g * this.m_emission.g, pixelColor.b * this.m_emission.b, pixelColor.a * this.m_emission.a);
			}
			return pixelColor;
		}

		public bool NonTexturePropertiesAreEqual(Material a, Material b)
		{
			return TextureBlenderFallback._compareColor(a, b, this.m_defaultColor, "_Color") && TextureBlenderFallback._compareFloat(a, b, this.m_defaultMetallic, "_Metallic") && TextureBlenderFallback._compareFloat(a, b, this.m_defaultGlossiness, "_Glossiness") && TextureBlenderFallback._compareColor(a, b, this.m_defaultEmission, "_EmissionColor");
		}

		public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
		{
			resultMaterial.SetColor("_Color", this.m_defaultColor);
			resultMaterial.SetFloat("_Metallic", this.m_defaultMetallic);
			resultMaterial.SetFloat("_Glossiness", this.m_defaultGlossiness);
			if (resultMaterial.GetTexture("_EmissionMap") == null)
			{
				resultMaterial.SetColor("_EmissionColor", Color.black);
				return;
			}
			resultMaterial.SetColor("_EmissionColor", Color.white);
		}

		public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
		{
			if (texPropertyName.name.Equals("_BumpMap"))
			{
				return new Color(0.5f, 0.5f, 1f);
			}
			if (texPropertyName.Equals("_MainTex"))
			{
				if (!(mat != null) || !mat.HasProperty("_Color"))
				{
					goto IL_1B3;
				}
				try
				{
					return mat.GetColor("_Color");
				}
				catch (Exception)
				{
					goto IL_1B3;
				}
			}
			if (texPropertyName.name.Equals("_MetallicGlossMap"))
			{
				if (mat != null && mat.HasProperty("_Metallic"))
				{
					try
					{
						float @float = mat.GetFloat("_Metallic");
						Color result = new Color(@float, @float, @float);
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								result.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						return result;
					}
					catch (Exception)
					{
						goto IL_1B3;
					}
				}
				return new Color(0f, 0f, 0f, 0.5f);
			}
			if (texPropertyName.name.Equals("_ParallaxMap"))
			{
				return new Color(0f, 0f, 0f, 0f);
			}
			if (texPropertyName.name.Equals("_OcclusionMap"))
			{
				return new Color(1f, 1f, 1f, 1f);
			}
			if (texPropertyName.name.Equals("_EmissionMap"))
			{
				if (mat != null)
				{
					if (mat.HasProperty("_EmissionColor"))
					{
						try
						{
							return mat.GetColor("_EmissionColor");
						}
						catch (Exception)
						{
							goto IL_1B3;
						}
					}
					return Color.black;
				}
			}
			else if (texPropertyName.name.Equals("_DetailMask"))
			{
				return new Color(0f, 0f, 0f, 0f);
			}
			IL_1B3:
			return new Color(1f, 1f, 1f, 0f);
		}

		private Color m_tintColor;

		private Color m_emission;

		private TextureBlenderStandardMetallic.Prop propertyToDo = TextureBlenderStandardMetallic.Prop.doNone;

		private Color m_defaultColor = Color.white;

		private float m_defaultMetallic;

		private float m_defaultGlossiness = 0.5f;

		private Color m_defaultEmission = Color.black;

		private enum Prop
		{
			doColor,
			doMetallic,
			doEmission,
			doNone
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB2_TextureBakeResults : ScriptableObject
{
	private void OnEnable()
	{
		if (this.version < 3230 && this.resultMaterials != null)
		{
			for (int i = 0; i < this.resultMaterials.Length; i++)
			{
				this.resultMaterials[i].considerMeshUVs = this.fixOutOfBoundsUVs;
			}
		}
		this.version = 3230;
	}

	public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos, List<Material> matsOnTargetRenderer)
	{
		HashSet<Material> hashSet = new HashSet<Material>(matsOnTargetRenderer);
		for (int i = 0; i < gos.Length; i++)
		{
			if (gos[i] == null)
			{
				Debug.LogError(string.Format("Game object {0} in list of objects to add was null", i));
				return null;
			}
			Material[] gomaterials = MB_Utility.GetGOMaterials(gos[i]);
			if (gomaterials.Length == 0)
			{
				Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", i));
				return null;
			}
			for (int j = 0; j < gomaterials.Length; j++)
			{
				if (!hashSet.Contains(gomaterials[j]))
				{
					hashSet.Add(gomaterials[j]);
				}
			}
		}
		Material[] array = new Material[hashSet.Count];
		hashSet.CopyTo(array);
		MB2_TextureBakeResults mb2_TextureBakeResults = (MB2_TextureBakeResults)ScriptableObject.CreateInstance(typeof(MB2_TextureBakeResults));
		List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k] != null)
			{
				MB_MaterialAndUVRect item = new MB_MaterialAndUVRect(array[k], new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), "");
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		Material[] array2 = mb2_TextureBakeResults.materials = new Material[list.Count];
		mb2_TextureBakeResults.resultMaterials = new MB_MultiMaterial[list.Count];
		for (int l = 0; l < list.Count; l++)
		{
			array2[l] = list[l].material;
			mb2_TextureBakeResults.resultMaterials[l] = new MB_MultiMaterial();
			List<Material> list2 = new List<Material>();
			list2.Add(list[l].material);
			mb2_TextureBakeResults.resultMaterials[l].sourceMaterials = list2;
			mb2_TextureBakeResults.resultMaterials[l].combinedMaterial = array2[l];
			mb2_TextureBakeResults.resultMaterials[l].considerMeshUVs = false;
		}
		if (array.Length == 1)
		{
			mb2_TextureBakeResults.doMultiMaterial = true;
		}
		else
		{
			mb2_TextureBakeResults.doMultiMaterial = false;
		}
		mb2_TextureBakeResults.materialsAndUVRects = list.ToArray();
		return mb2_TextureBakeResults;
	}

	public bool DoAnyResultMatsUseConsiderMeshUVs()
	{
		if (this.resultMaterials == null)
		{
			return false;
		}
		for (int i = 0; i < this.resultMaterials.Length; i++)
		{
			if (this.resultMaterials[i].considerMeshUVs)
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsMaterial(Material m)
	{
		for (int i = 0; i < this.materialsAndUVRects.Length; i++)
		{
			if (this.materialsAndUVRects[i].material == m)
			{
				return true;
			}
		}
		return false;
	}

	public string GetDescription()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Shaders:\n");
		HashSet<Shader> hashSet = new HashSet<Shader>();
		if (this.materialsAndUVRects != null)
		{
			for (int i = 0; i < this.materialsAndUVRects.Length; i++)
			{
				if (this.materialsAndUVRects[i].material != null)
				{
					hashSet.Add(this.materialsAndUVRects[i].material.shader);
				}
			}
		}
		foreach (Shader shader in hashSet)
		{
			stringBuilder.Append("  ").Append(shader.name).AppendLine();
		}
		stringBuilder.Append("Materials:\n");
		if (this.materialsAndUVRects != null)
		{
			for (int j = 0; j < this.materialsAndUVRects.Length; j++)
			{
				if (this.materialsAndUVRects[j].material != null)
				{
					stringBuilder.Append("  ").Append(this.materialsAndUVRects[j].material.name).AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}

	private const int VERSION = 3230;

	public int version;

	public MB_MaterialAndUVRect[] materialsAndUVRects;

	public MB_MultiMaterial[] resultMaterials;

	public bool doMultiMaterial;

	public Material[] materials;

	public bool fixOutOfBoundsUVs;

	public Material resultMaterial;

	public class Material2AtlasRectangleMapper
	{
		public Material2AtlasRectangleMapper(MB2_TextureBakeResults res)
		{
			this.tbr = res;
			this.matsAndSrcUVRect = res.materialsAndUVRects;
			this.numTimesMatAppearsInAtlas = new int[this.matsAndSrcUVRect.Length];
			for (int i = 0; i < this.matsAndSrcUVRect.Length; i++)
			{
				if (this.numTimesMatAppearsInAtlas[i] <= 1)
				{
					int num = 1;
					for (int j = i + 1; j < this.matsAndSrcUVRect.Length; j++)
					{
						if (this.matsAndSrcUVRect[i].material == this.matsAndSrcUVRect[j].material)
						{
							num++;
						}
					}
					this.numTimesMatAppearsInAtlas[i] = num;
					if (num > 1)
					{
						for (int k = i + 1; k < this.matsAndSrcUVRect.Length; k++)
						{
							if (this.matsAndSrcUVRect[i].material == this.matsAndSrcUVRect[k].material)
							{
								this.numTimesMatAppearsInAtlas[k] = num;
							}
						}
					}
				}
			}
		}

		public bool TryMapMaterialToUVRect(Material mat, Mesh m, int submeshIdx, int idxInResultMats, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisCache, out Rect rectInAtlas, out Rect encapsulatingRect, out Rect sourceMaterialTilingOut, ref string errorMsg, MB2_LogLevel logLevel)
		{
			if (this.tbr.materialsAndUVRects.Length == 0 && this.tbr.materials.Length != 0)
			{
				errorMsg = "The 'Texture Bake Result' needs to be re-baked to be compatible with this version of Mesh Baker. Please re-bake using the MB3_TextureBaker.";
				rectInAtlas = default(Rect);
				encapsulatingRect = default(Rect);
				sourceMaterialTilingOut = default(Rect);
				return false;
			}
			if (mat == null)
			{
				rectInAtlas = default(Rect);
				encapsulatingRect = default(Rect);
				sourceMaterialTilingOut = default(Rect);
				errorMsg = string.Format("Mesh {0} Had no material on submesh {1} cannot map to a material in the atlas", m.name, submeshIdx);
				return false;
			}
			if (submeshIdx >= m.subMeshCount)
			{
				errorMsg = "Submesh index is greater than the number of submeshes";
				rectInAtlas = default(Rect);
				encapsulatingRect = default(Rect);
				sourceMaterialTilingOut = default(Rect);
				return false;
			}
			int num = -1;
			for (int i = 0; i < this.matsAndSrcUVRect.Length; i++)
			{
				if (mat == this.matsAndSrcUVRect[i].material)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				rectInAtlas = default(Rect);
				encapsulatingRect = default(Rect);
				sourceMaterialTilingOut = default(Rect);
				errorMsg = string.Format("Material {0} could not be found in the Texture Bake Result", mat.name);
				return false;
			}
			if (!this.tbr.resultMaterials[idxInResultMats].considerMeshUVs)
			{
				if (this.numTimesMatAppearsInAtlas[num] != 1)
				{
					Debug.LogError("There is a problem with this TextureBakeResults. FixOutOfBoundsUVs is false and a material appears more than once.");
				}
				rectInAtlas = this.matsAndSrcUVRect[num].atlasRect;
				encapsulatingRect = this.matsAndSrcUVRect[num].samplingEncapsulatinRect;
				sourceMaterialTilingOut = this.matsAndSrcUVRect[num].sourceMaterialTiling;
				return true;
			}
			MB_Utility.MeshAnalysisResult[] array;
			if (!meshAnalysisCache.TryGetValue(m.GetInstanceID(), out array))
			{
				array = new MB_Utility.MeshAnalysisResult[m.subMeshCount];
				for (int j = 0; j < m.subMeshCount; j++)
				{
					MB_Utility.hasOutOfBoundsUVs(meshChannelCache.GetUv0Raw(m), m, ref array[j], j);
				}
				meshAnalysisCache.Add(m.GetInstanceID(), array);
			}
			bool flag = false;
			if (logLevel >= MB2_LogLevel.trace)
			{
				Debug.LogWarning(string.Format("Trying to find a rectangle in atlas capable of holding tiled sampling rect for mesh {0} using material {1}", m, mat));
			}
			for (int k = num; k < this.matsAndSrcUVRect.Length; k++)
			{
				if (this.matsAndSrcUVRect[k].material == mat)
				{
					Rect rect = default(Rect);
					Rect uvRect = array[submeshIdx].uvRect;
					Rect sourceMaterialTiling = this.matsAndSrcUVRect[k].sourceMaterialTiling;
					Rect samplingEncapsulatinRect = this.matsAndSrcUVRect[k].samplingEncapsulatinRect;
					MB3_UVTransformUtility.Canonicalize(ref samplingEncapsulatinRect, 0f, 0f);
					rect = MB3_UVTransformUtility.CombineTransforms(ref uvRect, ref sourceMaterialTiling);
					if (logLevel >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Concat(new string[]
						{
							"uvR=",
							uvRect.ToString("f5"),
							" matR=",
							sourceMaterialTiling.ToString("f5"),
							"Potential Rect ",
							rect.ToString("f5"),
							" encapsulating=",
							samplingEncapsulatinRect.ToString("f5")
						}));
					}
					MB3_UVTransformUtility.Canonicalize(ref rect, samplingEncapsulatinRect.x, samplingEncapsulatinRect.y);
					if (logLevel >= MB2_LogLevel.trace)
					{
						Debug.Log("Potential Rect Cannonical " + rect.ToString("f5") + " encapsulating=" + samplingEncapsulatinRect.ToString("f5"));
					}
					if (MB3_UVTransformUtility.RectContains(ref samplingEncapsulatinRect, ref rect))
					{
						if (logLevel >= MB2_LogLevel.trace)
						{
							Debug.Log(string.Concat(new object[]
							{
								"Found rect in atlas capable of containing tiled sampling rect for mesh ",
								m,
								" at idx=",
								k
							}));
						}
						num = k;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				rectInAtlas = this.matsAndSrcUVRect[num].atlasRect;
				encapsulatingRect = this.matsAndSrcUVRect[num].samplingEncapsulatinRect;
				sourceMaterialTilingOut = this.matsAndSrcUVRect[num].sourceMaterialTiling;
				return true;
			}
			rectInAtlas = default(Rect);
			encapsulatingRect = default(Rect);
			sourceMaterialTilingOut = default(Rect);
			errorMsg = string.Format("Could not find a tiled rectangle in the atlas capable of containing the uv and material tiling on mesh {0} for material {1}", m.name, mat);
			return false;
		}

		private MB2_TextureBakeResults tbr;

		private int[] numTimesMatAppearsInAtlas;

		private MB_MaterialAndUVRect[] matsAndSrcUVRect;
	}
}

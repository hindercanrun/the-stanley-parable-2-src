using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MeshCombinerSingle : MB3_MeshCombiner
	{
		public override MB2_TextureBakeResults textureBakeResults
		{
			set
			{
				if (this.mbDynamicObjectsInCombinedMesh.Count > 0 && this._textureBakeResults != value && this._textureBakeResults != null && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("If Texture Bake Result is changed then objects currently in combined mesh may be invalid.");
				}
				this._textureBakeResults = value;
			}
		}

		public override MB_RenderType renderType
		{
			set
			{
				if (value == MB_RenderType.skinnedMeshRenderer && this._renderType == MB_RenderType.meshRenderer && this.boneWeights.Length != this.verts.Length)
				{
					Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleteing the CombinedMesh scene object.");
				}
				this._renderType = value;
			}
		}

		public override GameObject resultSceneObject
		{
			set
			{
				if (this._resultSceneObject != value)
				{
					this._targetRenderer = null;
					if (this._mesh != null && this._LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Result Scene Object was changed when this mesh baker component had a reference to a mesh. If mesh is being used by another object make sure to reset the mesh to none before baking to avoid overwriting the other mesh.");
					}
				}
				this._resultSceneObject = value;
			}
		}

		private MB3_MeshCombinerSingle.MB_DynamicGameObject instance2Combined_MapGet(int gameObjectID)
		{
			return this._instance2combined_map[gameObjectID];
		}

		private void instance2Combined_MapAdd(int gameObjectID, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			this._instance2combined_map.Add(gameObjectID, dgo);
		}

		private void instance2Combined_MapRemove(int gameObjectID)
		{
			this._instance2combined_map.Remove(gameObjectID);
		}

		private bool instance2Combined_MapTryGetValue(int gameObjectID, out MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			return this._instance2combined_map.TryGetValue(gameObjectID, out dgo);
		}

		private int instance2Combined_MapCount()
		{
			return this._instance2combined_map.Count;
		}

		private void instance2Combined_MapClear()
		{
			this._instance2combined_map.Clear();
		}

		private bool instance2Combined_MapContainsKey(int gameObjectID)
		{
			return this._instance2combined_map.ContainsKey(gameObjectID);
		}

		public override int GetNumObjectsInCombined()
		{
			return this.mbDynamicObjectsInCombinedMesh.Count;
		}

		public override List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			list.AddRange(this.objectsInCombinedMesh);
			return list;
		}

		public Mesh GetMesh()
		{
			if (this._mesh == null)
			{
				this._mesh = new Mesh();
			}
			return this._mesh;
		}

		public Transform[] GetBones()
		{
			return this.bones;
		}

		public override int GetLightmapIndex()
		{
			if (this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				return this.lightmapIndex;
			}
			return -1;
		}

		public override int GetNumVerticesFor(GameObject go)
		{
			return this.GetNumVerticesFor(go.GetInstanceID());
		}

		public override int GetNumVerticesFor(int instanceID)
		{
			MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject;
			if (this.instance2Combined_MapTryGetValue(instanceID, out mb_DynamicGameObject))
			{
				return mb_DynamicGameObject.numVerts;
			}
			return -1;
		}

		public override Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap()
		{
			Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue> dictionary = new Dictionary<MB3_MeshCombiner.MBBlendShapeKey, MB3_MeshCombiner.MBBlendShapeValue>();
			for (int i = 0; i < this.blendShapesInCombined.Length; i++)
			{
				MB3_MeshCombiner.MBBlendShapeValue mbblendShapeValue = new MB3_MeshCombiner.MBBlendShapeValue();
				mbblendShapeValue.combinedMeshGameObject = this._targetRenderer.gameObject;
				mbblendShapeValue.blendShapeIndex = i;
				dictionary.Add(new MB3_MeshCombiner.MBBlendShapeKey(this.blendShapesInCombined[i].gameObjectID, this.blendShapesInCombined[i].indexInSource), mbblendShapeValue);
			}
			return dictionary;
		}

		private void _initialize(int numResultMats)
		{
			if (this.mbDynamicObjectsInCombinedMesh.Count == 0)
			{
				this.lightmapIndex = -1;
			}
			if (this._mesh == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("_initialize Creating new Mesh", Array.Empty<object>());
				}
				this._mesh = this.GetMesh();
			}
			if (this.instance2Combined_MapCount() != this.mbDynamicObjectsInCombinedMesh.Count)
			{
				this.instance2Combined_MapClear();
				for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					if (this.mbDynamicObjectsInCombinedMesh[i] != null)
					{
						this.instance2Combined_MapAdd(this.mbDynamicObjectsInCombinedMesh[i].instanceID, this.mbDynamicObjectsInCombinedMesh[i]);
					}
				}
				this.boneWeights = this._mesh.boneWeights;
			}
			if (this.objectsInCombinedMesh.Count == 0 && this.submeshTris.Length != numResultMats)
			{
				this.submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[numResultMats];
				for (int j = 0; j < this.submeshTris.Length; j++)
				{
					this.submeshTris[j] = new MB3_MeshCombinerSingle.SerializableIntArray(0);
				}
			}
			if (this.mbDynamicObjectsInCombinedMesh.Count > 0 && this.mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 && this.renderType == MB_RenderType.skinnedMeshRenderer && this.boneWeights.Length != 0)
			{
				for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[k];
					HashSet<int> hashSet = new HashSet<int>();
					for (int l = mb_DynamicGameObject.vertIdx; l < mb_DynamicGameObject.vertIdx + mb_DynamicGameObject.numVerts; l++)
					{
						if (this.boneWeights[l].weight0 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex0);
						}
						if (this.boneWeights[l].weight1 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex1);
						}
						if (this.boneWeights[l].weight2 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex2);
						}
						if (this.boneWeights[l].weight3 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex3);
						}
					}
					mb_DynamicGameObject.indexesOfBonesUsed = new int[hashSet.Count];
					hashSet.CopyTo(mb_DynamicGameObject.indexesOfBonesUsed);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log(string.Format("_initialize numObjsInCombined={0}", this.mbDynamicObjectsInCombinedMesh.Count));
			}
		}

		private bool _collectMaterialTriangles(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
		{
			int num = m.subMeshCount;
			if (sharedMaterials.Length < num)
			{
				num = sharedMaterials.Length;
			}
			dgo._tmpSubmeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[num];
			dgo.targetSubmeshIdxs = new int[num];
			for (int i = 0; i < num; i++)
			{
				if (this._textureBakeResults.doMultiMaterial)
				{
					if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[i]))
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Object ",
							dgo.name,
							" has a material that was not found in the result materials maping. ",
							sharedMaterials[i]
						}));
						return false;
					}
					dgo.targetSubmeshIdxs[i] = (int)sourceMats2submeshIdx_map[sharedMaterials[i]];
				}
				else
				{
					dgo.targetSubmeshIdxs[i] = 0;
				}
				dgo._tmpSubmeshTris[i] = new MB3_MeshCombinerSingle.SerializableIntArray();
				dgo._tmpSubmeshTris[i].data = m.GetTriangles(i);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"Collecting triangles for: ",
						dgo.name,
						" submesh:",
						i,
						" maps to submesh:",
						dgo.targetSubmeshIdxs[i],
						" added:",
						dgo._tmpSubmeshTris[i].data.Length
					}), new object[]
					{
						this.LOG_LEVEL
					});
				}
			}
			return true;
		}

		private bool _collectOutOfBoundsUVRects2(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResults, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache)
		{
			if (this._textureBakeResults == null)
			{
				Debug.LogError("Need to bake textures into combined material");
				return false;
			}
			MB_Utility.MeshAnalysisResult[] array;
			if (meshAnalysisResults.TryGetValue(m.GetInstanceID(), out array))
			{
				dgo.obUVRects = new Rect[sharedMaterials.Length];
				for (int i = 0; i < dgo.obUVRects.Length; i++)
				{
					dgo.obUVRects[i] = array[i].uvRect;
				}
			}
			else
			{
				int subMeshCount = m.subMeshCount;
				int num = subMeshCount;
				if (sharedMaterials.Length < subMeshCount)
				{
					num = sharedMaterials.Length;
				}
				dgo.obUVRects = new Rect[num];
				array = new MB_Utility.MeshAnalysisResult[subMeshCount];
				for (int j = 0; j < subMeshCount; j++)
				{
					int num2 = dgo.targetSubmeshIdxs[j];
					if (this._textureBakeResults.resultMaterials[num2].considerMeshUVs)
					{
						MB_Utility.hasOutOfBoundsUVs(meshChannelCache.GetUv0Raw(m), m, ref array[j], j);
						Rect uvRect = array[j].uvRect;
						if (j < num)
						{
							dgo.obUVRects[j] = uvRect;
						}
					}
				}
				meshAnalysisResults.Add(m.GetInstanceID(), array);
			}
			return true;
		}

		private bool _validateTextureBakeResults()
		{
			if (this._textureBakeResults == null)
			{
				Debug.LogError("Texture Bake Results is null. Can't combine meshes.");
				return false;
			}
			if (this._textureBakeResults.materialsAndUVRects == null || this._textureBakeResults.materialsAndUVRects.Length == 0)
			{
				Debug.LogError("Texture Bake Results has no materials in material to sourceUVRect map. Try baking materials. Can't combine meshes.");
				return false;
			}
			if (this._textureBakeResults.resultMaterials == null || this._textureBakeResults.resultMaterials.Length == 0)
			{
				if (this._textureBakeResults.materialsAndUVRects == null || this._textureBakeResults.materialsAndUVRects.Length == 0 || this._textureBakeResults.doMultiMaterial || !(this._textureBakeResults.resultMaterial != null))
				{
					Debug.LogError("Texture Bake Results has no result materials. Try baking materials. Can't combine meshes.");
					return false;
				}
				MB_MultiMaterial[] array = this._textureBakeResults.resultMaterials = new MB_MultiMaterial[1];
				array[0] = new MB_MultiMaterial();
				array[0].combinedMaterial = this._textureBakeResults.resultMaterial;
				array[0].considerMeshUVs = this._textureBakeResults.fixOutOfBoundsUVs;
				List<Material> list = array[0].sourceMaterials = new List<Material>();
				for (int i = 0; i < this._textureBakeResults.materialsAndUVRects.Length; i++)
				{
					if (!list.Contains(this._textureBakeResults.materialsAndUVRects[i].material))
					{
						list.Add(this._textureBakeResults.materialsAndUVRects[i].material);
					}
				}
			}
			return true;
		}

		private bool _validateMeshFlags()
		{
			if (this.mbDynamicObjectsInCombinedMesh.Count > 0 && ((!this._doNorm && this.doNorm) || (!this._doTan && this.doTan) || (!this._doCol && this.doCol) || (!this._doUV && this.doUV) || (!this._doUV3 && this.doUV3) || (!this._doUV4 && this.doUV4)))
			{
				Debug.LogError("The channels have changed. There are already objects in the combined mesh that were added with a different set of channels.");
				return false;
			}
			this._doNorm = this.doNorm;
			this._doTan = this.doTan;
			this._doCol = this.doCol;
			this._doUV = this.doUV;
			this._doUV3 = this.doUV3;
			this._doUV4 = this.doUV4;
			return true;
		}

		private bool _showHide(GameObject[] goToShow, GameObject[] goToHide)
		{
			if (goToShow == null)
			{
				goToShow = this.empty;
			}
			if (goToHide == null)
			{
				goToHide = this.empty;
			}
			int numResultMats = 1;
			if (this._textureBakeResults.doMultiMaterial)
			{
				numResultMats = this._textureBakeResults.resultMaterials.Length;
			}
			this._initialize(numResultMats);
			for (int i = 0; i < goToHide.Length; i++)
			{
				if (!this.instance2Combined_MapContainsKey(goToHide[i].GetInstanceID()))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Trying to hide an object " + goToHide[i] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					}
					return false;
				}
			}
			for (int j = 0; j < goToShow.Length; j++)
			{
				if (!this.instance2Combined_MapContainsKey(goToShow[j].GetInstanceID()))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Trying to show an object " + goToShow[j] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
					}
					return false;
				}
			}
			for (int k = 0; k < goToHide.Length; k++)
			{
				this._instance2combined_map[goToHide[k].GetInstanceID()].show = false;
			}
			for (int l = 0; l < goToShow.Length; l++)
			{
				this._instance2combined_map[goToShow[l].GetInstanceID()].show = true;
			}
			return true;
		}

		private bool _addToCombined(GameObject[] goToAdd, int[] goToDelete, bool disableRendererInSource)
		{
			MB3_MeshCombinerSingle.<>c__DisplayClass55_0 CS$<>8__locals1 = new MB3_MeshCombinerSingle.<>c__DisplayClass55_0();
			if (!this._validateTextureBakeResults())
			{
				return false;
			}
			if (!this._validateMeshFlags())
			{
				return false;
			}
			if (!this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return false;
			}
			if (this.outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				if (this._targetRenderer == null || !(this._targetRenderer is SkinnedMeshRenderer))
				{
					Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
					return false;
				}
				if (((SkinnedMeshRenderer)this.targetRenderer).sharedMesh != this._mesh)
				{
					Debug.LogError("The combined mesh was not assigned to the targetRenderer. Try using buildSceneMeshObject to set up the combined mesh correctly");
				}
			}
			if (this._doBlendShapes && this.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				Debug.LogError("If doBlendShapes is set then RenderType must be skinnedMeshRenderer.");
				return false;
			}
			if (goToAdd == null)
			{
				CS$<>8__locals1._goToAdd = this.empty;
			}
			else
			{
				CS$<>8__locals1._goToAdd = (GameObject[])goToAdd.Clone();
			}
			int[] array;
			if (goToDelete == null)
			{
				array = this.emptyIDs;
			}
			else
			{
				array = (int[])goToDelete.Clone();
			}
			if (this._mesh == null)
			{
				this.DestroyMesh();
			}
			MB2_TextureBakeResults.Material2AtlasRectangleMapper material2AtlasRectangleMapper = new MB2_TextureBakeResults.Material2AtlasRectangleMapper(this.textureBakeResults);
			int num = 1;
			if (this._textureBakeResults.doMultiMaterial)
			{
				num = this._textureBakeResults.resultMaterials.Length;
			}
			this._initialize(num);
			if (this.submeshTris.Length != num)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"The number of submeshes ",
					this.submeshTris.Length,
					" in the combined mesh was not equal to the number of result materials ",
					num,
					" in the Texture Bake Result"
				}));
				return false;
			}
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"==== Calling _addToCombined objs adding:",
					CS$<>8__locals1._goToAdd.Length,
					" objs deleting:",
					array.Length,
					" fixOutOfBounds:",
					this.textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs().ToString(),
					" doMultiMaterial:",
					this.textureBakeResults.doMultiMaterial.ToString(),
					" disableRenderersInSource:",
					disableRendererInSource.ToString()
				}), new object[]
				{
					this.LOG_LEVEL
				});
			}
			if (this._textureBakeResults.resultMaterials == null || this._textureBakeResults.resultMaterials.Length == 0)
			{
				this._textureBakeResults.resultMaterials = new MB_MultiMaterial[1];
				this._textureBakeResults.resultMaterials[0] = new MB_MultiMaterial();
				this._textureBakeResults.resultMaterials[0].combinedMaterial = this._textureBakeResults.resultMaterial;
				this._textureBakeResults.resultMaterials[0].considerMeshUVs = false;
				List<Material> list = this._textureBakeResults.resultMaterials[0].sourceMaterials = new List<Material>();
				for (int i3 = 0; i3 < this._textureBakeResults.materialsAndUVRects.Length; i3++)
				{
					list.Add(this._textureBakeResults.materialsAndUVRects[i3].material);
				}
			}
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			for (int j = 0; j < num; j++)
			{
				MB_MultiMaterial mb_MultiMaterial = this._textureBakeResults.resultMaterials[j];
				for (int k = 0; k < mb_MultiMaterial.sourceMaterials.Count; k++)
				{
					if (mb_MultiMaterial.sourceMaterials[k] == null)
					{
						Debug.LogError("Found null material in source materials for combined mesh materials " + j);
						return false;
					}
					if (!orderedDictionary.Contains(mb_MultiMaterial.sourceMaterials[k]))
					{
						orderedDictionary.Add(mb_MultiMaterial.sourceMaterials[k], j);
					}
				}
			}
			int num2 = 0;
			int[] array2 = new int[num];
			int num3 = 0;
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] array3 = null;
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> hashSet2 = new HashSet<MB3_MeshCombinerSingle.BoneAndBindpose>();
			if (this.renderType == MB_RenderType.skinnedMeshRenderer && array.Length != 0)
			{
				array3 = this._buildBoneIdx2dgoMap();
			}
			for (int l = 0; l < array.Length; l++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject;
				if (this.instance2Combined_MapTryGetValue(array[l], out mb_DynamicGameObject))
				{
					num2 += mb_DynamicGameObject.numVerts;
					num3 += mb_DynamicGameObject.numBlendShapes;
					if (this.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						for (int m = 0; m < mb_DynamicGameObject.indexesOfBonesUsed.Length; m++)
						{
							if (array3[mb_DynamicGameObject.indexesOfBonesUsed[m]].Contains(mb_DynamicGameObject))
							{
								array3[mb_DynamicGameObject.indexesOfBonesUsed[m]].Remove(mb_DynamicGameObject);
								if (array3[mb_DynamicGameObject.indexesOfBonesUsed[m]].Count == 0)
								{
									hashSet.Add(mb_DynamicGameObject.indexesOfBonesUsed[m]);
								}
							}
						}
					}
					for (int n = 0; n < mb_DynamicGameObject.submeshNumTris.Length; n++)
					{
						array2[n] += mb_DynamicGameObject.submeshNumTris[n];
					}
				}
				else if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Trying to delete an object that is not in combined mesh");
				}
			}
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list2 = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache = new MB3_MeshCombinerSingle.MeshChannelsCache(this);
			int num4 = 0;
			int[] array4 = new int[num];
			int num5 = 0;
			Dictionary<Transform, int> dictionary2 = new Dictionary<Transform, int>();
			for (int num6 = 0; num6 < this.bones.Length; num6++)
			{
				dictionary2.Add(this.bones[num6], num6);
			}
			int i;
			int i2;
			for (i = 0; i < CS$<>8__locals1._goToAdd.Length; i = i2 + 1)
			{
				if (!this.instance2Combined_MapContainsKey(CS$<>8__locals1._goToAdd[i].GetInstanceID()) || Array.FindIndex<int>(array, (int o) => o == CS$<>8__locals1._goToAdd[i].GetInstanceID()) != -1)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = new MB3_MeshCombinerSingle.MB_DynamicGameObject();
					GameObject gameObject = CS$<>8__locals1._goToAdd[i];
					Material[] gomaterials = MB_Utility.GetGOMaterials(gameObject);
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log(string.Format("Getting {0} shared materials for {1}", gomaterials.Length, gameObject));
					}
					if (gomaterials == null)
					{
						Debug.LogError("Object " + gameObject.name + " does not have a Renderer");
						CS$<>8__locals1._goToAdd[i] = null;
						return false;
					}
					Mesh mesh = MB_Utility.GetMesh(gameObject);
					if (mesh == null)
					{
						Debug.LogError("Object " + gameObject.name + " MeshFilter or SkinedMeshRenderer had no mesh");
						CS$<>8__locals1._goToAdd[i] = null;
						return false;
					}
					if (MBVersion.IsRunningAndMeshNotReadWriteable(mesh))
					{
						Debug.LogError("Object " + gameObject.name + " Mesh Importer has read/write flag set to 'false'. This needs to be set to 'true' in order to read data from this mesh.");
						CS$<>8__locals1._goToAdd[i] = null;
						return false;
					}
					Rect[] array5 = new Rect[gomaterials.Length];
					Rect[] array6 = new Rect[gomaterials.Length];
					Rect[] array7 = new Rect[gomaterials.Length];
					string message = "";
					for (int num7 = 0; num7 < gomaterials.Length; num7++)
					{
						object obj = orderedDictionary[gomaterials[num7]];
						if (obj == null)
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Source object ",
								gameObject.name,
								" used a material ",
								gomaterials[num7],
								" that was not in the baked materials."
							}));
							return false;
						}
						int idxInResultMats = (int)obj;
						if (!material2AtlasRectangleMapper.TryMapMaterialToUVRect(gomaterials[num7], mesh, num7, idxInResultMats, meshChannelsCache, dictionary, out array5[num7], out array6[num7], out array7[num7], ref message, this.LOG_LEVEL))
						{
							Debug.LogError(message);
							CS$<>8__locals1._goToAdd[i] = null;
							return false;
						}
					}
					if (CS$<>8__locals1._goToAdd[i] != null)
					{
						list2.Add(mb_DynamicGameObject2);
						mb_DynamicGameObject2.name = string.Format("{0} {1}", CS$<>8__locals1._goToAdd[i].ToString(), CS$<>8__locals1._goToAdd[i].GetInstanceID());
						mb_DynamicGameObject2.instanceID = CS$<>8__locals1._goToAdd[i].GetInstanceID();
						mb_DynamicGameObject2.uvRects = array5;
						mb_DynamicGameObject2.encapsulatingRect = array6;
						mb_DynamicGameObject2.sourceMaterialTiling = array7;
						mb_DynamicGameObject2.numVerts = mesh.vertexCount;
						if (this._doBlendShapes)
						{
							mb_DynamicGameObject2.numBlendShapes = mesh.blendShapeCount;
						}
						Renderer renderer = MB_Utility.GetRenderer(gameObject);
						if (this.renderType == MB_RenderType.skinnedMeshRenderer)
						{
							this._CollectBonesToAddForDGO(mb_DynamicGameObject2, dictionary2, hashSet, hashSet2, renderer, meshChannelsCache);
						}
						if (this.lightmapIndex == -1)
						{
							this.lightmapIndex = renderer.lightmapIndex;
						}
						if (this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
						{
							if (this.lightmapIndex != renderer.lightmapIndex && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " has a different lightmap index. Lightmapping will not work.");
							}
							if (!MBVersion.GetActive(gameObject) && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " is inactive. Can only get lightmap index of active objects.");
							}
							if (renderer.lightmapIndex == -1 && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " does not have an index to a lightmap.");
							}
						}
						mb_DynamicGameObject2.lightmapIndex = renderer.lightmapIndex;
						mb_DynamicGameObject2.lightmapTilingOffset = MBVersion.GetLightmapTilingOffset(renderer);
						if (!this._collectMaterialTriangles(mesh, mb_DynamicGameObject2, gomaterials, orderedDictionary))
						{
							return false;
						}
						mb_DynamicGameObject2.meshSize = renderer.bounds.size;
						mb_DynamicGameObject2.submeshNumTris = new int[num];
						mb_DynamicGameObject2.submeshTriIdxs = new int[num];
						if (this.textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs() && !this._collectOutOfBoundsUVRects2(mesh, mb_DynamicGameObject2, gomaterials, orderedDictionary, dictionary, meshChannelsCache))
						{
							return false;
						}
						num4 += mb_DynamicGameObject2.numVerts;
						num5 += mb_DynamicGameObject2.numBlendShapes;
						for (int num8 = 0; num8 < mb_DynamicGameObject2._tmpSubmeshTris.Length; num8++)
						{
							array4[mb_DynamicGameObject2.targetSubmeshIdxs[num8]] += mb_DynamicGameObject2._tmpSubmeshTris[num8].data.Length;
						}
						mb_DynamicGameObject2.invertTriangles = this.IsMirrored(gameObject.transform.localToWorldMatrix);
					}
				}
				else
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Object " + CS$<>8__locals1._goToAdd[i].name + " has already been added");
					}
					CS$<>8__locals1._goToAdd[i] = null;
				}
				i2 = i;
			}
			for (int num9 = 0; num9 < CS$<>8__locals1._goToAdd.Length; num9++)
			{
				if (CS$<>8__locals1._goToAdd[num9] != null && disableRendererInSource)
				{
					MB_Utility.DisableRendererInSource(CS$<>8__locals1._goToAdd[num9]);
					if (this.LOG_LEVEL == MB2_LogLevel.trace)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Disabling renderer on ",
							CS$<>8__locals1._goToAdd[num9].name,
							" id=",
							CS$<>8__locals1._goToAdd[num9].GetInstanceID()
						}));
					}
				}
			}
			int num10 = this.verts.Length + num4 - num2;
			int num11 = this.bindPoses.Length + hashSet2.Count - hashSet.Count;
			int[] array8 = new int[num];
			int num12 = this.blendShapes.Length + num5 - num3;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Verts adding:",
					num4,
					" deleting:",
					num2,
					" submeshes:",
					array8.Length,
					" bones:",
					num11,
					" blendShapes:",
					num12
				}));
			}
			for (int num13 = 0; num13 < array8.Length; num13++)
			{
				array8[num13] = this.submeshTris[num13].data.Length + array4[num13] - array2[num13];
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"    submesh :",
						num13,
						" already contains:",
						this.submeshTris[num13].data.Length,
						" tris to be Added:",
						array4[num13],
						" tris to be Deleted:",
						array2[num13]
					}), Array.Empty<object>());
				}
			}
			if (num10 > 65534)
			{
				Debug.LogError("Cannot add objects. Resulting mesh will have more than 64k vertices. Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. You don't have to re-configure the MB2_TextureBaker. Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
				return false;
			}
			Vector3[] destinationArray = null;
			Vector4[] destinationArray2 = null;
			Vector2[] destinationArray3 = null;
			Vector2[] destinationArray4 = null;
			Vector2[] array9 = null;
			Vector2[] array10 = null;
			Color[] array11 = null;
			MB3_MeshCombinerSingle.MBBlendShape[] array12 = null;
			Vector3[] array13 = new Vector3[num10];
			if (this._doNorm)
			{
				destinationArray = new Vector3[num10];
			}
			if (this._doTan)
			{
				destinationArray2 = new Vector4[num10];
			}
			if (this._doUV)
			{
				destinationArray3 = new Vector2[num10];
			}
			if (this._doUV3)
			{
				array9 = new Vector2[num10];
			}
			if (this._doUV4)
			{
				array10 = new Vector2[num10];
			}
			if (this.doUV2())
			{
				destinationArray4 = new Vector2[num10];
			}
			if (this._doCol)
			{
				array11 = new Color[num10];
			}
			if (this._doBlendShapes)
			{
				array12 = new MB3_MeshCombinerSingle.MBBlendShape[num12];
			}
			BoneWeight[] array14 = new BoneWeight[num10];
			Matrix4x4[] array15 = new Matrix4x4[num11];
			Transform[] array16 = new Transform[num11];
			MB3_MeshCombinerSingle.SerializableIntArray[] array17 = new MB3_MeshCombinerSingle.SerializableIntArray[num];
			for (int num14 = 0; num14 < array17.Length; num14++)
			{
				array17[num14] = new MB3_MeshCombinerSingle.SerializableIntArray(array8[num14]);
			}
			for (int num15 = 0; num15 < array.Length; num15++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject3 = null;
				if (this.instance2Combined_MapTryGetValue(array[num15], out mb_DynamicGameObject3))
				{
					mb_DynamicGameObject3._beingDeleted = true;
				}
			}
			this.mbDynamicObjectsInCombinedMesh.Sort();
			int num16 = 0;
			int num17 = 0;
			int[] array18 = new int[num];
			int num18 = 0;
			for (int num19 = 0; num19 < this.mbDynamicObjectsInCombinedMesh.Count; num19++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject4 = this.mbDynamicObjectsInCombinedMesh[num19];
				if (!mb_DynamicGameObject4._beingDeleted)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Copying obj in combined arrays idx:" + num19, new object[]
						{
							this.LOG_LEVEL
						});
					}
					Array.Copy(this.verts, mb_DynamicGameObject4.vertIdx, array13, num16, mb_DynamicGameObject4.numVerts);
					if (this._doNorm)
					{
						Array.Copy(this.normals, mb_DynamicGameObject4.vertIdx, destinationArray, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doTan)
					{
						Array.Copy(this.tangents, mb_DynamicGameObject4.vertIdx, destinationArray2, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doUV)
					{
						Array.Copy(this.uvs, mb_DynamicGameObject4.vertIdx, destinationArray3, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doUV3)
					{
						Array.Copy(this.uv3s, mb_DynamicGameObject4.vertIdx, array9, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doUV4)
					{
						Array.Copy(this.uv4s, mb_DynamicGameObject4.vertIdx, array10, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this.doUV2())
					{
						Array.Copy(this.uv2s, mb_DynamicGameObject4.vertIdx, destinationArray4, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doCol)
					{
						Array.Copy(this.colors, mb_DynamicGameObject4.vertIdx, array11, num16, mb_DynamicGameObject4.numVerts);
					}
					if (this._doBlendShapes)
					{
						Array.Copy(this.blendShapes, mb_DynamicGameObject4.blendShapeIdx, array12, num17, mb_DynamicGameObject4.numBlendShapes);
					}
					if (this.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						Array.Copy(this.boneWeights, mb_DynamicGameObject4.vertIdx, array14, num16, mb_DynamicGameObject4.numVerts);
					}
					for (int num20 = 0; num20 < num; num20++)
					{
						int[] data = this.submeshTris[num20].data;
						int num21 = mb_DynamicGameObject4.submeshTriIdxs[num20];
						int num22 = mb_DynamicGameObject4.submeshNumTris[num20];
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							MB2_Log.LogDebug(string.Concat(new object[]
							{
								"    Adjusting submesh triangles submesh:",
								num20,
								" startIdx:",
								num21,
								" num:",
								num22,
								" nsubmeshTris:",
								array17.Length,
								" targSubmeshTidx:",
								array18.Length
							}), new object[]
							{
								this.LOG_LEVEL
							});
						}
						for (int num23 = num21; num23 < num21 + num22; num23++)
						{
							data[num23] -= num18;
						}
						Array.Copy(data, num21, array17[num20].data, array18[num20], num22);
					}
					mb_DynamicGameObject4.vertIdx = num16;
					mb_DynamicGameObject4.blendShapeIdx = num17;
					for (int num24 = 0; num24 < array18.Length; num24++)
					{
						mb_DynamicGameObject4.submeshTriIdxs[num24] = array18[num24];
						array18[num24] += mb_DynamicGameObject4.submeshNumTris[num24];
					}
					num17 += mb_DynamicGameObject4.numBlendShapes;
					num16 += mb_DynamicGameObject4.numVerts;
				}
				else
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Not copying obj: " + num19, new object[]
						{
							this.LOG_LEVEL
						});
					}
					num18 += mb_DynamicGameObject4.numVerts;
				}
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				this._CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(hashSet, hashSet2, array16, array15, array14, num2);
			}
			for (int num25 = this.mbDynamicObjectsInCombinedMesh.Count - 1; num25 >= 0; num25--)
			{
				if (this.mbDynamicObjectsInCombinedMesh[num25]._beingDeleted)
				{
					this.instance2Combined_MapRemove(this.mbDynamicObjectsInCombinedMesh[num25].instanceID);
					this.objectsInCombinedMesh.RemoveAt(num25);
					this.mbDynamicObjectsInCombinedMesh.RemoveAt(num25);
				}
			}
			this.verts = array13;
			if (this._doNorm)
			{
				this.normals = destinationArray;
			}
			if (this._doTan)
			{
				this.tangents = destinationArray2;
			}
			if (this._doUV)
			{
				this.uvs = destinationArray3;
			}
			if (this._doUV3)
			{
				this.uv3s = array9;
			}
			if (this._doUV4)
			{
				this.uv4s = array10;
			}
			if (this.doUV2())
			{
				this.uv2s = destinationArray4;
			}
			if (this._doCol)
			{
				this.colors = array11;
			}
			if (this._doBlendShapes)
			{
				this.blendShapes = array12;
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				this.boneWeights = array14;
			}
			int num26 = this.bones.Length - hashSet.Count;
			this.bindPoses = array15;
			this.bones = array16;
			this.submeshTris = array17;
			int num27 = 0;
			foreach (MB3_MeshCombinerSingle.BoneAndBindpose boneAndBindpose in hashSet2)
			{
				array16[num26 + num27] = boneAndBindpose.bone;
				array15[num26 + num27] = boneAndBindpose.bindPose;
				num27++;
			}
			for (int num28 = 0; num28 < list2.Count; num28++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject5 = list2[num28];
				GameObject gameObject2 = CS$<>8__locals1._goToAdd[num28];
				int num29 = num16;
				int index = num17;
				Mesh mesh2 = MB_Utility.GetMesh(gameObject2);
				Matrix4x4 localToWorldMatrix = gameObject2.transform.localToWorldMatrix;
				Matrix4x4 matrix4x = localToWorldMatrix;
				matrix4x[0, 3] = (matrix4x[1, 3] = (matrix4x[2, 3] = 0f));
				array13 = meshChannelsCache.GetVertices(mesh2);
				Vector3[] array19 = null;
				Vector4[] array20 = null;
				if (this._doNorm)
				{
					array19 = meshChannelsCache.GetNormals(mesh2);
				}
				if (this._doTan)
				{
					array20 = meshChannelsCache.GetTangents(mesh2);
				}
				if (this.renderType != MB_RenderType.skinnedMeshRenderer)
				{
					for (int num30 = 0; num30 < array13.Length; num30++)
					{
						int num31 = num29 + num30;
						this.verts[num29 + num30] = localToWorldMatrix.MultiplyPoint3x4(array13[num30]);
						if (this._doNorm)
						{
							this.normals[num31] = matrix4x.MultiplyPoint3x4(array19[num30]);
							this.normals[num31] = this.normals[num31].normalized;
						}
						if (this._doTan)
						{
							float w = array20[num30].w;
							Vector3 v = matrix4x.MultiplyPoint3x4(array20[num30]);
							v.Normalize();
							this.tangents[num31] = v;
							this.tangents[num31].w = w;
						}
					}
				}
				else
				{
					if (this._doNorm)
					{
						array19.CopyTo(this.normals, num29);
					}
					if (this._doTan)
					{
						array20.CopyTo(this.tangents, num29);
					}
					array13.CopyTo(this.verts, num29);
				}
				int num32 = mesh2.subMeshCount;
				if (mb_DynamicGameObject5.uvRects.Length < num32)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + mb_DynamicGameObject5.name + " has more submeshes than materials", Array.Empty<object>());
					}
					num32 = mb_DynamicGameObject5.uvRects.Length;
				}
				else if (mb_DynamicGameObject5.uvRects.Length > num32 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + mb_DynamicGameObject5.name + " has fewer submeshes than materials");
				}
				if (this._doUV)
				{
					this._copyAndAdjustUVsFromMesh(mb_DynamicGameObject5, mesh2, num29, meshChannelsCache);
				}
				if (this.doUV2())
				{
					this._copyAndAdjustUV2FromMesh(mb_DynamicGameObject5, mesh2, num29, meshChannelsCache);
				}
				if (this._doUV3)
				{
					array9 = meshChannelsCache.GetUv3(mesh2);
					array9.CopyTo(this.uv3s, num29);
				}
				if (this._doUV4)
				{
					array10 = meshChannelsCache.GetUv4(mesh2);
					array10.CopyTo(this.uv4s, num29);
				}
				if (this._doCol)
				{
					array11 = meshChannelsCache.GetColors(mesh2);
					array11.CopyTo(this.colors, num29);
				}
				if (this._doBlendShapes)
				{
					array12 = meshChannelsCache.GetBlendShapes(mesh2, mb_DynamicGameObject5.instanceID);
					array12.CopyTo(this.blendShapes, index);
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					Renderer renderer2 = MB_Utility.GetRenderer(gameObject2);
					this._AddBonesToNewBonesArrayAndAdjustBWIndexes(mb_DynamicGameObject5, renderer2, num29, array16, array14, meshChannelsCache);
				}
				for (int num33 = 0; num33 < array18.Length; num33++)
				{
					mb_DynamicGameObject5.submeshTriIdxs[num33] = array18[num33];
				}
				for (int num34 = 0; num34 < mb_DynamicGameObject5._tmpSubmeshTris.Length; num34++)
				{
					int[] data2 = mb_DynamicGameObject5._tmpSubmeshTris[num34].data;
					for (int num35 = 0; num35 < data2.Length; num35++)
					{
						data2[num35] += num29;
					}
					if (mb_DynamicGameObject5.invertTriangles)
					{
						for (int num36 = 0; num36 < data2.Length; num36 += 3)
						{
							int num37 = data2[num36];
							data2[num36] = data2[num36 + 1];
							data2[num36 + 1] = num37;
						}
					}
					int num38 = mb_DynamicGameObject5.targetSubmeshIdxs[num34];
					data2.CopyTo(this.submeshTris[num38].data, array18[num38]);
					mb_DynamicGameObject5.submeshNumTris[num38] += data2.Length;
					array18[num38] += data2.Length;
				}
				mb_DynamicGameObject5.vertIdx = num16;
				mb_DynamicGameObject5.blendShapeIdx = num17;
				this.instance2Combined_MapAdd(gameObject2.GetInstanceID(), mb_DynamicGameObject5);
				this.objectsInCombinedMesh.Add(gameObject2);
				this.mbDynamicObjectsInCombinedMesh.Add(mb_DynamicGameObject5);
				num16 += array13.Length;
				if (this._doBlendShapes)
				{
					num17 += array12.Length;
				}
				for (int num39 = 0; num39 < mb_DynamicGameObject5._tmpSubmeshTris.Length; num39++)
				{
					mb_DynamicGameObject5._tmpSubmeshTris[num39] = null;
				}
				mb_DynamicGameObject5._tmpSubmeshTris = null;
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"Added to combined:",
						mb_DynamicGameObject5.name,
						" verts:",
						array13.Length,
						" bindPoses:",
						array15.Length
					}), new object[]
					{
						this.LOG_LEVEL
					});
				}
			}
			if (this.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects)
			{
				this._copyUV2unchangedToSeparateRects();
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug("===== _addToCombined completed. Verts in buffer: " + this.verts.Length, new object[]
				{
					this.LOG_LEVEL
				});
			}
			return true;
		}

		private void _copyAndAdjustUVsFromMesh(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache)
		{
			Vector2[] uv0Raw = meshChannelsCache.GetUv0Raw(mesh);
			bool flag = true;
			if (!this._textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs())
			{
				Rect rhs = new Rect(0f, 0f, 1f, 1f);
				bool flag2 = true;
				for (int i = 0; i < this._textureBakeResults.materialsAndUVRects.Length; i++)
				{
					if (this._textureBakeResults.materialsAndUVRects[i].atlasRect != rhs)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					flag = false;
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						Debug.Log("All atlases have only one texture in atlas UVs will be copied without adjusting");
					}
				}
			}
			if (flag)
			{
				int[] array = new int[uv0Raw.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = -1;
				}
				bool flag3 = false;
				for (int k = 0; k < dgo.targetSubmeshIdxs.Length; k++)
				{
					int[] array2;
					if (dgo._tmpSubmeshTris != null)
					{
						array2 = dgo._tmpSubmeshTris[k].data;
					}
					else
					{
						array2 = mesh.GetTriangles(k);
					}
					DRect drect = new DRect(dgo.uvRects[k]);
					DRect drect2;
					if (this.textureBakeResults.resultMaterials[dgo.targetSubmeshIdxs[k]].considerMeshUVs)
					{
						drect2 = new DRect(dgo.obUVRects[k]);
					}
					else
					{
						drect2 = new DRect(0.0, 0.0, 1.0, 1.0);
					}
					DRect drect3 = new DRect(dgo.sourceMaterialTiling[k]);
					DRect drect4 = new DRect(dgo.encapsulatingRect[k]);
					DRect drect5 = MB3_UVTransformUtility.InverseTransform(ref drect4);
					DRect drect6 = MB3_UVTransformUtility.InverseTransform(ref drect2);
					DRect drect7 = MB3_UVTransformUtility.CombineTransforms(ref drect2, ref drect3);
					DRect drect8 = MB3_UVTransformUtility.CombineTransforms(ref drect7, ref drect5);
					DRect drect9 = MB3_UVTransformUtility.CombineTransforms(ref drect6, ref drect8);
					drect9 = MB3_UVTransformUtility.CombineTransforms(ref drect9, ref drect);
					Rect rect = drect9.GetRect();
					foreach (int num in array2)
					{
						if (array[num] == -1)
						{
							array[num] = k;
							Vector2 vector = uv0Raw[num];
							vector.x = rect.x + vector.x * rect.width;
							vector.y = rect.y + vector.y * rect.height;
							this.uvs[vertsIdx + num] = vector;
						}
						if (array[num] != k)
						{
							flag3 = true;
						}
					}
				}
				if (flag3 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning(dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
				}
			}
			else
			{
				uv0Raw.CopyTo(this.uvs, vertsIdx);
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", uv0Raw.Length));
			}
		}

		private void _copyAndAdjustUV2FromMesh(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelsCache)
		{
			Vector2[] uv = meshChannelsCache.GetUv2(mesh);
			if (this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
				Vector2 vector = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
				Vector2 a = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
				for (int i = 0; i < uv.Length; i++)
				{
					Vector2 b;
					b.x = vector.x * uv[i].x;
					b.y = vector.y * uv[i].y;
					this.uv2s[vertsIdx + i] = a + b;
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + uv.Length);
					return;
				}
			}
			else
			{
				uv.CopyTo(this.uv2s, vertsIdx);
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + uv.Length);
				}
			}
		}

		public override void UpdateSkinnedMeshApproximateBounds()
		{
			this.UpdateSkinnedMeshApproximateBoundsFromBounds();
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBones()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.bones.Length == 0)
			{
				if (this.verts.Length != 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBonesStatic(this.bones, (SkinnedMeshRenderer)this.targetRenderer);
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.verts.Length == 0 || this.mbDynamicObjectsInCombinedMesh.Count == 0)
			{
				if (this.verts.Length != 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Nothing in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(this.objectsInCombinedMesh, (SkinnedMeshRenderer)this.targetRenderer);
		}

		private int _getNumBones(Renderer r)
		{
			if (this.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				return 0;
			}
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).bones.Length;
			}
			if (r is MeshRenderer)
			{
				return 1;
			}
			Debug.LogError("Could not _getNumBones. Object does not have a renderer");
			return 0;
		}

		private Transform[] _getBones(Renderer r)
		{
			return MBVersion.GetBones(r);
		}

		public override void Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
		{
			bool flag = false;
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				flag = true;
			}
			this.Apply(true, true, this._doNorm, this._doTan, this._doUV, this.doUV2(), this._doUV3, this._doUV4, this.doCol, flag, this.doBlendShapes, uv2GenerationMethod);
		}

		public virtual void ApplyShowHide()
		{
			if (this._validationLevel >= MB2_ValidationLevel.quick && !this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return;
			}
			if (this._mesh != null)
			{
				if (this.renderType == MB_RenderType.meshRenderer)
				{
					MBVersion.MeshClear(this._mesh, true);
					this._mesh.vertices = this.verts;
				}
				MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisWithShowHideApplied = this.GetSubmeshTrisWithShowHideApplied();
				if (this.textureBakeResults.doMultiMaterial)
				{
					int numNonZeroLengthSubmeshTris = this._mesh.subMeshCount = this._numNonZeroLengthSubmeshTris(submeshTrisWithShowHideApplied);
					int num = 0;
					for (int i = 0; i < submeshTrisWithShowHideApplied.Length; i++)
					{
						if (submeshTrisWithShowHideApplied[i].data.Length != 0)
						{
							this._mesh.SetTriangles(submeshTrisWithShowHideApplied[i].data, num);
							num++;
						}
					}
					this._updateMaterialsOnTargetRenderer(submeshTrisWithShowHideApplied, numNonZeroLengthSubmeshTris);
				}
				else
				{
					this._mesh.triangles = submeshTrisWithShowHideApplied[0].data;
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (this.verts.Length == 0)
					{
						this.targetRenderer.enabled = false;
					}
					else
					{
						this.targetRenderer.enabled = true;
					}
					bool updateWhenOffscreen = ((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = true;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = updateWhenOffscreen;
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ApplyShowHide");
					return;
				}
			}
			else
			{
				Debug.LogError("Need to add objects to this meshbaker before calling ApplyShowHide");
			}
		}

		public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			if (this._validationLevel >= MB2_ValidationLevel.quick && !this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return;
			}
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("Apply called tri={0} vert={1} norm={2} tan={3} uv={4} col={5} uv3={6} uv4={7} uv2={8} bone={9} blendShape{10} meshID={11}", new object[]
					{
						triangles,
						vertices,
						normals,
						tangents,
						uvs,
						colors,
						uv3,
						uv4,
						uv2,
						bones,
						this.blendShapes,
						this._mesh.GetInstanceID()
					}));
				}
				if (triangles || this._mesh.vertexCount != this.verts.Length)
				{
					if (triangles && !vertices && !normals && !tangents && !uvs && !colors && !uv3 && !uv4 && !uv2 && !bones)
					{
						MBVersion.MeshClear(this._mesh, true);
					}
					else
					{
						MBVersion.MeshClear(this._mesh, false);
					}
				}
				if (vertices)
				{
					Vector3[] array = this.verts;
					if (this.verts.Length != 0)
					{
						if (this._recenterVertsToBoundsCenter && this._renderType == MB_RenderType.meshRenderer)
						{
							array = new Vector3[this.verts.Length];
							Vector3 vector = this.verts[0];
							Vector3 vector2 = this.verts[0];
							for (int i = 1; i < this.verts.Length; i++)
							{
								Vector3 vector3 = this.verts[i];
								if (vector.x < vector3.x)
								{
									vector.x = vector3.x;
								}
								if (vector.y < vector3.y)
								{
									vector.y = vector3.y;
								}
								if (vector.z < vector3.z)
								{
									vector.z = vector3.z;
								}
								if (vector2.x > vector3.x)
								{
									vector2.x = vector3.x;
								}
								if (vector2.y > vector3.y)
								{
									vector2.y = vector3.y;
								}
								if (vector2.z > vector3.z)
								{
									vector2.z = vector3.z;
								}
							}
							Vector3 vector4 = (vector + vector2) / 2f;
							for (int j = 0; j < this.verts.Length; j++)
							{
								array[j] = this.verts[j] - vector4;
							}
							this.targetRenderer.transform.position = vector4;
						}
						else
						{
							this.targetRenderer.transform.position = Vector3.zero;
						}
					}
					this._mesh.vertices = array;
				}
				if (triangles && this._textureBakeResults)
				{
					if (this._textureBakeResults == null)
					{
						Debug.LogError("Texture Bake Result was not set.");
					}
					else
					{
						MB3_MeshCombinerSingle.SerializableIntArray[] submeshTrisWithShowHideApplied = this.GetSubmeshTrisWithShowHideApplied();
						int numNonZeroLengthSubmeshTris = this._mesh.subMeshCount = this._numNonZeroLengthSubmeshTris(submeshTrisWithShowHideApplied);
						int num = 0;
						for (int k = 0; k < submeshTrisWithShowHideApplied.Length; k++)
						{
							if (submeshTrisWithShowHideApplied[k].data.Length != 0)
							{
								this._mesh.SetTriangles(submeshTrisWithShowHideApplied[k].data, num);
								num++;
							}
						}
						this._updateMaterialsOnTargetRenderer(submeshTrisWithShowHideApplied, numNonZeroLengthSubmeshTris);
					}
				}
				if (normals)
				{
					if (this._doNorm)
					{
						this._mesh.normals = this.normals;
					}
					else
					{
						Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
					}
				}
				if (tangents)
				{
					if (this._doTan)
					{
						this._mesh.tangents = this.tangents;
					}
					else
					{
						Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents");
					}
				}
				if (uvs)
				{
					if (this._doUV)
					{
						this._mesh.uv = this.uvs;
					}
					else
					{
						Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs");
					}
				}
				if (colors)
				{
					if (this._doCol)
					{
						this._mesh.colors = this.colors;
					}
					else
					{
						Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors");
					}
				}
				if (uv3)
				{
					if (this._doUV3)
					{
						MBVersion.MeshAssignUV3(this._mesh, this.uv3s);
					}
					else
					{
						Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s");
					}
				}
				if (uv4)
				{
					if (this._doUV4)
					{
						MBVersion.MeshAssignUV4(this._mesh, this.uv4s);
					}
					else
					{
						Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s");
					}
				}
				if (uv2)
				{
					if (this.doUV2())
					{
						this._mesh.uv2 = this.uv2s;
					}
					else
					{
						Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + this.lightmapOption);
					}
				}
				bool flag = false;
				if (this.renderType != MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
				{
					if (uv2GenerationMethod != null)
					{
						uv2GenerationMethod(this._mesh, this.uv2UnwrappingParamsHardAngle, this.uv2UnwrappingParamsPackMargin);
						if (this.LOG_LEVEL >= MB2_LogLevel.trace)
						{
							Debug.Log("generating new UV2 layout for the combined mesh ");
						}
					}
					else
					{
						Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
					}
					flag = true;
				}
				else if (this.renderType == MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("UV2 cannot be generated for SkinnedMeshRenderer objects.");
				}
				if (this.renderType != MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && !flag)
				{
					Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (this.verts.Length == 0)
					{
						this.targetRenderer.enabled = false;
					}
					else
					{
						this.targetRenderer.enabled = true;
					}
					bool updateWhenOffscreen = ((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = true;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = updateWhenOffscreen;
				}
				if (bones)
				{
					this._mesh.bindposes = this.bindPoses;
					this._mesh.boneWeights = this.boneWeights;
				}
				if (blendShapesFlag && (MBVersion.GetMajorVersion() > 5 || (MBVersion.GetMajorVersion() == 5 && MBVersion.GetMinorVersion() >= 3)))
				{
					if (this.blendShapesInCombined.Length != this.blendShapes.Length)
					{
						this.blendShapesInCombined = new MB3_MeshCombinerSingle.MBBlendShape[this.blendShapes.Length];
					}
					Vector3[] array2 = new Vector3[this.verts.Length];
					Vector3[] array3 = new Vector3[this.verts.Length];
					Vector3[] array4 = new Vector3[this.verts.Length];
					MBVersion.ClearBlendShapes(this._mesh);
					for (int l = 0; l < this.blendShapes.Length; l++)
					{
						MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.instance2Combined_MapGet(this.blendShapes[l].gameObjectID);
						if (mb_DynamicGameObject != null)
						{
							for (int m = 0; m < this.blendShapes[l].frames.Length; m++)
							{
								MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = this.blendShapes[l].frames[m];
								int vertIdx = mb_DynamicGameObject.vertIdx;
								Array.Copy(mbblendShapeFrame.vertices, 0, array2, vertIdx, this.blendShapes[l].frames[m].vertices.Length);
								Array.Copy(mbblendShapeFrame.normals, 0, array3, vertIdx, this.blendShapes[l].frames[m].normals.Length);
								Array.Copy(mbblendShapeFrame.tangents, 0, array4, vertIdx, this.blendShapes[l].frames[m].tangents.Length);
								MBVersion.AddBlendShapeFrame(this._mesh, this.blendShapes[l].name + this.blendShapes[l].gameObjectID, mbblendShapeFrame.frameWeight, array2, array3, array4);
								this._ZeroArray(array2, vertIdx, this.blendShapes[l].frames[m].vertices.Length);
								this._ZeroArray(array3, vertIdx, this.blendShapes[l].frames[m].normals.Length);
								this._ZeroArray(array4, vertIdx, this.blendShapes[l].frames[m].tangents.Length);
							}
						}
						else
						{
							Debug.LogError("InstanceID in blend shape that was not in instance2combinedMap");
						}
						this.blendShapesInCombined[l] = this.blendShapes[l];
					}
					((SkinnedMeshRenderer)this._targetRenderer).sharedMesh = null;
					((SkinnedMeshRenderer)this._targetRenderer).sharedMesh = this._mesh;
				}
				if (triangles || vertices)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("recalculating bounds on mesh.");
					}
					this._mesh.RecalculateBounds();
				}
				if (this._optimizeAfterBake && !Application.isPlaying)
				{
					MBVersion.OptimizeMesh(this._mesh);
					return;
				}
			}
			else
			{
				Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
			}
		}

		private int _numNonZeroLengthSubmeshTris(MB3_MeshCombinerSingle.SerializableIntArray[] subTris)
		{
			int num = 0;
			for (int i = 0; i < subTris.Length; i++)
			{
				if (subTris[i].data.Length != 0)
				{
					num++;
				}
			}
			return num;
		}

		private void _updateMaterialsOnTargetRenderer(MB3_MeshCombinerSingle.SerializableIntArray[] subTris, int numNonZeroLengthSubmeshTris)
		{
			if (subTris.Length != this.textureBakeResults.resultMaterials.Length)
			{
				Debug.LogError("Mismatch between number of submeshes and number of result materials");
			}
			Material[] array = new Material[numNonZeroLengthSubmeshTris];
			int num = 0;
			for (int i = 0; i < subTris.Length; i++)
			{
				if (subTris[i].data.Length != 0)
				{
					array[num] = this._textureBakeResults.resultMaterials[i].combinedMaterial;
					num++;
				}
			}
			this.targetRenderer.materials = array;
		}

		public MB3_MeshCombinerSingle.SerializableIntArray[] GetSubmeshTrisWithShowHideApplied()
		{
			bool flag = false;
			for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
			{
				if (!this.mbDynamicObjectsInCombinedMesh[i].show)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				int[] array = new int[this.submeshTris.Length];
				MB3_MeshCombinerSingle.SerializableIntArray[] array2 = new MB3_MeshCombinerSingle.SerializableIntArray[this.submeshTris.Length];
				for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[j];
					if (mb_DynamicGameObject.show)
					{
						for (int k = 0; k < mb_DynamicGameObject.submeshNumTris.Length; k++)
						{
							array[k] += mb_DynamicGameObject.submeshNumTris[k];
						}
					}
				}
				for (int l = 0; l < array2.Length; l++)
				{
					array2[l] = new MB3_MeshCombinerSingle.SerializableIntArray(array[l]);
				}
				int[] array3 = new int[array2.Length];
				for (int m = 0; m < this.mbDynamicObjectsInCombinedMesh.Count; m++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject2 = this.mbDynamicObjectsInCombinedMesh[m];
					if (mb_DynamicGameObject2.show)
					{
						for (int n = 0; n < this.submeshTris.Length; n++)
						{
							int[] data = this.submeshTris[n].data;
							int num = mb_DynamicGameObject2.submeshTriIdxs[n];
							int num2 = num + mb_DynamicGameObject2.submeshNumTris[n];
							for (int num3 = num; num3 < num2; num3++)
							{
								array2[n].data[array3[n]] = data[num3];
								array3[n]++;
							}
						}
					}
				}
				return array2;
			}
			return this.submeshTris;
		}

		public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false)
		{
			this._updateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
		}

		private void _updateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("UpdateGameObjects called on " + gos.Length + " objects.");
			}
			int numResultMats = 1;
			if (this.textureBakeResults.doMultiMaterial)
			{
				numResultMats = this.textureBakeResults.resultMaterials.Length;
			}
			this._initialize(numResultMats);
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache = new MB3_MeshCombinerSingle.MeshChannelsCache(this);
			for (int i = 0; i < gos.Length; i++)
			{
				this._updateGameObject(gos[i], updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo, meshChannelCache);
			}
			if (recalcBounds)
			{
				this._mesh.RecalculateBounds();
			}
		}

		private void _updateGameObject(GameObject go, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache)
		{
			MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = null;
			if (!this.instance2Combined_MapTryGetValue(go.GetInstanceID(), out mb_DynamicGameObject))
			{
				Debug.LogError("Object " + go.name + " has not been added");
				return;
			}
			Mesh mesh = MB_Utility.GetMesh(go);
			if (mb_DynamicGameObject.numVerts != mesh.vertexCount)
			{
				Debug.LogError("Object " + go.name + " source mesh has been modified since being added. To update it must have the same number of verts");
				return;
			}
			if (this._doUV && updateUV)
			{
				this._copyAndAdjustUVsFromMesh(mb_DynamicGameObject, mesh, mb_DynamicGameObject.vertIdx, meshChannelCache);
			}
			if (this.doUV2() && updateUV2)
			{
				this._copyAndAdjustUV2FromMesh(mb_DynamicGameObject, mesh, mb_DynamicGameObject.vertIdx, meshChannelCache);
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer && updateSkinningInfo)
			{
				Renderer renderer = MB_Utility.GetRenderer(go);
				BoneWeight[] array = meshChannelCache.GetBoneWeights(renderer, mb_DynamicGameObject.numVerts);
				Transform[] array2 = this._getBones(renderer);
				int num = mb_DynamicGameObject.vertIdx;
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (array2[array[i].boneIndex0] != this.bones[this.boneWeights[num].boneIndex0])
					{
						flag = true;
						break;
					}
					this.boneWeights[num].weight0 = array[i].weight0;
					this.boneWeights[num].weight1 = array[i].weight1;
					this.boneWeights[num].weight2 = array[i].weight2;
					this.boneWeights[num].weight3 = array[i].weight3;
					num++;
				}
				if (flag)
				{
					Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + mb_DynamicGameObject.name);
				}
			}
			Matrix4x4 localToWorldMatrix = go.transform.localToWorldMatrix;
			if (updateVertices)
			{
				Vector3[] vertices = meshChannelCache.GetVertices(mesh);
				for (int j = 0; j < vertices.Length; j++)
				{
					this.verts[mb_DynamicGameObject.vertIdx + j] = localToWorldMatrix.MultiplyPoint3x4(vertices[j]);
				}
			}
			localToWorldMatrix[0, 3] = (localToWorldMatrix[1, 3] = (localToWorldMatrix[2, 3] = 0f));
			if (this._doNorm && updateNormals)
			{
				Vector3[] array3 = meshChannelCache.GetNormals(mesh);
				for (int k = 0; k < array3.Length; k++)
				{
					int num2 = mb_DynamicGameObject.vertIdx + k;
					this.normals[num2] = localToWorldMatrix.MultiplyPoint3x4(array3[k]);
					this.normals[num2] = this.normals[num2].normalized;
				}
			}
			if (this._doTan && updateTangents)
			{
				Vector4[] array4 = meshChannelCache.GetTangents(mesh);
				for (int l = 0; l < array4.Length; l++)
				{
					int num3 = mb_DynamicGameObject.vertIdx + l;
					float w = array4[l].w;
					Vector3 v = localToWorldMatrix.MultiplyPoint3x4(array4[l]);
					v.Normalize();
					this.tangents[num3] = v;
					this.tangents[num3].w = w;
				}
			}
			if (this._doCol && updateColors)
			{
				Color[] array5 = meshChannelCache.GetColors(mesh);
				for (int m = 0; m < array5.Length; m++)
				{
					this.colors[mb_DynamicGameObject.vertIdx + m] = array5[m];
				}
			}
			if (this._doUV3 && updateUV3)
			{
				Vector2[] uv = meshChannelCache.GetUv3(mesh);
				for (int n = 0; n < uv.Length; n++)
				{
					this.uv3s[mb_DynamicGameObject.vertIdx + n] = uv[n];
				}
			}
			if (this._doUV4 && updateUV4)
			{
				Vector2[] uv2 = meshChannelCache.GetUv4(mesh);
				for (int num4 = 0; num4 < uv2.Length; num4++)
				{
					this.uv4s[mb_DynamicGameObject.vertIdx + num4] = uv2[num4];
				}
			}
		}

		public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide)
		{
			if (this.textureBakeResults == null)
			{
				Debug.LogError("TextureBakeResults must be set.");
				return false;
			}
			return this._showHide(toShow, toHide);
		}

		public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
		{
			int[] array = null;
			if (deleteGOs != null)
			{
				array = new int[deleteGOs.Length];
				for (int i = 0; i < deleteGOs.Length; i++)
				{
					if (deleteGOs[i] == null)
					{
						Debug.LogError("The " + i + "th object on the list of objects to delete is 'Null'");
					}
					else
					{
						array[i] = deleteGOs[i].GetInstanceID();
					}
				}
			}
			return this.AddDeleteGameObjectsByID(gos, array, disableRendererInSource);
		}

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
		{
			if (this.validationLevel > MB2_ValidationLevel.none)
			{
				if (gos != null)
				{
					for (int i = 0; i < gos.Length; i++)
					{
						if (gos[i] == null)
						{
							Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
							return false;
						}
						if (this.validationLevel >= MB2_ValidationLevel.robust)
						{
							for (int j = i + 1; j < gos.Length; j++)
							{
								if (gos[i] == gos[j])
								{
									Debug.LogError("GameObject " + gos[i] + " appears twice in list of game objects to add");
									return false;
								}
							}
						}
					}
				}
				if (deleteGOinstanceIDs != null && this.validationLevel >= MB2_ValidationLevel.robust)
				{
					for (int k = 0; k < deleteGOinstanceIDs.Length; k++)
					{
						for (int l = k + 1; l < deleteGOinstanceIDs.Length; l++)
						{
							if (deleteGOinstanceIDs[k] == deleteGOinstanceIDs[l])
							{
								Debug.LogError("GameObject " + deleteGOinstanceIDs[k] + "appears twice in list of game objects to delete");
								return false;
							}
						}
					}
				}
			}
			if (this._usingTemporaryTextureBakeResult && gos != null && gos.Length != 0)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			if (this._textureBakeResults == null && gos != null && gos.Length != 0 && gos[0] != null && !this._CreateTemporaryTextrueBakeResult(gos, this.GetMaterialsOnTargetRenderer()))
			{
				return false;
			}
			this.BuildSceneMeshObject(gos, false);
			if (!this._addToCombined(gos, deleteGOinstanceIDs, disableRendererInSource))
			{
				Debug.LogError("Failed to add/delete objects to combined mesh");
				return false;
			}
			if (this.targetRenderer != null)
			{
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					((SkinnedMeshRenderer)this.targetRenderer).bones = this.bones;
					this.UpdateSkinnedMeshApproximateBoundsFromBounds();
				}
				this.targetRenderer.lightmapIndex = this.GetLightmapIndex();
			}
			return true;
		}

		public override bool CombinedMeshContains(GameObject go)
		{
			return this.objectsInCombinedMesh.Contains(go);
		}

		public override void ClearBuffers()
		{
			this.verts = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector4[0];
			this.uvs = new Vector2[0];
			this.uv2s = new Vector2[0];
			this.uv3s = new Vector2[0];
			this.uv4s = new Vector2[0];
			this.colors = new Color[0];
			this.bones = new Transform[0];
			this.bindPoses = new Matrix4x4[0];
			this.boneWeights = new BoneWeight[0];
			this.submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[0];
			this.blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[0];
			if (this.blendShapesInCombined == null)
			{
				this.blendShapesInCombined = new MB3_MeshCombinerSingle.MBBlendShape[0];
			}
			else
			{
				for (int i = 0; i < this.blendShapesInCombined.Length; i++)
				{
					this.blendShapesInCombined[i].frames = new MB3_MeshCombinerSingle.MBBlendShapeFrame[0];
				}
			}
			this.mbDynamicObjectsInCombinedMesh.Clear();
			this.objectsInCombinedMesh.Clear();
			this.instance2Combined_MapClear();
			if (this._usingTemporaryTextureBakeResult)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.LogDebug("ClearBuffers called", Array.Empty<object>());
			}
		}

		public override void ClearMesh()
		{
			if (this._mesh != null)
			{
				MBVersion.MeshClear(this._mesh, false);
			}
			else
			{
				this._mesh = new Mesh();
			}
			this.ClearBuffers();
		}

		public override void DestroyMesh()
		{
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", Array.Empty<object>());
				}
				MB_Utility.Destroy(this._mesh);
			}
			this._mesh = new Mesh();
			this.ClearBuffers();
		}

		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
		{
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", Array.Empty<object>());
				}
				editorMethods.Destroy(this._mesh);
			}
			this._mesh = new Mesh();
			this.ClearBuffers();
		}

		public bool ValidateTargRendererAndMeshAndResultSceneObj()
		{
			if (this._resultSceneObject == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Result Scene Object was not set.");
				}
				return false;
			}
			if (this._targetRenderer == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer was not set.");
				}
				return false;
			}
			if (this._targetRenderer.transform.parent != this._resultSceneObject.transform)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer game object is not a child of Result Scene Object was not set.");
				}
				return false;
			}
			if (this._renderType == MB_RenderType.skinnedMeshRenderer)
			{
				if (!(this._targetRenderer is SkinnedMeshRenderer))
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Render Type is skinned mesh renderer but Target Renderer is not.");
					}
					return false;
				}
				if (((SkinnedMeshRenderer)this._targetRenderer).sharedMesh != this._mesh)
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Target renderer mesh is not equal to mesh.");
					}
					return false;
				}
			}
			if (this._renderType == MB_RenderType.meshRenderer)
			{
				if (!(this._targetRenderer is MeshRenderer))
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Render Type is mesh renderer but Target Renderer is not.");
					}
					return false;
				}
				MeshFilter component = this._targetRenderer.GetComponent<MeshFilter>();
				if (this._mesh != component.sharedMesh)
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Target renderer mesh is not equal to mesh.");
					}
					return false;
				}
			}
			return true;
		}

		internal static Renderer BuildSceneHierarchPreBake(MB3_MeshCombinerSingle mom, GameObject root, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
		{
			if (mom._LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("Building Scene Hierarchy createNewChild=" + createNewChild.ToString());
			}
			MeshFilter meshFilter = null;
			MeshRenderer meshRenderer = null;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			Transform transform = null;
			if (root == null)
			{
				Debug.LogError("root was null.");
				return null;
			}
			if (mom.textureBakeResults == null)
			{
				Debug.LogError("textureBakeResults must be set.");
				return null;
			}
			if (root.GetComponent<Renderer>() != null)
			{
				Debug.LogError("root game object cannot have a renderer component");
				return null;
			}
			if (!createNewChild)
			{
				if (mom.targetRenderer != null && mom.targetRenderer.transform.parent == root.transform)
				{
					transform = mom.targetRenderer.transform;
				}
				else
				{
					Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
					if (componentsInChildren.Length == 1)
					{
						if (componentsInChildren[0].transform.parent != root.transform)
						{
							Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
						}
						transform = componentsInChildren[0].transform;
					}
				}
			}
			if (transform != null && transform.parent != root.transform)
			{
				transform = null;
			}
			if (transform == null)
			{
				transform = new GameObject(mom.name + "-mesh")
				{
					transform = 
					{
						parent = root.transform
					}
				}.transform;
			}
			transform.parent = root.transform;
			GameObject gameObject = transform.gameObject;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					MB_Utility.Destroy(component);
				}
				MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					MB_Utility.Destroy(component2);
				}
				skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer == null)
				{
					skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			else
			{
				SkinnedMeshRenderer component3 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component3 != null)
				{
					MB_Utility.Destroy(component3);
				}
				meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.bones = mom.GetBones();
				bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				skinnedMeshRenderer.updateWhenOffscreen = true;
				skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
			}
			MB3_MeshCombinerSingle._ConfigureSceneHierarch(mom, root, meshRenderer, meshFilter, skinnedMeshRenderer, m, objsToBeAdded);
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				return skinnedMeshRenderer;
			}
			return meshRenderer;
		}

		public static void BuildPrefabHierarchy(MB3_MeshCombinerSingle mom, GameObject instantiatedPrefabRoot, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			MeshRenderer meshRenderer = null;
			MeshFilter meshFilter = null;
			Transform transform = new GameObject(mom.name + "-mesh")
			{
				transform = 
				{
					parent = instantiatedPrefabRoot.transform
				}
			}.transform;
			transform.parent = instantiatedPrefabRoot.transform;
			GameObject gameObject = transform.gameObject;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					MB_Utility.Destroy(component);
				}
				MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					MB_Utility.Destroy(component2);
				}
				skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer == null)
				{
					skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			else
			{
				SkinnedMeshRenderer component3 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component3 != null)
				{
					MB_Utility.Destroy(component3);
				}
				meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.bones = mom.GetBones();
				bool updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				skinnedMeshRenderer.updateWhenOffscreen = true;
				skinnedMeshRenderer.updateWhenOffscreen = updateWhenOffscreen;
			}
			MB3_MeshCombinerSingle._ConfigureSceneHierarch(mom, instantiatedPrefabRoot, meshRenderer, meshFilter, skinnedMeshRenderer, m, objsToBeAdded);
			if (mom.targetRenderer != null)
			{
				Material[] array = new Material[mom.targetRenderer.sharedMaterials.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = mom.targetRenderer.sharedMaterials[i];
				}
				if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					skinnedMeshRenderer.sharedMaterial = null;
					skinnedMeshRenderer.sharedMaterials = array;
					return;
				}
				meshRenderer.sharedMaterial = null;
				meshRenderer.sharedMaterials = array;
			}
		}

		private static void _ConfigureSceneHierarch(MB3_MeshCombinerSingle mom, GameObject root, MeshRenderer mr, MeshFilter mf, SkinnedMeshRenderer smr, Mesh m, GameObject[] objsToBeAdded = null)
		{
			GameObject gameObject;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				gameObject = smr.gameObject;
				smr.sharedMesh = m;
				smr.lightmapIndex = mom.GetLightmapIndex();
			}
			else
			{
				gameObject = mr.gameObject;
				mf.sharedMesh = m;
				mr.lightmapIndex = mom.GetLightmapIndex();
			}
			if (mom.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || mom.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
			{
				gameObject.isStatic = true;
			}
			if (objsToBeAdded != null && objsToBeAdded.Length != 0 && objsToBeAdded[0] != null)
			{
				bool flag = true;
				bool flag2 = true;
				string tag = objsToBeAdded[0].tag;
				int layer = objsToBeAdded[0].layer;
				for (int i = 0; i < objsToBeAdded.Length; i++)
				{
					if (objsToBeAdded[i] != null)
					{
						if (!objsToBeAdded[i].tag.Equals(tag))
						{
							flag = false;
						}
						if (objsToBeAdded[i].layer != layer)
						{
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					root.tag = tag;
					gameObject.tag = tag;
				}
				if (flag2)
				{
					root.layer = layer;
					gameObject.layer = layer;
				}
			}
		}

		public void BuildSceneMeshObject(GameObject[] gos = null, bool createNewChild = false)
		{
			if (this._resultSceneObject == null)
			{
				this._resultSceneObject = new GameObject("CombinedMesh-" + base.name);
			}
			this._targetRenderer = MB3_MeshCombinerSingle.BuildSceneHierarchPreBake(this, this._resultSceneObject, this.GetMesh(), createNewChild, gos);
		}

		private bool IsMirrored(Matrix4x4 tm)
		{
			Vector3 lhs = tm.GetRow(0);
			Vector3 rhs = tm.GetRow(1);
			Vector3 rhs2 = tm.GetRow(2);
			lhs.Normalize();
			rhs.Normalize();
			rhs2.Normalize();
			return Vector3.Dot(Vector3.Cross(lhs, rhs), rhs2) < 0f;
		}

		public override void CheckIntegrity()
		{
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[i];
					HashSet<int> hashSet = new HashSet<int>();
					HashSet<int> hashSet2 = new HashSet<int>();
					for (int j = mb_DynamicGameObject.vertIdx; j < mb_DynamicGameObject.vertIdx + mb_DynamicGameObject.numVerts; j++)
					{
						hashSet.Add(this.boneWeights[j].boneIndex0);
						hashSet.Add(this.boneWeights[j].boneIndex1);
						hashSet.Add(this.boneWeights[j].boneIndex2);
						hashSet.Add(this.boneWeights[j].boneIndex3);
					}
					for (int k = 0; k < mb_DynamicGameObject.indexesOfBonesUsed.Length; k++)
					{
						hashSet2.Add(mb_DynamicGameObject.indexesOfBonesUsed[k]);
					}
					hashSet2.ExceptWith(hashSet);
					if (hashSet2.Count > 0)
					{
						Debug.LogError(string.Concat(new object[]
						{
							"The bone indexes were not the same. ",
							hashSet.Count,
							" ",
							hashSet2.Count
						}));
					}
					for (int l = 0; l < mb_DynamicGameObject.indexesOfBonesUsed.Length; l++)
					{
						if (l < 0 || l > this.bones.Length)
						{
							Debug.LogError("Bone index was out of bounds.");
						}
					}
					if (this.renderType == MB_RenderType.skinnedMeshRenderer && mb_DynamicGameObject.indexesOfBonesUsed.Length < 1)
					{
						Debug.Log("DGO had no bones");
					}
				}
			}
			if (this.doBlendShapes && this.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				Debug.LogError("Blend shapes can only be used with skinned meshes.");
			}
		}

		private void _ZeroArray(Vector3[] arr, int idx, int length)
		{
			int num = idx + length;
			for (int i = idx; i < num; i++)
			{
				arr[i] = Vector3.zero;
			}
		}

		private List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] _buildBoneIdx2dgoMap()
		{
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] array = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[this.bones.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			}
			for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[j];
				for (int k = 0; k < mb_DynamicGameObject.indexesOfBonesUsed.Length; k++)
				{
					array[mb_DynamicGameObject.indexesOfBonesUsed[k]].Add(mb_DynamicGameObject);
				}
			}
			return array;
		}

		private void _CollectBonesToAddForDGO(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Dictionary<Transform, int> bone2idx, HashSet<int> boneIdxsToDelete, HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAdd, Renderer r, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache)
		{
			Matrix4x4[] array = dgo._tmpCachedBindposes = meshChannelCache.GetBindposes(r);
			BoneWeight[] array2 = dgo._tmpCachedBoneWeights = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
			Transform[] array3 = dgo._tmpCachedBones = this._getBones(r);
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < array2.Length; i++)
			{
				hashSet.Add(array2[i].boneIndex0);
				hashSet.Add(array2[i].boneIndex1);
				hashSet.Add(array2[i].boneIndex2);
				hashSet.Add(array2[i].boneIndex3);
			}
			int[] array4 = new int[hashSet.Count];
			hashSet.CopyTo(array4);
			for (int j = 0; j < array4.Length; j++)
			{
				bool flag = false;
				int num = array4[j];
				int num2;
				if (bone2idx.TryGetValue(array3[num], out num2) && array3[num] == this.bones[num2] && !boneIdxsToDelete.Contains(num2) && array[num] == this.bindPoses[num2])
				{
					flag = true;
				}
				if (!flag)
				{
					MB3_MeshCombinerSingle.BoneAndBindpose item = new MB3_MeshCombinerSingle.BoneAndBindpose(array3[num], array[num]);
					if (!bonesToAdd.Contains(item))
					{
						bonesToAdd.Add(item);
					}
				}
			}
			dgo._tmpIndexesOfSourceBonesUsed = array4;
		}

		private void _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(HashSet<int> boneIdxsToDeleteHS, HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAdd, Transform[] nbones, Matrix4x4[] nbindPoses, BoneWeight[] nboneWeights, int totalDeleteVerts)
		{
			if (boneIdxsToDeleteHS.Count > 0)
			{
				int[] array = new int[boneIdxsToDeleteHS.Count];
				boneIdxsToDeleteHS.CopyTo(array);
				Array.Sort<int>(array);
				int[] array2 = new int[this.bones.Length];
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this.bones.Length; i++)
				{
					if (num2 < array.Length && array[num2] == i)
					{
						num2++;
						array2[i] = -1;
					}
					else
					{
						array2[i] = num;
						nbones[num] = this.bones[i];
						nbindPoses[num] = this.bindPoses[i];
						num++;
					}
				}
				int num3 = this.boneWeights.Length - totalDeleteVerts;
				for (int j = 0; j < num3; j++)
				{
					nboneWeights[j].boneIndex0 = array2[nboneWeights[j].boneIndex0];
					nboneWeights[j].boneIndex1 = array2[nboneWeights[j].boneIndex1];
					nboneWeights[j].boneIndex2 = array2[nboneWeights[j].boneIndex2];
					nboneWeights[j].boneIndex3 = array2[nboneWeights[j].boneIndex3];
				}
				for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[k];
					for (int l = 0; l < mb_DynamicGameObject.indexesOfBonesUsed.Length; l++)
					{
						mb_DynamicGameObject.indexesOfBonesUsed[l] = array2[mb_DynamicGameObject.indexesOfBonesUsed[l]];
					}
				}
				return;
			}
			Array.Copy(this.bones, nbones, this.bones.Length);
			Array.Copy(this.bindPoses, nbindPoses, this.bindPoses.Length);
		}

		private void _AddBonesToNewBonesArrayAndAdjustBWIndexes(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Renderer r, int vertsIdx, Transform[] nbones, BoneWeight[] nboneWeights, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache)
		{
			Transform[] tmpCachedBones = dgo._tmpCachedBones;
			Matrix4x4[] tmpCachedBindposes = dgo._tmpCachedBindposes;
			BoneWeight[] tmpCachedBoneWeights = dgo._tmpCachedBoneWeights;
			int[] array = new int[tmpCachedBones.Length];
			for (int i = 0; i < dgo._tmpIndexesOfSourceBonesUsed.Length; i++)
			{
				int num = dgo._tmpIndexesOfSourceBonesUsed[i];
				for (int j = 0; j < nbones.Length; j++)
				{
					if (tmpCachedBones[num] == nbones[j] && tmpCachedBindposes[num] == this.bindPoses[j])
					{
						array[num] = j;
						break;
					}
				}
			}
			for (int k = 0; k < tmpCachedBoneWeights.Length; k++)
			{
				int num2 = vertsIdx + k;
				nboneWeights[num2].boneIndex0 = array[tmpCachedBoneWeights[k].boneIndex0];
				nboneWeights[num2].boneIndex1 = array[tmpCachedBoneWeights[k].boneIndex1];
				nboneWeights[num2].boneIndex2 = array[tmpCachedBoneWeights[k].boneIndex2];
				nboneWeights[num2].boneIndex3 = array[tmpCachedBoneWeights[k].boneIndex3];
				nboneWeights[num2].weight0 = tmpCachedBoneWeights[k].weight0;
				nboneWeights[num2].weight1 = tmpCachedBoneWeights[k].weight1;
				nboneWeights[num2].weight2 = tmpCachedBoneWeights[k].weight2;
				nboneWeights[num2].weight3 = tmpCachedBoneWeights[k].weight3;
			}
			for (int l = 0; l < dgo._tmpIndexesOfSourceBonesUsed.Length; l++)
			{
				dgo._tmpIndexesOfSourceBonesUsed[l] = array[dgo._tmpIndexesOfSourceBonesUsed[l]];
			}
			dgo.indexesOfBonesUsed = dgo._tmpIndexesOfSourceBonesUsed;
			dgo._tmpIndexesOfSourceBonesUsed = null;
			dgo._tmpCachedBones = null;
			dgo._tmpCachedBindposes = null;
			dgo._tmpCachedBoneWeights = null;
		}

		private void _copyUV2unchangedToSeparateRects()
		{
			int padding = 16;
			List<Vector2> list = new List<Vector2>();
			float num = 1E+11f;
			float num2 = 0f;
			for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
			{
				float magnitude = this.mbDynamicObjectsInCombinedMesh[i].meshSize.magnitude;
				if (magnitude > num2)
				{
					num2 = magnitude;
				}
				if (magnitude < num)
				{
					num = magnitude;
				}
			}
			float num3 = 1000f;
			float num4 = 10f;
			float num5 = 0f;
			float num6;
			if (num2 - num > num3 - num4)
			{
				num6 = (num3 - num4) / (num2 - num);
				num5 = num4 - num * num6;
			}
			else
			{
				num6 = num3 / num2;
			}
			for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
			{
				float num7 = this.mbDynamicObjectsInCombinedMesh[j].meshSize.magnitude;
				num7 = num7 * num6 + num5;
				Vector2 item = Vector2.one * num7;
				list.Add(item);
			}
			AtlasPackingResult[] rects = new MB2_TexturePacker
			{
				doPowerOfTwoTextures = false
			}.GetRects(list, 8192, padding);
			for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mb_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[k];
				float x;
				float num8 = x = this.uv2s[mb_DynamicGameObject.vertIdx].x;
				float y;
				float num9 = y = this.uv2s[mb_DynamicGameObject.vertIdx].y;
				int num10 = mb_DynamicGameObject.vertIdx + mb_DynamicGameObject.numVerts;
				for (int l = mb_DynamicGameObject.vertIdx; l < num10; l++)
				{
					if (this.uv2s[l].x < x)
					{
						x = this.uv2s[l].x;
					}
					if (this.uv2s[l].x > num8)
					{
						num8 = this.uv2s[l].x;
					}
					if (this.uv2s[l].y < y)
					{
						y = this.uv2s[l].y;
					}
					if (this.uv2s[l].y > num9)
					{
						num9 = this.uv2s[l].y;
					}
				}
				Rect rect = rects[0].rects[k];
				for (int m = mb_DynamicGameObject.vertIdx; m < num10; m++)
				{
					float num11 = num8 - x;
					float num12 = num9 - y;
					if (num11 == 0f)
					{
						num11 = 1f;
					}
					if (num12 == 0f)
					{
						num12 = 1f;
					}
					this.uv2s[m].x = (this.uv2s[m].x - x) / num11 * rect.width + rect.x;
					this.uv2s[m].y = (this.uv2s[m].y - y) / num12 * rect.height + rect.y;
				}
			}
		}

		public override List<Material> GetMaterialsOnTargetRenderer()
		{
			List<Material> list = new List<Material>();
			if (this._targetRenderer != null)
			{
				list.AddRange(this._targetRenderer.sharedMaterials);
			}
			return list;
		}

		[SerializeField]
		protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();

		[SerializeField]
		private int lightmapIndex = -1;

		[SerializeField]
		private List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		private Dictionary<int, MB3_MeshCombinerSingle.MB_DynamicGameObject> _instance2combined_map = new Dictionary<int, MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		[SerializeField]
		private Vector3[] verts = new Vector3[0];

		[SerializeField]
		private Vector3[] normals = new Vector3[0];

		[SerializeField]
		private Vector4[] tangents = new Vector4[0];

		[SerializeField]
		private Vector2[] uvs = new Vector2[0];

		[SerializeField]
		private Vector2[] uv2s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv3s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv4s = new Vector2[0];

		[SerializeField]
		private Color[] colors = new Color[0];

		[SerializeField]
		private Matrix4x4[] bindPoses = new Matrix4x4[0];

		[SerializeField]
		private Transform[] bones = new Transform[0];

		[SerializeField]
		internal MB3_MeshCombinerSingle.MBBlendShape[] blendShapes = new MB3_MeshCombinerSingle.MBBlendShape[0];

		[SerializeField]
		internal MB3_MeshCombinerSingle.MBBlendShape[] blendShapesInCombined = new MB3_MeshCombinerSingle.MBBlendShape[0];

		[SerializeField]
		private MB3_MeshCombinerSingle.SerializableIntArray[] submeshTris = new MB3_MeshCombinerSingle.SerializableIntArray[0];

		[SerializeField]
		private Mesh _mesh;

		private BoneWeight[] boneWeights = new BoneWeight[0];

		private GameObject[] empty = new GameObject[0];

		private int[] emptyIDs = new int[0];

		[Serializable]
		public class SerializableIntArray
		{
			public SerializableIntArray()
			{
			}

			public SerializableIntArray(int len)
			{
				this.data = new int[len];
			}

			public int[] data;
		}

		[Serializable]
		public class MB_DynamicGameObject : IComparable<MB3_MeshCombinerSingle.MB_DynamicGameObject>
		{
			public int CompareTo(MB3_MeshCombinerSingle.MB_DynamicGameObject b)
			{
				return this.vertIdx - b.vertIdx;
			}

			public int instanceID;

			public string name;

			public int vertIdx;

			public int blendShapeIdx;

			public int numVerts;

			public int numBlendShapes;

			public int[] indexesOfBonesUsed = new int[0];

			public int lightmapIndex = -1;

			public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

			public Vector3 meshSize = Vector3.one;

			public bool show = true;

			public bool invertTriangles;

			public int[] submeshTriIdxs;

			public int[] submeshNumTris;

			public int[] targetSubmeshIdxs;

			public Rect[] uvRects;

			public Rect[] encapsulatingRect;

			public Rect[] sourceMaterialTiling;

			public Rect[] obUVRects;

			public bool _beingDeleted;

			public int _triangleIdxAdjustment;

			[NonSerialized]
			public MB3_MeshCombinerSingle.SerializableIntArray[] _tmpSubmeshTris;

			[NonSerialized]
			public Transform[] _tmpCachedBones;

			[NonSerialized]
			public Matrix4x4[] _tmpCachedBindposes;

			[NonSerialized]
			public BoneWeight[] _tmpCachedBoneWeights;

			[NonSerialized]
			public int[] _tmpIndexesOfSourceBonesUsed;
		}

		public class MeshChannels
		{
			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector4[] tangents;

			public Vector2[] uv0raw;

			public Vector2[] uv0modified;

			public Vector2[] uv2;

			public Vector2[] uv3;

			public Vector2[] uv4;

			public Color[] colors;

			public BoneWeight[] boneWeights;

			public Matrix4x4[] bindPoses;

			public int[] triangles;

			public MB3_MeshCombinerSingle.MBBlendShape[] blendShapes;
		}

		[Serializable]
		public class MBBlendShapeFrame
		{
			public float frameWeight;

			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector3[] tangents;
		}

		[Serializable]
		public class MBBlendShape
		{
			public int gameObjectID;

			public string name;

			public int indexInSource;

			public MB3_MeshCombinerSingle.MBBlendShapeFrame[] frames;
		}

		public class MeshChannelsCache
		{
			internal MeshChannelsCache(MB3_MeshCombinerSingle mcs)
			{
				this.mc = mcs;
			}

			internal Vector3[] GetVertices(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.vertices == null)
				{
					meshChannels.vertices = m.vertices;
				}
				return meshChannels.vertices;
			}

			internal Vector3[] GetNormals(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.normals == null)
				{
					meshChannels.normals = this._getMeshNormals(m);
				}
				return meshChannels.normals;
			}

			internal Vector4[] GetTangents(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.tangents == null)
				{
					meshChannels.tangents = this._getMeshTangents(m);
				}
				return meshChannels.tangents;
			}

			internal Vector2[] GetUv0Raw(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.uv0raw == null)
				{
					meshChannels.uv0raw = this._getMeshUVs(m);
				}
				return meshChannels.uv0raw;
			}

			internal Vector2[] GetUv0Modified(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.uv0modified == null)
				{
					meshChannels.uv0modified = null;
				}
				return meshChannels.uv0modified;
			}

			internal Vector2[] GetUv2(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.uv2 == null)
				{
					meshChannels.uv2 = this._getMeshUV2s(m);
				}
				return meshChannels.uv2;
			}

			internal Vector2[] GetUv3(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.uv3 == null)
				{
					meshChannels.uv3 = MBVersion.GetMeshUV3orUV4(m, true, this.mc.LOG_LEVEL);
				}
				return meshChannels.uv3;
			}

			internal Vector2[] GetUv4(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.uv4 == null)
				{
					meshChannels.uv4 = MBVersion.GetMeshUV3orUV4(m, false, this.mc.LOG_LEVEL);
				}
				return meshChannels.uv4;
			}

			internal Color[] GetColors(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.colors == null)
				{
					meshChannels.colors = this._getMeshColors(m);
				}
				return meshChannels.colors;
			}

			internal Matrix4x4[] GetBindposes(Renderer r)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(mesh.GetInstanceID(), meshChannels);
				}
				if (meshChannels.bindPoses == null)
				{
					meshChannels.bindPoses = MB3_MeshCombinerSingle.MeshChannelsCache._getBindPoses(r);
				}
				return meshChannels.bindPoses;
			}

			internal BoneWeight[] GetBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
			{
				Mesh mesh = MB_Utility.GetMesh(r.gameObject);
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(mesh.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(mesh.GetInstanceID(), meshChannels);
				}
				if (meshChannels.boneWeights == null)
				{
					meshChannels.boneWeights = MB3_MeshCombinerSingle.MeshChannelsCache._getBoneWeights(r, numVertsInMeshBeingAdded);
				}
				return meshChannels.boneWeights;
			}

			internal int[] GetTriangles(Mesh m)
			{
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.triangles == null)
				{
					meshChannels.triangles = m.triangles;
				}
				return meshChannels.triangles;
			}

			internal MB3_MeshCombinerSingle.MBBlendShape[] GetBlendShapes(Mesh m, int gameObjectID)
			{
				if (MBVersion.GetMajorVersion() <= 5 && (MBVersion.GetMajorVersion() != 5 || MBVersion.GetMinorVersion() < 3))
				{
					return new MB3_MeshCombinerSingle.MBBlendShape[0];
				}
				MB3_MeshCombinerSingle.MeshChannels meshChannels;
				if (!this.meshID2MeshChannels.TryGetValue(m.GetInstanceID(), out meshChannels))
				{
					meshChannels = new MB3_MeshCombinerSingle.MeshChannels();
					this.meshID2MeshChannels.Add(m.GetInstanceID(), meshChannels);
				}
				if (meshChannels.blendShapes == null)
				{
					MB3_MeshCombinerSingle.MBBlendShape[] array = new MB3_MeshCombinerSingle.MBBlendShape[m.blendShapeCount];
					int vertexCount = m.vertexCount;
					for (int i = 0; i < array.Length; i++)
					{
						MB3_MeshCombinerSingle.MBBlendShape mbblendShape = array[i] = new MB3_MeshCombinerSingle.MBBlendShape();
						mbblendShape.frames = new MB3_MeshCombinerSingle.MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(m, i)];
						mbblendShape.name = m.GetBlendShapeName(i);
						mbblendShape.indexInSource = i;
						mbblendShape.gameObjectID = gameObjectID;
						for (int j = 0; j < mbblendShape.frames.Length; j++)
						{
							MB3_MeshCombinerSingle.MBBlendShapeFrame mbblendShapeFrame = mbblendShape.frames[j] = new MB3_MeshCombinerSingle.MBBlendShapeFrame();
							mbblendShapeFrame.frameWeight = MBVersion.GetBlendShapeFrameWeight(m, i, j);
							mbblendShapeFrame.vertices = new Vector3[vertexCount];
							mbblendShapeFrame.normals = new Vector3[vertexCount];
							mbblendShapeFrame.tangents = new Vector3[vertexCount];
							MBVersion.GetBlendShapeFrameVertices(m, i, j, mbblendShapeFrame.vertices, mbblendShapeFrame.normals, mbblendShapeFrame.tangents);
						}
					}
					meshChannels.blendShapes = array;
					return meshChannels.blendShapes;
				}
				MB3_MeshCombinerSingle.MBBlendShape[] array2 = new MB3_MeshCombinerSingle.MBBlendShape[meshChannels.blendShapes.Length];
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = new MB3_MeshCombinerSingle.MBBlendShape();
					array2[k].name = meshChannels.blendShapes[k].name;
					array2[k].indexInSource = meshChannels.blendShapes[k].indexInSource;
					array2[k].frames = meshChannels.blendShapes[k].frames;
					array2[k].gameObjectID = gameObjectID;
				}
				return array2;
			}

			private Color[] _getMeshColors(Mesh m)
			{
				Color[] array = m.colors;
				if (array.Length == 0)
				{
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + m + " has no colors. Generating", Array.Empty<object>());
					}
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + m + " didn't have colors. Generating an array of white colors");
					}
					array = new Color[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Color.white;
					}
				}
				return array;
			}

			private Vector3[] _getMeshNormals(Mesh m)
			{
				Vector3[] normals = m.normals;
				if (normals.Length == 0)
				{
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + m + " has no normals. Generating", Array.Empty<object>());
					}
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + m + " didn't have normals. Generating normals.");
					}
					Mesh mesh = Object.Instantiate<Mesh>(m);
					mesh.RecalculateNormals();
					normals = mesh.normals;
					MB_Utility.Destroy(mesh);
				}
				return normals;
			}

			private Vector4[] _getMeshTangents(Mesh m)
			{
				Vector4[] array = m.tangents;
				if (array.Length == 0)
				{
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + m + " has no tangents. Generating", Array.Empty<object>());
					}
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + m + " didn't have tangents. Generating tangents.");
					}
					Vector3[] vertices = m.vertices;
					Vector2[] uv0Raw = this.GetUv0Raw(m);
					Vector3[] normals = this._getMeshNormals(m);
					array = new Vector4[m.vertexCount];
					for (int i = 0; i < m.subMeshCount; i++)
					{
						int[] triangles = m.GetTriangles(i);
						this._generateTangents(triangles, vertices, uv0Raw, normals, array);
					}
				}
				return array;
			}

			private Vector2[] _getMeshUVs(Mesh m)
			{
				Vector2[] array = m.uv;
				if (array.Length == 0)
				{
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + m + " has no uvs. Generating", Array.Empty<object>());
					}
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + m + " didn't have uvs. Generating uvs.");
					}
					array = new Vector2[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this._HALF_UV;
					}
				}
				return array;
			}

			private Vector2[] _getMeshUV2s(Mesh m)
			{
				Vector2[] array = m.uv2;
				if (array.Length == 0)
				{
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + m + " has no uv2s. Generating", Array.Empty<object>());
					}
					if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Mesh " + m + " didn't have uv2s. Generating uv2s.");
					}
					if (this.mc._lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects)
					{
						Debug.LogError("Mesh " + m + " did not have a UV2 channel. Nothing to copy when trying to copy UV2 to separate rects. The combined mesh will not lightmap properly. Try using generate new uv2 layout.");
					}
					array = new Vector2[m.vertexCount];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this._HALF_UV;
					}
				}
				return array;
			}

			public static Matrix4x4[] _getBindPoses(Renderer r)
			{
				if (r is SkinnedMeshRenderer)
				{
					return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
				}
				if (r is MeshRenderer)
				{
					Matrix4x4 identity = Matrix4x4.identity;
					return new Matrix4x4[]
					{
						identity
					};
				}
				Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
				return null;
			}

			public static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
			{
				if (r is SkinnedMeshRenderer)
				{
					return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
				}
				if (r is MeshRenderer)
				{
					BoneWeight boneWeight = default(BoneWeight);
					boneWeight.boneIndex0 = (boneWeight.boneIndex1 = (boneWeight.boneIndex2 = (boneWeight.boneIndex3 = 0)));
					boneWeight.weight0 = 1f;
					boneWeight.weight1 = (boneWeight.weight2 = (boneWeight.weight3 = 0f));
					BoneWeight[] array = new BoneWeight[numVertsInMeshBeingAdded];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = boneWeight;
					}
					return array;
				}
				Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
				return null;
			}

			private void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
			{
				int num = triangles.Length;
				int num2 = verts.Length;
				Vector3[] array = new Vector3[num2];
				Vector3[] array2 = new Vector3[num2];
				for (int i = 0; i < num; i += 3)
				{
					int num3 = triangles[i];
					int num4 = triangles[i + 1];
					int num5 = triangles[i + 2];
					Vector3 vector = verts[num3];
					Vector3 vector2 = verts[num4];
					Vector3 vector3 = verts[num5];
					Vector2 vector4 = uvs[num3];
					Vector2 vector5 = uvs[num4];
					Vector2 vector6 = uvs[num5];
					float num6 = vector2.x - vector.x;
					float num7 = vector3.x - vector.x;
					float num8 = vector2.y - vector.y;
					float num9 = vector3.y - vector.y;
					float num10 = vector2.z - vector.z;
					float num11 = vector3.z - vector.z;
					float num12 = vector5.x - vector4.x;
					float num13 = vector6.x - vector4.x;
					float num14 = vector5.y - vector4.y;
					float num15 = vector6.y - vector4.y;
					float num16 = num12 * num15 - num13 * num14;
					if (num16 == 0f)
					{
						Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
						return;
					}
					float num17 = 1f / num16;
					Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
					Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
					array[num3] += b;
					array[num4] += b;
					array[num5] += b;
					array2[num3] += b2;
					array2[num4] += b2;
					array2[num5] += b2;
				}
				for (int j = 0; j < num2; j++)
				{
					Vector3 vector7 = normals[j];
					Vector3 vector8 = array[j];
					Vector3 normalized = (vector8 - vector7 * Vector3.Dot(vector7, vector8)).normalized;
					outTangents[j] = new Vector4(normalized.x, normalized.y, normalized.z);
					outTangents[j].w = ((Vector3.Dot(Vector3.Cross(vector7, vector8), array2[j]) < 0f) ? -1f : 1f);
				}
			}

			private MB3_MeshCombinerSingle mc;

			protected Dictionary<int, MB3_MeshCombinerSingle.MeshChannels> meshID2MeshChannels = new Dictionary<int, MB3_MeshCombinerSingle.MeshChannels>();

			private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);
		}

		public struct BoneAndBindpose
		{
			public BoneAndBindpose(Transform t, Matrix4x4 bp)
			{
				this.bone = t;
				this.bindPose = bp;
			}

			public override bool Equals(object obj)
			{
				return obj is MB3_MeshCombinerSingle.BoneAndBindpose && this.bone == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bone && this.bindPose == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bindPose;
			}

			public override int GetHashCode()
			{
				return this.bone.GetInstanceID() % int.MaxValue ^ (int)this.bindPose[0, 0];
			}

			public Transform bone;

			public Matrix4x4 bindPose;
		}
	}
}

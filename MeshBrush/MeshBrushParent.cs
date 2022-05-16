using System;
using System.Collections;
using UnityEngine;

namespace MeshBrush
{
	public class MeshBrushParent : MonoBehaviour
	{
		private void Initialize()
		{
			this.paintedMeshes = base.GetComponentsInChildren<Transform>();
			this.meshFilters = base.GetComponentsInChildren<MeshFilter>();
		}

		public void FlagMeshesAsStatic()
		{
			this.Initialize();
			for (int i = this.paintedMeshes.Length - 1; i >= 0; i--)
			{
				this.paintedMeshes[i].gameObject.isStatic = true;
			}
		}

		public void UnflagMeshesAsStatic()
		{
			this.Initialize();
			for (int i = this.paintedMeshes.Length - 1; i >= 0; i--)
			{
				this.paintedMeshes[i].gameObject.isStatic = false;
			}
		}

		public int GetMeshCount()
		{
			this.Initialize();
			return this.meshFilters.Length;
		}

		public int GetTrisCount()
		{
			this.Initialize();
			if (this.meshFilters.Length != 0)
			{
				int num = 0;
				for (int i = this.meshFilters.Length - 1; i >= 0; i--)
				{
					num += this.meshFilters[i].sharedMesh.triangles.Length;
				}
				return num / 3;
			}
			return 0;
		}

		public void DeleteAllMeshes()
		{
			Object.DestroyImmediate(base.gameObject);
		}

		public void CombinePaintedMeshes(bool autoSelect, MeshFilter[] meshFilters)
		{
			if (meshFilters == null || meshFilters.Length == 0)
			{
				Debug.LogError("MeshBrush: The meshFilters array you passed as an argument to the CombinePaintedMeshes function is empty or null... Combining action cancelled!");
				return;
			}
			this.localTransformationMatrix = base.transform.worldToLocalMatrix;
			this.materialToMesh = new Hashtable();
			int num = 0;
			for (long num2 = 0L; num2 < (long)meshFilters.Length; num2 += 1L)
			{
				this.currentMeshFilter = meshFilters[(int)(checked((IntPtr)num2))];
				num += this.currentMeshFilter.sharedMesh.vertexCount;
				if (num > 64000)
				{
					return;
				}
			}
			for (long num3 = 0L; num3 < (long)meshFilters.Length; num3 += 1L)
			{
				checked
				{
					this.currentMeshFilter = meshFilters[(int)((IntPtr)num3)];
					this.currentRenderer = meshFilters[(int)((IntPtr)num3)].GetComponent<Renderer>();
					this.instance = default(CombineUtility.MeshInstance);
					this.instance.mesh = this.currentMeshFilter.sharedMesh;
				}
				if (this.currentRenderer != null && this.currentRenderer.enabled && this.instance.mesh != null)
				{
					this.instance.transform = this.localTransformationMatrix * this.currentMeshFilter.transform.localToWorldMatrix;
					this.materials = this.currentRenderer.sharedMaterials;
					for (int i = 0; i < this.materials.Length; i++)
					{
						this.instance.subMeshIndex = Math.Min(i, this.instance.mesh.subMeshCount - 1);
						this.objects = (ArrayList)this.materialToMesh[this.materials[i]];
						if (this.objects != null)
						{
							this.objects.Add(this.instance);
						}
						else
						{
							this.objects = new ArrayList();
							this.objects.Add(this.instance);
							this.materialToMesh.Add(this.materials[i], this.objects);
						}
					}
					Object.DestroyImmediate(this.currentRenderer.gameObject);
				}
			}
			foreach (object obj in this.materialToMesh)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				this.elements = (ArrayList)dictionaryEntry.Value;
				this.instances = (CombineUtility.MeshInstance[])this.elements.ToArray(typeof(CombineUtility.MeshInstance));
				GameObject gameObject = new GameObject("Combined mesh");
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent<MeshFilter>();
				gameObject.AddComponent<MeshRenderer>();
				gameObject.AddComponent<SaveCombinedMesh>();
				gameObject.GetComponent<Renderer>().material = (Material)dictionaryEntry.Key;
				gameObject.isStatic = true;
				this.currentMeshFilter = gameObject.GetComponent<MeshFilter>();
				this.currentMeshFilter.mesh = CombineUtility.Combine(this.instances, false);
			}
			base.gameObject.isStatic = true;
		}

		private Transform[] paintedMeshes;

		private MeshFilter[] meshFilters;

		private Matrix4x4 localTransformationMatrix;

		private Hashtable materialToMesh;

		private MeshFilter currentMeshFilter;

		private Renderer currentRenderer;

		private Material[] materials;

		private CombineUtility.MeshInstance instance;

		private CombineUtility.MeshInstance[] instances;

		private ArrayList objects;

		private ArrayList elements;
	}
}

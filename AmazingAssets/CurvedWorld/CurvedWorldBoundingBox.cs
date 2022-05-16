using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmazingAssets.CurvedWorld
{
	[ExecuteInEditMode]
	public class CurvedWorldBoundingBox : MonoBehaviour
	{
		private void OnEnable()
		{
			this.currentScale = -1f;
		}

		private void OnDisable()
		{
			if (this.bIsSkinned)
			{
				if (this.skinnedMeshRenderer != null)
				{
					this.skinnedMeshRenderer.sharedMesh.bounds = this.originalBounds;
					return;
				}
			}
			else if (this.meshFilter != null && this.meshFilter.sharedMesh != null)
			{
				this.meshFilter.sharedMesh.bounds = this.originalBounds;
			}
		}

		private void Start()
		{
			if (CurvedWorldBoundingBox.boundsDictionary == null)
			{
				CurvedWorldBoundingBox.boundsDictionary = new Dictionary<int, Bounds>();
			}
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			if (this.meshFilter != null && this.meshFilter.sharedMesh != null)
			{
				this.bIsSkinned = false;
				if (CurvedWorldBoundingBox.boundsDictionary.ContainsKey(this.meshFilter.sharedMesh.GetInstanceID()))
				{
					this.originalBounds = CurvedWorldBoundingBox.boundsDictionary[this.meshFilter.sharedMesh.GetInstanceID()];
				}
				else
				{
					this.originalBounds = this.meshFilter.sharedMesh.bounds;
					CurvedWorldBoundingBox.boundsDictionary.Add(this.meshFilter.sharedMesh.GetInstanceID(), this.originalBounds);
				}
				this.boundingBoxSize = this.originalBounds.size;
				float num = 1f;
				if (this.boundingBoxSize.x > num)
				{
					num = this.boundingBoxSize.x;
				}
				if (this.boundingBoxSize.y > num)
				{
					num = this.boundingBoxSize.y;
				}
				if (this.boundingBoxSize.z > num)
				{
					num = this.boundingBoxSize.z;
				}
				this.boundingBoxSize.x = (this.boundingBoxSize.y = (this.boundingBoxSize.z = num));
			}
			else if (this.skinnedMeshRenderer != null && this.skinnedMeshRenderer.sharedMesh != null)
			{
				this.bIsSkinned = true;
				if (CurvedWorldBoundingBox.boundsDictionary.ContainsKey(this.skinnedMeshRenderer.sharedMesh.GetInstanceID()))
				{
					this.originalBounds = CurvedWorldBoundingBox.boundsDictionary[this.skinnedMeshRenderer.sharedMesh.GetInstanceID()];
				}
				else
				{
					this.originalBounds = this.skinnedMeshRenderer.sharedMesh.bounds;
					CurvedWorldBoundingBox.boundsDictionary.Add(this.skinnedMeshRenderer.sharedMesh.GetInstanceID(), this.originalBounds);
				}
				this.boundingBoxSize = this.originalBounds.size;
				float num2 = 1f;
				if (this.boundingBoxSize.x > num2)
				{
					num2 = this.boundingBoxSize.x;
				}
				if (this.boundingBoxSize.y > num2)
				{
					num2 = this.boundingBoxSize.y;
				}
				if (this.boundingBoxSize.z > num2)
				{
					num2 = this.boundingBoxSize.z;
				}
				this.boundingBoxSize.x = (this.boundingBoxSize.y = (this.boundingBoxSize.z = num2));
			}
			this.currentScale = 0f;
		}

		private void Update()
		{
			if (this.currentScale != this.scale)
			{
				if (this.scale < 0f)
				{
					this.scale = 0f;
				}
				this.currentScale = this.scale;
				if (this.bIsSkinned)
				{
					if (this.skinnedMeshRenderer != null)
					{
						this.skinnedMeshRenderer.localBounds = new Bounds(this.skinnedMeshRenderer.localBounds.center, this.boundingBoxSize * this.scale);
						return;
					}
				}
				else if (this.meshFilter != null && this.meshFilter.sharedMesh != null)
				{
					this.meshFilter.sharedMesh.bounds = new Bounds(this.meshFilter.sharedMesh.bounds.center, this.boundingBoxSize * this.scale);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (this.visualizeInEditor)
			{
				Gizmos.color = Color.yellow;
				if (this.bIsSkinned && this.skinnedMeshRenderer != null && this.skinnedMeshRenderer.sharedMesh != null)
				{
					Gizmos.DrawWireCube(base.transform.TransformPoint(this.skinnedMeshRenderer.localBounds.center), this.boundingBoxSize * this.scale);
					return;
				}
				if (this.meshFilter != null && this.meshFilter.sharedMesh != null)
				{
					Gizmos.DrawWireCube(base.transform.TransformPoint(this.meshFilter.sharedMesh.bounds.center), this.boundingBoxSize * this.scale);
				}
			}
		}

		private void Reset()
		{
			this.scale = 1f;
			this.RecalculateBounds();
			this.Update();
		}

		public void RecalculateBounds()
		{
			if (this.bIsSkinned)
			{
				if (this.skinnedMeshRenderer != null)
				{
					this.skinnedMeshRenderer.sharedMesh.RecalculateBounds();
					this.originalBounds = this.skinnedMeshRenderer.sharedMesh.bounds;
					if (CurvedWorldBoundingBox.boundsDictionary != null && CurvedWorldBoundingBox.boundsDictionary.ContainsKey(this.skinnedMeshRenderer.sharedMesh.GetInstanceID()))
					{
						CurvedWorldBoundingBox.boundsDictionary[this.skinnedMeshRenderer.sharedMesh.GetInstanceID()] = this.originalBounds;
						return;
					}
				}
			}
			else if (this.meshFilter != null && this.meshFilter.sharedMesh != null)
			{
				this.meshFilter.sharedMesh.RecalculateBounds();
				this.originalBounds = this.meshFilter.sharedMesh.bounds;
				if (CurvedWorldBoundingBox.boundsDictionary != null && CurvedWorldBoundingBox.boundsDictionary.ContainsKey(this.meshFilter.sharedMesh.GetInstanceID()))
				{
					CurvedWorldBoundingBox.boundsDictionary[this.meshFilter.sharedMesh.GetInstanceID()] = this.originalBounds;
				}
			}
		}

		public float scale = 1f;

		private float currentScale;

		private Vector3 boundingBoxSize;

		private Bounds originalBounds;

		private SkinnedMeshRenderer skinnedMeshRenderer;

		private MeshFilter meshFilter;

		private bool bIsSkinned;

		private static Dictionary<int, Bounds> boundsDictionary;

		public bool visualizeInEditor;
	}
}

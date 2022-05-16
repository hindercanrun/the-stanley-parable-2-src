using System;
using UnityEngine;

namespace Polybrush
{
	[ExecuteInEditMode]
	public class z_AdditionalVertexStreams : MonoBehaviour
	{
		private MeshRenderer meshRenderer
		{
			get
			{
				if (this._meshRenderer == null)
				{
					this._meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
				}
				return this._meshRenderer;
			}
		}

		private void Start()
		{
			this.SetAdditionalVertexStreamsMesh(this.m_AdditionalVertexStreamMesh);
		}

		public void SetAdditionalVertexStreamsMesh(Mesh mesh)
		{
			this.m_AdditionalVertexStreamMesh = mesh;
			this.meshRenderer.additionalVertexStreams = mesh;
		}

		public Mesh m_AdditionalVertexStreamMesh;

		private MeshRenderer _meshRenderer;
	}
}

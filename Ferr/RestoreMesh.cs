using System;
using UnityEngine;

namespace Ferr
{
	public class RestoreMesh : MonoBehaviour
	{
		public Mesh OriginalMesh
		{
			get
			{
				return this._originalMesh;
			}
			set
			{
				this._originalMesh = value;
			}
		}

		public void Restore(bool aMaintainColors = true)
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component == null)
			{
				Debug.LogError("No mesh filter to restore to!", base.gameObject);
				return;
			}
			RecolorTree recolorTree = null;
			if (aMaintainColors)
			{
				recolorTree = new RecolorTree(component.sharedMesh, null, true, true, true);
			}
			component.sharedMesh = this._originalMesh;
			if (aMaintainColors)
			{
				ProceduralMeshUtil.EnsureProceduralMesh(component, true);
				Mesh sharedMesh = component.sharedMesh;
				recolorTree.Recolor(ref sharedMesh, null);
			}
		}

		[SerializeField]
		private Mesh _originalMesh;
	}
}

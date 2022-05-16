using System;
using UnityEngine;

namespace Ferr
{
	public class ProceduralMeshUtil
	{
		public static void EnsureProceduralMesh(MeshFilter aFilter, bool aCreateRestoreComponent = true)
		{
			if (!ProceduralMeshUtil.IsProceduralMesh(aFilter))
			{
				if (aCreateRestoreComponent)
				{
					RestoreMesh restoreMesh = aFilter.GetComponent<RestoreMesh>();
					if (restoreMesh == null)
					{
						restoreMesh = aFilter.gameObject.AddComponent<RestoreMesh>();
					}
					restoreMesh.OriginalMesh = aFilter.sharedMesh;
				}
				if (aFilter.sharedMesh == null)
				{
					aFilter.sharedMesh = new Mesh();
				}
				aFilter.sharedMesh = Object.Instantiate<Mesh>(aFilter.sharedMesh);
				aFilter.sharedMesh.name = ProceduralMeshUtil.MakeInstName(aFilter);
				return;
			}
			if (!ProceduralMeshUtil.IsCorrectName(aFilter))
			{
				aFilter.sharedMesh = Object.Instantiate<Mesh>(aFilter.sharedMesh);
				aFilter.sharedMesh.name = ProceduralMeshUtil.MakeInstName(aFilter);
			}
		}

		public static bool IsProceduralMesh(MeshFilter aFilter)
		{
			return !(aFilter == null) && !(aFilter.sharedMesh == null) && aFilter.sharedMesh.name.StartsWith("FerrProcMesh_");
		}

		public static string MakeInstName(MeshFilter aFilter)
		{
			return string.Format("{0}{1}_{2}", "FerrProcMesh_", aFilter.gameObject.name, aFilter.GetInstanceID());
		}

		public static bool IsCorrectName(MeshFilter aFilter)
		{
			return !(aFilter == null) && !(aFilter.sharedMesh == null) && aFilter.sharedMesh.name == ProceduralMeshUtil.MakeInstName(aFilter);
		}

		public const string cProcMeshPrefix = "FerrProcMesh_";
	}
}

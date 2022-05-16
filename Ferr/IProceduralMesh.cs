using System;
using UnityEngine;

namespace Ferr
{
	public interface IProceduralMesh
	{
		Mesh MeshData { get; }

		MeshFilter MeshFilter { get; }

		void Build(bool aFullBuild);
	}
}

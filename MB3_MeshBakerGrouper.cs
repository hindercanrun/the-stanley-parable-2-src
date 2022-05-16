using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB3_MeshBakerGrouper : MonoBehaviour
{
	private void OnDrawGizmosSelected()
	{
		if (this.grouper == null)
		{
			this.grouper = this.CreateGrouper(this.clusterType, this.data);
		}
		if (this.grouper.d == null)
		{
			this.grouper.d = this.data;
		}
		this.grouper.DrawGizmos(this.sourceObjectBounds);
	}

	public MB3_MeshBakerGrouperCore CreateGrouper(MB3_MeshBakerGrouper.ClusterType t, GrouperData data)
	{
		if (t == MB3_MeshBakerGrouper.ClusterType.grid)
		{
			this.grouper = new MB3_MeshBakerGrouperGrid(data);
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.pie)
		{
			this.grouper = new MB3_MeshBakerGrouperPie(data);
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.agglomerative)
		{
			MB3_TextureBaker component = base.GetComponent<MB3_TextureBaker>();
			List<GameObject> gos;
			if (component != null)
			{
				gos = component.GetObjectsToCombine();
			}
			else
			{
				gos = new List<GameObject>();
			}
			this.grouper = new MB3_MeshBakerGrouperCluster(data, gos);
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.none)
		{
			this.grouper = new MB3_MeshBakerGrouperNone(data);
		}
		return this.grouper;
	}

	public MB3_MeshBakerGrouperCore grouper;

	public MB3_MeshBakerGrouper.ClusterType clusterType;

	public GrouperData data = new GrouperData();

	[HideInInspector]
	public Bounds sourceObjectBounds = new Bounds(Vector3.zero, Vector3.one);

	public enum ClusterType
	{
		none,
		grid,
		pie,
		agglomerative
	}
}

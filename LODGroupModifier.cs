using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LODGroupModifier : MonoBehaviour
{
	private void Update()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.runSetEveryFrame || this.runOneVerify || this.runOneFind || this.runOneCopy)
		{
			this.runSetOnce = true;
		}
		if (!this.runSetOnce)
		{
			return;
		}
		if (this.runOneFind)
		{
			this.foundLODGroups = null;
		}
		this.runSetOnce = false;
		this.runOneVerify = false;
		this.runOneFind = false;
		bool flag = this.runOneCopy;
		this.runOneCopy = false;
		this.defintion.levels.Verify();
		if (this.runOneVerify)
		{
			return;
		}
		if (this.foundLODGroups == null)
		{
			this.foundLODGroups = new List<LODGroup>();
			foreach (LODGroup lodgroup in Object.FindObjectsOfType<LODGroup>())
			{
				foreach (string value in this.defintion.gameObjectNames)
				{
					if (lodgroup.name.Contains(value))
					{
						this.foundLODGroups.Add(lodgroup);
					}
				}
			}
		}
		if (flag && this.foundLODGroups.Count > 0)
		{
			LOD[] lods = this.foundLODGroups[0].GetLODs();
			this.defintion.levels.lod0 = lods[0].screenRelativeTransitionHeight;
			this.defintion.levels.lod1 = lods[1].screenRelativeTransitionHeight;
			this.defintion.levels.lod2 = lods[2].screenRelativeTransitionHeight;
			this.defintion.levels.lod3 = lods[3].screenRelativeTransitionHeight;
			this.defintion.levels.lod4 = lods[4].screenRelativeTransitionHeight;
			if (lods.Length > 5)
			{
				this.defintion.levels.cull = lods[5].screenRelativeTransitionHeight;
			}
		}
		if (this.runSetOnce)
		{
			foreach (LODGroup lodgroup2 in this.foundLODGroups)
			{
				LOD[] lods2 = lodgroup2.GetLODs();
				lods2[0].screenRelativeTransitionHeight = this.defintion.levels.lod0;
				lods2[1].screenRelativeTransitionHeight = this.defintion.levels.lod1;
				lods2[2].screenRelativeTransitionHeight = this.defintion.levels.lod2;
				lods2[3].screenRelativeTransitionHeight = this.defintion.levels.lod3;
				lods2[4].screenRelativeTransitionHeight = this.defintion.levels.lod4;
				if (lods2.Length > 5)
				{
					lods2[5].screenRelativeTransitionHeight = this.defintion.levels.cull;
				}
				lodgroup2.SetLODs(lods2);
				lodgroup2.RecalculateBounds();
			}
		}
	}

	private void OnValidate()
	{
	}

	public LODGroupModifier.LODObjectsDefinition defintion;

	public bool runSetEveryFrame;

	public bool runSetOnce;

	public bool runOneVerify;

	public bool runOneFind;

	public bool runOneCopy;

	public List<LODGroup> foundLODGroups;

	[Serializable]
	public class LODObjectsDefinition
	{
		public string[] gameObjectNames;

		public LODGroupModifier.LODObjectsDefinition.LODLevels levels;

		[Serializable]
		public class LODLevels
		{
			public void Verify()
			{
				if (this.lod4 < this.cull)
				{
					this.lod4 = this.cull;
				}
				if (this.lod3 < this.lod4)
				{
					this.lod3 = this.lod4;
				}
				if (this.lod2 < this.lod3)
				{
					this.lod2 = this.lod3;
				}
				if (this.lod1 < this.lod2)
				{
					this.lod1 = this.lod2;
				}
				if (this.lod0 < this.lod1)
				{
					this.lod0 = this.lod1;
				}
			}

			[Range(0f, 1f)]
			public float lod0 = 1f;

			[Range(0f, 1f)]
			public float lod1 = 0.7f;

			[Range(0f, 1f)]
			public float lod2 = 0.5f;

			[Range(0f, 1f)]
			public float lod3 = 0.25f;

			[Range(0f, 1f)]
			public float lod4 = 0.125f;

			[Range(0f, 1f)]
			public float cull = 0.05f;
		}
	}
}

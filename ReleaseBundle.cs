using System;
using Malee.List;
using UnityEngine;

public class ReleaseBundle : ScriptableObject
{
	public string RootFolder;

	public bool CanBeSceneExclusive = true;

	public bool NotInProjectRoot;

	public bool ForceInclusion;

	[Reorderable]
	public ReleaseBundle.VariantPresetsCollection VariantsAndPresets;

	[Serializable]
	public class VariantPresetsCollection : ReorderableArray<ReleaseBundle.VariantPresetPair>
	{
	}

	[Serializable]
	public class VariantPresetPair
	{
		public string VariantName;
	}
}

using System;
using System.Collections.Generic;
using Malee.List;
using UnityEngine;

namespace Bundlelizer
{
	[CreateAssetMenu(fileName = "DefaultAssetMapping", menuName = "Bundelizer/Asset Bundle Mapping")]
	public class AssetMap : ScriptableObject
	{
		public bool IsExcluded(string path)
		{
			for (int i = 0; i < this.ExcludePaths.Count; i++)
			{
				if (this.ExcludePaths[i].KeywordMatch(path))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsExcludedFromSceneDependencies(string path)
		{
			for (int i = 0; i < this.ExcludePaths.Count; i++)
			{
				AssetMap.ExcludePath excludePath = this.ExcludePaths[i];
				if (excludePath.ExcludeBehaviour == AssetMap.ExcludeBehaviours.ExcludeFromSceneDependencies && excludePath.KeywordMatch(path))
				{
					return true;
				}
			}
			return false;
		}

		[Reorderable]
		public AssetMap.ExcludePathList ExcludePaths;

		public List<AssetEntry> UpdatableAssets = new List<AssetEntry>();

		public List<AssetEntry> UpdatableScenes = new List<AssetEntry>();

		public List<AssetEntry> UnbundledTextures = new List<AssetEntry>();

		public List<AssetEntry> UnbundledAudio = new List<AssetEntry>();

		public List<AssetEntry> UnbundledShaders = new List<AssetEntry>();

		public List<AssetEntry> UnbundledMaterial = new List<AssetEntry>();

		public List<AssetEntry> UnbundledMeshes = new List<AssetEntry>();

		public List<AssetEntry> UnbundledModels = new List<AssetEntry>();

		public List<AssetEntry> UnbundledOther = new List<AssetEntry>();

		public List<AssetEntry> MigrationHistory = new List<AssetEntry>();

		public List<AssetEntry> UnusedTextures = new List<AssetEntry>();

		public List<AssetEntry> UnusedAudio = new List<AssetEntry>();

		public List<AssetEntry> UnusedShaders = new List<AssetEntry>();

		public List<AssetEntry> UnusedMaterials = new List<AssetEntry>();

		public List<AssetEntry> UnusedMeshes = new List<AssetEntry>();

		public List<AssetEntry> UnusedModels = new List<AssetEntry>();

		public List<AssetEntry> UnusedOther = new List<AssetEntry>();

		public List<DuplicateEntry> DuplicateAssets = new List<DuplicateEntry>();

		public List<DuplicateEntry> DuplicateSceneAssets = new List<DuplicateEntry>();

		public bool FoldTextures;

		public bool FoldAudio;

		public bool FoldShaders;

		public bool FoldMaterial;

		public bool FoldMeshes;

		public bool FoldModels;

		public bool FoldOther;

		public bool FoldHistory;

		public bool FoldUnusedTextures;

		public bool FoldUnusedAudio;

		public bool FoldUnusedShaders;

		public bool FoldUnusedMaterials;

		public bool FoldUnusedMeshes;

		public bool FoldUnusedModels;

		public bool FoldUnusedOther;

		[SerializeField]
		[HideInInspector]
		public Vector2 ScrollPos;

		[SerializeField]
		public List<string> WhiteList = new List<string>();

		[SerializeField]
		public List<string> BlackList = new List<string>();

		[SerializeField]
		public List<AssetEntry> SelectedList;

		[SerializeField]
		public int SelectedIndex;

		public enum PresetBehaviours
		{
			None,
			Apply
		}

		public enum ExcludeBehaviours
		{
			ExcludeAll,
			ExcludeFromSceneDependencies
		}

		[Serializable]
		public class ExcludePathList : ReorderableArray<AssetMap.ExcludePath>
		{
		}

		[Serializable]
		public class ExcludePath
		{
			public bool KeywordMatch(string path)
			{
				for (int i = 0; i < this.ExcludeFilepathKeywords.Length; i++)
				{
					if (path.Contains(this.ExcludeFilepathKeywords[i]))
					{
						return true;
					}
				}
				return false;
			}

			public string[] ExcludeFilepathKeywords;

			public AssetMap.ExcludeBehaviours ExcludeBehaviour;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;

namespace Bundlelizer
{
	[Serializable]
	public class AssetEntry : IComparable
	{
		public AssetEntry(string assetPath, bool doUpdate)
		{
			this.AssetPath = assetPath;
			this.DoUpdate = doUpdate;
		}

		public void SetNewPath(string newPath)
		{
			this.PreviousAssetPath = this.AssetPath;
			this.AssetPath = newPath;
		}

		public void SetBundleInfo(string currentBundle, string currentVariant, string newBundle, string newVariant)
		{
			this.CurrentBundle = currentBundle;
			this.CurrentVariant = currentVariant;
			this.NewBundle = newBundle;
			this.NewVariant = newVariant;
		}

		public void UpdateName(string newName, string extension)
		{
			string fileName = Path.GetFileName(this.AssetPath);
			string str = this.AssetPath.Remove(this.AssetPath.Length - fileName.Length);
			this.AssetPath = str + newName + extension;
			if (this.PreviousAssetPath != null && this.PreviousAssetPath != "")
			{
				string str2 = this.PreviousAssetPath.Remove(this.PreviousAssetPath.Length - fileName.Length);
				this.PreviousAssetPath = str2 + newName + extension;
			}
		}

		public int CompareTo(object obj)
		{
			return this.AssetPath.CompareTo((obj as AssetEntry).AssetPath);
		}

		public string AssetPath;

		public string PreviousAssetPath;

		public bool DoUpdate;

		public string CurrentBundle;

		public string CurrentVariant;

		public string NewBundle;

		public string NewVariant;

		public List<string> Dependants = new List<string>();
	}
}

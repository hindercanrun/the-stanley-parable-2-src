using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bundlelizer
{
	[Serializable]
	public class DuplicateEntry : IComparable
	{
		public DuplicateEntry(string name)
		{
			this.DuplicateName = name;
		}

		public void AddDuplicatePath(AssetEntry newEntry)
		{
			this.DuplicateEntries.Add(newEntry);
		}

		public int CompareTo(object obj)
		{
			return this.DuplicateName.CompareTo((obj as DuplicateEntry).DuplicateName);
		}

		[SerializeField]
		public List<AssetEntry> DuplicateEntries = new List<AssetEntry>();

		[SerializeField]
		public string DuplicateName;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace MeshBrush
{
	public static class FavouriteTemplatesUtility
	{
		public static XDocument SaveFavouriteTemplates(List<string> favouriteTemplates, string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath", "MeshBrush: the specified file path is null or empty (and thus invalid). Couldn't save favourite templates list...");
			}
			if (favouriteTemplates == null)
			{
				throw new ArgumentNullException("favouriteTemplates", "MeshBrush: The passed list of favourite templates is null. Cancelling saving operation...");
			}
			for (int i = favouriteTemplates.Count - 1; i >= 0; i--)
			{
				if (!File.Exists(favouriteTemplates[i]))
				{
					favouriteTemplates.RemoveAt(i);
				}
			}
			object[] array = new object[1];
			array[0] = new XElement("favouriteMeshBrushTemplates", from template in favouriteTemplates
			select new XElement("template", new XElement("path", template)));
			XDocument xdocument = new XDocument(array);
			xdocument.Save(filePath);
			return xdocument;
		}

		public static List<string> LoadFavouriteTemplates(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				throw new ArgumentException("MeshBrush: the specified file path is invalid or doesn't exist! Can't load favourite templates list...", "filePath");
			}
			return new List<string>(from path in XDocument.Load(filePath).Descendants("path")
			select path.Value);
		}

		public static bool LoadFavouriteTemplates(string filePath, List<string> targetList)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				throw new ArgumentException("MeshBrush: the specified file path is invalid or doesn't exist! Can't load favourite templates list...", "filePath");
			}
			if (targetList == null)
			{
				throw new ArgumentNullException("targetList", "MeshBrush: cannot write favourite templates to the specified target list because it is null.");
			}
			try
			{
				targetList.Clear();
				foreach (XElement xelement in XDocument.Load(filePath).Descendants("path"))
				{
					targetList.Add(xelement.Value);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("MeshBrush: loading favourite templates list failed. Error message: " + ex.Message);
				return false;
			}
			return true;
		}
	}
}

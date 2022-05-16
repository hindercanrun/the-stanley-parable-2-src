using System;
using System.Collections.Generic;
using UnityEngine;

public class ImportedParent : MonoBehaviour
{
	public void Cleanup()
	{
		List<ImportedObject> list = new List<ImportedObject>();
		for (int i = 0; i < this.importedObjects.Count; i++)
		{
			if (this.importedObjects[i])
			{
				list.Add(this.importedObjects[i]);
			}
		}
		this.importedObjects = list;
	}

	public GameObject Find(string name)
	{
		for (int i = 0; i < this.importedObjects.Count; i++)
		{
			if (this.importedObjects[i].gameObject.name == name)
			{
				return this.importedObjects[i].gameObject;
			}
		}
		return null;
	}

	public GameObject[] FindWild(string name)
	{
		List<GameObject> list = new List<GameObject>();
		if (name[name.Length - 1] == '*')
		{
			string b = name.Substring(0, name.Length - 1);
			for (int i = 0; i < this.importedObjects.Count; i++)
			{
				if (this.importedObjects[i].gameObject.name.Substring(0, this.importedObjects[i].gameObject.name.Length - 1) == b)
				{
					list.Add(this.importedObjects[i].gameObject);
				}
			}
		}
		if (list.Count > 0)
		{
			return list.ToArray();
		}
		return null;
	}

	[HideInInspector]
	public List<ImportedObject> importedObjects = new List<ImportedObject>();
}

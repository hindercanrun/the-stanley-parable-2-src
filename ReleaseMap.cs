using System;
using System.Collections.Generic;
using Malee.List;
using UnityEngine;

[CreateAssetMenu(fileName = "New Release Map", menuName = "Release Map")]
public class ReleaseMap : ScriptableObject
{
	[Reorderable]
	public ReleaseMap.ReleaseScenes Scenes;

	public string RootFolder = "Project";

	public List<ReleaseBundle> Bundles = new List<ReleaseBundle>();

	[Serializable]
	public class ReleaseScene
	{
		public string SceneName;

		public SceneAssetSet Set;
	}

	[Serializable]
	public class ReleaseScenes : ReorderableArray<ReleaseMap.ReleaseScene>
	{
	}
}

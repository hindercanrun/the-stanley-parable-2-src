using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Model List")]
public class ModelList : ScriptableObject
{
	public List<GameObject> models = new List<GameObject>();
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Material List")]
public class MaterialList : ScriptableObject
{
	public List<Material> materials = new List<Material>();
}

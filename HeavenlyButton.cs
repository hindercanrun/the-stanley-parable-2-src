using System;
using System.Collections.Generic;
using UnityEngine;

public class HeavenlyButton : MonoBehaviour
{
	public List<HeavenlyButton.OnOffMaterials> colourSelection = new List<HeavenlyButton.OnOffMaterials>();

	[InspectorButton("TestOff", null)]
	public Skin offSkin;

	[InspectorButton("TestOn", null)]
	public Skin onSkin;

	public SkinnedMeshRenderer meshRenderer;

	[Header("Colour is selected in editor time! -rax")]
	[InspectorButton("SetSkinMaterials", "Set Skin Materials")]
	[SerializeField]
	private int colourIndex;

	[Serializable]
	public class OnOffMaterials
	{
		public Material offMaterial;

		public Material onMaterial;
	}
}

using System;
using UnityEngine;

[ExecuteInEditMode]
public class VMFImport : MonoBehaviour
{
	[SerializeField]
	private string VMFPath = "Assets/Resources/Mapsrc/rate_your_experience.vmf";

	[SerializeField]
	private GameObject importParent;

	[SerializeField]
	private Object VMFasset;

	[SerializeField]
	private TextAsset logfile;

	[SerializeField]
	private GameObject[] modifiedObjects;
}

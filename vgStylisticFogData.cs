using System;
using UnityEngine;

public class vgStylisticFogData : MonoBehaviour
{
	public float startDistance = 50f;

	public float endDistance = 250f;

	public float intensityScale = 1f;

	public float offsetFromAToB;

	public Texture fogColorTextureFromAToB;

	public Texture fogColorTextureFromBToA;

	public Transform transformObjectA;

	public Transform transformObjectB;
}

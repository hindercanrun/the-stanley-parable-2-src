using System;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
	public void Start()
	{
		this.ChangeMaterial();
	}

	public void ChangeMaterial()
	{
		this.targetRenderer.sharedMaterial = this.materials[Random.Range(0, this.materials.Length)];
	}

	public Renderer targetRenderer;

	public Material[] materials;
}

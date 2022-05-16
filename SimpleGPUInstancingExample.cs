using System;
using UnityEngine;

public class SimpleGPUInstancingExample : MonoBehaviour
{
	private void Awake()
	{
		this.InstancedMaterial.enableInstancing = true;
		float num = 4f;
		for (int i = 0; i < 1000; i++)
		{
			Component component = Object.Instantiate<Transform>(this.Prefab, new Vector3(Random.Range(-num, num), num + Random.Range(-num, num), Random.Range(-num, num)), Quaternion.identity);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			Color value = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			materialPropertyBlock.SetColor("_Color", value);
			component.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
		}
	}

	public Transform Prefab;

	public Material InstancedMaterial;
}

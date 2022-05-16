using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoMobileShaderSwitch : MonoBehaviour
{
	private void Awake()
	{
		this.switchMaterials = !this.UseDefaultConfigurationConfigurable.GetBooleanValue();
		if (this.switchMaterials)
		{
			this.UpdateMaterials();
			SceneManager.sceneLoaded += this.SceneLoaded;
		}
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= this.SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		base.StartCoroutine(this.DelayUpdate());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.UpdateMaterials();
		}
	}

	private IEnumerator DelayUpdate()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this.UpdateMaterials();
		yield break;
	}

	private void UpdateMaterials()
	{
		this.renderers = Object.FindObjectsOfType<Renderer>();
		List<Material> list = new List<Material>();
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Material material in array[i].sharedMaterials)
			{
				if (material != null && material.shader != null && !list.Contains(material))
				{
					Shader shader = null;
					if (this.m_ReplacementList.GetReplacement(material.shader, out shader))
					{
						material.shader = shader;
						list.Add(material);
					}
				}
			}
		}
	}

	[SerializeField]
	private AutoMobileShaderSwitch.ReplacementList m_ReplacementList;

	[SerializeField]
	private Configurable UseDefaultConfigurationConfigurable;

	[SerializeField]
	private Renderer[] renderers = new Renderer[0];

	private bool switchMaterials;

	[Serializable]
	public class ReplacementDefinition
	{
		public Shader original;

		public Shader replacement;
	}

	[Serializable]
	public class ReplacementList
	{
		public bool GetReplacement(Shader currentShader, out Shader replacement)
		{
			foreach (AutoMobileShaderSwitch.ReplacementDefinition replacementDefinition in this.items)
			{
				if (replacementDefinition.original == currentShader)
				{
					replacement = replacementDefinition.replacement;
					return true;
				}
			}
			replacement = null;
			return false;
		}

		public AutoMobileShaderSwitch.ReplacementDefinition[] items = new AutoMobileShaderSwitch.ReplacementDefinition[0];
	}
}

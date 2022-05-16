using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class vgStylisticFogAdder : MonoBehaviour
{
	private void Start()
	{
		this.fog = StanleyController.Instance.cam.gameObject.AddComponent<vgStylisticFog>();
		this.fog.fogShader = this.shaderToSet;
		this.fog._customFogData = this.fogDataToSet;
		SceneManager.sceneUnloaded += this.SceneManager_sceneUnloaded;
		RenderSettings.fogStartDistance = 5000f;
		RenderSettings.fogEndDistance = 20000f;
	}

	private void SceneManager_sceneUnloaded(Scene arg0)
	{
		if (arg0.name.StartsWith("Loading"))
		{
			return;
		}
		Object.Destroy(this.fog);
		SceneManager.sceneUnloaded -= this.SceneManager_sceneUnloaded;
	}

	public Shader shaderToSet;

	public vgStylisticFogData fogDataToSet;

	private vgStylisticFog fog;
}

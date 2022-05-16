using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
	private void Awake()
	{
		if (this.raycastTarget != null)
		{
			MainCamera.RaycastTarget = this.raycastTarget;
		}
		this.UpdatePortals();
		GameMaster.OnPause += this.OnPause;
		GameMaster.OnResume += this.OnResume;
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	private void Start()
	{
		MainCamera.Camera = base.GetComponent<Camera>();
		this.blur.enabled = false;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.OnResume();
	}

	private void OnPause()
	{
		this.HandleBlur();
	}

	private void OnResume()
	{
		this.blur.enabled = false;
		MainCamera.BlurValue = 0f;
	}

	private void OnDestroy()
	{
		GameMaster.OnPause -= this.OnPause;
		GameMaster.OnResume -= this.OnResume;
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	private void HandleBlur()
	{
		if (this.TSPVersion.GetIntValue() == 1)
		{
			this.blur.material.SetColor("_Tint", Color.red);
		}
		else
		{
			this.blur.material.SetColor("_Tint", Color.white);
		}
		this.blur.BlurAmount = 0f;
		this.blur.enabled = true;
	}

	private void Update()
	{
		if (this.blur.enabled)
		{
			float num = Mathf.Lerp(this.blur.BlurAmount, 1.5f, Time.unscaledDeltaTime * 2f);
			this.blur.BlurAmount = num;
			MainCamera.BlurValue = num;
		}
	}

	public void UpdatePortals()
	{
		MainCamera.UseVicinityRenderingOnly = this.DefaultConfigurationConfigurable.GetBooleanValue();
		MainCamera.Portals = Object.FindObjectsOfType<EasyPortal>();
		this.portalGameObjects = new GameObject[MainCamera.Portals.Length];
		for (int i = 0; i < MainCamera.Portals.Length; i++)
		{
			this.portalGameObjects[i] = MainCamera.Portals[i].gameObject;
		}
	}

	private void SortArray()
	{
		Array.Sort<EasyPortal>(MainCamera.Portals);
		this.portals = MainCamera.Portals;
	}

	private void OnPreCull()
	{
		for (int i = 0; i < MainCamera.Portals.Length; i++)
		{
			if (!MainCamera.Portals[i].disabled)
			{
				MainCamera.Portals[i].Render();
			}
		}
		for (int j = 0; j < MainCamera.Portals.Length; j++)
		{
			if (!MainCamera.Portals[j].disabled)
			{
				MainCamera.Portals[j].PostPortalRender();
			}
		}
	}

	public static Camera Camera;

	public static Transform RaycastTarget;

	public static float BlurValue;

	[SerializeField]
	private MobileBlur blur;

	public static bool UseVicinityRenderingOnly;

	public static EasyPortal[] Portals;

	[SerializeField]
	private BooleanConfigurable DefaultConfigurationConfigurable;

	[SerializeField]
	private IntConfigurable TSPVersion;

	[SerializeField]
	private Transform raycastTarget;

	[SerializeField]
	private GameObject[] portalGameObjects;

	[SerializeField]
	private EasyPortal[] portals;
}

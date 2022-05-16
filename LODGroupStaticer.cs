using System;
using UnityEngine;

[ExecuteInEditMode]
public class LODGroupStaticer : MonoBehaviour
{
	private void Start()
	{
		if (Application.isPlaying)
		{
			this.refresh = this.runOnceOnStart;
			return;
		}
		this.refresh = false;
	}

	private void OnGUI()
	{
		if (!this.showDebugButtons)
		{
			return;
		}
		GUILayout.Space(120f);
		GUILayout.Label("LOD bias = " + QualitySettings.lodBias, Array.Empty<GUILayoutOption>());
		foreach (float num in this.lodBiases)
		{
			if (GUILayout.Button("LOD bias = " + num, Array.Empty<GUILayoutOption>()))
			{
				QualitySettings.lodBias = num;
				this.refresh = true;
			}
		}
		if (GUILayout.Button(this.useBiasMultipliers ? "Using Bias Multiplier" : "NOT Using Bias Multiplier", Array.Empty<GUILayoutOption>()))
		{
			this.useBiasMultipliers = !this.useBiasMultipliers;
			this.refresh = true;
		}
	}

	private void Update()
	{
		if (this.calculateStaticLODLevel != null)
		{
			bool flag = this.verboseMode;
			this.verboseMode = true;
			this.CalculateStaticLODLevel(this.calculateStaticLODLevel);
			this.verboseMode = flag;
			this.calculateStaticLODLevel = null;
		}
		if (this.old_staticifyLODGroups != this.staticifyLODGroups)
		{
			this.refresh = true;
			this.old_staticifyLODGroups = this.staticifyLODGroups;
		}
		if (this.refresh)
		{
			if (this.staticifyLODGroups)
			{
				this.StaticifyLODGroups();
			}
			else
			{
				this.ResetLODGoups();
			}
			this.refresh = false;
		}
	}

	private int CalculateStaticLODLevel(LODGroup lg)
	{
		if (this.verboseMode)
		{
			Debug.Log(lg, lg);
		}
		float num = Vector3.Distance(lg.transform.TransformPoint(lg.localReferencePoint), base.transform.position);
		bool flag = this.verboseMode;
		LODGroupStaticerBiasMultiplier component = lg.GetComponent<LODGroupStaticerBiasMultiplier>();
		float num2 = (component == null || !this.useBiasMultipliers) ? 1f : component.GetBias();
		float num3 = lg.size / num * QualitySettings.lodBias * num2;
		if (this.verboseMode)
		{
			Debug.Log("LOD dist: " + num3);
		}
		LOD[] lods = lg.GetLODs();
		int num4 = 0;
		for (int i = 0; i < lods.Length; i++)
		{
			if (num3 < lods[i].screenRelativeTransitionHeight)
			{
				num4 = i + 1;
			}
		}
		if (this.verboseMode)
		{
			Debug.Log("LOD level: " + num4);
		}
		return num4;
	}

	private void ResetLODGoups()
	{
		foreach (LODGroup lodgroup in Object.FindObjectsOfType<LODGroup>())
		{
			lodgroup.enabled = true;
			LOD[] lods = lodgroup.GetLODs();
			for (int j = 0; j < lods.Length; j++)
			{
				foreach (Renderer renderer in lods[j].renderers)
				{
					if (renderer != null)
					{
						renderer.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	private void StaticifyLODGroups()
	{
		foreach (LODGroup lodgroup in Object.FindObjectsOfType<LODGroup>())
		{
			if (!(lodgroup.GetComponent<LODGroupStaticerIgnore>() != null))
			{
				LOD[] lods = lodgroup.GetLODs();
				int num = this.CalculateStaticLODLevel(lodgroup);
				lodgroup.enabled = false;
				for (int j = 0; j < lods.Length; j++)
				{
					if (this.verboseMode)
					{
						Debug.Log(j + ": " + lods[j].screenRelativeTransitionHeight);
					}
					foreach (Renderer renderer in lods[j].renderers)
					{
						if (this.verboseMode)
						{
							Debug.Log("Disabled: " + renderer);
						}
						if (renderer != null)
						{
							renderer.gameObject.SetActive(false);
						}
					}
				}
				if (this.verboseMode)
				{
					Debug.Log("lod level: " + num);
				}
				if (num != lods.Length)
				{
					foreach (Renderer renderer2 in lods[num].renderers)
					{
						if (this.verboseMode)
						{
							Debug.Log("Enabled: " + renderer2);
						}
						if (renderer2 != null)
						{
							renderer2.gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	private bool old_staticifyLODGroups;

	public bool staticifyLODGroups;

	public bool refresh;

	public bool runOnceOnStart = true;

	public bool useBiasMultipliers = true;

	public bool showDebugButtons;

	public LODGroup calculateStaticLODLevel;

	private float[] lodBiases = new float[]
	{
		2f,
		1.5f,
		1f,
		0.75f,
		0.7f,
		0.65f,
		0.6f,
		0.55f,
		0.5f,
		0.25f,
		0.1f,
		0.01f
	};

	private bool verboseMode;
}

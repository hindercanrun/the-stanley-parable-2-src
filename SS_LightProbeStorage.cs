using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SS_LightProbeStorage : MonoBehaviour
{
	[ContextMenu("Grab Harmonics")]
	public void GrabHarmonics()
	{
		if (!this.lightProbeComponent)
		{
			this.lightProbeComponent = base.GetComponent<LightProbeGroup>();
		}
		if (!this.lightProbeComponent)
		{
			return;
		}
		this.positions = this.lightProbeComponent.probePositions;
		Transform transform = base.transform;
		for (int i = 0; i < this.positions.Length; i++)
		{
			this.positions[i] += transform.position;
			this.positions[i] *= 100f;
			this.positions[i] = new Vector3(Mathf.Floor(this.positions[i].x), Mathf.Floor(this.positions[i].y), Mathf.Floor(this.positions[i].z));
			this.positions[i] *= 0.01f;
		}
		this.harmonics = new SS_LightProbeStorage.TCachedHarmonics[this.positions.Length];
		Vector3[] array = LightmapSettings.lightProbes.positions;
		SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
		for (int j = 0; j < this.positions.Length; j++)
		{
			int harmonicsIdByPosition = this.GetHarmonicsIdByPosition(ref array, this.positions[j], this.precision);
			if (harmonicsIdByPosition > -1)
			{
				SphericalHarmonicsL2 sphericalHarmonicsL = bakedProbes[harmonicsIdByPosition];
				SS_LightProbeStorage.TCachedHarmonics tcachedHarmonics = default(SS_LightProbeStorage.TCachedHarmonics);
				tcachedHarmonics.c1 = new float[9];
				tcachedHarmonics.c2 = new float[9];
				tcachedHarmonics.c3 = new float[9];
				for (int k = 0; k < 9; k++)
				{
					tcachedHarmonics.c1[k] = sphericalHarmonicsL[0, k];
				}
				for (int l = 0; l < 9; l++)
				{
					tcachedHarmonics.c2[l] = sphericalHarmonicsL[1, l];
				}
				for (int m = 0; m < 9; m++)
				{
					tcachedHarmonics.c3[m] = sphericalHarmonicsL[2, m];
				}
				this.harmonics[j] = tcachedHarmonics;
			}
			else
			{
				Debug.LogWarning("Can't find harmonics #" + j);
			}
		}
		this.harmonicsCount = ((this.harmonics != null) ? this.harmonics.Length : 0);
	}

	private void OnEnable()
	{
		if (this.applyHarmonicsOnEnable)
		{
			this.ApplyHarmonics();
		}
	}

	private void Start()
	{
		if (this.applyHarmonicsOnStart)
		{
			this.ApplyHarmonics();
		}
	}

	[ContextMenu("Apply Harmonics")]
	public void ApplyHarmonics()
	{
		if (this.harmonics == null || this.positions == null || this.harmonics.Length == 0 || this.positions.Length == 0)
		{
			Debug.LogWarning("Trying to apply uninitialized lightprobes!");
			return;
		}
		SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
		Vector3[] array = LightmapSettings.lightProbes.positions;
		for (int i = 0; i < this.harmonics.Length; i++)
		{
			if (this.GetHarmonicsIdByPosition(ref array, this.positions[i], this.precision) > -1)
			{
				for (int j = 0; j < 9; j++)
				{
					bakedProbes[i][0, j] = this.harmonics[i].c1[j] * this.harmonicsMul;
				}
				for (int k = 0; k < 9; k++)
				{
					bakedProbes[i][1, k] = this.harmonics[i].c2[k] * this.harmonicsMul;
				}
				for (int l = 0; l < 9; l++)
				{
					bakedProbes[i][2, l] = this.harmonics[i].c3[l] * this.harmonicsMul;
				}
			}
			else
			{
				Debug.LogWarning("Can't find baked probe #" + i);
			}
		}
		LightmapSettings.lightProbes.bakedProbes = bakedProbes;
	}

	public int GetHarmonicsIdByPosition(ref Vector3[] pss, Vector3 pos, float precision)
	{
		if (precision == 0f)
		{
			for (int i = 0; i < pss.Length; i++)
			{
				if (pss[i] == pos)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < pss.Length; j++)
			{
				Vector3 vector = pss[j] - pos;
				if (Mathf.Abs(vector.x) <= precision && Mathf.Abs(vector.y) <= precision && Mathf.Abs(vector.z) <= precision)
				{
					return j;
				}
			}
		}
		return -1;
	}

	public bool applyHarmonicsOnEnable = true;

	public bool applyHarmonicsOnStart = true;

	[Range(0f, 2f)]
	[Tooltip("Multiplier for spherical harmonics value")]
	public float harmonicsMul = 1f;

	public LightProbeGroup lightProbeComponent;

	[HideInInspector]
	public Vector3[] positions;

	[HideInInspector]
	public SS_LightProbeStorage.TCachedHarmonics[] harmonics;

	public int harmonicsCount;

	[Header("Position Precision")]
	[Range(0f, 0.1f)]
	public float precision;

	[Serializable]
	public struct TCachedHarmonics
	{
		public float[] c1;

		public float[] c2;

		public float[] c3;
	}
}

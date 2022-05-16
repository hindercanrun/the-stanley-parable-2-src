using System;
using UnityEngine;

public class CullForSwitchController : MonoBehaviour
{
	private void Start()
	{
		if (Application.isPlaying)
		{
			this.DoCulling();
		}
	}

	public static bool IsSwitchEnvironment
	{
		get
		{
			return PlatformSettings.Instance.isSwitch.GetBooleanValue();
		}
	}

	private bool ShouldCull
	{
		get
		{
			if (this.itemsAreCulled == CullForSwitchController.CullMode.CullForSwitchAsExpected)
			{
				return CullForSwitchController.IsSwitchEnvironment;
			}
			if (this.itemsAreCulled == CullForSwitchController.CullMode.ForceCull)
			{
				return true;
			}
			if (this.itemsAreCulled == CullForSwitchController.CullMode.ForceDoNotCull)
			{
				return false;
			}
			Debug.LogWarning("CullMode invalid");
			return false;
		}
	}

	[ContextMenu("Set To Cull")]
	private void SetCull()
	{
		this.itemsAreCulled = CullForSwitchController.CullMode.ForceCull;
		this.DoCulling();
	}

	[ContextMenu("Set To Not Cull")]
	private void SetNotCull()
	{
		this.itemsAreCulled = CullForSwitchController.CullMode.ForceDoNotCull;
		this.DoCulling();
	}

	[ContextMenu("Set To Cull On Switch")]
	private void SetCullOnSwitch()
	{
		this.itemsAreCulled = CullForSwitchController.CullMode.CullForSwitchAsExpected;
		this.DoCulling();
	}

	[ContextMenu("Do Culling Executions")]
	private void DoCulling()
	{
		bool shouldCull = this.ShouldCull;
		foreach (CullForSwitch cullForSwitch in Resources.FindObjectsOfTypeAll<CullForSwitch>())
		{
			cullForSwitch.gameObject.SetActive(shouldCull == cullForSwitch.invertCull);
		}
		if (this.doFarZSwitch)
		{
			this.farZVolume.farZ = (shouldCull ? this.culledFarZVolume : this.unculledFarZVolume);
		}
	}

	public CullForSwitchController.CullMode itemsAreCulled;

	public bool doFarZSwitch = true;

	public FarZVolume farZVolume;

	public float culledFarZVolume = 1000f;

	public float unculledFarZVolume = 5000f;

	public Camera mainCamera_AffectsEditorOnly;

	public enum CullMode
	{
		CullForSwitchAsExpected,
		ForceCull,
		ForceDoNotCull
	}
}

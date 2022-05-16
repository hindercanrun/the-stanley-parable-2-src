using System;
using UnityEngine;

public class NonRendererFootstepType : MonoBehaviour
{
	public StanleyData.FootstepSounds FootstepType
	{
		get
		{
			return this.footstepType;
		}
	}

	public bool ForceOverrideMaterial
	{
		get
		{
			return this.forceOverrideMaterial;
		}
	}

	[SerializeField]
	private StanleyData.FootstepSounds footstepType;

	[SerializeField]
	private bool forceOverrideMaterial;
}

using System;
using UnityEngine;

public class CharacterHeightVolume : MonoBehaviour
{
	private void Awake()
	{
		this.position = base.transform.position;
		this.halfExtents = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		this.halfExtents *= 0.5f;
		this.orientation = base.transform.rotation;
		if (this.SetOnAwake)
		{
			StanleyController.Instance.SetCharacterHeightMultiplier(this.characterHeightMultiplier);
		}
	}

	private void Update()
	{
	}

	public float characterHeightMultiplier = 1f;

	public CharacterHeightVolume.ExitColliderMode exitColliderMode;

	private Vector3 halfExtents;

	private Vector3 position;

	private Quaternion orientation;

	private LayerMask layerMask = 512;

	private bool touchingLastFrame;

	private bool isEnabled = true;

	[SerializeField]
	private bool SetOnAwake;

	public enum ExitColliderMode
	{
		ResetToOneOnExit,
		DoesNotResetOnExit
	}
}

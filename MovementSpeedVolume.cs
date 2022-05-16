using System;
using UnityEngine;

public class MovementSpeedVolume : MonoBehaviour
{
	private void Awake()
	{
		this.position = base.transform.position;
		this.halfExtents = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		this.halfExtents *= 0.5f;
		this.orientation = base.transform.rotation;
	}

	private void Update()
	{
		if (Physics.OverlapBox(this.position, this.halfExtents, this.orientation, this.layerMask).Length != 0)
		{
			if (!this.touchingLastFrame)
			{
				StanleyController.Instance.SetMovementSpeedMultiplier(this.movementSpeedMultiplier);
				StanleyController.Instance.WalkingSpeedAffectsFootstepSoundSpeed = this.walkingSpeedShouldAffectFootstepSpeed;
			}
			this.touchingLastFrame = true;
			return;
		}
		if (this.touchingLastFrame && this.exitColliderMode == MovementSpeedVolume.ExitColliderMode.ResetToOneOnExit)
		{
			StanleyController.Instance.SetMovementSpeedMultiplier(1f);
		}
		this.touchingLastFrame = false;
	}

	public float movementSpeedMultiplier = 1f;

	public bool walkingSpeedShouldAffectFootstepSpeed;

	public MovementSpeedVolume.ExitColliderMode exitColliderMode;

	private Vector3 halfExtents;

	private Vector3 position;

	private Quaternion orientation;

	private LayerMask layerMask = 512;

	private bool touchingLastFrame;

	private bool isEnabled = true;

	public enum ExitColliderMode
	{
		ResetToOneOnExit,
		DoesNotResetOnExit
	}
}

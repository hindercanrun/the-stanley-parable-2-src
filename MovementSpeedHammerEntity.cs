using System;

public class MovementSpeedHammerEntity : HammerEntity
{
	public void Input_SetMovementSpeed()
	{
		StanleyController.Instance.SetMovementSpeedMultiplier(this.movementSpeedMultiplier);
		StanleyController.Instance.WalkingSpeedAffectsFootstepSoundSpeed = this.walkingSpeedShouldAffectFootstepSpeed;
	}

	public float movementSpeedMultiplier = 1f;

	public bool walkingSpeedShouldAffectFootstepSpeed;
}

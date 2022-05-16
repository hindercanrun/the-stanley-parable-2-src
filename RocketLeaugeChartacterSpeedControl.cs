using System;
using UnityEngine;

public class RocketLeaugeChartacterSpeedControl : MonoBehaviour
{
	public static RocketLeaugeChartacterSpeedControl Instance { get; private set; }

	private void Awake()
	{
		RocketLeaugeChartacterSpeedControl.Instance = this;
	}

	public float NormalizedSpeed { get; private set; }

	private void Update()
	{
		this.forwardMovement = Singleton<GameMaster>.Instance.stanleyActions.Movement.Y;
		this.sideMovement = Singleton<GameMaster>.Instance.stanleyActions.Movement.X;
		if (this.forwardMovement != 0f || this.sideMovement != 0f)
		{
			this.timeMovingForward += Time.deltaTime;
		}
		else
		{
			this.timeMovingForward -= Time.deltaTime * 3f;
		}
		this.timeMovingForward = Mathf.Clamp(this.timeMovingForward, 0f, this.timeToMaxSpeed);
		this.NormalizedSpeed = Mathf.InverseLerp(0f, this.timeToMaxSpeed, this.timeMovingForward);
		this.characterSpeedMultiplier = Mathf.Lerp(1f, this.speedAtMax, this.NormalizedSpeed);
		StanleyController.Instance.SetMovementSpeedMultiplier(this.characterSpeedMultiplier);
		StanleyController.Instance.FieldOfViewAdditiveModifier = this.fovByNormalizedSpeed.Evaluate(this.NormalizedSpeed);
		StanleyController.Instance.WalkingSpeedAffectsFootstepSoundSpeedScale = this.footstepSoundSpeedScale;
	}

	public float timeToMaxSpeed = 2.5f;

	public float speedAtMax = 4f;

	[Range(0f, 1f)]
	public float footstepSoundSpeedScale;

	public AnimationCurve fovByNormalizedSpeed;

	[Header("DEBUG")]
	public float timeMovingForward;

	public float characterSpeedMultiplier;

	public float forwardMovement;

	public float sideMovement;
}

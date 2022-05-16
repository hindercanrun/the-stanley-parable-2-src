using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RL_Ball : MonoBehaviour
{
	public Rigidbody rb
	{
		get
		{
			if (!(this._rb != null))
			{
				return base.GetComponent<Rigidbody>();
			}
			return this._rb;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Object componentInParent = collision.gameObject.GetComponentInParent<StanleyController>();
		RL_Ball componentInParent2 = collision.gameObject.GetComponentInParent<RL_Ball>();
		if (!(componentInParent != null))
		{
			if (componentInParent2 != null)
			{
				this.OnBallCollisionEnter(collision.GetContact(0).normal.normalized);
				return;
			}
			this.surfaceObjectsTouched.Add(collision.collider);
			if (this.rollingSurfaceColliderCount == 0)
			{
				this.OnSurfaceCollisionEnter(collision.GetContact(0).normal.normalized);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		this.surfaceObjectsTouched.Remove(collision.collider);
	}

	private void OnBallCollisionEnter(Vector3 surfaceNormal)
	{
		float num = Vector3.Dot(surfaceNormal, this.rb.velocity.normalized);
		float num2 = Mathf.InverseLerp(0f, this.ballCollisionSpeedForMaxVolume, this.rb.velocity.magnitude);
		this.PlaySoftBounce(num * num2);
		this.bounceCount++;
	}

	private void OnSurfaceCollisionEnter(Vector3 surfaceNormal)
	{
		float num = Vector3.Dot(surfaceNormal, this.rb.velocity.normalized);
		float num2 = Mathf.InverseLerp(0f, this.surfaceCollisionSpeedForMaxVolume, this.rb.velocity.magnitude);
		if (this.bounceCount == 0)
		{
			this.PlayHardBounce(num * num2);
		}
		else
		{
			this.PlaySoftBounce(num * num2);
		}
		this.bounceCount++;
	}

	private void PlayHardBounce(float volumeMultiplier)
	{
		this.bounceSound.Play();
		this.bounceSound.GetComponent<AudioSource>().volume *= volumeMultiplier;
	}

	private void PlaySoftBounce(float volumeMultiplier)
	{
		this.bounceOnBallSound.Play();
		this.bounceOnBallSound.GetComponent<AudioSource>().volume *= volumeMultiplier;
	}

	private void PlaySoftKick(float volumeMultiplier)
	{
		this.kickSound.Play();
		this.kickSound.GetComponent<AudioSource>().volume *= volumeMultiplier;
	}

	private void PlayHardKick(float volumeMultiplier)
	{
		this.kickPulseSound.Play();
		this.kickPulseSound.GetComponent<AudioSource>().volume *= volumeMultiplier;
	}

	private void UpdateRollingColliderCount()
	{
		int num = 3;
		if (this.surfaceCollidersCount.Count < num)
		{
			this.surfaceCollidersCount.AddLast(this.surfaceObjectsTouched.Count);
		}
		if (this.surfaceCollidersCount.Count >= num)
		{
			this.surfaceCollidersCount.RemoveFirst();
		}
		this.rollingSurfaceColliderCount = 0;
		foreach (int b in this.surfaceCollidersCount)
		{
			this.rollingSurfaceColliderCount = Mathf.Max(this.rollingSurfaceColliderCount, b);
		}
	}

	private bool IsRolling
	{
		get
		{
			return this.rollingSurfaceColliderCount > 0;
		}
	}

	private float Radius
	{
		get
		{
			return base.GetComponentInChildren<SphereCollider>().radius * base.transform.lossyScale.x;
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			this.meshRenderer.material.SetFloat(this.flashShaderKey, this.flashAmount);
		}
		else
		{
			this.meshRenderer.sharedMaterial.SetFloat(this.flashShaderKey, this.flashAmount);
		}
		if (Application.isPlaying)
		{
			if (base.transform.position.y < this.depthToDisableRenderer)
			{
				base.gameObject.SetActive(false);
			}
			this.UpdateRollingColliderCount();
			this.ballVelocity = this.rb.velocity.magnitude;
			bool flag = this.IsRolling;
			if (this.IsRolling)
			{
				this.footstepTypeUnderBall = this.FindFootstepFromMaterialUnderBall();
				if (this.footstepTypeUnderBall == StanleyData.FootstepSounds.Grass)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.rollingSpeedNormalized = this.rollVolumeBySpeed.Evaluate(this.rb.velocity.magnitude);
				this.rollingLoop.volume = Mathf.MoveTowards(this.rollingLoop.volume, this.rollingSpeedNormalized, 4f);
			}
			else
			{
				this.footstepTypeUnderBall = StanleyData.FootstepSounds.None;
				this.rollingLoop.volume = 0f;
			}
			this.DEBUG_DIST = this.DistanceToStanleyCenter;
			bool flag2 = false;
			if (Singleton<GameMaster>.Instance.stanleyActions.UseAction.LastValue > 0f)
			{
				this.kickTime += Time.deltaTime;
			}
			else
			{
				this.kickTime = 0f;
			}
			if (Singleton<GameMaster>.Instance.stanleyActions.UseAction.Value == 0f && Singleton<GameMaster>.Instance.stanleyActions.UseAction.LastValue > 0f)
			{
				flag2 = true;
			}
			this.coolDownTimeRemaining -= Time.deltaTime;
			if (this.coolDownTimeRemaining <= 0f)
			{
				if (this.IsCollisionWithStanley(0.25f))
				{
					this.coolDownTimeRemaining = this.coolDownPeriod;
					this.bounceCount = 0;
					this.AddKickForceToBall(Mathf.Lerp(this.touchForce, this.touchForceAtFullSpeed, RocketLeaugeChartacterSpeedControl.Instance.NormalizedSpeed));
					this.PlaySoftKick(RocketLeaugeChartacterSpeedControl.Instance.NormalizedSpeed);
				}
				Vector3 lookDirection;
				if (this.IsCollisionWithStanley(this.clickDistance) && flag2 && this.IsStanleyLookingAtMe(out lookDirection))
				{
					this.coolDownTimeRemaining = this.coolDownPeriod;
					float t = Mathf.InverseLerp(0f, this.longClickTime, this.kickTime);
					float force = Mathf.Lerp(this.clickForce, this.longClickForce, t);
					this.bounceCount = 0;
					this.AddKickForceToBall(force, lookDirection, 1f);
					this.PlayHardKick(Mathf.Lerp(0.5f, 1f, t));
				}
			}
		}
	}

	private bool IsStanleyLookingAtMe(out Vector3 lookDirection)
	{
		lookDirection = StanleyController.Instance.camTransform.forward;
		foreach (RaycastHit raycastHit in Physics.RaycastAll(StanleyController.Instance.camTransform.position, StanleyController.Instance.camTransform.forward, this.clickDistance * 2f))
		{
			RL_Ball componentInParent = raycastHit.collider.GetComponentInParent<RL_Ball>();
			if (componentInParent != null && componentInParent == this)
			{
				return true;
			}
		}
		return false;
	}

	private void AddKickForceToBall(float force)
	{
		this.AddKickForceToBall(force, Vector3.up, 0f);
	}

	private void AddKickForceToBall(float force, Vector3 lookDirection, float lookBlend)
	{
		Vector3 a = Vector3.Slerp((base.transform.position - StanleyController.StanleyPosition).normalized, lookDirection, lookBlend);
		base.GetComponent<Rigidbody>().AddForce(a * force);
	}

	private float DistanceToStanleyCenter
	{
		get
		{
			return Vector3.Distance(StanleyController.StanleyPosition, base.transform.position);
		}
	}

	private bool IsCollisionWithStanley(float distanceToStanley)
	{
		float num = this.Radius + distanceToStanley;
		if (this.DistanceToStanleyCenter < num + StanleyController.Instance.GetComponent<CharacterController>().height)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, num, this.stanleyMask.value);
			if (array.Length != 0)
			{
				if (Array.FindIndex<Collider>(array, (Collider c) => c.gameObject == StanleyController.Instance.gameObject) != -1)
				{
					return true;
				}
			}
		}
		return false;
	}

	private StanleyData.FootstepSounds FindFootstepFromMaterialUnderBall()
	{
		foreach (RaycastHit raycastHit in Physics.RaycastAll(new Ray(base.transform.position, Vector3.down), this.Radius + 0.25f))
		{
			if (!(raycastHit.collider.GetComponentInChildren<RL_Ball>() != null))
			{
				MeshCollider meshCollider = raycastHit.collider as MeshCollider;
				if (meshCollider != null && meshCollider.sharedMesh != null)
				{
					Mesh sharedMesh = meshCollider.sharedMesh;
					Renderer component = raycastHit.collider.GetComponent<Renderer>();
					int triangleIndex = raycastHit.triangleIndex;
					if (sharedMesh.triangles.Length >= triangleIndex * 3 + 3 && triangleIndex >= 0)
					{
						int num = sharedMesh.triangles[triangleIndex * 3];
						int num2 = sharedMesh.triangles[triangleIndex * 3 + 1];
						int num3 = sharedMesh.triangles[triangleIndex * 3 + 2];
						int num4 = -1;
						for (int j = 0; j < sharedMesh.subMeshCount; j++)
						{
							int[] triangles = sharedMesh.GetTriangles(j);
							for (int k = 0; k < triangles.Length; k += 3)
							{
								if (triangles[k] == num && triangles[k + 1] == num2 && triangles[k + 2] == num3)
								{
									num4 = j;
									break;
								}
							}
							if (num4 != -1)
							{
								break;
							}
						}
						if (num4 != -1)
						{
							if (component.materials[num4].HasProperty("_FootstepType"))
							{
								return (StanleyData.FootstepSounds)component.materials[num4].GetInt("_FootstepType");
							}
							return StanleyData.FootstepSounds.Missing;
						}
					}
				}
				else
				{
					MeshRenderer component2 = raycastHit.collider.GetComponent<MeshRenderer>();
					if (component2 != null && component2.sharedMaterials.Length != 0 && component2.sharedMaterial.HasProperty("_FootstepType"))
					{
						return (StanleyData.FootstepSounds)component2.sharedMaterial.GetFloat("_FootstepType");
					}
				}
			}
		}
		return StanleyData.FootstepSounds.None;
	}

	[Header("Visuals")]
	[Range(0f, 1f)]
	public float flashAmount;

	public MeshRenderer meshRenderer;

	public string flashShaderKey;

	[Header("Kick Mechanics")]
	public LayerMask stanleyMask;

	public float coolDownPeriod = 1f;

	private float coolDownTimeRemaining;

	public float touchForce = 350f;

	public float touchForceAtFullSpeed = 350f;

	public float clickForce = 1000f;

	public float longClickForce = 2000f;

	public float longClickTime = 0.25f;

	public float clickDistance = 4f;

	[Header("Audio")]
	public PlaySoundFromAudioCollection kickSound;

	public PlaySoundFromAudioCollection kickPulseSound;

	public PlaySoundFromAudioCollection bounceSound;

	public PlaySoundFromAudioCollection bounceOnBallSound;

	public AudioSource rollingLoop;

	public float ballCollisionSpeedForMaxVolume = 8f;

	public float surfaceCollisionSpeedForMaxVolume = 8f;

	public AnimationCurve rollVolumeBySpeed;

	[Header("Other")]
	public float depthToDisableRenderer = -32f;

	[Header("DEBUG")]
	public float kickTime;

	public float DEBUG_DIST;

	public List<Collider> surfaceObjectsTouched = new List<Collider>();

	public LinkedList<int> surfaceCollidersCount = new LinkedList<int>();

	public int rollingSurfaceColliderCount = -1;

	public float rollingSpeedNormalized;

	public int bounceCount;

	public float ballVelocity;

	public StanleyData.FootstepSounds footstepTypeUnderBall;

	private Rigidbody _rb;
}

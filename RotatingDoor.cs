using System;
using System.Collections.Generic;
using UnityEngine;

public class RotatingDoor : HammerEntity
{
	public bool PlayerTouchingDoor { get; set; }

	public bool IsMoving
	{
		get
		{
			return this.currentAngle != this.targetAngle;
		}
	}

	private void Awake()
	{
		this.hasDoorBlocker = (this.doorBlocker != null);
		if (this.brushAxisFixup)
		{
			this.dirChange *= -1f;
			if (this.reverseDirection)
			{
				this.dirChange *= -1f;
			}
		}
		this._renderer = base.GetComponent<MeshRenderer>();
		this._collider = base.GetComponent<BoxCollider>();
		if (this.renderColor != Color.white)
		{
			this._renderer.material.SetColor("_Color", this.renderColor);
		}
		if (!this.isDrawing)
		{
			this._renderer.enabled = false;
			this._collider.enabled = false;
		}
		this.closedRotation = base.transform.rotation;
		this.openRotation = Quaternion.AngleAxis(this.maxAngle * this.dirChange, this.axis) * base.transform.rotation;
		this.currentAngle = this.spawnAngle * this.dirChange;
		this.targetAngle = this.currentAngle;
		base.transform.rotation = Quaternion.AngleAxis(this.currentAngle, this.axis) * base.transform.rotation;
		this.source = base.GetComponent<AudioSource>();
	}

	private void OnValidate()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (component.interpolation != RigidbodyInterpolation.Interpolate)
			{
				Debug.LogWarning("Please set door rigidbody interpolation to Interpolate", this);
			}
			if (component.collisionDetectionMode != CollisionDetectionMode.Continuous && component.collisionDetectionMode != CollisionDetectionMode.ContinuousSpeculative)
			{
				Debug.LogWarning("Please set door rigidbody collision mode to Continuous Speculative", this);
			}
		}
		if (this.doorBlocker != null && this.doorBlocker.transform.parent != base.transform)
		{
			this.doorBlocker = null;
		}
	}

	[ContextMenu("SetTargetAngleTo0")]
	public void SetTargetAngleTo0()
	{
		this.targetAngle = 0f;
	}

	[ContextMenu("SetTargetAngleTo90")]
	public void SetTargetAngleTo90()
	{
		this.targetAngle = 90f;
	}

	private void Update()
	{
		if (!this.isDrawing)
		{
			return;
		}
		if (this.currentAngle == this.targetAngle)
		{
			return;
		}
		if (this.touchingPlayer)
		{
			return;
		}
		if (this.PlayerTouchingDoor)
		{
			return;
		}
		if (this.directions == RotatingDoor.DoorDirections.BackwardOnly && this.targetAngle > 0f)
		{
			this.targetAngle = 0f;
		}
		if (this.directions == RotatingDoor.DoorDirections.ForwardOnly && this.targetAngle < 0f)
		{
			this.targetAngle = 0f;
		}
		this.currentAngle = Mathf.MoveTowards(this.currentAngle, this.targetAngle, this.speed * Singleton<GameMaster>.Instance.GameDeltaTime);
		base.transform.rotation = Quaternion.AngleAxis(this.currentAngle, this.axis) * this.closedRotation;
		if (this.currentAngle == this.targetAngle)
		{
			if (Mathf.Abs(this.currentAngle) == this.maxAngle)
			{
				base.FireOutput(Outputs.OnFullyOpen);
				this.PlaySound(RotatingDoor.Sounds.Open);
				return;
			}
			if (this.currentAngle == 0f)
			{
				base.FireOutput(Outputs.OnFullyClosed);
				this.PlaySound(RotatingDoor.Sounds.Close);
			}
		}
	}

	[ContextMenu("Update Door Blocker Collider")]
	private void UpdateDoorBlockerColliderDymanicPosition()
	{
		float x = base.GetComponent<BoxCollider>().size.x;
		float num = Quaternion.Angle(this.closedRotation, base.transform.rotation);
		if (this.reverseDoorBlockerDirection)
		{
			num *= -1f;
		}
		this.doorBlocker.transform.localPosition = new Vector3(-x, 0f, 0f);
		this.doorBlocker.transform.localEulerAngles = new Vector3(0f, 0f, (180f - num) / 2f);
		float num2 = 2f * x * Mathf.Sin(0.017453292f * num / 2f);
		Vector3 size = this.doorBlocker.size;
		Vector3 center = this.doorBlocker.center;
		size.x = num2;
		center.x = num2 / 2f;
		this.doorBlocker.size = size;
		this.doorBlocker.center = center;
	}

	private void DoorBlockerUpdate()
	{
		if (!this.IsMoving)
		{
			if (this.doorBlocker.enabled)
			{
				this.doorBlocker.enabled = false;
			}
			return;
		}
		if (!this.doorBlocker.enabled)
		{
			this.doorBlocker.enabled = true;
		}
		if (this.doorBlockerMode == RotatingDoor.DoorBlockerMode.DynamicAndEnabledOnMoving)
		{
			this.UpdateDoorBlockerColliderDymanicPosition();
		}
	}

	private void PlaySound(RotatingDoor.Sounds sound)
	{
		if (!this.source)
		{
			return;
		}
		switch (sound)
		{
		case RotatingDoor.Sounds.Open:
			if (this.openClips.Count > 0)
			{
				this.source.pitch = this.openPitchRange.Random();
				this.source.volume = this.openVol;
				this.source.PlayOneShot(this.openClips[Random.Range(0, this.openClips.Count)]);
				return;
			}
			break;
		case RotatingDoor.Sounds.Close:
			if (this.closeClips.Count > 0)
			{
				this.source.pitch = this.closePitchRange.Random();
				this.source.volume = this.closeVol;
				this.source.PlayOneShot(this.closeClips[Random.Range(0, this.closeClips.Count)]);
				return;
			}
			break;
		case RotatingDoor.Sounds.Move:
			if (this.moveClips.Count > 0)
			{
				this.source.pitch = this.movePitchRange.Random();
				this.source.volume = this.moveVol;
				this.source.PlayOneShot(this.moveClips[Random.Range(0, this.moveClips.Count)]);
				return;
			}
			break;
		case RotatingDoor.Sounds.Locked:
			if (this.lockedClips.Count > 0)
			{
				this.source.pitch = this.lockedPitchRange.Random();
				this.source.volume = this.lockedVol;
				this.source.PlayOneShot(this.lockedClips[Random.Range(0, this.lockedClips.Count)]);
			}
			break;
		default:
			return;
		}
	}

	private void FixedUpdate()
	{
		if (this.hasDoorBlocker)
		{
			this.DoorBlockerUpdate();
		}
		this.touchingPlayer = false;
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player")
		{
			this.touchingPlayer = true;
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			this.touchingPlayer = true;
		}
	}

	public void Input_EnableDraw()
	{
		this._renderer.enabled = true;
		this._collider.enabled = true;
	}

	public void Input_DisableDraw()
	{
		this._renderer.enabled = false;
		this._collider.enabled = false;
	}

	public override void Use()
	{
		if (this.usable)
		{
			if (this.isLocked)
			{
				base.FireOutput(Outputs.OnLockedUse);
				this.PlaySound(RotatingDoor.Sounds.Locked);
			}
			this.Input_Toggle();
		}
	}

	public void Input_Toggle()
	{
		if (this.isLocked)
		{
			return;
		}
		if (this.targetAngle != this.currentAngle)
		{
			return;
		}
		if (Mathf.Abs(this.targetAngle) == this.maxAngle)
		{
			this.Input_Close();
			return;
		}
		if (this.targetAngle == 0f)
		{
			this.Input_Open();
		}
	}

	public void Input_Open()
	{
		if (this.isLocked)
		{
			return;
		}
		base.FireOutput(Outputs.OnOpen);
		if (this.directions == RotatingDoor.DoorDirections.BackwardOnly)
		{
			this.targetAngle = -this.maxAngle;
		}
		else
		{
			this.targetAngle = this.maxAngle;
		}
		this.PlaySound(RotatingDoor.Sounds.Move);
		this.targetAngle *= this.dirChange;
	}

	public void Input_Close()
	{
		this.targetAngle = 0f;
		base.FireOutput(Outputs.OnClose);
		if (Mathf.Abs(this.currentAngle) == 90f)
		{
			this.PlaySound(RotatingDoor.Sounds.Move);
		}
	}

	public void Input_Lock()
	{
		this.isLocked = true;
	}

	public void Input_Unlock()
	{
		this.isLocked = false;
	}

	private MeshRenderer _renderer;

	private BoxCollider _collider;

	public bool isDrawing = true;

	public float spawnAngle;

	private float currentAngle;

	private float targetAngle;

	public float maxAngle = 90f;

	public bool reverseDirection;

	public bool brushAxisFixup;

	private float dirChange = 1f;

	public Vector3 axis = Vector3.up;

	public float speed = 120f;

	public bool isLocked;

	public RotatingDoor.DoorDirections directions;

	public Color renderColor = Color.white;

	public bool usable;

	private bool touchingPlayer;

	private Quaternion closedRotation;

	private Quaternion openRotation;

	public List<AudioClip> openClips;

	public float openVol = 1f;

	public MinMax openPitchRange;

	public List<AudioClip> closeClips;

	public float closeVol = 1f;

	public MinMax closePitchRange;

	public List<AudioClip> moveClips;

	public float moveVol = 1f;

	public MinMax movePitchRange;

	public List<AudioClip> lockedClips;

	public float lockedVol = 1f;

	public MinMax lockedPitchRange;

	private AudioSource source;

	[Header("Null for no blocker, must be a direct child of this class")]
	public BoxCollider doorBlocker;

	private bool hasDoorBlocker;

	public RotatingDoor.DoorBlockerMode doorBlockerMode;

	[InspectorButton("MIGRATION_PerformMeshColliderSwapWithBoxCollider", null)]
	public bool reverseDoorBlockerDirection;

	public enum DoorDirections
	{
		Both,
		ForwardOnly,
		BackwardOnly
	}

	public enum DoorBlockerMode
	{
		StaticAndEnabledOnMoving,
		DynamicAndEnabledOnMoving
	}

	private enum Sounds
	{
		Open,
		Close,
		Move,
		Locked
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanleyController : PortalTraveller
{
	public static StanleyController Instance
	{
		get
		{
			if (StanleyController._instance == null)
			{
				GameMaster instance = Singleton<GameMaster>.Instance;
				StanleyController._instance = ((instance != null) ? instance.RespawnStanley() : null);
			}
			return StanleyController._instance;
		}
	}

	public Transform stanleyTransform { get; private set; }

	public Camera cam { get; private set; }

	public Camera currentCam { get; private set; }

	public Transform camParent { get; private set; }

	public Transform camTransform { get; private set; }

	public float WalkingSpeedMultiplier { get; private set; } = 1f;

	public bool WalkingSpeedAffectsFootstepSoundSpeed
	{
		set
		{
			this.WalkingSpeedAffectsFootstepSoundSpeedScale = (float)(value ? 1 : 0);
		}
	}

	public float WalkingSpeedAffectsFootstepSoundSpeedScale { get; set; }

	public void SetMovementSpeedMultiplier(float newMultiplier)
	{
		this.WalkingSpeedMultiplier = newMultiplier;
	}

	public void SetCharacterHeightMultiplier(float newMultiplier)
	{
		this.characterHeightMultipler = newMultiplier;
	}

	public bool ForceStayCrouched { get; set; }

	private bool SnapToNewHeightNextFrame { get; set; }

	public bool ForceCrouched { get; set; }

	public void ResetVelocity()
	{
		this.movement = (this.movementGoal = Vector3.zero);
	}

	public float FieldOfView
	{
		get
		{
			return this.FieldOfViewBase + this.FieldOfViewAdditiveModifier;
		}
		set
		{
			this.FieldOfViewAdditiveModifier = value - this.FieldOfViewBase;
		}
	}

	public float FieldOfViewBase { get; private set; }

	public float FieldOfViewAdditiveModifier { get; set; }

	private void OnFOVChange(LiveData liveData)
	{
		float fieldOfViewBase = this.FieldOfViewBase;
		float fieldOfViewAdditiveModifier = this.FieldOfViewAdditiveModifier;
		float num = fieldOfViewBase + fieldOfViewAdditiveModifier;
		this.FieldOfViewBase = liveData.FloatValue;
		if (this.FieldOfViewAdditiveModifier != 0f)
		{
			this.FieldOfViewAdditiveModifier = num - this.FieldOfViewBase;
		}
	}

	private void Awake()
	{
		if (StanleyController._instance == null)
		{
			StanleyController._instance = this;
		}
		if (StanleyController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (this != StanleyController.Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		this.stanleyTransform = base.transform;
		this.character = base.GetComponent<CharacterController>();
		this.cam = base.GetComponentInChildren<Camera>();
		this.camTransform = this.cam.transform;
		this.camParent = this.camTransform.parent;
		AssetBundleControl.OnScenePreLoad = (Action)Delegate.Combine(AssetBundleControl.OnScenePreLoad, new Action(this.OnScenePreLoad));
		AssetBundleControl.OnSceneReady = (Action)Delegate.Combine(AssetBundleControl.OnSceneReady, new Action(this.OnSceneReady));
		this.AColor = this.cam.gameObject.GetComponent<AmplifyColorBase>();
		this.camParentOrigLocalPos = this.camParent.localPosition;
		this.CreateFootstepSources();
		this.CreateFootstepDictionary();
		this.mainCamera = base.GetComponentInChildren<MainCamera>();
		FloatConfigurable floatConfigurable = this.fovSettingConfigurable;
		floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(floatConfigurable.OnValueChanged, new Action<LiveData>(this.OnFOVChange));
		this.FieldOfViewBase = this.fovSettingConfigurable.GetFloatValue();
		this.gravityMultiplier = this.groundedGravityMultiplier;
	}

	private void Start()
	{
		this.OnSceneReady();
	}

	private void OnDestroy()
	{
		FloatConfigurable floatConfigurable = this.fovSettingConfigurable;
		floatConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(floatConfigurable.OnValueChanged, new Action<LiveData>(this.OnFOVChange));
		AssetBundleControl.OnSceneReady = (Action)Delegate.Remove(AssetBundleControl.OnSceneReady, new Action(this.OnSceneReady));
	}

	private void CreateFootstepDictionary()
	{
		for (int i = 0; i < this.footstepCollections.Length; i++)
		{
			AudioCollection audioCollection = this.footstepCollections[i];
			int index = audioCollection.GetIndex();
			if (audioCollection != null && !this.footstepDictionary.ContainsKey(index))
			{
				this.footstepDictionary.Add(index, audioCollection);
			}
		}
	}

	private void CreateFootstepSources()
	{
		int num = 8;
		this.footstepSources = new AudioSource[num];
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.footstepSource.gameObject);
			this.footstepSources[i] = gameObject.GetComponent<AudioSource>();
		}
		for (int j = 0; j < this.footstepSources.Length; j++)
		{
			this.footstepSources[j].transform.parent = this.footstepSource.transform;
		}
	}

	private void OnScenePreLoad()
	{
		Singleton<GameMaster>.Instance;
		this.WalkingSpeedMultiplier = 1f;
		this.WalkingSpeedAffectsFootstepSoundSpeed = false;
		this.characterHeightMultipler = 1f;
		this.ResetVelocity();
		this.FieldOfViewAdditiveModifier = 0f;
		this.wasCrouching = false;
		this.ForceCrouched = false;
		this.ForceStayCrouched = false;
		this.SnapToNewHeightNextFrame = true;
		if (this.cam != null)
		{
			this.cam.farClipPlane = 50f;
		}
		this.cam.gameObject.transform.localPosition = Vector3.zero;
		this.cam.gameObject.transform.localRotation = Quaternion.identity;
		this.Bucket.SetWalkingSpeed(0f);
		this.Bucket.SetAnimationSpeed(1f);
		this.FreezeMotionAndView();
	}

	private void OnSceneReady()
	{
		Singleton<GameMaster>.Instance;
		this.masterStartFound = false;
		this.inAirTimer = 0f;
		this.outOfBoundsReported = false;
		this.executeJump = false;
		this.jumpValue = 0f;
		this.jumpTime = 0f;
	}

	private void FixedUpdate()
	{
		this.velocityAccumulation += this.character.velocity.magnitude * Time.fixedDeltaTime;
		this.velocityAccumulation += Mathf.Abs(this.character.velocity.y * Time.fixedDeltaTime) * this.footstepsYMultiplier;
	}

	public static bool AltKeyPressed
	{
		get
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}
	}

	private void Update()
	{
		if (!Singleton<GameMaster>.Instance.IsLoading && GameMaster.ONMAINMENUORSETTINGS)
		{
			AudioListener.volume = Singleton<GameMaster>.Instance.masterVolume;
		}
		StanleyController.StanleyPosition = base.transform.position;
		this.cam.fieldOfView = this.FieldOfViewBase + this.FieldOfViewAdditiveModifier;
		if (!this.viewFrozen)
		{
			this.View();
		}
		if (!this.motionFrozen)
		{
			this.Movement();
			this.UpdateCurrentlyStandingOn();
			this.Footsteps();
			this.ClickingOnThings();
		}
		else if (this.character.enabled)
		{
			this.character.Move(Vector2.zero);
		}
		if (!this.viewFrozen)
		{
			this.FloatCamera();
		}
		if (BucketController.HASBUCKET)
		{
			if (this.character.enabled && this.grounded)
			{
				this.Bucket.SetWalkingSpeed(this.character.velocity.magnitude / (this.walkingSpeed * this.WalkingSpeedMultiplier));
				return;
			}
			this.Bucket.SetWalkingSpeed(0f);
		}
	}

	private float DeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	private float SensitivityRemap(float n)
	{
		return n * n + 0.5f * n + 0.5f;
	}

	private void View()
	{
		Vector2 vector = Singleton<GameMaster>.Instance.stanleyActions.View.Vector;
		Vector2 vector2 = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		float n = Mathf.InverseLerp(this.mouseSensitivityConfigurable.MinValue, this.mouseSensitivityConfigurable.MaxValue, this.mouseSensitivityConfigurable.GetFloatValue());
		float num = this.SensitivityRemap(n);
		vector2.x *= num;
		vector2.y *= num;
		if (this.invertYConfigurable.GetBooleanValue())
		{
			vector2.y *= -1f;
		}
		float n2 = Mathf.InverseLerp(this.controllerSensitivityConfigurable.MinValue, this.controllerSensitivityConfigurable.MaxValue, this.controllerSensitivityConfigurable.GetFloatValue());
		float num2 = this.SensitivityRemap(n2);
		vector.x *= num2;
		vector.y *= num2;
		if (this.invertYConfigurable.GetBooleanValue())
		{
			vector.y *= -1f;
		}
		if (Time.deltaTime > 0f)
		{
			vector2 /= Time.deltaTime * 60f;
		}
		if (Input.touchCount > 0)
		{
			vector2 = Vector2.zero;
		}
		float num3 = -1f;
		if (this.yInvert)
		{
			num3 = 1f;
		}
		vector2 = new Vector2(vector2.x * this.mouseSensitivityX, vector2.y * num3 * this.mouseSensitivityY);
		vector = new Vector2(vector.x * this.controllerSensitivityX, vector.y * num3 * this.controllerSensitivityY);
		Vector2 vector3 = Vector2.zero;
		vector3 = vector2 + vector * 0.25f;
		vector3 *= this.DeltaTime * 70f;
		Quaternion lhs = Quaternion.AngleAxis(vector3.x, Vector3.up);
		this.viewPitch += vector3.y;
		this.viewPitch = Mathf.Clamp(this.viewPitch, -90f, 90f);
		if (BackgroundCamera.OnRotationUpdate != null)
		{
			BackgroundCamera.OnRotationUpdate(new Vector3(vector3.y, vector3.x));
		}
		this.camParent.localRotation = Quaternion.AngleAxis(this.viewPitch, Vector3.right);
		base.transform.rotation = lhs * base.transform.rotation;
	}

	private bool RayHitGround(Vector3 offset, out GameObject hitGameObject, out int hitTriangleIndex)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + offset, Vector3.down, out raycastHit, 3f, this.groundedLayers))
		{
			this.hitNormal = raycastHit.normal;
			hitGameObject = raycastHit.collider.gameObject;
			hitTriangleIndex = raycastHit.triangleIndex;
			return true;
		}
		hitGameObject = null;
		hitTriangleIndex = -1;
		return false;
	}

	private void Movement()
	{
		this.grounded = this.character.isGrounded;
		float y = Singleton<GameMaster>.Instance.stanleyActions.Movement.Y;
		float x = Singleton<GameMaster>.Instance.stanleyActions.Movement.X;
		this.movementInput.x = x;
		this.movementInput.z = y;
		if (PlatformSettings.Instance.isStandalone.GetBooleanValue() && this.mouseWalkConfigurable.GetBooleanValue() && Input.GetMouseButton(1) && Input.GetMouseButton(0))
		{
			this.movementInput.z = 1f;
		}
		this.movementInput = Vector3.ClampMagnitude(this.movementInput, 1f) * (this.executeJump ? 0.5f : 1f);
		if (this.movementInput.magnitude > 0f)
		{
			this.movementGoal = Vector3.Lerp(this.movementGoal, this.movementInput, this.DeltaTime * this.runAcceleration);
		}
		else
		{
			this.movementGoal = Vector3.Lerp(this.movementGoal, this.movementInput, this.DeltaTime * this.runDeacceleration);
		}
		if (!this.executeJump && this.jumpConfigurable.GetBooleanValue() && Singleton<GameMaster>.Instance.stanleyActions.JumpAction.WasPressed)
		{
			if (!this.executeJump && StanleyController.OnActuallyJumping != null)
			{
				StanleyController.OnActuallyJumping();
			}
			this.executeJump = true;
		}
		if (this.executeJump)
		{
			this.jumpTime += this.DeltaTime * this.jumpAccerlation;
			this.jumpValue = this.jumpCurve.Evaluate(Mathf.Clamp01(this.jumpTime)) * this.jumpPower;
		}
		if (this.jumpTime >= 1f / this.jumpAccerlation * this.jumpAccerlation)
		{
			this.executeJump = false;
			this.jumpValue = 0f;
			this.jumpTime = 0f;
		}
		bool flag = Singleton<GameMaster>.Instance.stanleyActions.Crouch.IsPressed;
		if (this.wasCrouching && this.ForceStayCrouched)
		{
			flag = true;
		}
		if (this.ForceCrouched)
		{
			flag = true;
		}
		float num;
		if (flag)
		{
			num = this.crouchedColliderHeight;
		}
		else
		{
			num = this.uncrouchedColliderHeight;
		}
		this.character.height = Mathf.SmoothStep(this.character.height, num, this.crouchSmoothing);
		if (this.SnapToNewHeightNextFrame)
		{
			this.character.height = num;
			this.SnapToNewHeightNextFrame = false;
		}
		this.cameraParent.localPosition = Vector3.up * this.character.height / 2f * this.characterHeightMultipler;
		this.camParentOrigLocalPos = this.camParent.localPosition;
		this.wasCrouching = flag;
		this.movement = this.movementGoal * this.walkingSpeed * this.WalkingSpeedMultiplier;
		this.movement = base.transform.TransformDirection(this.movement);
		this.movement += Vector3.up * this.jumpValue;
		Action<Vector3> onPositionUpdate = BackgroundCamera.OnPositionUpdate;
		if (onPositionUpdate != null)
		{
			onPositionUpdate(new Vector3(0f, this.character.velocity.y, 0f));
		}
		RotatingDoor rotatingDoor = this.WillHitDoor(this.movement * Singleton<GameMaster>.Instance.GameDeltaTime);
		if (rotatingDoor == null)
		{
			if (this.lastHitRotatingDoor != null)
			{
				this.lastHitRotatingDoor.PlayerTouchingDoor = false;
				this.lastHitRotatingDoor = null;
			}
		}
		else
		{
			if (this.lastHitRotatingDoor != null && this.lastHitRotatingDoor != rotatingDoor)
			{
				Debug.LogWarning("Player is hitting multiple doors this should not happen!\n" + this.lastHitRotatingDoor.name + "\n" + rotatingDoor.name);
			}
			this.lastHitRotatingDoor = rotatingDoor;
			this.lastHitRotatingDoor.PlayerTouchingDoor = true;
		}
		this.UpdateInAir(!this.grounded);
		if (!this.grounded)
		{
			this.gravityMultiplier = Mathf.Lerp(this.gravityMultiplier, 1f, Singleton<GameMaster>.Instance.GameDeltaTime * this.gravityFallAcceleration);
			this.movement *= this.inAirMovementMultiplier;
		}
		else
		{
			this.gravityMultiplier = this.groundedGravityMultiplier;
		}
		if (flag)
		{
			this.movement *= this.crouchMovementMultiplier;
		}
		if (this.character.enabled)
		{
			this.character.Move((this.movement + Vector3.up * this.maxGravity * this.gravityMultiplier) * Singleton<GameMaster>.Instance.GameDeltaTime);
		}
	}

	private void UpdateInAir(bool inAir)
	{
		if (!this.outOfBoundsReported && inAir && this.character.velocity.y <= -1f && !GameMaster.PAUSEMENUACTIVE && !Singleton<GameMaster>.Instance.IsLoading)
		{
			this.inAirTimer += Time.deltaTime;
			if (this.inAirTimer >= this.inAirLimit)
			{
				if (StanleyController.OnOutOfBounds != null)
				{
					StanleyController.OnOutOfBounds();
				}
				this.outOfBoundsReported = true;
				return;
			}
		}
		else
		{
			this.inAirTimer = 0f;
		}
	}

	private RotatingDoor WillHitDoor(Vector3 motion)
	{
		Vector3 a = this.character.transform.position + motion + this.character.center;
		foreach (Collider collider in Physics.OverlapCapsule(a - Vector3.up * this.character.height / 2f, a + Vector3.up * this.character.height / 2f, this.character.radius))
		{
			RotatingDoor component = collider.gameObject.GetComponent<RotatingDoor>();
			if (component != null)
			{
				Vector3 b = collider.ClosestPoint(this.character.transform.position);
				Vector3 a2 = this.character.transform.position - b;
				float magnitude = a2.magnitude;
				if (magnitude < this.character.radius)
				{
					if (magnitude == 0f)
					{
						Debug.LogError("[StanleyController] Distance to door should NEVER be zero, check if door collider is MeshCollider, replace it with a Box Collider please");
					}
					else
					{
						Vector3 a3 = a2 / magnitude;
						float d = this.character.radius - magnitude;
						this.character.Move(a3 * d);
					}
				}
				return component;
			}
		}
		return null;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		this.hitNormal = hit.normal;
	}

	private void ClickingOnThings()
	{
		if (!Singleton<GameMaster>.Instance.FullScreenMoviePlaying && Singleton<GameMaster>.Instance.stanleyActions.UseAction.WasPressed)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.camParent.position, this.camParent.forward, out raycastHit, this.armReach, this.clickLayers, QueryTriggerInteraction.Ignore))
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				HammerEntity component = gameObject.GetComponent<HammerEntity>();
				if (component != null)
				{
					component.Use();
				}
				else
				{
					this.PlayKeyboardSound();
				}
				if (StanleyController.OnInteract != null)
				{
					StanleyController.OnInteract(gameObject);
					return;
				}
			}
			else
			{
				this.PlayKeyboardSound();
				if (StanleyController.OnInteract != null)
				{
					StanleyController.OnInteract(null);
				}
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && this.character.enabled)
		{
			this.character.Move(Vector2.zero);
		}
	}

	private void PlayFootstepSound(AudioEntry entry)
	{
		AudioClip clip2;
		bool clip = entry.GetClip(out clip2);
		AudioSource availableFootstepSource = this.GetAvailableFootstepSource();
		if (clip && availableFootstepSource != null)
		{
			availableFootstepSource.clip = clip2;
			availableFootstepSource.pitch = entry.GetPitch();
			availableFootstepSource.volume = entry.GetVolume();
			availableFootstepSource.Play();
		}
	}

	private AudioSource GetAvailableFootstepSource()
	{
		for (int i = 0; i < this.footstepSources.Length; i++)
		{
			AudioSource audioSource = this.footstepSources[i];
			if (!audioSource.isPlaying)
			{
				return audioSource;
			}
		}
		return null;
	}

	private void PlayKeyboardSound()
	{
		this.useSource.Stop();
		(Singleton<GameMaster>.Instance.barking ? this.barkingCollection : this.keyboardCollection).SetVolumeAndPitchAndPlayClip(this.useSource);
	}

	public void Teleport(StanleyController.TeleportType style, Vector3 destination, Vector3 up, bool useAngle, bool freezeAtStartOfTeleport = true, bool unfreezeAtEndOfTeleport = true, Transform orientationTransform = null)
	{
		this.Teleport(style, base.transform.position, destination, up, useAngle, freezeAtStartOfTeleport, unfreezeAtEndOfTeleport, orientationTransform);
	}

	public void Teleport(StanleyController.TeleportType style, Vector3 landmark, Vector3 destination, Vector3 up, bool useAngle, bool freezeAtStartOfTeleport = true, bool unfreezeAtEndOfTeleport = true, Transform orientationTransform = null)
	{
		bool flag = true;
		if (freezeAtStartOfTeleport)
		{
			this.FreezeMotionAndView();
		}
		switch (style)
		{
		case StanleyController.TeleportType.PlayerStart:
			flag = !this.masterStartFound;
			break;
		case StanleyController.TeleportType.PlayerStartMaster:
			this.masterStartFound = true;
			break;
		case StanleyController.TeleportType.TriggerTeleport:
			flag = true;
			break;
		}
		if (flag)
		{
			Vector3 b = base.transform.position - landmark;
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(destination, Vector3.down), out raycastHit, 2f, this.groundedLayers, QueryTriggerInteraction.Ignore))
			{
				Vector3 vector = destination + b;
				float num = StanleyController.Instance.character.height / 2f + StanleyController.Instance.character.skinWidth;
				base.transform.position = new Vector3(vector.x, raycastHit.point.y + num, vector.z);
			}
			else
			{
				base.transform.position = destination + b + Vector3.up * 0.05f;
			}
			this.velocityAccumulation = 0f;
			if (useAngle && orientationTransform != null)
			{
				base.transform.rotation = orientationTransform.rotation;
				base.transform.Rotate(90f, 0f, 0f, Space.Self);
				float num2 = Vector3.Angle(base.transform.up, Vector3.up);
				base.transform.Rotate(-num2, 0f, 0f, Space.Self);
				float value = Vector3.Angle(base.transform.forward, -orientationTransform.up);
				this.viewPitch = Mathf.Clamp(value, -90f, 90f);
				this.camParent.localRotation = Quaternion.AngleAxis(this.viewPitch, Vector3.right);
			}
			else if (useAngle)
			{
				Vector3 rhs = Vector3.Cross(Vector3.up, up);
				Vector3 vector2 = Vector3.Cross(up, rhs);
				Vector3 vector3 = new Vector3(up.x, 0f, up.z);
				if (vector3 != Vector3.zero)
				{
					base.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
				}
				Vector3 vector4 = new Vector3(0f, vector2.y, 0f);
				if (vector4 != Vector3.zero)
				{
					this.viewPitch = Vector3.Angle(Vector3.up, vector4);
					this.camParent.localRotation = Quaternion.AngleAxis(this.viewPitch, Vector3.right);
					if (BackgroundCamera.OnAlignToTransform != null)
					{
						BackgroundCamera.OnAlignToTransform(this.camParent);
					}
				}
			}
			if ((style == StanleyController.TeleportType.PlayerStart || style == StanleyController.TeleportType.PlayerStartMaster) && StanleyController.OnTeleportToPlayerStart != null)
			{
				StanleyController.OnTeleportToPlayerStart();
			}
		}
		if (unfreezeAtEndOfTeleport)
		{
			this.UnfreezeMotionAndView();
		}
	}

	public void Deparent(bool kill = false)
	{
		GameObject gameObject = null;
		if (base.transform.parent != null)
		{
			gameObject = base.transform.parent.gameObject;
		}
		base.transform.parent = null;
		if (kill && gameObject != null)
		{
			Object.Destroy(gameObject);
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ParentTo(Transform adopter)
	{
		base.transform.parent = adopter;
	}

	public void EnableCamera()
	{
		this.cam.enabled = true;
		this.currentCam = this.cam;
	}

	public void DisableCamera(Camera replacement)
	{
		this.cam.enabled = false;
		this.currentCam = replacement;
	}

	public void FreezeMotion(bool commandFreezeMotion = false)
	{
		this.motionFrozen = true;
		this.character.enabled = false;
		if (commandFreezeMotion)
		{
			this.frozenFromCommandMotion = true;
		}
	}

	public void UnfreezeMotion(bool commandUnfreezeMotion = false)
	{
		if (!commandUnfreezeMotion && this.frozenFromCommandMotion)
		{
			return;
		}
		this.motionFrozen = false;
		this.character.enabled = true;
		if (commandUnfreezeMotion && this.frozenFromCommandMotion)
		{
			this.frozenFromCommandMotion = false;
		}
	}

	public void FreezeView(bool commandFreezeView = false)
	{
		this.viewFrozen = true;
		if (commandFreezeView)
		{
			this.frozenFromCommandView = true;
		}
	}

	public void UnfreezeView(bool commandUnfreezeView = false)
	{
		if (!commandUnfreezeView && this.frozenFromCommandView)
		{
			return;
		}
		this.viewFrozen = false;
		if (commandUnfreezeView)
		{
			this.frozenFromCommandView = false;
		}
	}

	public void ResetClientCommandFreezes()
	{
		this.frozenFromCommandView = false;
		this.frozenFromCommandMotion = false;
	}

	public void FreezeMotionAndView()
	{
		this.FreezeMotion(false);
		this.FreezeView(false);
	}

	public void UnfreezeMotionAndView()
	{
		this.UnfreezeMotion(false);
		this.UnfreezeView(false);
	}

	public void StartFloating()
	{
		this.floatingPos = 0f;
		base.StartCoroutine(this.FloatFadeInOut(1f));
	}

	public void EndFloating()
	{
		base.StartCoroutine(this.FloatFadeInOut(0f));
	}

	private IEnumerator FloatFadeInOut(float targetStrength)
	{
		while (this.floatingStrength != targetStrength)
		{
			this.floatingStrength = Mathf.MoveTowards(this.floatingStrength, targetStrength, Singleton<GameMaster>.Instance.GameDeltaTime / 2f);
			yield return null;
		}
		yield break;
	}

	private void FloatCamera()
	{
		float d = Mathf.Pow(Mathf.Sin(this.floatingPos), 2f) * 0.33f;
		this.camParent.localPosition = this.camParentOrigLocalPos + Vector3.up * d * this.floatingStrength;
		this.floatingPos += Singleton<GameMaster>.Instance.GameDeltaTime / 3f;
	}

	public void NewMapReset()
	{
		this.EnableCamera();
		this.camParent.localRotation = Quaternion.identity;
		this.masterStartFound = false;
		if (this.mainCamera != null)
		{
			this.mainCamera.UpdatePortals();
		}
	}

	private void UpdateCurrentlyStandingOn()
	{
		this.standingOnUpdateTimer += Singleton<GameMaster>.Instance.GameDeltaTime;
		if (this.standingOnUpdateTimer >= this.standingOnUpdateLimit)
		{
			this.FindFootstepMaterial();
			this.standingOnUpdateTimer = 0f;
		}
	}

	private void Footsteps()
	{
		float num = (this.currentFootstepAudioCollection != null) ? this.currentFootstepAudioCollection.AverageDuration : 0.69f;
		float walkingSpeedMultiplier = this.WalkingSpeedMultiplier;
		float num2 = Mathf.Lerp(num * this.WalkingSpeedMultiplier, num, this.WalkingSpeedAffectsFootstepSoundSpeedScale);
		if (this.velocityAccumulation >= num2)
		{
			this.velocityAccumulation = 0f;
			int key = (int)this.currentlyStandingOn;
			if (this.footstepDictionary.TryGetValue(key, out this.currentFootstepAudioCollection))
			{
				AudioSource availableFootstepSource = this.GetAvailableFootstepSource();
				if (availableFootstepSource != null)
				{
					this.currentFootstepAudioCollection.SetVolumeAndPitchAndPlayClip(availableFootstepSource);
				}
			}
		}
	}

	private void FindFootstepMaterial()
	{
		StanleyData.FootstepSounds footstepSounds = this.currentlyStandingOn;
		GameObject gameObject;
		int triIndex;
		if (this.RayHitGround(Vector3.up * 0.1f, out gameObject, out triIndex))
		{
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			NonRendererFootstepType component2 = gameObject.GetComponent<NonRendererFootstepType>();
			if (component2 != null && (!(component != null) || !component2.ForceOverrideMaterial))
			{
				this.currentlyStandingOn = component2.FootstepType;
				if (this.playFootstepOnNewMaterial && footstepSounds != this.currentlyStandingOn)
				{
					this.velocityAccumulation = 99f;
				}
				return;
			}
			if (component != null)
			{
				MeshCollider component3 = gameObject.GetComponent<MeshCollider>();
				Material material;
				if (component3 != null && component.sharedMaterials.Length >= 2 && this.GetMaterialFromTriangleIndex(component, component3, triIndex, out material))
				{
					if (material.HasProperty("_FootstepType"))
					{
						this.currentlyStandingOn = (StanleyData.FootstepSounds)material.GetFloat("_FootstepType");
						if (this.playFootstepOnNewMaterial && footstepSounds != this.currentlyStandingOn)
						{
							this.velocityAccumulation = 99f;
						}
						return;
					}
				}
				else
				{
					for (int i = 0; i < component.sharedMaterials.Length; i++)
					{
						Material material2 = component.sharedMaterials[i];
						if (material2 != null && material2.HasProperty("_FootstepType"))
						{
							this.currentlyStandingOn = (StanleyData.FootstepSounds)material2.GetFloat("_FootstepType");
							if (this.playFootstepOnNewMaterial && footstepSounds != this.currentlyStandingOn)
							{
								this.velocityAccumulation = 99f;
							}
							return;
						}
					}
				}
			}
		}
		this.currentlyStandingOn = StanleyData.FootstepSounds.Silence;
	}

	private bool GetMaterialFromTriangleIndex(MeshRenderer mRenderer, MeshCollider mCollider, int triIndex, out Material foundMaterial)
	{
		if (!(mCollider.sharedMesh == null))
		{
			Mesh sharedMesh = mCollider.sharedMesh;
			if (sharedMesh.isReadable && sharedMesh.triangles.Length >= triIndex * 3 + 3)
			{
				if (triIndex < 0)
				{
					string.Concat(new object[]
					{
						"triangle index is less than zero ",
						sharedMesh.name,
						" of ",
						mRenderer.name,
						" index = ",
						triIndex
					});
				}
				else
				{
					int num = sharedMesh.triangles[triIndex * 3];
					int num2 = sharedMesh.triangles[triIndex * 3 + 1];
					int num3 = sharedMesh.triangles[triIndex * 3 + 2];
					int num4 = -1;
					for (int i = 0; i < sharedMesh.subMeshCount; i++)
					{
						int[] triangles = sharedMesh.GetTriangles(i);
						for (int j = 0; j < triangles.Length; j += 3)
						{
							if (triangles[j] == num && triangles[j + 1] == num2 && triangles[j + 2] == num3)
							{
								num4 = i;
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
						foundMaterial = mRenderer.sharedMaterials[num4];
						return foundMaterial != null;
					}
				}
			}
		}
		foundMaterial = null;
		return false;
	}

	public void StartPostProcessFade(Texture2D lut, float startVal, float endVal, float duration)
	{
		this.AColor.LutBlendTexture = lut;
		base.StartCoroutine(this.PostProcessFade(startVal, endVal, duration));
	}

	private IEnumerator PostProcessFade(float start, float end, float duration)
	{
		float startTime = Singleton<GameMaster>.Instance.GameTime;
		float endTime = startTime + duration;
		while (Singleton<GameMaster>.Instance.GameTime < endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			this.AColor.BlendAmount = Mathf.Lerp(start, end, t);
			yield return null;
		}
		this.AColor.BlendAmount = end;
		if (end == 0f)
		{
			this.AColor.LutBlendTexture = null;
		}
		yield break;
	}

	public void SetFarZ(float value, FarZVolume.CameraModes cameraMode = FarZVolume.CameraModes.RenderSkybox)
	{
		if (this.FarZCoroutine != null)
		{
			base.StopCoroutine(this.FarZCoroutine);
		}
		this.FarZCoroutine = this.FarZ(value, cameraMode);
		base.StartCoroutine(this.FarZCoroutine);
	}

	private IEnumerator FarZ(float value, FarZVolume.CameraModes cameraMode = FarZVolume.CameraModes.RenderSkybox)
	{
		float startTime = Singleton<GameMaster>.Instance.GameTime;
		float endTime = startTime + 1f;
		float startZ = this.cam.farClipPlane;
		if (cameraMode != FarZVolume.CameraModes.RenderSkybox)
		{
			if (cameraMode == FarZVolume.CameraModes.DepthOnly)
			{
				this.cam.clearFlags = CameraClearFlags.Depth;
			}
		}
		else
		{
			this.cam.clearFlags = CameraClearFlags.Skybox;
		}
		while (Singleton<GameMaster>.Instance.GameTime < endTime)
		{
			float t = Mathf.InverseLerp(startTime, endTime, Singleton<GameMaster>.Instance.GameTime);
			this.cam.farClipPlane = Mathf.Lerp(startZ, value, t);
			yield return null;
		}
		this.cam.farClipPlane = value;
		yield break;
	}

	private void LogController()
	{
		if (!(Singleton<GameMaster>.Instance.stanleyActions.Movement.Vector != Vector2.zero))
		{
			Singleton<GameMaster>.Instance.stanleyActions.View.Vector != Vector2.zero;
		}
		bool wasPressed = Singleton<GameMaster>.Instance.stanleyActions.UseAction.WasPressed;
		bool wasPressed2 = Singleton<GameMaster>.Instance.stanleyActions.JumpAction.WasPressed;
	}

	private static StanleyController _instance;

	public static Action<GameObject> OnInteract;

	public static Vector3 StanleyPosition;

	public static Action OnActuallyJumping;

	public static Action OnTeleportToPlayerStart;

	private CharacterController character;

	private Vector3 camParentOrigLocalPos;

	private float floatingStrength;

	private float floatingPos;

	private float viewPitch;

	private RotatingDoor lastHitRotatingDoor;

	public LayerMask clickLayers;

	public LayerMask groundedLayers;

	public Texture fadeTexture;

	private bool fading;

	private Color fadeColor = Color.black;

	private bool fadeHold;

	private IEnumerator fadeCoroutine;

	private IEnumerator FarZCoroutine;

	private AmplifyColorBase AColor;

	private Vector3 movementInput;

	private bool masterStartFound;

	public Transform spinner;

	public bool motionFrozen;

	public bool viewFrozen;

	private StanleyData.FootstepSounds currentlyStandingOn = StanleyData.FootstepSounds.Silence;

	private GameObject currentlyStandingOnGameObject;

	private Material currentlyStandingOnMaterial;

	[Header("Audio")]
	public float stepDistance = 0.3f;

	public float footstepVolume = 0.4f;

	[SerializeField]
	private AudioCollection barkingCollection;

	[SerializeField]
	private AudioCollection keyboardCollection;

	[SerializeField]
	private AudioCollection[] footstepCollections;

	[SerializeField]
	private AudioSource footstepSource;

	[NonSerialized]
	private AudioSource[] footstepSources;

	[SerializeField]
	private AudioSource useSource;

	[Header("Movement")]
	[SerializeField]
	private float walkingSpeed = 3f;

	[SerializeField]
	private float runAcceleration = 14f;

	[SerializeField]
	private float runDeacceleration = 10f;

	[SerializeField]
	private float inAirMovementMultiplier = 0.7f;

	[SerializeField]
	private float crouchMovementMultiplier = 0.65f;

	[Header("Audio Movement")]
	[SerializeField]
	[Range(0f, 1f)]
	private float footstepsYMultiplier = 0.01f;

	[SerializeField]
	[Range(0f, 1f)]
	private float standingOnUpdateLimit = 0.5f;

	[SerializeField]
	private bool playFootstepOnNewMaterial = true;

	private AudioCollection currentFootstepAudioCollection;

	[Header("Gravity")]
	[SerializeField]
	private float maxGravity = -19f;

	[SerializeField]
	[Range(0f, 1f)]
	private float groundedGravityMultiplier = 0.1f;

	[SerializeField]
	private float gravityFallAcceleration = 1f;

	[NonSerialized]
	private float gravityMultiplier;

	[SerializeField]
	private BooleanConfigurable jumpConfigurable;

	private float velocityAccumulation;

	[Header("Controls")]
	public float mouseSensitivityX;

	public float mouseSensitivityY;

	public float controllerSensitivityX;

	public float controllerSensitivityY;

	public bool yInvert;

	public float armReach = 1f;

	private Vector3 slidingDirection;

	private Vector3 hitNormal;

	private bool isSliding;

	[Header("Jump")]
	[SerializeField]
	private AnimationCurve jumpCurve;

	[SerializeField]
	private float jumpAccerlation = 50f;

	[SerializeField]
	private float jumpPower = 50f;

	private float jumpValue;

	private float jumpTime;

	private bool executeJump;

	[Header("Crouch")]
	[SerializeField]
	private Transform cameraParent;

	[SerializeField]
	private float uncrouchedColliderHeight = 0.632f;

	[SerializeField]
	private float crouchedColliderHeight = 0.316f;

	[SerializeField]
	private float crouchSmoothing = 0.3f;

	[SerializeField]
	private float characterHeightMultipler = 1f;

	private bool wasCrouching;

	public BucketController Bucket;

	[SerializeField]
	private StanleyData stanleyData;

	private Dictionary<int, AudioCollection> footstepDictionary = new Dictionary<int, AudioCollection>();

	private Vector3 movementGoal = Vector3.zero;

	private Vector3 movement = Vector3.zero;

	private MainCamera mainCamera;

	[SerializeField]
	private FloatConfigurable fovSettingConfigurable;

	[SerializeField]
	private FloatConfigurable mouseSensitivityConfigurable;

	[SerializeField]
	private FloatConfigurable controllerSensitivityConfigurable;

	[SerializeField]
	private BooleanConfigurable invertYConfigurable;

	[SerializeField]
	private BooleanConfigurable mouseWalkConfigurable;

	private float inAirLimit = 12f;

	private bool grounded;

	private float inAirTimer;

	private bool outOfBoundsReported;

	public static Action OnOutOfBounds;

	[Header("Debug")]
	[SerializeField]
	private GameObject lightmapDebugUIPrefab;

	private GameObject lightmapDebugUIInstance;

	private bool frozenFromCommandMotion;

	private bool frozenFromCommandView;

	private float standingOnUpdateTimer;

	private const bool FOOTSTEP_TYPE_DEBUG = false;

	private const bool FOOTSTEP_TIMING_DEBUG = false;

	public enum TeleportType
	{
		PlayerStart,
		PlayerStartMaster,
		TriggerTeleport
	}
}

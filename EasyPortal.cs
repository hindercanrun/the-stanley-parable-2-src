using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EasyPortal : MonoBehaviour, IComparable<EasyPortal>
{
	public Transform Destination
	{
		get
		{
			return this.destination;
		}
	}

	public bool Disabled
	{
		get
		{
			return this.disabled;
		}
	}

	public bool DisableRefProbeCopyOnThisPortalsRefProbe
	{
		get
		{
			return this.disableRefProbeCopyOnThisPortalsRefProbe;
		}
	}

	public ReflectionProbe RoomReflectionProbe
	{
		get
		{
			if (this.roomReflectionProbe == null && !this.hasAttemptedRoomReflectionProbeAutoFind)
			{
				this.FindClosestRefProbe();
				this.hasAttemptedRoomReflectionProbeAutoFind = true;
			}
			if (this.roomReflectionProbe != null)
			{
				string text = " for " + base.name;
				if (!this.roomReflectionProbe.name.EndsWith(text))
				{
					ReflectionProbe reflectionProbe = this.roomReflectionProbe;
					reflectionProbe.name += text;
				}
			}
			return this.roomReflectionProbe;
		}
	}

	private void FindClosestRefProbe()
	{
		Transform transform = (this.roomReflectionProbeLocation != null) ? this.roomReflectionProbeLocation : base.transform;
		List<ReflectionProbe> list = new List<ReflectionProbe>();
		TSPArea componentInParent = base.GetComponentInParent<TSPArea>();
		if (componentInParent != null)
		{
			list.AddRange(componentInParent.GetComponentsInChildren<ReflectionProbe>());
		}
		else
		{
			list.AddRange(Object.FindObjectsOfType<ReflectionProbe>());
		}
		float num = float.PositiveInfinity;
		ReflectionProbe x = null;
		foreach (ReflectionProbe reflectionProbe in list)
		{
			float num2 = Vector3.Distance(transform.position, reflectionProbe.transform.position);
			if (num2 < num)
			{
				num = num2;
				x = reflectionProbe;
			}
		}
		if (x != null)
		{
			this.roomReflectionProbe = x;
		}
	}

	public void MoveToLinkedPosition(Transform transformToMove, Transform targetAtLinkedPortal)
	{
		if (this.helperTransform == null)
		{
			return;
		}
		Transform transform = this.helperTransform;
		Transform transform2 = base.transform;
		Vector3 position = transform.InverseTransformPoint(targetAtLinkedPortal.position);
		Vector3 position2 = transform2.TransformPoint(position);
		Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
		Matrix4x4 matrix4x = transform2.localToWorldMatrix * transform.worldToLocalMatrix * localToWorldMatrix;
		transformToMove.transform.position = position2;
		transformToMove.transform.rotation = matrix4x.rotation;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * this.renderingDistance);
		Transform transform = (this.destination != null) ? this.destination : ((this.linkedPortal != null) ? this.linkedPortal.transform : null);
		if (transform == null)
		{
			return;
		}
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(Vector3.zero + Vector3.up * 0.05f, Vector3.forward * this.portalCameraFarClip + Vector3.up * 0.05f);
	}

	private void Awake()
	{
		this.screenCollider = this.screen.GetComponent<BoxCollider>();
		this.screenCollider.enabled = true;
		this.screen.material.SetFloat("_StaticLerp", 1f);
		if (this.portalStyle == EasyPortal.PortalStyles.LinkedPortal)
		{
			this.destination = this.linkedPortal.transform;
		}
		this.screen.gameObject.name = base.name + "_Screen";
		this.portalScreen = this.screen.gameObject.AddComponent<PortalScreen>();
		PortalScreen portalScreen = this.portalScreen;
		portalScreen.OnVisible = (Action)Delegate.Combine(portalScreen.OnVisible, new Action(this.OnVisible));
		PortalScreen portalScreen2 = this.portalScreen;
		portalScreen2.OnInvisible = (Action)Delegate.Combine(portalScreen2.OnInvisible, new Action(this.OnInvisible));
		this.trackedTravellers = new List<PortalTraveller>();
		this.screen.material.SetInt("displayMask", 1);
		if (this.portalCam != null)
		{
			this.portalCam.eventMask = 0;
			this.portalCam.enabled = false;
		}
		if (this.extraCam != null)
		{
			this.extraCam.eventMask = 0;
			this.extraCam.enabled = false;
		}
	}

	private void Start()
	{
		this.UpdateHelperTransform();
		if (MainCamera.Camera != null)
		{
			this.SetupCamera();
		}
		if (!this.useCustomScreenCollider)
		{
			this.UpdateCollider();
		}
		this.gotDisableProbe = (this.disableRefProbeWhenActive != null);
	}

	private void UpdateCollider()
	{
		this.screenCollider.center = new Vector3(0f, 0.5f, 0f);
		this.screenCollider.size = new Vector3(1f, 1f, this.colliderLength);
	}

	private void SetupCamera()
	{
		this.playerCam = MainCamera.Camera;
		this.raycastMask = (this.playerCam.cullingMask & -4194305);
		this.raycastTarget = MainCamera.RaycastTarget;
		this.portalCam = base.GetComponentInChildren<Camera>();
		this.portalCam.enabled = false;
		if (MainCamera.UseVicinityRenderingOnly)
		{
			this.renderingStyle = EasyPortal.RenderingStyles.RenderInVicinity;
		}
		this.SetSecondPortalLayer(this.renderSecondInvisiblePortalLayer);
		this.portalCam.gameObject.AddComponent<EasyPortalPostEffect>().postprocessMaterial = new Material(Shader.Find("Hidden/ClearAlphaChannel"));
		this.FindRecursivePortals(ref this.recursiveVisiblePortals, true);
		this.UpdateStaticTexture(true, true);
		if (this.extraCam != null)
		{
			this.CreateExtraTexture();
		}
		this.setup = true;
	}

	private void UpdateHelperTransform()
	{
		if (this.helperTransform == null)
		{
			this.helperTransform = new GameObject("HelperTransform").transform;
			this.helperTransform.name = base.gameObject.name + "_HelperTransform";
		}
		this.helperTransform.parent = this.destination.transform;
		this.helperTransform.localPosition = Vector3.zero;
		this.helperTransform.rotation = this.destination.rotation;
		this.helperTransform.localScale = Vector3.one;
		this.helperTransform.Rotate(0f, 180f, 0f);
	}

	public void SetSecondPortalLayer(bool status)
	{
		if (this.useCustomLayerMaskRendering)
		{
			this.portalCam.cullingMask = this.customMask;
			return;
		}
		if (status)
		{
			this.portalCam.cullingMask = (this.playerCam.cullingMask & -1048577);
			return;
		}
		this.portalCam.cullingMask = (this.playerCam.cullingMask & -1048577);
		this.portalCam.cullingMask = (this.portalCam.cullingMask & -2097153);
	}

	public bool PlayerFarFromPortal()
	{
		return this.playerCam == null || Vector3.Distance(base.transform.position, this.playerCam.transform.position) > this.renderingDistance;
	}

	private void Update()
	{
		if (!this.setup && MainCamera.Camera != null)
		{
			this.SetupCamera();
		}
		this.HandleTextureRelease();
		if (this.disabled)
		{
			return;
		}
		this.HandleTravellers();
		if (this.debugStatic)
		{
			foreach (Ray ray in this.regularRaysToDraw)
			{
				Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.yellow);
			}
			foreach (Ray ray2 in this.wrongRaysToDraw)
			{
				Debug.DrawRay(ray2.origin, ray2.direction * 1000f, Color.red);
			}
			foreach (Ray ray3 in this.hitRaysToDraw)
			{
				Debug.DrawRay(ray3.origin, ray3.direction * 1000f, Color.green);
			}
		}
	}

	private void HandleTextureRelease()
	{
		if (this.markedForRelease)
		{
			bool flag = false;
			EasyPortal.PortalStyles portalStyles = this.portalStyle;
			if (portalStyles != EasyPortal.PortalStyles.LinkedPortal)
			{
				if (portalStyles == EasyPortal.PortalStyles.DestinationOnly)
				{
					flag = this.PlayerFarFromPortal();
				}
			}
			else
			{
				flag = (this.PlayerFarFromPortal() && this.linkedPortal.PlayerFarFromPortal());
			}
			if (flag)
			{
				this.ReleaseTextures();
			}
		}
	}

	public void SetExtraCam(bool status)
	{
		this.extraCamEnabled = status;
	}

	private void LateUpdate()
	{
		if (this.gotDisableProbe)
		{
			this.disableRefProbeWhenActive.enabled = this.disabled;
		}
		if (this.disabled)
		{
			return;
		}
		this.visibleFromCam = CameraUtility.VisibleFromCamera(this.screen, this.playerCam);
		if (this.renderingStyle == EasyPortal.RenderingStyles.RenderInVicinity || this.renderingStyle == EasyPortal.RenderingStyles.AlwaysRender)
		{
			this.staticLerp = 0f;
			this.screen.material.SetFloat("_StaticLerp", this.staticLerp);
			return;
		}
		this.playerVeryCloseToPortal = (Vector3.Distance(base.transform.position, this.raycastTarget.transform.position) <= this.notStaticDistance);
		this.portalSeesPlayer = this.RaycastPortalToPlayer(4f, 4f);
		if (GameMaster.PAUSEMENUACTIVE || this.playerVeryCloseToPortal || this.forcedRendering || this.portalSeesPlayer || this.justTeleported)
		{
			this.staticLerp = 0f;
			this.forcedRendering = false;
		}
		else
		{
			this.staticLerp = 1f;
		}
		this.screen.material.SetFloat("_StaticLerp", this.staticLerp);
	}

	private void HandleTravellers()
	{
		for (int i = 0; i < this.trackedTravellers.Count; i++)
		{
			PortalTraveller portalTraveller = this.trackedTravellers[i];
			Transform transform = portalTraveller.transform;
			Matrix4x4 matrix4x = this.helperTransform.localToWorldMatrix * base.transform.worldToLocalMatrix * transform.localToWorldMatrix;
			Vector3 vector = transform.position - base.transform.position;
			int num = Math.Sign(Vector3.Dot(vector, base.transform.forward));
			int num2 = Math.Sign(Vector3.Dot(portalTraveller.previousOffsetFromPortal, base.transform.forward));
			if (num != num2)
			{
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				portalTraveller.Teleport(base.transform, this.destination, matrix4x.GetColumn(3), matrix4x.rotation);
				if (portalTraveller.InstantiateClone)
				{
					portalTraveller.graphicsClone.transform.SetPositionAndRotation(position, rotation);
				}
				if (this.linkedPortal != null)
				{
					this.linkedPortal.OnTravellerEnterPortal(portalTraveller);
				}
				this.trackedTravellers.RemoveAt(i);
				i--;
				this.justTeleported = true;
				if (this.linkedPortal != null)
				{
					this.linkedPortal.justTeleported = true;
				}
				this.screen.enabled = false;
				this.SetVisiblePortalsVisible();
			}
			else
			{
				if (portalTraveller.InstantiateClone)
				{
					portalTraveller.graphicsClone.transform.SetPositionAndRotation(matrix4x.GetColumn(3), matrix4x.rotation);
				}
				portalTraveller.previousOffsetFromPortal = vector;
			}
		}
	}

	private bool RaycastPortalToPlayer(float xSamples = 4f, float ySamples = 4f)
	{
		Vector3 position = this.raycastTarget.transform.position;
		if (this.onlyCheckForwardDirection)
		{
			Vector3 rhs = base.transform.position - position;
			if (Vector3.Dot(base.transform.forward, rhs) < 0f)
			{
				return false;
			}
		}
		if (Vector3.Distance(position, base.transform.position) > this.renderingDistance)
		{
			return false;
		}
		MeshRenderer meshRenderer = this.screen;
		float x = this.screen.transform.lossyScale.x;
		float y = this.screen.transform.lossyScale.y;
		float z = this.screen.transform.lossyScale.z;
		Physics.queriesHitBackfaces = true;
		int num = 0;
		while ((float)num <= xSamples)
		{
			float num2 = (float)num / xSamples * x;
			int num3 = 0;
			while ((float)num3 <= ySamples)
			{
				float y2 = (float)num3 / ySamples * y;
				Vector3 vector = new Vector3(num2 - x / 2f, y2, z);
				vector = base.transform.TransformPoint(vector);
				Ray ray = new Ray(vector, position - vector);
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, (position - vector).normalized * this.renderingDistance, out raycastHit, this.renderingDistance, this.raycastMask, QueryTriggerInteraction.Ignore))
				{
					if (raycastHit.collider.tag.Equals("Player"))
					{
						Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(vector, raycastHit.point), Color.green);
						Physics.queriesHitBackfaces = false;
						return true;
					}
					if (raycastHit.collider != null)
					{
						Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(vector, raycastHit.point), Color.yellow);
					}
				}
				else
				{
					Debug.DrawRay(ray.origin, ray.direction * float.PositiveInfinity, Color.red);
				}
				num3++;
			}
			num++;
		}
		Physics.queriesHitBackfaces = false;
		return false;
	}

	private bool IsValidRenderingStyle()
	{
		switch (this.renderingStyle)
		{
		case EasyPortal.RenderingStyles.AlwaysRender:
			return true;
		case EasyPortal.RenderingStyles.RenderInVicinity:
			return this.distanceToPortal <= this.renderingDistance;
		case EasyPortal.RenderingStyles.RaycastBeforeRender:
			return this.portalSeesPlayer;
		default:
			return false;
		}
	}

	private bool DestinationSeesPortal(EasyPortal portalToCheck, float xSamples = 4f, float ySamples = 4f)
	{
		MeshRenderer meshRenderer = portalToCheck.screen;
		float x = meshRenderer.transform.lossyScale.x;
		float y = meshRenderer.transform.lossyScale.y;
		float z = meshRenderer.transform.lossyScale.z;
		Physics.queriesHitBackfaces = true;
		int num = 0;
		while ((float)num <= xSamples)
		{
			float num2 = (float)num / xSamples * x;
			int num3 = 0;
			while ((float)num3 <= ySamples)
			{
				float y2 = (float)num3 / ySamples * y;
				Vector3 vector = new Vector3(num2 - x / 2f, y2, z);
				vector = portalToCheck.transform.TransformPoint(vector);
				if (this.portalStyle == EasyPortal.PortalStyles.LinkedPortal)
				{
					Vector3 direction = this.linkedPortal.transform.position + Vector3.up - vector;
					Ray item = new Ray(vector, direction);
					RaycastHit raycastHit;
					if (Physics.Raycast(vector, direction.normalized * portalToCheck.renderingDistance, out raycastHit, portalToCheck.renderingDistance, this.playerCam.cullingMask, QueryTriggerInteraction.Collide))
					{
						if (raycastHit.collider.tag.Equals("Portal"))
						{
							bool flag = false;
							if (raycastHit.collider.gameObject == this.linkedPortal.screen.gameObject)
							{
								flag = true;
							}
							else
							{
								this.wrongRaysToDraw.Add(item);
							}
							Debug.DrawRay(item.origin, item.direction * Vector3.Distance(vector, raycastHit.point), flag ? Color.green : Color.magenta);
							Physics.queriesHitBackfaces = false;
							if (flag)
							{
								this.hitRaysToDraw.Add(item);
								return true;
							}
						}
						else if (raycastHit.collider != null)
						{
							this.regularRaysToDraw.Add(item);
							Debug.DrawRay(item.origin, item.direction * Vector3.Distance(vector, raycastHit.point), Color.yellow);
						}
					}
					else
					{
						Debug.DrawRay(item.origin, item.direction * float.PositiveInfinity, Color.red);
					}
				}
				num3++;
			}
			num++;
		}
		Physics.queriesHitBackfaces = false;
		return false;
	}

	public void SetNewDestination(Transform newDestination)
	{
		EasyPortal x = newDestination.GetComponent<EasyPortal>();
		if (x == null)
		{
			PortalDoor component = newDestination.GetComponent<PortalDoor>();
			if (component != null)
			{
				x = component.EasyPortal;
			}
			if (x == null)
			{
				PortalDoor component2 = newDestination.GetComponent<PortalDoor>();
				if (component2 != null)
				{
					x = component2.EasyPortal;
				}
			}
		}
		if (x != null)
		{
			this.portalStyle = EasyPortal.PortalStyles.LinkedPortal;
			this.linkedPortal = x;
			this.destination = this.linkedPortal.transform;
		}
		else
		{
			this.portalStyle = EasyPortal.PortalStyles.DestinationOnly;
			this.destination = newDestination;
			if (this.printDebug)
			{
				Debug.Log("New destination " + newDestination.name + " has no portal component!");
			}
		}
		this.UpdateHelperTransform();
	}

	public void PrePortalRender()
	{
	}

	public void SetStatus(bool status, bool release = false)
	{
		this.disabled = !status;
		this.screen.enabled = status;
		if (this.screenCollider != null)
		{
			this.screenCollider.enabled = status;
		}
		if (!status)
		{
			this.trackedTravellers.Clear();
			if (release)
			{
				this.ReleaseTextures();
			}
		}
	}

	public void SetStatus(bool status)
	{
		this.SetStatus(status, true);
	}

	private void OnVisible()
	{
		if (this.disabled)
		{
			return;
		}
		this.CreateStaticTexture();
		if (this.extraCam != null)
		{
			this.CreateExtraTexture();
		}
	}

	private void OnInvisible()
	{
		this.markedForRelease = true;
	}

	private void OnDestroy()
	{
		this.ReleaseTextures();
		PortalScreen portalScreen = this.portalScreen;
		portalScreen.OnVisible = (Action)Delegate.Remove(portalScreen.OnVisible, new Action(this.OnVisible));
		PortalScreen portalScreen2 = this.portalScreen;
		portalScreen2.OnInvisible = (Action)Delegate.Remove(portalScreen2.OnInvisible, new Action(this.OnInvisible));
	}

	private void ReleaseTextures()
	{
		if (this.viewTexture != null)
		{
			this.viewTexture.Release();
		}
		this.viewTexture = null;
		if (this.staticViewTexture != null)
		{
			this.staticViewTexture.Release();
		}
		this.staticViewTexture = null;
		if (this.extraTexture != null)
		{
			this.extraTexture.Release();
		}
		this.extraTexture = null;
		this.markedForRelease = false;
	}

	public void Render()
	{
		if (this.disabled || !this.setup || this.portalCam == null)
		{
			return;
		}
		this.distanceToPortal = Vector3.Distance(this.playerCam.transform.position, base.transform.position);
		this.portalCam.farClipPlane = this.distanceToPortal + this.portalCameraFarClip;
		Debug.DrawLine(this.playerCam.transform.position, base.transform.position, Color.cyan);
		if (!this.visibleFromCam)
		{
			if (this.visible)
			{
				this.CreateStaticTexture();
				this.UpdateStaticTexture(true, false);
			}
			this.visible = false;
			if (this.printDebug)
			{
				Debug.Log("Not Rendering because not in view");
			}
			return;
		}
		if (this.justTeleported || this.renderingStyle == EasyPortal.RenderingStyles.AlwaysRender)
		{
			this.screen.material.SetFloat("_StaticLerp", 0f);
			this.RenderRecursiveVisiblePortals();
			this.visible = true;
			this.justTeleported = false;
			this.screen.enabled = true;
		}
		else
		{
			if (this.printDebug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"DISTANCE : ",
					this.distanceToPortal,
					" PORTAL SEES PLAYER : ",
					this.portalSeesPlayer.ToString()
				}));
			}
			if (this.distanceToPortal <= this.notStaticDistance)
			{
				if (this.printDebug)
				{
					Debug.Log("Rendering because closer than notStaticDistance");
				}
				this.visible = true;
			}
			else if (this.distanceToPortal > this.notStaticDistance && this.distanceToPortal <= this.renderingDistance)
			{
				bool flag = this.IsValidRenderingStyle();
				if (!flag && this.visible)
				{
					this.CreateStaticTexture();
					this.UpdateStaticTexture(!this.visibleFromCam, false);
				}
				this.visible = flag;
				if (this.printDebug)
				{
					Debug.Log(string.Concat(new object[]
					{
						"Rendering style : ",
						this.renderingStyle,
						" is valid : ",
						this.visible.ToString()
					}));
				}
			}
			else
			{
				if (this.printDebug)
				{
					Debug.Log("NOT RENDERING");
				}
				if (this.visible)
				{
					this.CreateStaticTexture();
					this.UpdateStaticTexture(!this.visibleFromCam, false);
				}
				this.visible = false;
			}
		}
		if (this.visible)
		{
			this.CreateViewTexture();
			this.portalCam.projectionMatrix = this.playerCam.projectionMatrix;
			Vector3 position = base.transform.InverseTransformPoint(this.playerCam.transform.position);
			Vector3 position2 = this.helperTransform.TransformPoint(position);
			Matrix4x4 localToWorldMatrix = this.playerCam.transform.localToWorldMatrix;
			Matrix4x4 matrix4x = this.helperTransform.localToWorldMatrix * base.transform.worldToLocalMatrix * localToWorldMatrix;
			this.portalCam.transform.position = position2;
			this.portalCam.transform.rotation = matrix4x.rotation;
			if (this.linkedPortal != null)
			{
				this.linkedPortal.screen.enabled = false;
			}
			this.HandleOcclusionCulling();
			this.SetNearClipPlane(null);
			this.RenderRecursiveVisiblePortals();
			this.portalCam.targetTexture = this.viewTexture;
			if (this.extraCamEnabled && this.extraCam != null)
			{
				this.extraCam.fieldOfView = this.playerCam.fieldOfView;
				this.extraCam.targetTexture = this.extraTexture;
				this.extraCam.Render();
				this.screen.material.SetTexture("_ExtraTex", this.extraTexture);
			}
			else if (this.extraCam != null)
			{
				this.extraCam.enabled = false;
			}
			this.portalCam.Render();
			this.screen.material.SetTexture("_MainTex", this.viewTexture);
			this.ClearRecursive();
			if (this.linkedPortal != null && !this.linkedPortal.disabled)
			{
				this.linkedPortal.screen.enabled = true;
			}
		}
	}

	public void ClearRecursive()
	{
		for (int i = 0; i < MainCamera.Portals.Length; i++)
		{
			EasyPortal easyPortal = MainCamera.Portals[i];
			easyPortal.screen.material.SetTexture("_MainTex", easyPortal.viewTexture);
		}
	}

	public void RenderRecursiveVisiblePortals()
	{
		for (int i = 0; i < this.recursiveVisiblePortals.Count; i++)
		{
			EasyPortal easyPortal = this.recursiveVisiblePortals[i];
			if (!easyPortal.gameObject.activeInHierarchy)
			{
				return;
			}
			if (CameraUtility.VisibleFromCamera(easyPortal.screen, this.portalCam))
			{
				easyPortal.ForceRenderingFromCamera(this.portalCam, 0);
			}
		}
	}

	public void SetVisiblePortalsVisible()
	{
		for (int i = 0; i < this.recursiveVisiblePortals.Count; i++)
		{
			if (CameraUtility.VisibleFromCamera(this.recursiveVisiblePortals[i].screen, this.playerCam))
			{
				this.recursiveVisiblePortals[i].justTeleported = true;
				Matrix4x4 value = this.playerCam.projectionMatrix * this.playerCam.worldToCameraMatrix;
				this.recursiveVisiblePortals[i].screen.material.SetMatrix("_WorldToCam", value);
			}
		}
	}

	private void FindRecursivePortals(ref List<EasyPortal> visiblePortalList, bool returnPortalsOnly = false)
	{
		for (int i = 0; i < MainCamera.Portals.Length; i++)
		{
			EasyPortal easyPortal = MainCamera.Portals[i];
			if (easyPortal != this && easyPortal != this.linkedPortal && returnPortalsOnly && this.DestinationSeesPortal(easyPortal, 4f, 4f) && !this.visiblePortals.Contains(easyPortal))
			{
				this.visiblePortals.Add(easyPortal);
			}
		}
		if (returnPortalsOnly)
		{
			for (int j = 0; j < this.visiblePortals.Count; j++)
			{
				EasyPortal item = this.visiblePortals[j];
				if (!visiblePortalList.Contains(item))
				{
					visiblePortalList.Add(item);
				}
			}
		}
		else
		{
			for (int k = 0; k < this.visiblePortals.Count; k++)
			{
				this.visiblePortals[k].linkedPortal.ForceRenderingFromCamera(this.portalCam, 0);
			}
		}
		this.visiblePortals.Clear();
	}

	private void ForceRenderingFromCamera(Camera cam, int currentDepth = 0)
	{
		if (this.disabled)
		{
			return;
		}
		this.SetSecondPortalLayer(false);
		this.portalCam.projectionMatrix = cam.projectionMatrix;
		Vector3 position = base.transform.InverseTransformPoint(cam.transform.position);
		Vector3 position2 = this.helperTransform.TransformPoint(position);
		Matrix4x4 localToWorldMatrix = cam.transform.localToWorldMatrix;
		Matrix4x4 matrix4x = this.helperTransform.localToWorldMatrix * base.transform.worldToLocalMatrix * localToWorldMatrix;
		this.portalCam.transform.position = position2;
		this.portalCam.transform.rotation = matrix4x.rotation;
		this.CreateRecursiveTexture();
		this.portalCam.targetTexture = this.recursiveTexture;
		this.screen.material.SetTexture("_MainTex", this.recursiveTexture);
		this.screen.material.SetFloat("_StaticLerp", 0f);
		this.SetNearClipPlane(cam);
		this.HandleOcclusionCulling();
		if (this.linkedPortal != null)
		{
			this.linkedPortal.screen.enabled = false;
		}
		this.portalCam.Render();
		if (this.linkedPortal != null && !this.linkedPortal.disabled)
		{
			this.linkedPortal.screen.enabled = true;
		}
		this.forcedRendering = true;
	}

	private void UpdateStaticTexture(bool neutralPosition = false, bool fixedDistance = false)
	{
		if (this.debugStatic)
		{
			Debug.Log("Updating static texture. Neutral : " + neutralPosition.ToString());
		}
		Vector3 position = this.playerCam.transform.position;
		Quaternion rotation = this.playerCam.transform.rotation;
		if (neutralPosition)
		{
			float d = Mathf.Clamp(Vector3.Distance(this.playerCam.transform.position, base.transform.position), 12f, this.renderingDistance);
			if (fixedDistance)
			{
				d = 3.5f;
			}
			this.playerCam.transform.position = base.transform.TransformPoint(Vector3.forward * d + Vector3.up * 0.6f);
			this.playerCam.transform.rotation = Quaternion.LookRotation(base.transform.position + Vector3.up - this.playerCam.transform.position, Vector3.up);
		}
		this.portalCam.targetTexture = this.staticViewTexture;
		Vector3 position2 = base.transform.InverseTransformPoint(this.playerCam.transform.position);
		Vector3 position3 = this.helperTransform.TransformPoint(position2);
		Matrix4x4 localToWorldMatrix = this.playerCam.transform.localToWorldMatrix;
		Matrix4x4 matrix4x = this.helperTransform.localToWorldMatrix * base.transform.worldToLocalMatrix * localToWorldMatrix;
		this.portalCam.transform.position = position3;
		this.portalCam.transform.rotation = matrix4x.rotation;
		this.HandleOcclusionCulling();
		this.SetNearClipPlane(null);
		if (this.linkedPortal != null)
		{
			this.linkedPortal.screen.enabled = false;
		}
		this.portalCam.Render();
		if (this.linkedPortal != null && !this.linkedPortal.disabled)
		{
			this.linkedPortal.screen.enabled = true;
		}
		this.camMatrix = this.playerCam.projectionMatrix * this.playerCam.worldToCameraMatrix;
		this.screen.material.SetMatrix("_WorldToCam", this.camMatrix);
		this.playerCam.transform.position = position;
		this.playerCam.transform.rotation = rotation;
	}

	private void HandleOcclusionCulling()
	{
		if (this.disableCameraOcclusionCulling)
		{
			this.portalCam.useOcclusionCulling = false;
			return;
		}
		if (Vector3.Distance(base.transform.position, this.playerCam.transform.position) <= this.occlusionCullingDisableDistance)
		{
			this.portalCam.useOcclusionCulling = false;
			return;
		}
		this.portalCam.useOcclusionCulling = true;
	}

	private void CreateStaticTexture()
	{
		if (this.staticViewTexture == null || this.staticViewTexture.width != Screen.width / 2 || this.staticViewTexture.height != Screen.height / 2)
		{
			if (this.staticViewTexture != null)
			{
				this.staticViewTexture.Release();
			}
			this.staticViewTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 32, RenderTextureFormat.Default);
			this.screen.material.SetTexture("_StaticTex", this.staticViewTexture);
		}
	}

	private void CreateViewTexture()
	{
		if (this.viewTexture == null || this.viewTexture.width != Screen.width || this.viewTexture.height != Screen.height)
		{
			if (this.viewTexture != null)
			{
				this.viewTexture.Release();
			}
			this.viewTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.Default);
			this.viewTexture.antiAliasing = 2;
			this.portalCam.targetTexture = this.viewTexture;
			this.screen.material.SetTexture("_MainTex", this.viewTexture);
		}
	}

	private void CreateExtraTexture()
	{
		if (this.extraTexture == null || this.extraTexture.width != Screen.width || this.extraTexture.height != Screen.height)
		{
			if (this.extraTexture != null)
			{
				this.extraTexture.Release();
			}
			this.extraTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.Default);
			this.extraCam.targetTexture = this.extraTexture;
			this.screen.material.SetTexture("_ExtraTex", this.extraTexture);
		}
	}

	private void CreateRecursiveTexture()
	{
		if (this.recursiveTexture == null || this.recursiveTexture.width != Screen.width || this.recursiveTexture.height != Screen.height)
		{
			if (this.recursiveTexture != null)
			{
				this.recursiveTexture.Release();
			}
			this.recursiveTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.Default);
		}
	}

	public void PostPortalRender()
	{
		if (this.playerCam != null)
		{
			this.ProtectScreenFromClipping(this.playerCam.transform.position);
		}
	}

	private float ProtectScreenFromClipping(Vector3 viewPoint)
	{
		if (this.screen == null)
		{
			return 0f;
		}
		float num = this.playerCam.nearClipPlane * Mathf.Tan(this.playerCam.fieldOfView * 0.5f * 0.017453292f);
		float magnitude = new Vector3(num * this.playerCam.aspect, num, this.playerCam.nearClipPlane).magnitude;
		Transform transform = this.screen.transform;
		bool flag = Vector3.Dot(base.transform.forward, base.transform.position - viewPoint) > 0f;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, magnitude);
		transform.localPosition = Vector3.forward * magnitude * (flag ? 0.5f : -0.5f);
		return magnitude;
	}

	private void SetNearClipPlane(Camera customCam = null)
	{
		Transform transform = base.transform;
		int num = Math.Sign(Vector3.Dot(transform.forward, base.transform.position - this.playerCam.transform.position));
		Camera camera = (customCam != null) ? customCam : this.playerCam;
		Vector3 lhs = camera.worldToCameraMatrix.MultiplyPoint(transform.position);
		Vector3 vector = camera.worldToCameraMatrix.MultiplyVector(transform.forward) * (float)num;
		float num2 = -Vector3.Dot(lhs, vector) + this.nearClipOffset;
		if (this.useOblique && Mathf.Abs(num2) > this.nearClipLimit)
		{
			Vector4 clipPlane = new Vector4(vector.x, vector.y, vector.z, num2);
			this.portalCam.projectionMatrix = camera.CalculateObliqueMatrix(clipPlane);
			return;
		}
		this.portalCam.nearClipPlane = Mathf.Abs(num2);
		this.portalCam.projectionMatrix = this.playerCam.projectionMatrix;
	}

	private void OnTravellerEnterPortal(PortalTraveller traveller)
	{
		if (!this.trackedTravellers.Contains(traveller))
		{
			traveller.EnterPortalThreshold();
			traveller.previousOffsetFromPortal = traveller.transform.position - base.transform.position;
			this.trackedTravellers.Add(traveller);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.disabled)
		{
			return;
		}
		PortalTraveller component = other.GetComponent<PortalTraveller>();
		if (component != null)
		{
			this.OnTravellerEnterPortal(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.disabled)
		{
			return;
		}
		PortalTraveller component = other.GetComponent<PortalTraveller>();
		if (component && this.trackedTravellers.Contains(component))
		{
			component.ExitPortalThreshold();
			this.trackedTravellers.Remove(component);
		}
	}

	private int SideOfPortal(Vector3 pos)
	{
		return Math.Sign(Vector3.Dot(pos - base.transform.position, base.transform.forward));
	}

	private bool SameSideOfPortal(Vector3 posA, Vector3 posB)
	{
		return this.SideOfPortal(posA) == this.SideOfPortal(posB);
	}

	private Vector3 portalCamPos
	{
		get
		{
			return this.portalCam.transform.position;
		}
	}

	private void OnValidate()
	{
	}

	public float GetDistanceToPlayer()
	{
		return Vector3.Distance(base.transform.position, this.playerCam.transform.position);
	}

	public float CompareForwardToPlayerCam()
	{
		return Vector3.Dot(Vector3.ProjectOnPlane(this.playerCam.transform.forward, Vector3.up), Vector3.ProjectOnPlane(base.transform.forward, Vector3.up));
	}

	int IComparable<EasyPortal>.CompareTo(EasyPortal other)
	{
		if (this.GetDistanceToPlayer() <= other.GetDistanceToPlayer())
		{
			return -1;
		}
		return 1;
	}

	[SerializeField]
	private EasyPortal.PortalStyles portalStyle;

	[Header("Main Settings")]
	public EasyPortal linkedPortal;

	[SerializeField]
	private Transform destination;

	public MeshRenderer screen;

	private PortalScreen portalScreen;

	public float colliderLength = 100f;

	[Header("Rendering Settings")]
	[SerializeField]
	private EasyPortal.RenderingStyles renderingStyle = EasyPortal.RenderingStyles.RaycastBeforeRender;

	public float renderingDistance = 12f;

	public float portalCameraFarClip = 12f;

	[Header("Advanced Rendering Settings")]
	[SerializeField]
	private float nearClipOffset = 0.01f;

	[SerializeField]
	private float nearClipLimit = 0.2f;

	[SerializeField]
	private bool onlyCheckForwardDirection;

	[SerializeField]
	private bool useOblique = true;

	[SerializeField]
	private bool useCustomLayerMaskRendering;

	[SerializeField]
	private LayerMask customMask;

	public float occlusionCullingDisableDistance = 5f;

	[SerializeField]
	private bool renderSecondInvisiblePortalLayer;

	[Header("Customization")]
	[SerializeField]
	private Camera extraCam;

	[SerializeField]
	private bool extraCamEnabled;

	[SerializeField]
	private bool useCustomScreenCollider;

	[SerializeField]
	private bool printDebug;

	[Header("Debug - Dont change these")]
	[SerializeField]
	private float distanceToPortal;

	[SerializeField]
	public bool justTeleported;

	[SerializeField]
	private bool visible;

	[SerializeField]
	public bool disabled;

	[SerializeField]
	private bool portalSeesPlayer;

	[SerializeField]
	private List<EasyPortal> recursiveVisiblePortals;

	[HideInInspector]
	[SerializeField]
	private bool setup;

	[HideInInspector]
	public RenderTexture viewTexture;

	[HideInInspector]
	public RenderTexture staticViewTexture;

	[HideInInspector]
	public RenderTexture recursiveTexture;

	[HideInInspector]
	public RenderTexture extraTexture;

	[HideInInspector]
	public Camera portalCam;

	public Transform helperTransform;

	private Camera playerCam;

	private Transform raycastTarget;

	private float staticLerp;

	private Matrix4x4 camMatrix;

	[SerializeField]
	private List<PortalTraveller> trackedTravellers = new List<PortalTraveller>();

	private List<EasyPortal> visiblePortals = new List<EasyPortal>();

	private float notStaticDistance = 2f;

	private bool playerVeryCloseToPortal;

	private bool visibleFromCam;

	private bool forcedRendering;

	private BoxCollider screenCollider;

	private List<Ray> regularRaysToDraw = new List<Ray>();

	private List<Ray> wrongRaysToDraw = new List<Ray>();

	private List<Ray> hitRaysToDraw = new List<Ray>();

	[Header("Reflection Probe Stuff")]
	[InspectorButton("FindClosestRefProbe", "Find Closest Ref Probe")]
	[SerializeField]
	[FormerlySerializedAs("closestRefProbe")]
	private ReflectionProbe roomReflectionProbe;

	[SerializeField]
	private Transform roomReflectionProbeLocation;

	[SerializeField]
	private bool disableRefProbeCopyOnThisPortalsRefProbe;

	public bool debugStatic;

	private bool hasAttemptedRoomReflectionProbeAutoFind;

	private LayerMask raycastMask;

	private bool disableCameraOcclusionCulling = true;

	[Header("The Reflection Probe on the other side of the door that needs to be disabled if the door is being useda as a portal")]
	public ReflectionProbe disableRefProbeWhenActive;

	private bool gotDisableProbe;

	private bool markedForRelease;

	public enum RenderingFallbacks
	{
		NoFallback,
		LastRenderedFrame
	}

	public enum RenderingStyles
	{
		AlwaysRender,
		RenderInVicinity,
		RaycastBeforeRender
	}

	public enum PortalStyles
	{
		LinkedPortal,
		DestinationOnly
	}
}

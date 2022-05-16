using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class BucketController : MonoBehaviour
{
	private void Awake()
	{
		BucketController.HASBUCKET = this.BucketConfigurable.GetBooleanValue();
		this.bucketAnimator = base.GetComponent<Animator>();
		this.bucketAudioSource = base.GetComponent<AudioSource>();
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	private void Start()
	{
		SimpleEvent simpleEvent = this.dropImmediateEvent;
		simpleEvent.OnCall = (Action)Delegate.Combine(simpleEvent.OnCall, new Action(this.DropBucketImmediate));
		this.OnSceneReady();
		base.InvokeRepeating("UpdatePortals", 0f, 0.5f);
	}

	private void OnDestroy()
	{
		SimpleEvent simpleEvent = this.dropImmediateEvent;
		simpleEvent.OnCall = (Action)Delegate.Remove(simpleEvent.OnCall, new Action(this.DropBucketImmediate));
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	private void OnSceneReady()
	{
		this.portalProbeCopies.Clear();
		this.allPortals.Clear();
		this.allPortals.AddRange(Object.FindObjectsOfType<EasyPortal>());
		this.cachedPlayerTransform = StanleyController.Instance.transform;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.OnSceneReady();
	}

	private EasyPortal FindClosestPortal(Transform reference)
	{
		if (reference == null)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		EasyPortal result = null;
		foreach (EasyPortal easyPortal in this.allPortals)
		{
			if (!(easyPortal == null) && !easyPortal.Disabled && easyPortal.isActiveAndEnabled)
			{
				float num2 = Vector3.Distance(reference.position, easyPortal.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = easyPortal;
				}
			}
		}
		return result;
	}

	private ReflectionProbe GetReflectionProbeCopy(EasyPortal easyPortal)
	{
		if (!this.portalProbeCopies.ContainsKey(easyPortal))
		{
			if (easyPortal.RoomReflectionProbe == null)
			{
				Debug.LogWarning("Easyportal " + easyPortal + " has no reflection probe set", easyPortal);
				return null;
			}
			this.portalProbeCopies[easyPortal] = Object.Instantiate<GameObject>(easyPortal.RoomReflectionProbe.gameObject).GetComponent<ReflectionProbe>();
			if (Mathf.Abs(Mathf.Abs(Quaternion.Angle(easyPortal.transform.rotation, easyPortal.linkedPortal.transform.rotation)) - 90f) < 0.1f)
			{
				Vector3 size = easyPortal.RoomReflectionProbe.size;
				this.portalProbeCopies[easyPortal].size = new Vector3(size.z, size.y, size.x);
			}
			this.portalProbeCopies[easyPortal].bakedTexture = easyPortal.RoomReflectionProbe.bakedTexture;
		}
		return this.portalProbeCopies[easyPortal];
	}

	internal void PlayAdditiveAnimation(string v)
	{
		this.bucketAnimator.GetNextAnimatorClipInfo(this.bucketAnimator.GetLayerIndex("Additive"));
		if (BucketController.HASBUCKET)
		{
			this.bucketAnimator.SetTrigger("Additive/" + v);
		}
	}

	private void UpdatePortals()
	{
		this.closestPortalMain = this.FindClosestPortal(this.cachedPlayerTransform);
		if (this.closestPortalMain == null)
		{
			return;
		}
		this.closestPortalPair = this.closestPortalMain.linkedPortal;
		this.closestPortalMainRPCopy = this.GetReflectionProbeCopy(this.closestPortalMain);
		this.closestPortalPairRPCopy = this.GetReflectionProbeCopy(this.closestPortalPair);
		foreach (EasyPortal easyPortal in this.portalProbeCopies.Keys)
		{
			bool active = false;
			if ((easyPortal == this.closestPortalMain || easyPortal == this.closestPortalPair) && !this.closestPortalMain.DisableRefProbeCopyOnThisPortalsRefProbe)
			{
				active = true;
			}
			this.portalProbeCopies[easyPortal].gameObject.SetActive(active);
		}
		if (this.closestPortalMainRPCopy == null || this.closestPortalPairRPCopy == null)
		{
			Debug.LogError("Could not get reflection probe copies");
			return;
		}
		this.distanceToPortal = Vector3.Distance(this.closestPortalMain.transform.position, this.bucketRenderer.transform.position);
		this.buketAtOffsetPosition = (this.distanceToPortal < this.turnOnDistance);
		this.closestPortalPair.MoveToLinkedPosition(this.closestPortalMainRPCopy.transform, this.closestPortalMain.RoomReflectionProbe.transform);
		this.closestPortalMain.MoveToLinkedPosition(this.closestPortalPairRPCopy.transform, this.closestPortalPair.RoomReflectionProbe.transform);
	}

	private void OnBucketPickup(bool suppressAudio)
	{
		this.OnBucketPickup(suppressAudio, false);
	}

	private void OnBucketPickup(bool suppressAudio, bool instantBucket)
	{
		if (instantBucket)
		{
			this.bucketAnimator.SetTrigger("InstaBucket");
		}
		this.bucketAnimator.SetBool("HasBucket", true);
		if (!suppressAudio)
		{
			this.bucketAudioSource.Play();
		}
		BucketController.HASBUCKET = true;
	}

	private void OnBucketRemoval(bool instantBucket)
	{
		if (instantBucket)
		{
			this.bucketAnimator.SetTrigger("InstaBucket");
		}
		this.bucketAnimator.SetBool("HasBucket", false);
		this.bucketAudioSource.Stop();
		BucketController.HASBUCKET = false;
	}

	public void DropBucketImmediate()
	{
		this.SetBucket(false, true, true, true);
	}

	public void SetBucket(bool status)
	{
		this.SetBucket(status, false, false, false);
	}

	public void SetBucketWithConfigurable(bool status)
	{
		this.SetBucket(status, true, false, false);
	}

	public void SetBucket(bool status, bool setConfigurable, bool suppressAudio, bool instantBucket)
	{
		if (status)
		{
			this.OnBucketPickup(suppressAudio, instantBucket);
		}
		else
		{
			this.OnBucketRemoval(instantBucket);
		}
		if (setConfigurable)
		{
			this.BucketConfigurable.SetValue(status);
		}
	}

	public void SetWalkingSpeed(float speed)
	{
		this.bucketAnimator.SetFloat("WalkingSpeed", speed, this.bucketMovementDamp, Time.deltaTime);
	}

	public void SetAnimationSpeed(float speed)
	{
		this.bucketAnimator.SetFloat("AnimationSpeed", speed, this.bucketMovementDamp, Time.deltaTime);
	}

	public void SetAnimationSpeedImmediate(float speed)
	{
		this.bucketAnimator.SetFloat("AnimationSpeed", speed);
	}

	public static bool HASBUCKET;

	[SerializeField]
	private Animator bucketAnimator;

	[SerializeField]
	private Transform bucketModel;

	[SerializeField]
	private AudioSource bucketAudioSource;

	[SerializeField]
	private float bucketMovementDamp = 0.8f;

	[SerializeField]
	private BooleanConfigurable BucketConfigurable;

	[SerializeField]
	private SimpleEvent dropImmediateEvent;

	[SerializeField]
	private MeshRenderer bucketRenderer;

	[SerializeField]
	private Transform bucketOffset;

	[SerializeField]
	private BooleanConfigurable defaultConfigurationConfigurable;

	private bool disableReflectionProbeLogic;

	private List<EasyPortal> allPortals = new List<EasyPortal>();

	private Dictionary<EasyPortal, ReflectionProbe> portalProbeCopies = new Dictionary<EasyPortal, ReflectionProbe>();

	public Vector3 reflectionProbeOffset = Vector3.down * 2000f;

	public bool buketAtOffsetPosition = true;

	public float distanceToPortal;

	public float turnOnDistance = 0.5f;

	[Header("Debug")]
	public EasyPortal closestPortalMain;

	public EasyPortal closestPortalPair;

	public ReflectionProbe closestPortalMainRPCopy;

	public ReflectionProbe closestPortalPairRPCopy;

	private Transform cachedPlayerTransform;

	private bool BucketBeginSet;
}

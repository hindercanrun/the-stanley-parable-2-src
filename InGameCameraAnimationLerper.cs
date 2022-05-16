using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class InGameCameraAnimationLerper : MonoBehaviour
{
	private Transform Parent
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.editorTestCameraParent;
			}
			return StanleyController.Instance.camParent;
		}
	}

	private Camera Camera
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.editorTestCamera;
			}
			return StanleyController.Instance.cam;
		}
	}

	private float OriginalFOV
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.editorOriginalFOV;
			}
			return StanleyController.Instance.FieldOfViewBase;
		}
	}

	private float FOV
	{
		get
		{
			if (!Application.isPlaying)
			{
				return this.editorTestCamera.fieldOfView;
			}
			return StanleyController.Instance.FieldOfView;
		}
		set
		{
			if (Application.isPlaying)
			{
				StanleyController.Instance.FieldOfView = value;
				return;
			}
			this.editorTestCamera.fieldOfView = value;
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			this.isLerping = false;
			this.editorTestCameraParent.gameObject.SetActive(false);
			this.editorTestCamera.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (this.isLerping)
		{
			this.Camera.transform.position = Vector3.Lerp(this.Parent.TransformPoint(Vector3.zero), this.cameraAnimationTarget.position, this.lerpValue);
			this.Camera.transform.rotation = Quaternion.Slerp(this.Parent.rotation, this.cameraAnimationTarget.rotation, this.lerpValue);
			this.FOV = Mathf.Lerp(this.OriginalFOV, this.targetFOV, this.lerpValue);
		}
	}

	public void StartLerping()
	{
		this.isLerping = true;
		this.lerpValue = 0f;
		StanleyController.Instance.FreezeMotionAndView();
		this.animator.SetTrigger("Do Transition");
		StanleyController.Instance.Bucket.SetAnimationSpeedImmediate(0.25f);
		StanleyController.Instance.Bucket.SetBucket(false);
	}

	public void AnimationEndedEvent()
	{
		UnityEvent unityEvent = this.onAnimationFinished;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	[Range(0f, 1f)]
	public float lerpValue;

	public bool isLerping;

	public Animator animator;

	public Transform cameraAnimationTarget;

	public float targetFOV = 72f;

	public Camera editorTestCamera;

	public Transform editorTestCameraParent;

	public float editorOriginalFOV = 72f;

	public UnityEvent onAnimationFinished;
}

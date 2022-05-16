using System;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
	private void Awake()
	{
		BackgroundCamera.OnRotationUpdate = (Action<Vector3>)Delegate.Combine(BackgroundCamera.OnRotationUpdate, new Action<Vector3>(this.UpdateRotation));
		BackgroundCamera.OnAlignToTransform = (Action<Transform>)Delegate.Combine(BackgroundCamera.OnAlignToTransform, new Action<Transform>(this.AlignToTransform));
		BackgroundCamera.OnPositionUpdate = (Action<Vector3>)Delegate.Combine(BackgroundCamera.OnPositionUpdate, new Action<Vector3>(this.UpdatePosition));
		if (this.backgroundCamera != null)
		{
			this.moveTransform.localPosition = Vector3.zero;
		}
		if (this.targetCamera != null)
		{
			this.AssignTargetCamera(this.targetCamera);
		}
	}

	private void OnDestroy()
	{
		BackgroundCamera.OnRotationUpdate = (Action<Vector3>)Delegate.Remove(BackgroundCamera.OnRotationUpdate, new Action<Vector3>(this.UpdateRotation));
		BackgroundCamera.OnAlignToTransform = (Action<Transform>)Delegate.Remove(BackgroundCamera.OnAlignToTransform, new Action<Transform>(this.AlignToTransform));
		BackgroundCamera.OnPositionUpdate = (Action<Vector3>)Delegate.Remove(BackgroundCamera.OnPositionUpdate, new Action<Vector3>(this.UpdatePosition));
	}

	private void Start()
	{
		BackgroundCamera.CameraMovementTypes cameraMovementTypes = this.movementType;
		if (cameraMovementTypes == BackgroundCamera.CameraMovementTypes.RelativeTransform)
		{
			this.AssignMainCameraAsTargetTransform();
			return;
		}
		if (cameraMovementTypes != BackgroundCamera.CameraMovementTypes.CharacterController)
		{
			return;
		}
		this.GetCharacterControllerFromMainCamera();
	}

	private void LateUpdate()
	{
		if (this.targetTransform == null)
		{
			this.AssignMainCameraAsTargetTransform();
		}
		this.backgroundCamera.fieldOfView = this.targetCamera.fieldOfView;
		BackgroundCamera.CameraMovementTypes cameraMovementTypes = this.movementType;
		if (cameraMovementTypes != BackgroundCamera.CameraMovementTypes.RelativeTransform)
		{
			if (cameraMovementTypes != BackgroundCamera.CameraMovementTypes.CharacterController)
			{
				return;
			}
			if (this.character == null)
			{
				this.GetCharacterControllerFromMainCamera();
			}
			if (this.moveTransform == null)
			{
				return;
			}
			Vector3 direction = Vector3.Scale(this.character.velocity * Time.deltaTime, this.parallaxStrength);
			this.moveTransform.Translate(this.yawTransform.TransformDirection(this.character.transform.InverseTransformDirection(direction)), Space.World);
			return;
		}
		else
		{
			if (this.moveTransform == null || this.targetCamera == null)
			{
				return;
			}
			if (!this.externalRotationUpdate)
			{
				this.moveTransform.rotation = this.targetTransform.rotation;
			}
			float num = this.sceneCenter.position.x - this.targetTransform.position.x;
			float num2 = this.sceneCenter.position.y - this.targetTransform.position.y;
			float num3 = this.sceneCenter.position.z - this.targetTransform.position.z;
			this.moveTransform.localPosition = new Vector3(-num * this.parallaxStrength.x, -num2 * this.parallaxStrength.y, -num3 * this.parallaxStrength.z);
			return;
		}
	}

	public void UpdateRotation(Vector3 rotationDelta)
	{
		if (!this.externalRotationUpdate)
		{
			return;
		}
		this.viewPitch += rotationDelta.x;
		this.viewPitch = Mathf.Clamp(this.viewPitch, -90f, 90f);
		Quaternion lhs = Quaternion.AngleAxis(rotationDelta.y, Vector3.up);
		this.pitchTransform.localRotation = Quaternion.AngleAxis(this.viewPitch, Vector3.right);
		this.yawTransform.rotation = lhs * this.yawTransform.rotation;
	}

	public void UpdatePosition(Vector3 movement)
	{
		Vector3 translation = Vector3.Scale(movement, this.parallaxStrength);
		this.moveTransform.Translate(translation, Space.World);
	}

	public void AlignToTransform(Transform alignTransform)
	{
		BackgroundCamera.CameraMovementTypes cameraMovementTypes = this.movementType;
		if (cameraMovementTypes != BackgroundCamera.CameraMovementTypes.RelativeTransform && cameraMovementTypes == BackgroundCamera.CameraMovementTypes.CharacterController)
		{
			Quaternion lhs = Quaternion.AngleAxis(alignTransform.eulerAngles.y, Vector3.up);
			this.yawTransform.rotation = lhs * this.yawTransform.rotation;
			this.viewPitch = alignTransform.eulerAngles.x;
		}
	}

	public void GetCharacterControllerFromMainCamera()
	{
		this.character = Camera.main.gameObject.GetComponentInParent<CharacterController>();
	}

	public void AssignSceneCenter(Transform newSceneCenter)
	{
		this.sceneCenter = newSceneCenter;
	}

	public void AssignMainCameraAsTargetTransform()
	{
		if (Camera.main != null)
		{
			this.AssignTargetCamera(Camera.main);
		}
	}

	public void AssignTargetCamera(Camera newTargetCamera)
	{
		this.targetCamera = newTargetCamera;
		this.targetTransform = newTargetCamera.transform;
		this.backgroundCamera.fieldOfView = newTargetCamera.fieldOfView;
	}

	public static Action<Vector3> OnRotationUpdate;

	public static Action<Vector3> OnPositionUpdate;

	public static Action<Transform> OnAlignToTransform;

	[SerializeField]
	private BackgroundCamera.CameraMovementTypes movementType;

	[SerializeField]
	private bool externalRotationUpdate;

	[SerializeField]
	private Camera backgroundCamera;

	[SerializeField]
	private Transform moveTransform;

	[SerializeField]
	private Transform pitchTransform;

	[SerializeField]
	private Transform yawTransform;

	[SerializeField]
	private CharacterController character;

	[SerializeField]
	private Camera targetCamera;

	[SerializeField]
	private Transform sceneCenter;

	[SerializeField]
	private Vector3 parallaxStrength = new Vector3(0.0005f, 5E-05f, 0.0005f);

	private Transform targetTransform;

	private float viewPitch;

	public enum CameraMovementTypes
	{
		RelativeTransform,
		CharacterController
	}
}

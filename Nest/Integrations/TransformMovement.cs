using System;
using Nest.Util;
using UnityEngine;

namespace Nest.Integrations
{
	[AddComponentMenu("Cast/Integrations/Transform Movement")]
	public class TransformMovement : BaseIntegration
	{
		public Transform TargetTransform
		{
			get
			{
				return this._targetTransform;
			}
			set
			{
				if (this._targetTransform != null)
				{
					this.OnDisable();
				}
				this._targetTransform = value;
				if (this._targetTransform != null)
				{
					this.OnEnable();
				}
			}
		}

		public override float InputValue
		{
			set
			{
				if (base.enabled && this._targetTransform != null)
				{
					if (this._translationEnabled)
					{
						this.UpdatePosition(value);
					}
					if (this._rotationEnabled)
					{
						this.UpdateRotation(value);
					}
					if (this._scaleEnabled)
					{
						this.UpdateScale(value);
					}
				}
			}
		}

		private Vector3 TranslationVector
		{
			get
			{
				switch (this._translationMode)
				{
				case TransformMovement.Translation.XAxis:
					return Vector3.right;
				case TransformMovement.Translation.YAxis:
					return Vector3.up;
				case TransformMovement.Translation.ZAxis:
					return Vector3.forward;
				case TransformMovement.Translation.Vector:
					return this._translationVector;
				}
				return this._randomVectorT;
			}
		}

		private Vector3 RotationAxis
		{
			get
			{
				switch (this._rotationMode)
				{
				case TransformMovement.Rotation.XAxis:
					return Vector3.right;
				case TransformMovement.Rotation.YAxis:
					return Vector3.up;
				case TransformMovement.Rotation.ZAxis:
					return Vector3.forward;
				case TransformMovement.Rotation.Vector:
					return this._rotationAxis;
				}
				return this._randomVectorR;
			}
		}

		private Vector3 ScaleVector
		{
			get
			{
				if (this._scaleMode == TransformMovement.Scale.Uniform)
				{
					return Vector3.one;
				}
				if (this._scaleMode == TransformMovement.Scale.Vector)
				{
					return this._scaleVector;
				}
				return this._randomVectorS;
			}
		}

		private void UpdatePosition(float value)
		{
			float d = Mathf.Lerp(this._translationAmount0, this._translationAmount1, value);
			Vector3 vector = this.TranslationVector * d;
			if (this._addToOriginal)
			{
				vector += this._originalPosition;
			}
			this._targetTransform.localPosition = vector;
		}

		private void UpdateRotation(float value)
		{
			Quaternion quaternion = Quaternion.AngleAxis(Mathf.Lerp(this._rotationAngle0, this._rotationAngle1, value), this.RotationAxis);
			if (this._addToOriginal)
			{
				quaternion = this._originalRotation * quaternion;
			}
			this._targetTransform.localRotation = quaternion;
		}

		private void UpdateScale(float value)
		{
			float d = Mathf.Lerp(this._scaleAmount0, this._scaleAmount1, value);
			Vector3 vector = this.ScaleVector * d;
			if (this._addToOriginal)
			{
				vector += this._originalScale;
			}
			this._targetTransform.localScale = vector;
		}

		private void OnEnable()
		{
			if (this._targetTransform == null)
			{
				this._targetTransform = base.transform;
			}
			this._originalPosition = this._targetTransform.localPosition;
			this._originalRotation = this._targetTransform.localRotation;
			this._originalScale = this._targetTransform.localScale;
		}

		private void OnDisable()
		{
			if (this._targetTransform != null)
			{
				this._targetTransform.localPosition = this._originalPosition;
				this._targetTransform.localRotation = this._originalRotation;
				this._targetTransform.localScale = this._originalScale;
			}
		}

		private void Start()
		{
			this._randomVectorT = Random.onUnitSphere;
			this._randomVectorR = Random.onUnitSphere;
			this._randomVectorS = new Vector3(Random.value, Random.value, Random.value);
			this._rigidBody = this._targetTransform.GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			if (this._referenceTransform == null)
			{
				return;
			}
			TransformMovement.Rotation rotationMode = this._rotationMode;
			if (rotationMode != TransformMovement.Rotation.Mirror)
			{
				if (rotationMode == TransformMovement.Rotation.LookTowards)
				{
					if (this._rigidBody != null)
					{
						Vector3 currentError = this._rigidBody.angularVelocity * -1f;
						Debug.DrawRay(base.transform.position, this._rigidBody.angularVelocity * 10f, Color.black);
						Vector3 vector = this._angularVelocityController.Update(currentError, Time.deltaTime);
						Debug.DrawRay(base.transform.position, vector, Color.green);
						this._rigidBody.AddTorque(vector);
						Vector3 vector2 = this._referenceTransform.position - base.transform.position;
						Debug.DrawRay(base.transform.position, vector2, Color.magenta);
						Vector3 forward = base.transform.forward;
						Debug.DrawRay(base.transform.position, forward * 15f, Color.blue);
						Vector3 currentError2 = Vector3.Cross(forward, vector2);
						Vector3 torque = this._headingController.Update(currentError2, Time.deltaTime);
						this._rigidBody.AddTorque(torque);
					}
					else
					{
						base.transform.LookAt(this._referenceTransform.position);
					}
				}
			}
			else if (this._rigidBody != null)
			{
				this._rigidBody.rotation = this._referenceTransform.rotation;
			}
			else
			{
				base.transform.rotation = this._referenceTransform.rotation;
			}
			TransformMovement.Translation translationMode = this._translationMode;
			if (translationMode == TransformMovement.Translation.Mirror)
			{
				base.transform.localPosition = this._referenceTransform.localPosition;
			}
		}

		public void ResetPosition()
		{
			if (this._rigidBody != null)
			{
				this._rigidBody.position = this._originalPosition;
				return;
			}
			this._targetTransform.localPosition = this._originalPosition;
		}

		public void ResetPosition(Transform tr)
		{
			this._originalPosition = tr.position;
			this.ResetPosition();
		}

		[SerializeField]
		private bool _translationEnabled;

		[SerializeField]
		private TransformMovement.Translation _translationMode;

		[SerializeField]
		private Vector3 _translationVector = Vector3.forward;

		[SerializeField]
		private float _translationAmount0;

		[SerializeField]
		private float _translationAmount1 = 10f;

		[SerializeField]
		private bool _rotationEnabled;

		[SerializeField]
		private TransformMovement.Rotation _rotationMode;

		[SerializeField]
		private Vector3 _rotationAxis = Vector3.up;

		[SerializeField]
		private float _rotationAngle0;

		[SerializeField]
		private float _rotationAngle1 = 90f;

		private readonly VectorPid _angularVelocityController = new VectorPid(33.7766f, 0f, 0.2553191f);

		private readonly VectorPid _headingController = new VectorPid(9.244681f, 0f, 0.06382979f);

		[SerializeField]
		private TransformMovement.Scale _scaleMode;

		[SerializeField]
		private Vector3 _scaleVector = Vector3.one;

		[SerializeField]
		private float _scaleAmount0;

		[SerializeField]
		private float _scaleAmount1 = 1f;

		[SerializeField]
		private bool _scaleEnabled;

		[SerializeField]
		private Transform _targetTransform;

		[SerializeField]
		private Transform _referenceTransform;

		[SerializeField]
		private bool _addToOriginal = true;

		private Vector3 _originalPosition;

		private Quaternion _originalRotation;

		private Vector3 _originalScale;

		private Vector3 _randomVectorT;

		private Vector3 _randomVectorR;

		private Vector3 _randomVectorS;

		private Rigidbody _rigidBody;

		private float _rotationSpeed;

		public enum Translation
		{
			XAxis,
			YAxis,
			ZAxis,
			Mirror,
			Vector,
			Random
		}

		public enum Rotation
		{
			XAxis,
			YAxis,
			ZAxis,
			Mirror,
			LookTowards,
			Vector,
			Random
		}

		public enum Scale
		{
			Uniform,
			Vector,
			Random
		}
	}
}

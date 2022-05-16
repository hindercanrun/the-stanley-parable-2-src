using System;
using UnityEngine;

namespace AmazingAssets.CurvedWorld
{
	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class CurvedWorldCamera : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.activeCamera == null)
			{
				this.activeCamera = base.GetComponent<Camera>();
			}
		}

		private void OnDisable()
		{
			if (this.activeCamera != null)
			{
				this.activeCamera.ResetCullingMatrix();
			}
		}

		private void Start()
		{
			if (this.activeCamera == null)
			{
				this.activeCamera = base.GetComponent<Camera>();
			}
		}

		private void Update()
		{
			if (this.activeCamera == null)
			{
				this.activeCamera = base.GetComponent<Camera>();
			}
			if (this.activeCamera == null)
			{
				base.enabled = false;
				return;
			}
			if (this.nearClipPlane >= this.activeCamera.farClipPlane)
			{
				this.nearClipPlane = this.activeCamera.farClipPlane - 0.01f;
			}
			if (this.matrixType == CurvedWorldCamera.MATRIX_TYPE.Perspective)
			{
				this.fieldOfView = Mathf.Clamp(this.fieldOfView, 1f, 179f);
				this.activeCamera.cullingMatrix = Matrix4x4.Perspective(this.fieldOfView, 1f, this.activeCamera.nearClipPlane, this.activeCamera.farClipPlane) * this.activeCamera.worldToCameraMatrix;
				return;
			}
			this.size = ((this.size < 1f) ? 1f : this.size);
			this.activeCamera.cullingMatrix = Matrix4x4.Ortho(-this.size, this.size, -this.size, this.size, this.nearClipPlaneSameAsCamera ? this.activeCamera.nearClipPlane : this.nearClipPlane, this.activeCamera.farClipPlane) * this.activeCamera.worldToCameraMatrix;
		}

		private void Reset()
		{
			if (this.activeCamera != null)
			{
				this.activeCamera.ResetCullingMatrix();
				this.fieldOfView = this.activeCamera.fieldOfView;
				this.size = this.activeCamera.orthographicSize;
				this.nearClipPlane = this.activeCamera.nearClipPlane;
			}
		}

		public CurvedWorldCamera.MATRIX_TYPE matrixType;

		[Range(1f, 179f)]
		public float fieldOfView = 60f;

		public float size = 5f;

		public bool nearClipPlaneSameAsCamera = true;

		public float nearClipPlane = 0.3f;

		private Camera activeCamera;

		public enum MATRIX_TYPE
		{
			Perspective,
			Orthographic
		}
	}
}

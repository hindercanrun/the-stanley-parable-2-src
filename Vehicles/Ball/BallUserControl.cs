using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Ball
{
	public class BallUserControl : MonoBehaviour
	{
		private void Awake()
		{
			this.ball = base.GetComponent<Ball>();
			if (Camera.main != null)
			{
				this.cam = Camera.main.transform;
				return;
			}
			Debug.LogWarning("Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
		}

		private void Update()
		{
			float axis = CrossPlatformInputManager.GetAxis("Horizontal");
			float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
			this.jump = CrossPlatformInputManager.GetButton("Jump");
			if (this.cam != null)
			{
				this.camForward = Vector3.Scale(this.cam.forward, new Vector3(1f, 0f, 1f)).normalized;
				this.move = (axis2 * this.camForward + axis * this.cam.right).normalized;
				return;
			}
			this.move = (axis2 * Vector3.forward + axis * Vector3.right).normalized;
		}

		private void FixedUpdate()
		{
			this.ball.Move(this.move, this.jump);
			this.jump = false;
		}

		private Ball ball;

		private Vector3 move;

		private Transform cam;

		private Vector3 camForward;

		private bool jump;
	}
}

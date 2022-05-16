using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Ball
{
	public class Ball : MonoBehaviour
	{
		private void Start()
		{
			this.m_Rigidbody = base.GetComponent<Rigidbody>();
			base.GetComponent<Rigidbody>().maxAngularVelocity = this.m_MaxAngularVelocity;
		}

		public void Move(Vector3 moveDirection, bool jump)
		{
			if (this.m_UseTorque)
			{
				this.m_Rigidbody.AddTorque(new Vector3(moveDirection.z, 0f, -moveDirection.x) * this.m_MovePower);
			}
			else
			{
				this.m_Rigidbody.AddForce(moveDirection * this.m_MovePower);
			}
			if (Physics.Raycast(base.transform.position, -Vector3.up, 1f) && jump)
			{
				this.m_Rigidbody.AddForce(Vector3.up * this.m_JumpPower, ForceMode.Impulse);
			}
		}

		[SerializeField]
		private float m_MovePower = 5f;

		[SerializeField]
		private bool m_UseTorque = true;

		[SerializeField]
		private float m_MaxAngularVelocity = 25f;

		[SerializeField]
		private float m_JumpPower = 2f;

		private const float k_GroundRayLength = 1f;

		private Rigidbody m_Rigidbody;
	}
}

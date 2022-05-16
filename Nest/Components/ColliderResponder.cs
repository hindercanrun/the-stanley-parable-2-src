using System;
using System.Linq;
using UnityEngine;

namespace Nest.Components
{
	[AddComponentMenu("Nest/Components/Collider Responder")]
	[RequireComponent(typeof(Rigidbody), typeof(Collider))]
	public class ColliderResponder : NestInput
	{
		private void OnCollisionEnter(Collision collision)
		{
			if (!this._tagValues.Contains(collision.gameObject.tag))
			{
				return;
			}
			if (collision.rigidbody != null)
			{
				this._force = collision.rigidbody.velocity.magnitude;
			}
			this.Invoke(ColliderResponder.CollisionType.Enter);
		}

		private void OnCollisionExit(Collision collision)
		{
			if (!this._tagValues.Contains(collision.gameObject.tag))
			{
				return;
			}
			this.Invoke(ColliderResponder.CollisionType.Exit);
		}

		private void OnCollisionStay(Collision collision)
		{
			if (!this._tagValues.Contains(collision.gameObject.tag))
			{
				return;
			}
			this.Invoke(ColliderResponder.CollisionType.Stay);
		}

		public void Invoke(ColliderResponder.CollisionType type)
		{
			if ((this.CollisionEvent & type) == (ColliderResponder.CollisionType)0)
			{
				return;
			}
			this.Value.CurrentValue = this._force;
			base.Invoke();
		}

		public ColliderResponder.CollisionType CollisionEvent = ColliderResponder.CollisionType.Enter;

		private float _force;

		[SerializeField]
		public int TagMask = -1;

		[SerializeField]
		private string[] _tagValues;

		[Flags]
		public enum CollisionType
		{
			Enter = 1,
			Stay = 2,
			Exit = 4,
			EnterAndExit = 5
		}
	}
}

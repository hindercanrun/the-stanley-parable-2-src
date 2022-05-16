using System;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class AICharacterControl : MonoBehaviour
	{
		public NavMeshAgent agent { get; private set; }

		public ThirdPersonCharacter character { get; private set; }

		private void Start()
		{
			this.agent = base.GetComponentInChildren<NavMeshAgent>();
			this.character = base.GetComponent<ThirdPersonCharacter>();
			this.agent.updateRotation = false;
			this.agent.updatePosition = true;
		}

		private void Update()
		{
			if (this.target != null)
			{
				this.agent.SetDestination(this.target.position);
			}
			if (this.agent.remainingDistance > this.agent.stoppingDistance)
			{
				this.character.Move(this.agent.desiredVelocity, false, false);
				return;
			}
			this.character.Move(Vector3.zero, false, false);
		}

		public void SetTarget(Transform target)
		{
			this.target = target;
		}

		public Transform target;
	}
}

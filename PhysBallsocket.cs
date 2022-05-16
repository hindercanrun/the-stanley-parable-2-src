using System;
using UnityEngine;

public class PhysBallsocket : HammerEntity
{
	private void OnValidate()
	{
		if (this.connectedEntity == null || this.connectedEntity.name != this.connectedEntityName)
		{
			GameObject exists = GameObject.Find(this.connectedEntityName);
			if (exists)
			{
				this.connectedEntity = exists;
			}
			else
			{
				this.connectedEntity = null;
			}
		}
		if (this.connectedEntity)
		{
			Rigidbody rigidbody = this.connectedEntity.GetComponent<Rigidbody>();
			Rigidbody rigidbody2 = base.GetComponent<Rigidbody>();
			ConfigurableJoint configurableJoint = base.GetComponent<ConfigurableJoint>();
			if (!rigidbody)
			{
				rigidbody = this.connectedEntity.AddComponent<Rigidbody>();
			}
			if (!rigidbody2)
			{
				rigidbody2 = base.gameObject.AddComponent<Rigidbody>();
			}
			if (!configurableJoint)
			{
				configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
			}
			rigidbody2.constraints = RigidbodyConstraints.FreezePosition;
			rigidbody2.useGravity = false;
			rigidbody.drag = 0.01f;
			configurableJoint.connectedBody = rigidbody;
			configurableJoint.xMotion = ConfigurableJointMotion.Locked;
			configurableJoint.yMotion = ConfigurableJointMotion.Locked;
			configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		}
	}

	public string connectedEntityName = "";

	[SerializeField]
	[HideInInspector]
	private GameObject connectedEntity;
}

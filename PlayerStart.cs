using System;
using UnityEngine;

public class PlayerStart : HammerEntity
{
	private void Start()
	{
		this.Respawn();
	}

	public void Respawn()
	{
		StanleyController.TeleportType style = StanleyController.TeleportType.PlayerStartMaster;
		if (!this.isMaster)
		{
			style = StanleyController.TeleportType.PlayerStart;
		}
		StanleyController.Instance.Teleport(style, base.transform.position, -base.transform.up, true, false, true, base.transform);
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		float num = 0.64f;
		for (int i = 0; i < 10; i++)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(Vector3.forward * Mathf.Lerp(num * 0.75f, num, (float)i / 9f), Vector3.down * 0.25f);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawCube(Vector3.zero + Vector3.forward * (num / 2f), new Vector3(0.05f, 0.05f, num));
	}

	public bool isMaster;
}

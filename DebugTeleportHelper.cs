using System;
using UnityEngine;

public class DebugTeleportHelper : MonoBehaviour
{
	[ContextMenu("Teleport Stanley")]
	private void TeleportStanley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		StanleyController.Instance.transform.position = base.transform.position;
	}
}

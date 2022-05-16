using System;
using UnityEngine;

public class SwitchMovableTree : MonoBehaviour
{
	private void Start()
	{
		this.MoveToPosition(CullForSwitchController.IsSwitchEnvironment);
	}

	[ContextMenu("Move All To Switch Position")]
	private void MoveAllToSwitchPosition()
	{
		SwitchMovableTree[] array = Object.FindObjectsOfType<SwitchMovableTree>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MoveToPosition(true);
		}
	}

	[ContextMenu("Move All To Normal Position")]
	private void MoveAllToNormalPosition()
	{
		SwitchMovableTree[] array = Object.FindObjectsOfType<SwitchMovableTree>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MoveToPosition(false);
		}
	}

	private void MoveToPosition(bool toSwitchPosition)
	{
		if (toSwitchPosition)
		{
			this.objectToMove.transform.position = this.switchPosition.position;
			return;
		}
		this.objectToMove.transform.localPosition = Vector3.zero;
	}

	public Transform switchPosition;

	public GameObject objectToMove;
}

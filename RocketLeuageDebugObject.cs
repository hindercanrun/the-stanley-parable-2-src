using System;
using UnityEngine;

public class RocketLeuageDebugObject : MonoBehaviour
{
	private void Start()
	{
		if (RocketLeuageDebugButtons.Instance != null)
		{
			RocketLeuageDebugButtons.Instance.RegisterObject(this.id, base.gameObject);
		}
	}

	[ContextMenu("Set To GameObject Name")]
	private void SetToGameObjectName()
	{
		this.id = base.gameObject.name;
	}

	public string id = "None";
}

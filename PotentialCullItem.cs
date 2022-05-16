using System;
using UnityEngine;

[ExecuteInEditMode]
public class PotentialCullItem : MonoBehaviour
{
	public GameObject TargetGameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	public bool definitelyCulled;
}

using System;
using UnityEngine;

[ExecuteInEditMode]
public class CullHelper : MonoBehaviour
{
	public static CullHelper Instance { get; private set; }

	[ContextMenu("Referesh Instance")]
	private void Awake()
	{
		if (CullHelper.Instance != null)
		{
			Debug.LogWarning("Duplicate Cull helper!");
			return;
		}
		CullHelper.Instance = this;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
			return;
		}
		if (this.disablePotentiallyCulledItems != this.disabledPotentialCulledItemsPrev || this.disableDefinitelyCulledItems != this.disableDefinitelyCulledItemsPrev)
		{
			this.disabledPotentialCulledItemsPrev = this.disablePotentiallyCulledItems;
			this.disableDefinitelyCulledItemsPrev = this.disableDefinitelyCulledItems;
			foreach (PotentialCullItem potentialCullItem in Resources.FindObjectsOfTypeAll(typeof(PotentialCullItem)))
			{
				if (potentialCullItem.definitelyCulled)
				{
					potentialCullItem.TargetGameObject.SetActive(!this.disableDefinitelyCulledItems);
				}
				else
				{
					potentialCullItem.TargetGameObject.SetActive(!this.disablePotentiallyCulledItems);
				}
			}
		}
	}

	public bool disablePotentiallyCulledItems;

	public bool disableDefinitelyCulledItems;

	private bool disabledPotentialCulledItemsPrev = true;

	private bool disableDefinitelyCulledItemsPrev = true;
}

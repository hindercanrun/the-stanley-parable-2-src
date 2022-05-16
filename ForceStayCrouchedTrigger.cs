using System;
using UnityEngine;

public class ForceStayCrouchedTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.forceCrouchRegardless)
		{
			StanleyController.Instance.ForceCrouched = true;
			return;
		}
		StanleyController.Instance.ForceStayCrouched = true;
	}

	private void OnTriggerStay(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.forceCrouchRegardless)
		{
			StanleyController.Instance.ForceCrouched = true;
			return;
		}
		StanleyController.Instance.ForceStayCrouched = true;
	}

	private void OnTriggerExit(Collider col)
	{
		if (!col.CompareTag("Player"))
		{
			return;
		}
		if (this.forceCrouchRegardless)
		{
			StanleyController.Instance.ForceCrouched = false;
			return;
		}
		StanleyController.Instance.ForceStayCrouched = false;
	}

	private void OnDestroy()
	{
		if (StanleyController.Instance != null)
		{
			StanleyController.Instance.ForceCrouched = false;
			StanleyController.Instance.ForceStayCrouched = false;
		}
	}

	public bool forceCrouchRegardless;
}

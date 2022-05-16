using System;
using UnityEngine;

public class PortalScreen : MonoBehaviour
{
	private void OnBecameVisible()
	{
		if (this.OnVisible != null)
		{
			this.OnVisible();
		}
	}

	private void OnBecameInvisible()
	{
		if (this.OnInvisible != null)
		{
			this.OnInvisible();
		}
	}

	public Action OnVisible;

	public Action OnInvisible;
}

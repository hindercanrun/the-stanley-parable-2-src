using System;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
	public void Open()
	{
		Application.OpenURL(this.url);
	}

	[SerializeField]
	private string url;
}

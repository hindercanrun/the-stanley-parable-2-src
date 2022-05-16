using System;
using UnityEngine;

public class CursorController : MonoBehaviour
{
	public static CursorController Instance { get; private set; }

	public void Awake()
	{
		CursorController.Instance = this;
		this.SetCursor();
	}

	public void SetCustomCursor(CursorProfile profile)
	{
		this.customProfile = profile;
		this.SetCursor();
	}

	public void ResetToDefault()
	{
		this.customProfile = null;
		this.SetCursor();
	}

	private void SetCursor()
	{
		if (!(this.customProfile == null))
		{
			Cursor.SetCursor(this.customProfile.cursorTexture, this.customProfile.hotspot, CursorMode.ForceSoftware);
			return;
		}
		if (this.defaultProfile == null || this.defaultProfile.cursorTexture == null)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			return;
		}
		Cursor.SetCursor(this.defaultProfile.cursorTexture, this.defaultProfile.hotspot, CursorMode.ForceSoftware);
	}

	public CursorProfile defaultProfile;

	private CursorProfile customProfile;
}

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cursor Profile", menuName = "Stanley/Cursor Profile")]
public class CursorProfile : ScriptableObject
{
	public void SetCursorToThis()
	{
		CursorController.Instance.SetCustomCursor(this);
	}

	public void ResetCursor()
	{
		CursorController.Instance.ResetToDefault();
	}

	public Texture2D cursorTexture;

	public Vector2 hotspot;
}

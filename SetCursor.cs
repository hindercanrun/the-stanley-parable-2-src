using System;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
	private void Update()
	{
		Cursor.SetCursor(this.cursorTexture, this.hotSpot, this.cursorMode);
	}

	public Texture2D cursorTexture;

	public CursorMode cursorMode = CursorMode.ForceSoftware;

	public Vector2 hotSpot = Vector2.zero;
}

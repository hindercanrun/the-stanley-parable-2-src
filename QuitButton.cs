using System;
using UnityEngine;

public class QuitButton : MenuButton
{
	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		Application.Quit();
	}
}

using System;
using UnityEngine;

public class ResumeButton : MenuButton
{
	protected override void Update()
	{
		base.Update();
		if (Singleton<GameMaster>.Instance.stanleyActions.JumpAction.WasPressed)
		{
			this.OnClick(default(Vector3));
		}
	}

	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		Singleton<GameMaster>.Instance.ClosePauseMenu(true);
	}
}

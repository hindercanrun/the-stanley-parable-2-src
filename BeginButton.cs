using System;
using UnityEngine;

public class BeginButton : MenuButton
{
	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		Singleton<GameMaster>.Instance.TSP_Reload();
		Singleton<GameMaster>.Instance.ClosePauseMenu(true);
	}
}

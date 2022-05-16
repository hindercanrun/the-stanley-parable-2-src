using System;
using UnityEngine;

public class MenuToggleButton : MenuButton
{
	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		if (this.toShow != null && this.toHide != null)
		{
			this.toShow.SetActive(!this.toShow.activeInHierarchy);
			this.toHide.SetActive(!this.toHide.activeInHierarchy);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (this.hijackCancel && Singleton<GameMaster>.Instance.stanleyActions.JumpAction.WasPressed)
		{
			this.OnClick(default(Vector3));
		}
	}

	public override void OnHover()
	{
		base.OnHover();
		if (this.useLeftRight && (Singleton<GameMaster>.Instance.stanleyActions.Left.WasPressed || Singleton<GameMaster>.Instance.stanleyActions.Right.WasPressed))
		{
			this.OnClick(default(Vector3));
		}
	}

	public GameObject toHide;

	public GameObject toShow;

	[Space(5f)]
	public bool hijackCancel;

	public bool useLeftRight;
}

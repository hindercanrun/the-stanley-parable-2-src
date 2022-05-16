using System;
using UnityEngine;

public class MenuCCButton : MenuButton
{
	private void OnEnable()
	{
		this.current = Singleton<GameMaster>.Instance.closedCaptionsOn;
		this.enabledText.SetActive(this.current);
		this.disabledText.SetActive(!this.current);
	}

	public override void OnHover()
	{
		base.OnHover();
		if (Singleton<GameMaster>.Instance.stanleyActions.Right.WasPressed)
		{
			this.OnClick(default(Vector3));
			return;
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Left.WasPressed)
		{
			this.OnClick(default(Vector3));
		}
	}

	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		this.current = Singleton<GameMaster>.Instance.closedCaptionsOn;
		this.enabledText.SetActive(!this.current);
		this.current = !this.current;
		this.disabledText.SetActive(!this.current);
	}

	public override void SaveChange()
	{
		base.SaveChange();
		if (this.original && this.original)
		{
			Singleton<GameMaster>.Instance.SetCaptionsActive(this.current);
		}
	}

	public GameObject enabledText;

	public GameObject disabledText;

	private bool current = true;
}

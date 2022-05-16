using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UI Screen Reference", menuName = "")]
public class UIScreenReference : ScriptableObject
{
	public void Call()
	{
		if (this.OnCall != null)
		{
			this.OnCall();
		}
	}

	public void CallWithPrevious(UIScreenReference previousScreen)
	{
		this.previous = previousScreen;
		this.previous.Hide();
		if (this.OnCall != null)
		{
			this.OnCall();
		}
	}

	public void CloseAndCallPrevious()
	{
		if (this.previous != null)
		{
			this.previous.Show();
			this.previous = null;
		}
		if (this.OnClose != null)
		{
			this.OnClose();
		}
	}

	public void Close()
	{
		if (this.OnClose != null)
		{
			this.OnClose();
		}
	}

	public void Hide()
	{
		if (this.OnHide != null)
		{
			this.OnHide();
		}
	}

	public void Show()
	{
		if (this.OnShow != null)
		{
			this.OnShow();
		}
	}

	public Action OnCall;

	public Action OnClose;

	public Action OnHide;

	public Action OnShow;

	[NonSerialized]
	private UIScreenReference previous;
}

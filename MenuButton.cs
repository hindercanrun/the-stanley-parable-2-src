using System;
using TMPro;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
	protected virtual void Update()
	{
		if (this.hoveringLastFrame && !this.hoveringThisFrame)
		{
			this.OnExit();
		}
		if (this.hoveringThisFrame)
		{
			this.hoveringThisFrame = false;
			this.hoveringLastFrame = true;
			return;
		}
		this.hoveringLastFrame = false;
	}

	public virtual void OnClick(Vector3 point = default(Vector3))
	{
	}

	public virtual void OnHold(Vector3 point = default(Vector3))
	{
	}

	public virtual void OnEnter()
	{
		for (int i = 0; i < this.text.Length; i++)
		{
			if (this.text[i] != null)
			{
				this.text[i].color = new Color(0.6f, 0.6f, 0.6f, 1f);
			}
		}
	}

	public virtual void OnExit()
	{
		for (int i = 0; i < this.text.Length; i++)
		{
			if (this.text[i] != null)
			{
				this.text[i].color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	public virtual void OnHover()
	{
		if (!this.hoveringLastFrame)
		{
			this.OnEnter();
		}
		this.hoveringThisFrame = true;
		if (this.control != null)
		{
			if (Singleton<GameMaster>.Instance.stanleyActions.Right.IsPressed)
			{
				if (this.pressedTimer <= 0f)
				{
					this.refireCounter++;
					this.pressedTimer = this.refireTime / Mathf.Clamp((float)this.refireCounter, 1f, 10f);
					this.control.OnInput(DPadDir.Right);
				}
				this.pressedTimer -= Time.deltaTime;
				return;
			}
			if (Singleton<GameMaster>.Instance.stanleyActions.Left.IsPressed)
			{
				if (this.pressedTimer <= 0f)
				{
					this.refireCounter++;
					this.pressedTimer = this.refireTime / Mathf.Clamp((float)this.refireCounter, 1f, 5f);
					this.control.OnInput(DPadDir.Left);
				}
				this.pressedTimer -= Time.deltaTime;
				return;
			}
			this.pressedTimer = 0f;
			this.refireCounter = 0;
		}
	}

	public virtual void OnInput(DPadDir direction)
	{
	}

	protected virtual void OnDisable()
	{
		this.hoveringThisFrame = false;
		this.hoveringLastFrame = false;
		this.OnExit();
	}

	public virtual void SaveChange()
	{
	}

	public bool original = true;

	private bool hoveringLastFrame;

	private bool hoveringThisFrame;

	public TextMeshProUGUI[] text;

	public MenuButton control;

	private float pressedTimer;

	private float refireTime = 0.25f;

	private int refireCounter;
}

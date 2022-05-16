using System;
using UnityEngine;

public class CursorHijack : MonoBehaviour
{
	private void Awake()
	{
		this.RT = base.GetComponent<RectTransform>();
		this.cursorRT = this.cursor.GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (Singleton<GameMaster>.Instance.stanleyActions.Up.IsPressed)
		{
			if (this.pressedTimer <= 0f)
			{
				this.refireCounter++;
				this.pressedTimer = this.refireTime / Mathf.Clamp((float)this.refireCounter, 1f, 10f);
				this.MoveCursor(DPadDir.Up);
			}
			this.pressedTimer -= Time.deltaTime;
			return;
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Down.IsPressed)
		{
			if (this.pressedTimer <= 0f)
			{
				this.refireCounter++;
				this.pressedTimer = this.refireTime / Mathf.Clamp((float)this.refireCounter, 1f, 5f);
				this.MoveCursor(DPadDir.Down);
			}
			this.pressedTimer -= Time.deltaTime;
			return;
		}
		this.pressedTimer = 0f;
		this.refireCounter = 0;
	}

	private void MoveCursor(DPadDir direction)
	{
		if (direction == DPadDir.Left || direction == DPadDir.Right)
		{
			return;
		}
		if (this.cursor.hoveringOver == "")
		{
			float num = float.PositiveInfinity;
			int num2 = -1;
			for (int i = 0; i < this.menuItems.Length; i++)
			{
				float num3 = Mathf.Abs(this.cursorRT.anchoredPosition3D.y - (-512f + this.RT.InverseTransformPoint(this.menuItems[i].position).y));
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				this.currentIndex = num2;
			}
		}
		else
		{
			if (direction == DPadDir.Up)
			{
				this.currentIndex--;
			}
			else if (direction == DPadDir.Down)
			{
				this.currentIndex++;
			}
			if (this.currentIndex < 0)
			{
				this.currentIndex = this.menuItems.Length - 1;
			}
			if (this.currentIndex > this.menuItems.Length - 1)
			{
				this.currentIndex = 0;
			}
		}
		this.cursorRT.anchoredPosition3D = new Vector3(1024f, -512f, 0f) + this.RT.InverseTransformPoint(this.menuItems[this.currentIndex].position);
	}

	public MenuCursor cursor;

	private RectTransform cursorRT;

	[Space(10f)]
	public RectTransform[] menuItems;

	private int currentIndex;

	private float pressedTimer;

	private float refireTime = 0.25f;

	private int refireCounter;

	private RectTransform RT;
}

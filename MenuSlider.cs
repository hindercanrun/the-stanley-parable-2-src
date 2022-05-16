using System;
using UnityEngine;

public class MenuSlider : MenuButton
{
	public float position
	{
		get
		{
			return this._position;
		}
		protected set
		{
			if (this._position != Mathf.Clamp01(value))
			{
				this._position = Mathf.Clamp01(value);
				this.Changed();
			}
		}
	}

	public override void OnClick(Vector3 point)
	{
		base.OnClick(point);
		this.ClickOrHold(point);
	}

	public override void OnHold(Vector3 point)
	{
		base.OnHold(point);
		this.ClickOrHold(point);
	}

	private void ClickOrHold(Vector3 point)
	{
		float x = this.foregroundRT.InverseTransformPoint(point).x;
		this.position = x / this.maxPixels;
	}

	public override void OnInput(DPadDir direction)
	{
		base.OnInput(direction);
		if (direction == DPadDir.Up || direction == DPadDir.Down)
		{
			return;
		}
		int num = 1;
		if (direction == DPadDir.Left)
		{
			num = -1;
		}
		if (direction == DPadDir.Right)
		{
			num = 1;
		}
		int num2 = Mathf.RoundToInt(this.position * (float)this.gradations);
		num2 += num;
		this.position = (float)num2 / (float)this.gradations;
	}

	protected virtual void Changed()
	{
		this.foregroundRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.maxPixels * this.position);
		this.pipRT.anchoredPosition3D = new Vector3(this.maxPixels * this.position, this.pipHeight, 0f);
	}

	public RectTransform foregroundRT;

	public RectTransform pipRT;

	private float maxPixels = 400f;

	private float pipHeight = 40f;

	private int gradations = 20;

	private float _position = 1f;
}

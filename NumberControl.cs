using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberControl : Selectable
{
	public override void Select()
	{
		base.Select();
		this.selected = true;
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		this.selected = false;
	}

	public override void OnMove(AxisEventData eventData)
	{
		MoveDirection moveDir = eventData.moveDir;
		if (moveDir == MoveDirection.Up)
		{
			this.UpdateNumberValue(1);
			return;
		}
		if (moveDir != MoveDirection.Down)
		{
			base.OnMove(eventData);
			return;
		}
		this.UpdateNumberValue(-1);
	}

	public void UpdateNumberValue(int changeValue)
	{
		this.UpdateNumberValue(changeValue, false);
	}

	public void UpdateNumberValue(int changeValue, bool silent)
	{
		if (!silent)
		{
			this.valueChangedEvent.Call();
		}
		this.numberValue += changeValue;
		NumberControl.NumberTypes numberTypes = this.numberType;
		if (numberTypes != NumberControl.NumberTypes.Minute)
		{
			if (numberTypes == NumberControl.NumberTypes.Hour)
			{
				if (this.numberValue >= 24)
				{
					this.numberValue = 0;
				}
				if (this.numberValue < 0)
				{
					this.numberValue = 23;
				}
			}
		}
		else
		{
			if (this.numberValue >= 60)
			{
				this.numberValue = 0;
				NumberControl numberControl = this.otherNumberControl;
				if (numberControl != null)
				{
					numberControl.UpdateNumberValue(1, true);
				}
			}
			if (this.numberValue < 0)
			{
				this.numberValue = 59;
				NumberControl numberControl2 = this.otherNumberControl;
				if (numberControl2 != null)
				{
					numberControl2.UpdateNumberValue(-1, true);
				}
			}
		}
		this.UpdateText();
	}

	private void UpdateText()
	{
		NumberControl.NumberTypes numberTypes = this.numberType;
		if (numberTypes == NumberControl.NumberTypes.Minute)
		{
			this.numberDisplay.text = IntroTimeDisplay.GetMinuteString(this.numberValue);
			return;
		}
		if (numberTypes != NumberControl.NumberTypes.Hour)
		{
			return;
		}
		string text;
		this.numberDisplay.text = IntroTimeDisplay.GetHourString(this.numberValue, out text);
		if (this.ampmDisplay != null)
		{
			this.ampmDisplay.text = text;
		}
	}

	[Space]
	[SerializeField]
	private NumberControl.NumberTypes numberType;

	private int numberValue;

	[SerializeField]
	private TextMeshProUGUI numberDisplay;

	[Header("used to update hours if minutes goes below or above 59|00 mark")]
	[SerializeField]
	private NumberControl otherNumberControl;

	[Header("leave null for 24 hour clock, otherwise 12 hour clock")]
	[SerializeField]
	private TextMeshProUGUI ampmDisplay;

	[SerializeField]
	private SimpleEvent valueChangedEvent;

	private bool selected;

	public enum NumberTypes
	{
		Minute,
		Hour
	}
}

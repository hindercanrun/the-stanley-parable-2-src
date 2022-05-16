using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerDownButton : Button, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDeselectHandler
{
	public override void OnPointerClick(PointerEventData eventData)
	{
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		base.OnPointerClick(eventData);
		this.isHolding = true;
		this.pointerDownEventData = eventData;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		this.isHolding = false;
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		this.isHolding = false;
	}

	protected override void OnDisable()
	{
		this.isHolding = false;
		base.OnDisable();
	}

	public void Update()
	{
		if (this.isHolding)
		{
			this.timeSinceHold += Time.deltaTime;
		}
		else
		{
			this.timeSinceHold = 0f;
			this.onRepeat = false;
		}
		if (!this.onRepeat)
		{
			if (this.timeSinceHold > this.repeatTimeInit)
			{
				base.OnPointerClick(this.pointerDownEventData);
				this.onRepeat = true;
				this.timeSinceHold -= this.repeatTimeInit;
				return;
			}
		}
		else if (this.timeSinceHold > this.repeatTime)
		{
			base.OnPointerClick(this.pointerDownEventData);
			this.timeSinceHold -= this.repeatTime;
		}
	}

	public bool repeatClickOnHold;

	public float repeatTimeInit = 0.8f;

	public float repeatTime = 0.2f;

	private float timeSinceHold;

	private bool onRepeat;

	private bool isHolding;

	private PointerEventData pointerDownEventData;
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class CreditsScroll : MonoBehaviour
{
	private float MaxScrollPosition
	{
		get
		{
			return this.scrollContents.sizeDelta.y - base.GetComponent<RectTransform>().sizeDelta.y;
		}
	}

	private void OnEnable()
	{
		this.position = 0f;
		this.holdPositionTimer = this.holdTime;
	}

	private void OnDisable()
	{
		this.position = -10000f;
	}

	private void Update()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		float num = this.scrollSpeed;
		if (Singleton<GameMaster>.Instance.stanleyActions.MenuConfirm.IsPressed || Singleton<GameMaster>.Instance.stanleyActions.MoveBackward.IsPressed)
		{
			num = this.fastScrollSpeed;
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.Up.IsPressed || Singleton<GameMaster>.Instance.stanleyActions.MoveForward.IsPressed)
		{
			num *= -1f;
		}
		if (this.holdPositionTimer > 0f)
		{
			this.holdPositionTimer -= Singleton<GameMaster>.Instance.GameDeltaTime;
		}
		else
		{
			this.position += num * Singleton<GameMaster>.Instance.GameDeltaTime;
		}
		this.position = Mathf.Clamp(this.position, 0f, this.MaxScrollPosition);
		float y = this.scrollContents.localPosition.y;
		this.scrollContents.localPosition = new Vector3(0f, this.position, 0f);
		if (y != 0f && y == this.scrollContents.localPosition.y)
		{
			UnityEvent unityEvent = this.onContentScrolledToBottom;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	public RectTransform scrollContents;

	public float scrollSpeed = 40f;

	public float fastScrollSpeed = 120f;

	public float holdTime = 1f;

	private float position;

	private float holdPositionTimer;

	public UnityEvent onContentScrolledToBottom;
}

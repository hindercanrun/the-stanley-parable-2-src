using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class FollowCursor : MonoBehaviour
{
	private void Awake()
	{
		this.cursorImage = base.GetComponent<RawImage>();
	}

	private void Start()
	{
		this.eventSystem = Object.FindObjectOfType<EventSystem>();
	}

	private void Update()
	{
		this.CheckStatus();
		switch (this.cursorState)
		{
		case FollowCursor.FakeCursorState.Disabled:
			this.cursorImage.enabled = false;
			base.transform.position = Input.mousePosition;
			return;
		case FollowCursor.FakeCursorState.Following:
			this.cursorImage.enabled = true;
			if (this.eventSystem != null && this.eventSystem.currentSelectedGameObject != null)
			{
				this.pointOfInterest = this.eventSystem.currentSelectedGameObject.transform.position;
				base.transform.position = Vector3.Lerp(base.transform.position, this.pointOfInterest, Time.deltaTime * 20f);
			}
			break;
		case FollowCursor.FakeCursorState.Hidden:
			break;
		default:
			return;
		}
	}

	private void CheckStatus()
	{
		if (Input.GetAxis("Mouse X") > 0f || Input.GetAxis("Mouse Y") > 0f)
		{
			this.cursorState = FollowCursor.FakeCursorState.Disabled;
			Cursor.visible = true;
			return;
		}
		if (Input.GetAxis("Horizontal") > 0f || Input.GetAxis("Vertical") > 0f)
		{
			this.cursorState = FollowCursor.FakeCursorState.Following;
			Cursor.visible = false;
		}
	}

	[SerializeField]
	private EventSystem eventSystem;

	private RawImage cursorImage;

	private Vector3 pointOfInterest;

	private FollowCursor.FakeCursorState cursorState = FollowCursor.FakeCursorState.Following;

	private enum FakeCursorState
	{
		Disabled,
		Following,
		Hidden
	}
}

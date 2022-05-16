using System;
using UnityEngine;

public class MenuCursor : MonoBehaviour
{
	public string hoveringOver { get; private set; }

	private void Awake()
	{
		this.RT = base.GetComponent<RectTransform>();
	}

	private void Update()
	{
		this.MovementController();
		this.MovementMouse();
		Ray ray = new Ray(this.camera.transform.position, base.transform.position - this.camera.transform.position);
		RaycastHit[] array = Physics.RaycastAll(ray.origin, ray.direction, 1000f);
		this.hoveringOver = "";
		for (int i = 0; i < array.Length; i++)
		{
			MenuButton component = array[i].collider.GetComponent<MenuButton>();
			if (component != null)
			{
				if (Singleton<GameMaster>.Instance.stanleyActions.UseAction.WasPressed)
				{
					component.OnHover();
					component.OnClick(array[i].point);
				}
				else if (Singleton<GameMaster>.Instance.stanleyActions.UseAction.IsPressed)
				{
					component.OnHover();
					component.OnHold(array[i].point);
				}
				else
				{
					component.OnHover();
				}
				this.hoveringOver = component.name;
			}
		}
	}

	private void MovementController()
	{
		Vector3 vector = Vector3.zero;
		vector += Vector3.right * Singleton<GameMaster>.Instance.stanleyActions.View.X;
		vector += Vector3.up * Singleton<GameMaster>.Instance.stanleyActions.View.Y;
		if (vector != Vector3.zero)
		{
			this.cursorSpeed += this.cursorAcceleration * Time.deltaTime;
			this.cursorSpeed = Mathf.Clamp(this.cursorSpeed, 0f, this.cursorMaxSpeed);
			vector = vector.normalized * Mathf.Pow(vector.magnitude, this.cursorRampPower);
			vector *= this.cursorSpeed;
			vector = this.RT.anchoredPosition3D + vector;
			vector.x = Mathf.Clamp(vector.x, 3f, 2048f);
			vector.y = Mathf.Clamp(vector.y, -1024f, -1f);
			this.RT.anchoredPosition = vector;
			return;
		}
		this.cursorSpeed = 0f;
	}

	private void MovementMouse()
	{
		Vector3 vector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * this.mouseSensitivity;
		vector = this.RT.anchoredPosition3D + vector;
		vector.x = Mathf.Clamp(vector.x, 3f, 2048f);
		vector.y = Mathf.Clamp(vector.y, -1024f, -1f);
		this.RT.anchoredPosition = vector;
	}

	private RectTransform RT;

	public GameObject camera;

	[Space(10f)]
	public float cursorMaxSpeed = 15f;

	public float cursorAcceleration = 1f;

	public float cursorRampPower = 2f;

	[Space(5f)]
	public float mouseSensitivity = 20f;

	private float cursorSpeed;

	private bool controller;

	private bool mouse = true;
}

using System;
using UnityEngine;

public class ExtendedFlycam : MonoBehaviour
{
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.cameraSensitivity * Time.deltaTime;
			this.rotationY += Input.GetAxis("Mouse Y") * this.cameraSensitivity * Time.deltaTime;
		}
		this.rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
		Quaternion quaternion = Quaternion.AngleAxis(this.rotationX, Vector3.up);
		quaternion *= Quaternion.AngleAxis(this.rotationY, Vector3.left);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 5f);
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			base.transform.position += base.transform.forward * (this.normalMoveSpeed * this.fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * (this.normalMoveSpeed * this.fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
			{
				base.transform.position += Vector3.up * this.climbSpeed * this.fastMoveFactor * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				base.transform.position -= Vector3.up * this.climbSpeed * this.fastMoveFactor * Time.deltaTime;
			}
		}
		else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			base.transform.position += base.transform.forward * (this.normalMoveSpeed * this.slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * (this.normalMoveSpeed * this.slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
			{
				base.transform.position += Vector3.up * this.climbSpeed * this.slowMoveFactor * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				base.transform.position -= Vector3.up * this.climbSpeed * this.slowMoveFactor * Time.deltaTime;
			}
		}
		else
		{
			base.transform.position += base.transform.forward * this.normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * this.normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
			{
				base.transform.position += Vector3.up * this.climbSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				base.transform.position -= Vector3.up * this.climbSpeed * Time.deltaTime;
			}
		}
		if (Input.GetKeyDown(KeyCode.End) || Input.GetKeyDown(KeyCode.Escape))
		{
			if (Cursor.lockState == CursorLockMode.None)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				return;
			}
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public float cameraSensitivity = 90f;

	public float climbSpeed = 4f;

	public float normalMoveSpeed = 10f;

	public float slowMoveFactor = 0.25f;

	public float fastMoveFactor = 3f;

	private float rotationX;

	private float rotationY;
}

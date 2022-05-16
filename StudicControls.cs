using System;
using UnityEngine;

public class StudicControls : MonoBehaviour
{
	private void Update()
	{
		base.transform.position += base.transform.right * this.speed * Time.deltaTime * Input.GetAxis("Horizontal") + base.transform.forward * this.speed * Time.deltaTime * Input.GetAxis("Vertical") + Vector3.up * this.speed * Time.deltaTime * (float)(Input.GetKey(KeyCode.Space) ? (Input.GetKey(KeyCode.LeftShift) ? -1 : 1) : 0);
		base.transform.eulerAngles = new Vector3(Input.mousePosition.y / this.mouseScale, Input.mousePosition.x / this.mouseScale);
	}

	public float speed = 10f;

	public float mouseScale = 32f;
}

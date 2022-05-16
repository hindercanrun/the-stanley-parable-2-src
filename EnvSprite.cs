using System;
using UnityEngine;

public class EnvSprite : HammerEntity
{
	private void Awake()
	{
		this._renderer = base.GetComponent<MeshRenderer>();
		if (this._renderer != null)
		{
			this._renderer.material.SetColor("_Color", this.color);
		}
		this.baseAlpha = this.color.a;
		this.alpha = 0f;
		this.isEnabled = this.startOn;
		if (!this.isEnabled && this._renderer != null)
		{
			this._renderer.enabled = false;
		}
		if (StanleyController.Instance == null)
		{
			this.originTransform = Object.FindObjectOfType<Camera>().transform;
			return;
		}
		this.originTransform = StanleyController.Instance.camTransform;
	}

	private void OnBecameVisible()
	{
		this.rendering = true;
	}

	private void OnBecameInvisible()
	{
		this.rendering = false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(base.transform.position, this.proxyRadius);
	}

	private void FixedUpdate()
	{
		if (!this.rendering)
		{
			return;
		}
		this.FireRays(4);
		float num = 0f;
		for (int i = 0; i < this.rayHistory.Length; i++)
		{
			if (this.rayHistory[i])
			{
				num += 1f / (float)this.rayHistory.Length;
			}
		}
		this.alpha = 0.5f * (this.alpha + num * this.baseAlpha);
		if (this.alpha != this.color.a)
		{
			this.color.a = this.alpha;
			if (this._renderer != null)
			{
				this._renderer.material.SetColor("_Color", this.color);
			}
		}
	}

	private void FireRays(int amt)
	{
		Vector3 position = this.originTransform.position;
		for (int i = 0; i < amt; i++)
		{
			Vector3 vector = base.transform.position;
			Vector3 vector2 = Quaternion.Euler(TimMaths.SphereRandom(), Random.Range(0f, 180f), 0f) * Vector3.forward * this.proxyRadius;
			vector2 = Quaternion.FromToRotation(Vector3.right, position - vector) * vector2;
			vector += vector2;
			Vector3 direction = vector - position;
			float maxDistance = Vector3.Distance(position, vector);
			RaycastHit raycastHit;
			if (Physics.Raycast(position, direction, out raycastHit, maxDistance, this.raycastMask))
			{
				this.rayHistory[this.rayIndex] = false;
			}
			else
			{
				this.rayHistory[this.rayIndex] = true;
			}
			this.rayIndex++;
			if (this.rayIndex >= this.rayHistory.Length)
			{
				this.rayIndex = 0;
			}
		}
	}

	public void Input_Color(string newColor)
	{
		string[] array = newColor.Split(new char[]
		{
			' '
		});
		if (array.Length != 3)
		{
			return;
		}
		Color color = new Color(float.Parse(array[0]) / 255f, float.Parse(array[1]) / 255f, float.Parse(array[2]) / 255f);
		this.color = color;
		this._renderer.material.SetColor("_Color", this.color);
	}

	public void Input_ShowSprite()
	{
		base.Input_Enable();
		if (this._renderer != null)
		{
			this._renderer.enabled = true;
		}
	}

	public void Input_HideSprite()
	{
		base.Input_Disable();
		if (this._renderer != null)
		{
			this._renderer.enabled = false;
		}
	}

	public bool startOn = true;

	public Color color = Color.white;

	private float baseAlpha = 1f;

	private float alpha = 1f;

	public float proxyRadius = 0.02f;

	private MeshRenderer _renderer;

	private bool rendering;

	private LayerMask raycastMask = 1;

	private bool[] rayHistory = new bool[32];

	private int rayIndex;

	private Transform originTransform;
}

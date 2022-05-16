using System;
using UnityEngine;

public class PropDynamic : HammerEntity
{
	private void Awake()
	{
		this._renderer = base.GetComponent<MeshRenderer>();
		this._collider = base.GetComponent<MeshCollider>();
		this._anim = base.GetComponent<Animator>();
		this.gotAnim = (this._anim != null);
		if (!this._renderer)
		{
			this._skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
			this._collider = base.GetComponentInChildren<MeshCollider>();
		}
		this.renderColor != Color.white;
		this.skins = base.GetComponents<Skin>();
		if (!this.isEnabled)
		{
			if (this._renderer)
			{
				this._renderer.enabled = false;
			}
			if (this._skinRenderer)
			{
				this._skinRenderer.enabled = false;
			}
			if (this._collider != null)
			{
				this._collider.enabled = false;
			}
		}
		if (this.startAnim != "")
		{
			this.Input_SetAnimation(this.startAnim);
		}
		if (this._anim != null)
		{
			this.originalAnimSpeed = this._anim.speed;
		}
		GameMaster.OnPause += this.Pause;
		GameMaster.OnResume += this.Resume;
	}

	private void OnDestroy()
	{
		GameMaster.OnPause -= this.Pause;
		GameMaster.OnResume -= this.Resume;
	}

	public override void Input_Enable()
	{
		base.Input_Enable();
		if (this._renderer)
		{
			this._renderer.enabled = true;
		}
		if (this._skinRenderer)
		{
			this._skinRenderer.enabled = true;
		}
		if (this._collider != null)
		{
			this._collider.enabled = true;
		}
	}

	public override void Input_Disable()
	{
		base.Input_Disable();
		if (this._renderer)
		{
			this._renderer.enabled = false;
		}
		if (this._skinRenderer)
		{
			this._skinRenderer.enabled = false;
		}
		if (this._collider != null)
		{
			this._collider.enabled = false;
		}
	}

	public void Input_SetAnimation(string anim)
	{
		if (this._anim)
		{
			this._anim.Play(anim);
		}
	}

	public void Input_Skin(float index)
	{
		int num = Mathf.FloorToInt(index);
		int i = 0;
		while (i < this.skins.Length)
		{
			if (this.skins[i].index == num)
			{
				if (this._renderer)
				{
					this._renderer.materials = this.skins[i].materials;
					return;
				}
				if (this._skinRenderer)
				{
					this._skinRenderer.materials = this.skins[i].materials;
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	public void Input_Color(string color)
	{
		string[] array = color.Split(new char[]
		{
			' '
		});
		if (array.Length != 3)
		{
			return;
		}
		Color color2 = new Color(float.Parse(array[0]) / 255f, float.Parse(array[1]) / 255f, float.Parse(array[2]) / 255f);
		this.renderColor = color2;
	}

	private void Pause()
	{
		if (this.gotAnim)
		{
			this._anim.speed = 0f;
		}
	}

	private void Resume()
	{
		if (this.gotAnim)
		{
			this._anim.speed = this.originalAnimSpeed;
		}
	}

	private MeshRenderer _renderer;

	private SkinnedMeshRenderer _skinRenderer;

	private MeshCollider _collider;

	private Animator _anim;

	private float originalAnimSpeed = 1f;

	public Color renderColor = Color.white;

	public Skin[] skins;

	private int currentSkin;

	public string startAnim = "";

	private bool gotAnim;
}

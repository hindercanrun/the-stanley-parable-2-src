using System;
using UnityEngine;

public class MoveLinear : HammerEntity
{
	private void Awake()
	{
		if (this.startLocked)
		{
			this.locked = true;
		}
		this.closedPos = base.transform.localPosition;
		if (this.moveDistance == 0f)
		{
			Bounds bounds = base.GetComponent<Renderer>().bounds;
			this.moveDistance = Vector3.Scale(this.moveDir, bounds.extents * 2f).magnitude;
		}
		if (this.multiplyMoveDirectionWithTransformWorldToLocalMatrix)
		{
			this.moveDir = base.transform.worldToLocalMatrix * this.moveDir;
		}
		this.openPos = this.closedPos + this.moveDir * (this.moveDistance - this.lip);
		this.targetPos = this.startPos;
		this.currentPos = this.targetPos;
		base.transform.localPosition = Vector3.Lerp(this.closedPos, this.openPos, this.currentPos);
	}

	private void Update()
	{
		if (!this.isEnabled)
		{
			return;
		}
		if (this.currentPos == this.targetPos)
		{
			return;
		}
		if (this.speed == 0f)
		{
			return;
		}
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, Vector3.Lerp(this.closedPos, this.openPos, this.targetPos), this.speed * Singleton<GameMaster>.Instance.GameDeltaTime);
		this.currentPos = TimMaths.Vector3InverseLerp(this.closedPos, this.openPos, base.transform.localPosition);
		if (this.currentPos == 1f)
		{
			base.FireOutput(Outputs.OnFullyOpen);
			return;
		}
		if (this.currentPos == 0f)
		{
			base.FireOutput(Outputs.OnFullyClosed);
		}
	}

	public void Input_Open()
	{
		if (!this.isEnabled)
		{
			return;
		}
		if (this.locked)
		{
			return;
		}
		this.targetPos = 1f;
	}

	public void Input_Close()
	{
		if (!this.isEnabled)
		{
			return;
		}
		if (this.locked)
		{
			return;
		}
		this.targetPos = 0f;
	}

	public void Input_Lock()
	{
		this.locked = true;
	}

	public void Input_Unlock()
	{
		this.locked = false;
	}

	public void Input_SetSpeed(float newSpeed)
	{
		newSpeed = (this.speed = newSpeed / 100f);
	}

	public Vector3 moveDir;

	public float moveDistance;

	public float startPos;

	public bool startLocked;

	public float lip;

	public float speed;

	private bool locked;

	private Vector3 closedPos;

	private Vector3 openPos;

	private float targetPos;

	private float currentPos;

	[SerializeField]
	private bool multiplyMoveDirectionWithTransformWorldToLocalMatrix;
}

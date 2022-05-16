using System;
using UnityEngine;

namespace Ferr
{
	[Serializable]
	public class LayoutAdvancer
	{
		public float X
		{
			get
			{
				return this.mPos.x;
			}
		}

		public float Y
		{
			get
			{
				return this.mPos.y;
			}
		}

		public LayoutAdvancer.Direction Dir
		{
			get
			{
				return this.mDirection;
			}
		}

		public LayoutAdvancer(Vector2 aStartLocation, LayoutAdvancer.Direction aDirection)
		{
			this.mPos = aStartLocation;
			this.mDirection = aDirection;
			this.mPrevPos = aStartLocation;
			this.mPrevious = Vector2.zero;
		}

		public void Step(Vector2 aSize)
		{
			this.Step(aSize.x, aSize.y);
		}

		public void Step(float aX, float aY)
		{
			this.mPrevPos.x = this.mPos.x;
			this.mPrevPos.y = this.mPos.y;
			if (this.mDirection == LayoutAdvancer.Direction.Horizontal)
			{
				this.mPos.x = this.mPos.x + aX;
			}
			if (this.mDirection == LayoutAdvancer.Direction.Vertical)
			{
				this.mPos.y = this.mPos.y + aY;
			}
			this.mPrevious.x = aX;
			this.mPrevious.y = aY;
		}

		public Rect GetRect()
		{
			return new Rect(this.mPrevPos.x, this.mPrevPos.y, this.mPrevious.x, this.mPrevious.y);
		}

		public Rect GetRect(float aOverrideDir)
		{
			if (this.mDirection == LayoutAdvancer.Direction.Vertical)
			{
				return new Rect(this.mPrevPos.x, this.mPrevPos.y, this.mPrevious.x, aOverrideDir);
			}
			return new Rect(this.mPrevPos.x, this.mPrevPos.y, aOverrideDir, this.mPrevious.y);
		}

		public Rect GetRect(float aOverrideWidth, float aOverrideHeight)
		{
			return new Rect(this.mPos.x, this.mPos.y, aOverrideWidth, aOverrideHeight);
		}

		public Rect GetRectPad(float aPaddingX, float aPaddingY)
		{
			return new Rect(this.mPrevPos.x + aPaddingX, this.mPrevPos.y + aPaddingY, this.mPrevious.x - aPaddingX * 2f, this.mPrevious.y - aPaddingY * 2f);
		}

		public Rect GetRectPad(float aPadding)
		{
			return this.GetRectPad(aPadding, aPadding);
		}

		[SerializeField]
		private Vector2 mPos;

		[SerializeField]
		private LayoutAdvancer.Direction mDirection;

		[SerializeField]
		private Vector2 mPrevious;

		[SerializeField]
		private Vector2 mPrevPos;

		public enum Direction
		{
			Vertical,
			Horizontal
		}
	}
}

using System;
using UnityEngine;

namespace Ferr
{
	public class LogoLock : MonoBehaviour
	{
		private void Awake()
		{
			if (this.mCamera == null)
			{
				this.mCamera = Camera.main;
			}
			base.transform.parent = this.mCamera.transform;
			base.transform.localPosition = LogoLock.GetLockPosition(this.mCamera, this.mLockHorizontal, this.mLockVertical, this.mPadding);
			float pixelScale = LogoLock.GetPixelScale(this.mCamera, base.GetComponent<SpriteRenderer>().sprite);
			base.transform.localScale = new Vector3(pixelScale, pixelScale, pixelScale) * this.mScale;
			base.transform.localRotation = Quaternion.identity;
		}

		private static float GetPixelScale(Camera aCam, Sprite aSprite)
		{
			ref Vector2 viewSizeAtDistance = LogoLock.GetViewSizeAtDistance(1f, aCam);
			float num = aSprite.textureRect.width / (float)Screen.width;
			return viewSizeAtDistance.x * num / (aSprite.bounds.extents.x * 2f);
		}

		private static Vector3 GetLockPosition(Camera aCam, LogoLock.LockPosition aHLock, LogoLock.LockPosition aVLock, float aPadding)
		{
			Vector3 zero = Vector3.zero;
			Vector2 viewSizeAtDistance = LogoLock.GetViewSizeAtDistance(1f, aCam);
			zero.z = 1f;
			aPadding *= 1f / (float)Screen.width * viewSizeAtDistance.x;
			if (aHLock == LogoLock.LockPosition.Left)
			{
				zero.x = -viewSizeAtDistance.x / 2f + aPadding;
			}
			else if (aHLock == LogoLock.LockPosition.Center)
			{
				zero.x = 0f;
			}
			else if (aHLock == LogoLock.LockPosition.Right)
			{
				zero.x = viewSizeAtDistance.x / 2f - aPadding;
			}
			if (aVLock == LogoLock.LockPosition.Left)
			{
				zero.y = viewSizeAtDistance.y / 2f - aPadding;
			}
			else if (aVLock == LogoLock.LockPosition.Center)
			{
				zero.y = 0f;
			}
			else if (aVLock == LogoLock.LockPosition.Right)
			{
				zero.y = -viewSizeAtDistance.y / 2f + aPadding;
			}
			return zero;
		}

		private static Vector2 GetViewSizeAtDistance(float aDist, Camera aCamera)
		{
			if (aCamera == null)
			{
				aCamera = Camera.main;
			}
			if (aCamera.orthographic)
			{
				return new Vector2((float)Screen.width / (float)Screen.height * aCamera.orthographicSize * 2f, aCamera.orthographicSize * 2f);
			}
			float num = 2f * aDist * Mathf.Tan(aCamera.fieldOfView * 0.5f * 0.017453292f);
			return new Vector2(num * aCamera.aspect, num);
		}

		[SerializeField]
		private Camera mCamera;

		[SerializeField]
		private LogoLock.LockPosition mLockHorizontal;

		[SerializeField]
		private LogoLock.LockPosition mLockVertical;

		[SerializeField]
		private float mPadding;

		[SerializeField]
		private float mScale = 1f;

		private enum LockPosition
		{
			Left,
			Center,
			Right
		}
	}
}

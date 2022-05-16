using System;
using UnityEngine;

namespace Ferr
{
	public class CameraShake : MonoBehaviour
	{
		private static CameraShake Instance
		{
			get
			{
				if (CameraShake.instance == null)
				{
					CameraShake.instance = CameraShake.Create();
				}
				return CameraShake.instance;
			}
		}

		private CameraShake()
		{
		}

		private static CameraShake Create()
		{
			return Camera.main.gameObject.AddComponent<CameraShake>();
		}

		private void LateUpdate()
		{
			float num = (Time.time - this.start) / this.duration;
			if (num <= 1f)
			{
				base.transform.position -= this.offset;
				this.offset = new Vector3(Random.Range(-this.magnitude.x, this.magnitude.x), Random.Range(-this.magnitude.y, this.magnitude.y), Random.Range(-this.magnitude.z, this.magnitude.z)) * this.curve.Evaluate(num);
				base.transform.position += this.offset;
				return;
			}
			base.transform.position -= this.offset;
			this.offset = Vector2.zero;
			base.enabled = false;
		}

		public static void Shake(Vector3 aMagnitude, float aDuration)
		{
			CameraShake.Instance.magnitude = aMagnitude;
			CameraShake.Instance.duration = aDuration;
			CameraShake.Instance.start = Time.time;
			CameraShake.Instance.transform.position -= CameraShake.Instance.offset;
			CameraShake.Instance.offset = Vector3.zero;
			CameraShake.Instance.enabled = true;
			CameraShake.Instance.curve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(1f, 0f)
			});
		}

		private static CameraShake instance;

		private Vector3 magnitude;

		private float duration;

		private float start;

		private Vector3 offset;

		private AnimationCurve curve;
	}
}

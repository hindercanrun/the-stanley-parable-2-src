using System;
using UnityEngine;

namespace Ferr
{
	[Serializable]
	public class JuiceData
	{
		public bool Update()
		{
			if (this.transform == null)
			{
				return true;
			}
			float num = Mathf.Min((Time.time - this.startTime) / this.duration, 1f);
			float num2 = this.start + this.curve.Evaluate(num) * (this.end - this.start);
			if (this.relative)
			{
				float time = Mathf.Max(0f, Mathf.Min((Time.time - Time.deltaTime - this.startTime) / this.duration, 1f));
				num2 -= this.start + this.curve.Evaluate(time) * (this.end - this.start);
			}
			JuiceType juiceType = this.type;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Vector3 vector = this.relative ? Vector3.zero : this.transform.localPosition;
			Vector3 vector2 = this.relative ? Vector3.zero : this.transform.localScale;
			Vector3 vector3 = this.relative ? Vector3.zero : this.transform.eulerAngles;
			if ((juiceType & JuiceType.TranslateX) > (JuiceType)0)
			{
				vector.x = num2;
				flag = true;
			}
			if ((juiceType & JuiceType.TranslateY) > (JuiceType)0)
			{
				vector.y = num2;
				flag = true;
			}
			if ((juiceType & JuiceType.TranslateZ) > (JuiceType)0)
			{
				vector.z = num2;
				flag = true;
			}
			if ((juiceType & JuiceType.ScaleX) > (JuiceType)0)
			{
				vector2.x = num2;
				flag2 = true;
			}
			if ((juiceType & JuiceType.ScaleY) > (JuiceType)0)
			{
				vector2.y = num2;
				flag2 = true;
			}
			if ((juiceType & JuiceType.ScaleZ) > (JuiceType)0)
			{
				vector2.z = num2;
				flag2 = true;
			}
			if ((juiceType & JuiceType.RotationX) > (JuiceType)0)
			{
				vector3.x = num2;
				flag3 = true;
			}
			if ((juiceType & JuiceType.RotationY) > (JuiceType)0)
			{
				vector3.y = num2;
				flag3 = true;
			}
			if ((juiceType & JuiceType.RotationZ) > (JuiceType)0)
			{
				vector3.z = num2;
				flag3 = true;
			}
			if (flag && this.relative)
			{
				this.transform.localPosition += vector;
			}
			else if (flag)
			{
				this.transform.localPosition = vector;
			}
			if (flag2 && this.relative)
			{
				this.transform.localScale += vector2;
			}
			else if (flag2)
			{
				this.transform.localScale = vector2;
			}
			if (flag3 && this.relative)
			{
				this.transform.localEulerAngles += vector3;
			}
			else if (flag3)
			{
				this.transform.localEulerAngles = vector3;
			}
			return num >= 1f;
		}

		public void Cancel()
		{
			this.startTime = -10000f;
			this.Update();
		}

		public JuiceType type;

		public Transform transform;

		public float start;

		public float end;

		public float duration;

		public float startTime;

		public bool relative;

		public AnimationCurve curve;

		public Action callback;
	}
}

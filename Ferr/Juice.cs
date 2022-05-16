using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr
{
	public class Juice : MonoBehaviour
	{
		private static Juice Instance
		{
			get
			{
				if (Juice.instance == null)
				{
					Juice.instance = Juice.Create();
				}
				return Juice.instance;
			}
		}

		private Juice()
		{
		}

		private static Juice Create()
		{
			return new GameObject("JuiceDriver").AddComponent<Juice>();
		}

		public static AnimationCurve SproingIn
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(0.7f, 1.1f),
					new Keyframe(0.85f, 0.9f),
					new Keyframe(1f, 1f)
				});
			}
		}

		public static AnimationCurve FastFalloff
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f, 1f, 1f),
					new Keyframe(0.25f, 0.8f, 1f, 1f),
					new Keyframe(1f, 1f)
				});
			}
		}

		public static AnimationCurve LateFalloff
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(0.75f, 0.25f),
					new Keyframe(1f, 1f)
				});
			}
		}

		public static AnimationCurve Wobble
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(0.25f, 1f),
					new Keyframe(0.75f, -1f),
					new Keyframe(1f, 0f)
				});
			}
		}

		public static AnimationCurve Linear
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f, 1f, 1f),
					new Keyframe(1f, 1f, 1f, 1f)
				});
			}
		}

		public static AnimationCurve Hop
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(0.5f, 1f),
					new Keyframe(1f, 0f)
				});
			}
		}

		public static AnimationCurve SharpHop
		{
			get
			{
				return new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 1f),
					new Keyframe(1f, 0f)
				});
			}
		}

		private void Update()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.list[i].Update())
				{
					if (this.list[i].callback != null)
					{
						this.list[i].callback();
					}
					this.list.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < this.listColor.Count; j++)
			{
				if (this.listColor[j].Update())
				{
					if (this.listColor[j].callback != null)
					{
						this.listColor[j].callback();
					}
					this.listColor.RemoveAt(j);
					j--;
				}
			}
			if (this.sleep && Time.realtimeSinceStartup > this.sleepTimer)
			{
				this.sleep = false;
				Time.timeScale = this.savedTimescale;
			}
		}

		public static void Add(Transform aTransform, JuiceType aType, AnimationCurve aCurve, float aStart = 0f, float aEnd = 1f, float aDuration = 1f, bool aRelative = false, Action aCallback = null)
		{
			JuiceData juiceData = new JuiceData();
			juiceData.transform = aTransform;
			juiceData.type = aType;
			juiceData.curve = aCurve;
			juiceData.start = aStart;
			juiceData.duration = aDuration;
			juiceData.startTime = Time.time;
			juiceData.end = aEnd;
			juiceData.relative = aRelative;
			juiceData.callback = aCallback;
			Juice.Instance.list.Add(juiceData);
			juiceData.Update();
		}

		public static void Scale(Transform aTransform, AnimationCurve aCurve, float aStart = 0f, float aEnd = 1f, float aDuration = 1f, bool aRelative = false, Action aCallback = null)
		{
			Juice.Add(aTransform, JuiceType.ScaleXYZ, aCurve, aStart, aEnd, aDuration, aRelative, aCallback);
		}

		public static void Scale(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration = 1f, bool aRelative = false, Action aCallback = null)
		{
			Juice.Add(aTransform, JuiceType.ScaleX, aCurve, aStart.x, aEnd.x, aDuration, aRelative, aCallback);
			Juice.Add(aTransform, JuiceType.ScaleY, aCurve, aStart.y, aEnd.y, aDuration, aRelative, aCallback);
			Juice.Add(aTransform, JuiceType.ScaleZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
		}

		public static void Rotate(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration = 1f, bool aRelative = false, Action aCallback = null)
		{
			Juice.Add(aTransform, JuiceType.RotationX, aCurve, aStart.x, aEnd.x, aDuration, aRelative, aCallback);
			Juice.Add(aTransform, JuiceType.RotationY, aCurve, aStart.y, aEnd.y, aDuration, aRelative, aCallback);
			Juice.Add(aTransform, JuiceType.RotationZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
		}

		public static void Translate(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration, bool aRelative = false, Action aCallback = null)
		{
			Vector3 vector = Vector3.zero;
			if (aTransform.parent != null)
			{
				vector = aTransform.parent.position;
			}
			Juice.Add(aTransform, JuiceType.TranslateX, aCurve, aStart.x - vector.x, aEnd.x - vector.x, aDuration, aRelative, null);
			Juice.Add(aTransform, JuiceType.TranslateY, aCurve, aStart.y - vector.y, aEnd.y - vector.y, aDuration, aRelative, null);
			Juice.Add(aTransform, JuiceType.TranslateZ, aCurve, aStart.z - vector.z, aEnd.z - vector.z, aDuration, aRelative, aCallback);
		}

		public static void TranslateLocal(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration, bool aRelative = false, Action aCallback = null)
		{
			Juice.Add(aTransform, JuiceType.TranslateX, aCurve, aStart.x, aEnd.x, aDuration, aRelative, null);
			Juice.Add(aTransform, JuiceType.TranslateY, aCurve, aStart.y, aEnd.y, aDuration, aRelative, null);
			Juice.Add(aTransform, JuiceType.TranslateZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
		}

		public static void Color(Material aRenderer, AnimationCurve aCurve, Color aStart, Color aEnd, float aDuration, Action aCallback = null)
		{
			JuiceDataColor juiceDataColor = new JuiceDataColor();
			juiceDataColor.renderer = aRenderer;
			juiceDataColor.curve = aCurve;
			juiceDataColor.start = aStart;
			juiceDataColor.duration = aDuration;
			juiceDataColor.startTime = Time.time;
			juiceDataColor.end = aEnd;
			juiceDataColor.callback = aCallback;
			Juice.Instance.listColor.Add(juiceDataColor);
			juiceDataColor.Update();
		}

		public static void Cancel(Transform aTransform, bool aFinishEffect = true)
		{
			for (int i = 0; i < Juice.Instance.list.Count; i++)
			{
				if (Juice.Instance.list[i].transform == aTransform)
				{
					if (aFinishEffect)
					{
						Juice.Instance.list[i].Cancel();
					}
					Juice.Instance.list.RemoveAt(i);
					i--;
				}
			}
		}

		public static void Cancel(Renderer aRenderer, bool aFinishEffect = true)
		{
			for (int i = 0; i < Juice.Instance.listColor.Count; i++)
			{
				if (Juice.Instance.listColor[i].renderer == aRenderer)
				{
					if (aFinishEffect)
					{
						Juice.Instance.listColor[i].Cancel();
					}
					Juice.Instance.listColor.RemoveAt(i);
					i--;
				}
			}
		}

		public static void SlowMo(float aSpeed)
		{
			Time.timeScale = aSpeed;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}

		public static void Sleep(float aDuration)
		{
			Juice.Instance.savedTimescale = ((Time.timeScale == 0f) ? Juice.Instance.savedTimescale : Time.timeScale);
			Time.timeScale = 0f;
			Juice.Instance.sleep = true;
			Juice.Instance.sleepTimer = Time.realtimeSinceStartup + aDuration;
		}

		public static void SleepMS(int aMilliseconds)
		{
			Juice.Sleep((float)aMilliseconds * 0.001f);
		}

		private static Juice instance;

		public List<JuiceData> list = new List<JuiceData>();

		public List<JuiceDataColor> listColor = new List<JuiceDataColor>();

		private float savedTimescale;

		private float sleepTimer;

		private bool sleep;
	}
}

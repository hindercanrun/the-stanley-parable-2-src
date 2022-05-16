using System;
using UnityEngine;

namespace Nest.Components
{
	[AddComponentMenu("Nest/Input/Trigger Look")]
	public class TriggerLook : NestInput
	{
		public static RaycastHit[] Results
		{
			get
			{
				return TriggerLook.results;
			}
		}

		public override void Start()
		{
			base.Start();
			if (TriggerLook.results == null)
			{
				TriggerLook.results = new RaycastHit[5];
			}
			if (this.Tracking == null)
			{
				this.Tracking = base.transform;
			}
		}

		public void FixedUpdate()
		{
			if (this.RestrictToTrigger && !this._withinTrigger)
			{
				return;
			}
			int num;
			if ((this.CameraEvent & (TriggerLook.CameraType.WithinCrosshair | TriggerLook.CameraType.OutsideCrosshair)) > (TriggerLook.CameraType)0 && (num = Physics.RaycastNonAlloc(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), TriggerLook.results)) > 0)
			{
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					RaycastHit raycastHit = TriggerLook.Results[i];
					if (!(raycastHit.transform == null))
					{
						flag |= (raycastHit.transform == this.Tracking);
					}
				}
				if (this.Invoke(flag ? TriggerLook.CameraType.WithinCrosshair : TriggerLook.CameraType.OutsideCrosshair))
				{
					return;
				}
			}
			Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
			if (vector.z > 0f && vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f)
			{
				if (this._timeWithinView <= 0f)
				{
					this.Invoke(TriggerLook.CameraType.Enter);
				}
				this._timeWithinView += Time.deltaTime;
				this.Invoke(TriggerLook.CameraType.WithinView);
				return;
			}
			if (this._timeWithinView > 0f)
			{
				this.Invoke(TriggerLook.CameraType.Exit);
				this._timeWithinView = 0f;
				return;
			}
			this.Invoke(TriggerLook.CameraType.OutOfView);
		}

		public void OnTriggerEnter(Collider other)
		{
			this._withinTrigger = other.CompareTag("Player");
		}

		public void OnTriggerExit(Collider other)
		{
			this._withinTrigger = !other.CompareTag("Player");
		}

		public bool Invoke(TriggerLook.CameraType type)
		{
			if ((this.CameraEvent & type) == (TriggerLook.CameraType)0)
			{
				return false;
			}
			if (this._previousState == type && base.CurrentEventType != NestInput.EventType.Float)
			{
				return true;
			}
			this._previousState = type;
			if ((this._eventComposition.First & type) != (TriggerLook.CameraType)0)
			{
				base.SetBool(false);
			}
			else if ((this._eventComposition.Second & type) != (TriggerLook.CameraType)0)
			{
				base.SetBool(true);
			}
			if (this.TrackTimeWithinView)
			{
				this.Value.CurrentValue = this._timeWithinView;
			}
			base.Invoke();
			return true;
		}

		public TriggerLook.CameraType CameraEvent = TriggerLook.CameraType.WithinView;

		[NestOption]
		public bool RestrictToTrigger = true;

		[NestOption]
		public bool TrackTimeWithinView;

		[NestOption]
		public Transform Tracking;

		private float _timeWithinView;

		private bool _withinTrigger;

		private TriggerLook.CameraType _previousState;

		private static RaycastHit[] results;

		[SerializeField]
		private TriggerLook.TypeComposition _eventComposition;

		[Flags]
		public enum CameraType
		{
			Enter = 1,
			WithinView = 2,
			OutOfView = 4,
			WithinCrosshair = 8,
			OutsideCrosshair = 16,
			Exit = 32
		}

		[Serializable]
		public struct TypeComposition
		{
			public TriggerLook.CameraType First;

			public TriggerLook.CameraType Second;
		}
	}
}

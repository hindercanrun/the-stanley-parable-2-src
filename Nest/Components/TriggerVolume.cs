using System;
using System.Linq;
using UnityEngine;

namespace Nest.Components
{
	[AddComponentMenu("Nest/Components/Trigger Volume")]
	[RequireComponent(typeof(Collider))]
	public class TriggerVolume : NestInput
	{
		public bool IsEnterAndExit
		{
			get
			{
				return this.TriggerEvent == (TriggerVolume.TriggerType.Enter | TriggerVolume.TriggerType.Exit);
			}
		}

		public bool IsEnterAndStay
		{
			get
			{
				return this.TriggerEvent == (TriggerVolume.TriggerType.Enter | TriggerVolume.TriggerType.Stay);
			}
		}

		public bool IsExitAndStay
		{
			get
			{
				return this.TriggerEvent == (TriggerVolume.TriggerType.Stay | TriggerVolume.TriggerType.Exit);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!this._tagValues.Contains(other.tag))
			{
				return;
			}
			if (this.IsEnterAndExit || this.IsEnterAndStay)
			{
				this.SetValue(true);
			}
			this.Invoke(TriggerVolume.TriggerType.Enter);
		}

		public void OnTriggerExit(Collider other)
		{
			if (!this._tagValues.Contains(other.tag))
			{
				return;
			}
			if (this.IsEnterAndExit)
			{
				this.SetValue(false);
			}
			this.Invoke(TriggerVolume.TriggerType.Exit);
		}

		public void Invoke(TriggerVolume.TriggerType type)
		{
			if ((this.TriggerEvent & type) == (TriggerVolume.TriggerType)0)
			{
				return;
			}
			base.Invoke();
		}

		public void SetValue(bool value)
		{
			base.SetBool(value);
		}

		public TriggerVolume.TriggerType TriggerEvent = TriggerVolume.TriggerType.Enter;

		[SerializeField]
		public int TagMask = -1;

		[SerializeField]
		private string[] _tagValues;

		[Flags]
		public enum TriggerType
		{
			Enter = 1,
			Stay = 2,
			Exit = 4
		}
	}
}

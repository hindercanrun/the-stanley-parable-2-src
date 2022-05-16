using System;
using UnityEngine;
using UnityEngine.UI;

namespace StanleyUI
{
	public class StanleyMenuPageButton : MonoBehaviour, ISelectableHolderScreen
	{
		public Selectable DefaultSelectable
		{
			get
			{
				return this.defaultSelectable;
			}
		}

		public Selectable LastSelectable
		{
			get
			{
				return this.lastSelectable;
			}
			set
			{
				this.lastSelectable = value;
			}
		}

		[SerializeField]
		private Selectable defaultSelectable;

		private Selectable lastSelectable;
	}
}

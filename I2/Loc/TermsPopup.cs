using System;
using UnityEngine;

namespace I2.Loc
{
	public class TermsPopup : PropertyAttribute
	{
		public TermsPopup(string filter = "")
		{
			this.Filter = filter;
		}

		public string Filter { get; private set; }
	}
}

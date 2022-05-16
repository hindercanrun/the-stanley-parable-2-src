using System;
using UnityEngine;

namespace Ferr
{
	public class InspectorName : PropertyAttribute
	{
		public InspectorName(string aName)
		{
			this.mName = aName;
		}

		public string mName;
	}
}

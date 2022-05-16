using System;
using UnityEngine;

namespace Nest.Integrations
{
	public class BaseIntegration : MonoBehaviour
	{
		public virtual float InputValue
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}

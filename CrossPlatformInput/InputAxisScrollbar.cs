﻿using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class InputAxisScrollbar : MonoBehaviour
	{
		private void Update()
		{
		}

		public void HandleInput(float value)
		{
			CrossPlatformInputManager.SetAxis(this.axis, value * 2f - 1f);
		}

		public string axis;
	}
}

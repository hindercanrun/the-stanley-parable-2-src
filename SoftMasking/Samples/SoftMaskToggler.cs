using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoftMasking.Samples
{
	public class SoftMaskToggler : MonoBehaviour
	{
		public void Toggle(bool enabled)
		{
			if (this.mask)
			{
				this.mask.GetComponent<SoftMask>().enabled = enabled;
				this.mask.GetComponent<Mask>().enabled = !enabled;
				if (!this.doNotTouchImage)
				{
					this.mask.GetComponent<Image>().enabled = !enabled;
				}
			}
		}

		public GameObject mask;

		public bool doNotTouchImage;
	}
}

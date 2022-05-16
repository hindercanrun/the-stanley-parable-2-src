using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr
{
	public static class RuntimeLayerUtil
	{
		public static int GetFreeLayer()
		{
			for (int i = 16; i < 32; i++)
			{
				if (LayerMask.LayerToName(i) == "" && (RuntimeLayerUtil.mReservedLayers == null || !RuntimeLayerUtil.mReservedLayers.Contains(i)))
				{
					return i;
				}
			}
			Debug.LogError("Ferr is looking for an unnamed render layer after 15, but none are free!");
			return -1;
		}

		public static void ReserveLayer(int aLayerID)
		{
			if (RuntimeLayerUtil.mReservedLayers == null)
			{
				RuntimeLayerUtil.mReservedLayers = new List<int>();
			}
			RuntimeLayerUtil.mReservedLayers.Add(aLayerID);
		}

		private static List<int> mReservedLayers;
	}
}

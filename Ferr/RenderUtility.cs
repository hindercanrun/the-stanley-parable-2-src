using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr
{
	public class RenderUtility
	{
		public static int GetFreeLayer()
		{
			for (int i = 16; i < 32; i++)
			{
				if (LayerMask.LayerToName(i) == "" && (RenderUtility.mReservedLayers == null || !RenderUtility.mReservedLayers.Contains(i)))
				{
					return i;
				}
			}
			Debug.LogError("Ferr is looking for an unnamed render layer after 15, but none are free!");
			return -1;
		}

		public static void ReserveLayer(int aLayerID)
		{
			if (RenderUtility.mReservedLayers == null)
			{
				RenderUtility.mReservedLayers = new List<int>();
			}
			RenderUtility.mReservedLayers.Add(aLayerID);
		}

		public static Camera CreateRenderCamera()
		{
			if (RenderUtility.mCamera == null)
			{
				RenderUtility.mCamera = new GameObject("Ferr Render Cam").AddComponent<Camera>();
				RenderUtility.mCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
				RenderUtility.mCamera.enabled = false;
			}
			return RenderUtility.mCamera;
		}

		private static List<int> mReservedLayers;

		private static Camera mCamera;
	}
}

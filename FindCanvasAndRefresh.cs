using System;
using UnityEngine;

public class FindCanvasAndRefresh : MonoBehaviour
{
	public void FindCanvasAndForceRefresh()
	{
		Canvas.ForceUpdateCanvases();
	}
}

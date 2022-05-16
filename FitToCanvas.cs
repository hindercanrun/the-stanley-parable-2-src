using System;
using UnityEngine;

public class FitToCanvas : MonoBehaviour
{
	private void Awake()
	{
		this.RT = base.GetComponent<RectTransform>();
		this.canvasRT = base.transform.parent.GetComponent<RectTransform>();
	}

	private void Update()
	{
		this.RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.canvasRT.rect.width);
		this.RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.canvasRT.rect.height);
	}

	private RectTransform RT;

	private RectTransform canvasRT;
}
